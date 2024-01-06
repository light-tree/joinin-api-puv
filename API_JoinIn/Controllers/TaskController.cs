using API_JoinIn.Utils.Email;
using API_JoinIn.Utils.Notification;
using API_JoinIn.Utils.Notification.Implements;
using BusinessObject.Data;
using BusinessObject.DTOs;
using BusinessObject.DTOs.Common;
using BusinessObject.Enums;
using BusinessObject.Models;
using DataAccess.Security;
using DataAccess.Services;
using DataAccess.Services.Implements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Net;
using System.Threading.Tasks;
using System.Transactions;

namespace API_JoinIn.Controllers
{
    [ApiController]
    [Authorize]
    [Route("tasks")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly IJwtService _jwtService;
        private readonly IEmailService _emailService;
        private readonly IGroupService _groupService;
        private readonly IMemberService _memberService;
        private readonly IAssignedTaskService _assignedTaskService;
        private readonly IUserService _userService;
        private readonly IHubContext<NotificationSignalSender> _hubContext;
        private readonly IConfiguration _configuration;
        private readonly INotificationService _notificationService;


        public TaskController(ITaskService taskService,
                              IJwtService jwtService,
                              IEmailService emailService,
                              IGroupService groupService,
                              IMemberService memberService,
                              IAssignedTaskService assignedTaskService,
                              IUserService userService,
                              IHubContext<NotificationSignalSender> hubContext,
                              IConfiguration configuration, 
                              INotificationService notificationService)
        {
            _taskService = taskService;
            _jwtService = jwtService;
            _emailService = emailService;
            _groupService = groupService;
            _memberService = memberService;
            _assignedTaskService = assignedTaskService;
            _userService = userService;
            _hubContext = hubContext;
            _configuration = configuration;
            _notificationService = notificationService;
        }

        ///<summary>
        ///Get tasks for user. Leave groupId null to get TO DO tasks (tasks that assigned to user), 
        ///pass in groupId to get all tasks of that group (include tasks that are not assigned to them)
        ///</summary>
        [HttpGet]
        public IActionResult FilterTasks(Guid? groupId, string? name, int? pageSize, int? page, string? orderBy, string? value)
        {
            Guid userId = Guid.Empty;
            var jwtToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var decodedToken = _jwtService.DecodeJwtToken(jwtToken);
            if (decodedToken != null)
            {
                var userIdClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == "Id");
                if (userIdClaim != null)
                {
                    userId = Guid.Parse(userIdClaim.Value);
                    // Do something with user ID here
                }
                else throw new Exception("Internal server error");
            }
            CommonResponse response = new CommonResponse();
            try
            {
                CommonResponse commonResponse = _taskService.FilterTasks(userId, groupId, name, pageSize, page, orderBy, value);
                return new OkObjectResult(commonResponse);
            }
            catch (Exception ex)
            {
                response.Status = StatusCodes.Status500InternalServerError;
                response.Message = ex.Message;
                return new OkObjectResult(response);
            }
        }

