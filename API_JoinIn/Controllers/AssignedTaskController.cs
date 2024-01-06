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
using System.Transactions;

namespace API_JoinIn.Controllers
{
    [ApiController]
    [Route("assigned-tasks")]
    public class AssignedTaskController : ControllerBase
    {
        private readonly IAssignedTaskService _assignedTaskService;
        private readonly IJwtService _jwtService;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        private readonly ITaskService _taskService;
        private readonly IGroupService _groupService;
        private readonly IMemberService _memberService;
        private readonly INotificationService _notificationService;
        private readonly IHubContext<NotificationSignalSender> _hubContext;
        private readonly IConfiguration _configuration;

        public AssignedTaskController(IAssignedTaskService assignedTaskService,
                                        IJwtService jwtService,
                                        IEmailService emailService,
                                        IUserService userService,
                                        ITaskService taskService,
                                        IGroupService groupService, 
                                        IMemberService memberService,
                                        INotificationService notificationService,
                                        IHubContext<NotificationSignalSender> hubContext,
                                        IConfiguration configuration
            )
        {
            _assignedTaskService = assignedTaskService;
            _jwtService = jwtService;
            _userService = userService;
            _emailService = emailService;
            _taskService = taskService;
            _groupService = groupService;
            _memberService = memberService;
            _notificationService = notificationService;
            _hubContext = hubContext;
            _configuration = configuration;
        }

        ///<summary>
        ///Group's leader or sub-leader assign task to members
        ///</summary>
        [HttpPut]
        public  IActionResult AssignTaskByTeamLeaders(AssignedTasksDTO assignedTasksDTO)
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

                    response.Data = _assignedTaskService.AssignTask(userId, assignedTasksDTO);

                    string taskLink = _configuration["BaseUrl"] + _configuration["TaskUrlLink"];
                    var task = _taskService.findById(assignedTasksDTO.TaskId);
                    var group = _groupService.GetGroupByGuid(task.GroupId);
                    var memberList = assignedTasksDTO.AssignedForIds;
                    foreach (var t in memberList)
                    {
                        var tmp = _memberService.findMemberByMemberId(t,task.GroupId);
                       _emailService.SendEmailNotifyAssignTaskToMember(tmp.User.Email,tmp.User.FullName,task.Name,group.Name,task.Description, task.StartDateDeadline.ToString("dd/MM/yyyy"), task.EndDateDeadline.ToString("dd/MM/yyyy"), task.ImportantLevel, task.Status);
                        string taskAssignMessage = NotificationMessage.BuildTaskAssignationMessage(task.Name, group.Name);
                        NotificationDTO notificationDTO = new NotificationDTO();
                        notificationDTO.link = taskLink + task.Id;
                        notificationDTO.message = taskAssignMessage;
                        notificationDTO.Image = group.Avatar;
                        notificationDTO.CreatedDate = DateTime.Now;
                        notificationDTO.UserId = tmp.UserId;
                        notificationDTO.Name = "Notification of " + tmp.User.FullName;
                        notificationDTO.Status = NotificationStatus.NOT_SEEN_YET;
                        notificationDTO.Type = NotificationType.TASK_ASSIGN;




                        string notification = JsonConvert.SerializeObject(notificationDTO);
                      
                        var n = _notificationService.AddNotification(
                         new Notification
                         {
                             Name = "Notification of " + tmp.User.FullName,
                             Content = notification,
                             Image = group.Avatar,
                             Status = NotificationStatus.NOT_SEEN_YET,
                             Type = NotificationType.TASK_ASSIGN,
                             UserId = tmp.User.Id,
                             CreatedDate = notificationDTO.CreatedDate
                         }
                         );
                        notificationDTO.Id = n.Id;
                        notification = JsonConvert.SerializeObject(notificationDTO);

                        
                        _hubContext.Clients.All.SendAsync(tmp.User.Id.ToString(), notification);

                    }


                    response.Status = StatusCodes.Status200OK;
                    response.Message = "Assign task success.";
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
    }
}
