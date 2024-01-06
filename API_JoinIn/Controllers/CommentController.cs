using API_JoinIn.Utils.Notification;
using API_JoinIn.Utils.Notification.Implements;
using BusinessObject.DTOs;
using BusinessObject.DTOs.Common;
using BusinessObject.Enums;
using BusinessObject.Models;
using DataAccess.Security;
using DataAccess.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace API_JoinIn.Controllers
{
    [ApiController]
    [Route("comments")]
    public class CommentController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly ICommentService _commentService;
        private readonly IHubContext<NotificationSignalSender> _hubContext;
        private readonly IConfiguration _configuration;
        private readonly INotificationService _notificationService;
        private readonly IGroupService _groupService;
        private readonly ITaskService _taskService;



        public CommentController(IJwtService jwtService,
                                 ICommentService commentService,
                                 IHubContext<NotificationSignalSender> hubContext,
                                 IConfiguration configuration,
                                 INotificationService notificationService,
                                 IGroupService groupService,
                                 ITaskService taskService)
        {
            _jwtService = jwtService;
            _commentService = commentService;
            _hubContext = hubContext;
            _configuration = configuration;
            _notificationService = notificationService;
            _groupService = groupService;
            _taskService = taskService;
        }
        ///<summary>
        ///Get all Comment in task
        ///</summary>
        [HttpGet]
        public IActionResult GetComments(Guid taskID)
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
                response.Status = StatusCodes.Status200OK;
                response.Message = "Get Success";
                response.Data = _commentService.GetComments(taskID);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                response.Status = StatusCodes.Status500InternalServerError;
                response.Message = ex.Message;
                return new OkObjectResult(response);
            }
        }

        [HttpPost]
        public IActionResult CreateComment(CommentDTOForCreating comment)
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
                response.Status = StatusCodes.Status200OK;
                response.Message = "Create Success";
                response.Data = _commentService.CreateComment(comment, userId);
                var task = _taskService.GetDetailById(comment.TaskId,userId);
                var listUser = task.AssignedFor;
                
                foreach (var u in listUser) {
                    if(u.Id == userId)
                    {
                        continue;
                    }
                    string groupLeavingMessage = NotificationMessage.BuildNewTaskCommentMessage(task.Name,task.Group.Name);
                    NotificationDTO notificationDTO = new NotificationDTO();
                    notificationDTO.message = groupLeavingMessage;
                    notificationDTO.CreatedDate = DateTime.Now;
                    notificationDTO.UserId = u.Id;
                    notificationDTO.Status = NotificationStatus.NOT_SEEN_YET;
                    notificationDTO.Type = NotificationType.NEW_TASK_COMMENT;
                    notificationDTO.Image = _configuration["NotificationImgLink"];
                    notificationDTO.Name = "Notification of " + u.FullName;

                    string notification = JsonConvert.SerializeObject(notificationDTO);
                   
                   var n =  _notificationService.AddNotification(
                     new Notification
                     {
                         Name = "Notification of " + u.FullName,
                         Content = notification,
                         Image = notificationDTO.Image,
                         Status = NotificationStatus.NOT_SEEN_YET,
                         Type = NotificationType.NEW_TASK_COMMENT,
                         UserId = notificationDTO.UserId
                     }
                     );
                    notificationDTO.Id = n.Id;
                    notification = JsonConvert.SerializeObject(notificationDTO);
                    _hubContext.Clients.All.SendAsync(u.Id.ToString(), notification);

                }
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                response.Status = StatusCodes.Status500InternalServerError;
                response.Message = ex.Message;
                return new OkObjectResult(response);
            }
        }

        [HttpDelete]
        public IActionResult DeleteComment(Guid commentID, Guid memberID)
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
                response.Status = StatusCodes.Status200OK;
                response.Message = "Delete Success";
                response.Data = _commentService.DeleteComment(commentID, memberID);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                response.Status = StatusCodes.Status500InternalServerError;
                response.Message = ex.Message;
                return new OkObjectResult(response);
            }
        }
    }
}