        ///<summary>
        ///Group's member get task's detail, include assigned members and sub-tasks
        ///</summary>
        [HttpGet]
        [Route("{id}")]
        public IActionResult GetTaskDetail(Guid id)
        {
            Guid userId = Guid.Empty;
            var jwtToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var decodedToken = _jwtService.DecodeJwtToken(jwtToken);
            if (decodedToken != null)
            {
                var userIdClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == "Id");
                if (userIdClaim != null)
                {
                    userId = Guid.Parse(userIdClaim.Value);
                    // Do something with user ID here
                }
                else throw new Exception("Internal server error");
            }
            CommonResponse response = new CommonResponse();
            try
            {
                response.Status = StatusCodes.Status200OK; ;
                response.Message = "Get task's detail success.";
                response.Data = _taskService.GetDetailById(id, userId);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                response.Status = StatusCodes.Status500InternalServerError;;
                response.Message = ex.Message;
                return new OkObjectResult(response);
            }
        }

        ///<summary>
        ///Leader or sub-leader create task
        ///</summary>
        [HttpPost]
        public IActionResult CreateTask(TaskDTOForCreating task)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    Guid userId = Guid.Empty;
                    var jwtToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                    var decodedToken = _jwtService.DecodeJwtToken(jwtToken);
                    if (decodedToken != null)
                    {
                        var userIdClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == "Id");
                        if (userIdClaim != null)
                        {
                            userId = Guid.Parse(userIdClaim.Value);
                            // Do something with user ID here
                        }
                        else throw new Exception("Internal server error");
                    }

                    response.Data = _taskService.CreateTask(task, userId);
                    response.Status = StatusCodes.Status200OK; ;
                    response.Message = "Create task success.";
                    scope.Complete();
                    return new OkObjectResult(response);
                }
            }
            catch (Exception ex)
            {
                response.Status = StatusCodes.Status500InternalServerError;;
                response.Message = ex.Message;
                return new OkObjectResult(response);
            }
        }

        ///<summary>
        ///Leader or sub-leader update task
        ///</summary>
        [HttpPut]
        [Route("team-leaders")]
        public IActionResult UpdateTaskByTeamLeaders(TaskDTOForUpdating task)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    Guid userId = Guid.Empty;
                    var jwtToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                    var decodedToken = _jwtService.DecodeJwtToken(jwtToken);
                    if (decodedToken != null)
                    {
                        var userIdClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == "Id");
                        if (userIdClaim != null)
                        {
                            userId = Guid.Parse(userIdClaim.Value);
                            // Do something with user ID here
                        }
                        else throw new Exception("Internal server error");
                    }

                    response.Data = _taskService.UpdateTask(task, userId);
                    response.Status = StatusCodes.Status200OK;
                    response.Message = "Update task success.";


                    var rs = _taskService.findByIdIncludeMember(task.Id);
                    string taskLink = _configuration["BaseUrl"] + _configuration["TaskUrlLink"];



                    if (rs != null && rs.AssignedTasks.Count > 0) {
                        var group = _groupService.GetGroupByGuid(rs.GroupId);
                        foreach (var t in rs.AssignedTasks)
                        {
                            
                            var tmp = _memberService.findMemberByMemberId(t.AssignedForId, rs.GroupId);
                            if (tmp.User.Id == userId)
                            {
                                continue;
                            }
                            _emailService.SendEmailNotifyChangeTaskToMember(tmp.User.Email, tmp.User.FullName, rs.Name, group.Name,rs.Description, rs.StartDateDeadline.ToString("dd/MM/yyyy"), rs.EndDateDeadline.ToString("dd/MM/yyyy"), rs.ImportantLevel, rs.Status);
                            string taskUpdateMessage = NotificationMessage.BuildTaskUpdateMessage(task.Name, group.Name);
                            NotificationDTO notificationDTO = new NotificationDTO();
                            notificationDTO.link = taskLink + task.Id;
                            notificationDTO.message = taskUpdateMessage;
                            notificationDTO.Image = group.Avatar;
                            notificationDTO.CreatedDate = DateTime.Now;
                            notificationDTO.UserId = tmp.UserId;
                            notificationDTO.Name = "Notification of " + tmp.User.FullName;
                            notificationDTO.Status = NotificationStatus.NOT_SEEN_YET;
                            notificationDTO.Type = NotificationType.TASK_UPDATE;
                            string notification =  JsonConvert.SerializeObject(notificationDTO);
                         
                            var n =_notificationService.AddNotification(
                             new Notification
                             { Name = "Notification of " + tmp.User.FullName,
                                 Content = notification,
                                 Image = group.Avatar,
                                 Status = NotificationStatus.NOT_SEEN_YET,
                                 Type = NotificationType.TASK_UPDATE,
                                 UserId = tmp.User.Id,
                                 CreatedDate = notificationDTO.CreatedDate

                             }   
                             );
                            notificationDTO.Id = n.Id;
                            notification = JsonConvert.SerializeObject(notificationDTO);
                            _hubContext.Clients.All.SendAsync(tmp.User.Id.ToString(), notification);
                        }
                    }

                    scope.Complete();
                    return new OkObjectResult(response);
                }
            }
            catch (Exception ex)
            {
                response.Status = StatusCodes.Status500InternalServerError; ;
                response.Message = ex.Message;
                return new OkObjectResult(response);
            }
        }

        ///<summary>
        ///Leader, sub-leader or assigned member update task's status as NOT_STARTED_YET ("status": 0), WORKING ("status": 1) or FINISHED ("status": 2)
        ///</summary>
        [HttpPut]
        [Route("team-member")]
        public IActionResult UpdateTaskByTeamMember(TaskDTOForUpdatingStatus task)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    Guid userId = Guid.Empty;
                    var jwtToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                    var decodedToken = _jwtService.DecodeJwtToken(jwtToken);
                    if (decodedToken != null)
                    {
                        var userIdClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == "Id");
                        if (userIdClaim != null)
                        {
                            userId = Guid.Parse(userIdClaim.Value);
                            // Do something with user ID here
                        }
                        else throw new Exception("Internal server error");
                    }

                    response.Data = _taskService.UpdateTaskStatus(task, userId);

                    // send email

                    var rs = _taskService.findByIdIncludeMember(task.Id);
                    string taskLink = _configuration["BaseUrl"] + _configuration["TaskUrlLink"];


                    if (rs != null && rs.AssignedTasks.Count > 0)
                    {
                        var group = _groupService.GetGroupByGuid(rs.GroupId);
                        foreach (var t in rs.AssignedTasks)
                        {
                            var tmp = _memberService.findMemberByMemberId(t.AssignedForId, rs.GroupId);
                            if (tmp.User.Id == userId)
                            {
                                continue;
                            }
                            _emailService.SendEmailNotifyChangeTaskToMember(tmp.User.Email, tmp.User.FullName, rs.Name, group.Name, rs.Description, rs.StartDateDeadline.ToString("dd/MM/yyyy"), rs.EndDateDeadline.ToString("dd/MM/yyyy"), rs.ImportantLevel, rs.Status);
                            string taskUpdateMessage = NotificationMessage.BuildTaskUpdateMessage(rs.Name, group.Name);
                            NotificationDTO notificationDTO = new NotificationDTO();
                            notificationDTO.link = taskLink + task.Id;
                            notificationDTO.message = taskUpdateMessage;
                            notificationDTO.message = taskUpdateMessage;
                            notificationDTO.Image = group.Avatar;
                            notificationDTO.CreatedDate = DateTime.Now;
                            notificationDTO.UserId = tmp.UserId;
                            notificationDTO.Name = "Notification of " + tmp.User.FullName;
                            notificationDTO.Status = NotificationStatus.NOT_SEEN_YET;
                            notificationDTO.Type = NotificationType.TASK_UPDATE;
                            string notification = JsonConvert.SerializeObject(notificationDTO);

                            var n = _notificationService.AddNotification(
                             new Notification
                             {
                                 Name = "Notification of " + tmp.User.FullName,
                                 Content = notification,
                                 Image = group.Avatar,
                                 Status = NotificationStatus.NOT_SEEN_YET,
                                 Type = NotificationType.TASK_UPDATE,
                                 UserId = tmp.User.Id,
                                 CreatedDate = notificationDTO.CreatedDate

                             }
                             );
                            notificationDTO.Id = n.Id;
                            notification = JsonConvert.SerializeObject(notificationDTO);

                            _hubContext.Clients.All.SendAsync(tmp.User.Id.ToString(), notification);

                        }
                    }


                    response.Status = StatusCodes.Status200OK;
                    response.Message = "Update task success.";
                    scope.Complete();
                    return new OkObjectResult(response);
                }
            }
            catch (Exception ex)
            {
                response.Status = StatusCodes.Status500InternalServerError; ;
                response.Message = ex.Message;
                return new OkObjectResult(response);
            }
        }

        ///<summary>
        ///Leader or sub-leader delete task
        ///</summary>
        [HttpDelete]
        public IActionResult DeleteTask(Guid taskId)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    Guid userId = Guid.Empty;
                    var jwtToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                    var decodedToken = _jwtService.DecodeJwtToken(jwtToken);
                    if (decodedToken != null)
                    {
                        var userIdClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == "Id");
                        if (userIdClaim != null)
                        {
                            userId = Guid.Parse(userIdClaim.Value);
                            // Do something with user ID here
                        }
                        else throw new Exception("Internal server error");
                    }
                    var oldTask = _taskService.findByIdIncludeMember(taskId);
                    var leader = _userService.FindUserById(userId);

                    var assignMemeberList = oldTask.AssignedTasks.ToList();
                 

                    int result = _taskService.DeleteTask(taskId, userId);
                    if (result != 0)
                    {
                        response.Status = StatusCodes.Status200OK;
                        response.Message = "Delete task success.";

                        if (assignMemeberList != null && assignMemeberList.Count > 0)
                        {
                            var group = _groupService.GetGroupByGuid(oldTask.GroupId);
                            foreach (var t in assignMemeberList)
                            {
                                var tmp = _memberService.findMemberByMemberId(t.AssignedForId, oldTask.GroupId);
                                if (tmp.User.Id == userId)
                                {
                                    continue;
                                }
                                _emailService.SendEmailNotificationDeleteTask(tmp.User.Email, oldTask.Name,oldTask.Description,leader.FullName);
                                string taskUpdateMessage = NotificationMessage.BuildTaskDeletetedMessage(oldTask.Name, group.Name);
                                NotificationDTO notificationDTO = new NotificationDTO();
                                notificationDTO.link = "";
                                notificationDTO.message = taskUpdateMessage;
                                notificationDTO.Image = group.Avatar;
                                notificationDTO.CreatedDate = DateTime.Now;
                                notificationDTO.UserId = tmp.UserId;
                                notificationDTO.Name = "Notification of " + tmp.User.FullName;
                                notificationDTO.Status = NotificationStatus.NOT_SEEN_YET;
                                notificationDTO.Type = NotificationType.TASK_DELETE;
                                string notification = JsonConvert.SerializeObject(notificationDTO);
                              var n =  _notificationService.AddNotification(
                                                                       new Notification
                                                                       {
                                                                           Name = "Notification of " + tmp.User.FullName,
                                                                           Content = notification,
                                                                           Image = group.Avatar,
                                                                           Status = NotificationStatus.NOT_SEEN_YET,
                                                                           Type = NotificationType.TASK_DELETE,
                                                                           UserId = tmp.User.Id,
                                                                           CreatedDate = DateTime.Now
                                                                       }
                                                                     );
                                notificationDTO.Id = n.Id;
                                notification = JsonConvert.SerializeObject(notificationDTO);
                                _hubContext.Clients.All.SendAsync(tmp.User.Id.ToString(), notification);
                            }
                        }

                        scope.Complete();
                        return new OkObjectResult(response);
                    }
                    else throw new Exception("Delete task not success");
                }
                    
            }
            catch (Exception ex)
            {
                response.Status = StatusCodes.Status500InternalServerError; ;
                response.Message = ex.Message;
                return new OkObjectResult(response);
            }
        }
    }
}
