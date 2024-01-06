using API_JoinIn.Utils.Notification;
using API_JoinIn.Utils.Notification.Implements;
using BusinessObject.DTOs.Common;
using BusinessObject.DTOs.User;
using BusinessObject.Enums;
using BusinessObject.Models;
using DataAccess.Repositories;
using DataAccess.Security;
using DataAccess.Services;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace API_JoinIn.Controllers
{
    [ApiController]
    [Route("notifications")]
    public class NotificationController : ControllerBase
    {
        private readonly Microsoft.AspNetCore.SignalR.IHubContext<NotificationSignalSender> _hubContext;
        private readonly INotificationService _notificationService;
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;

        public NotificationController(Microsoft.AspNetCore.SignalR.IHubContext<NotificationSignalSender> hubContext,
                                        INotificationService notificationService,
                                        IUserService userService,
                                        IJwtService jwtService)
        {
            _hubContext = hubContext;
            this._notificationService = notificationService;
            this._userService = userService;
            this._jwtService = jwtService;
        }
        ///<summary>
        ///Get notification of current user
        ///</summary>
        
        [HttpGet]
       // [Authorize]
        public IActionResult FilterNotification(DateTime StartDate,
                                               NotificationType notificationType,
                                               NotificationStatus notificationStatus,
                                               int pageSize = 5,
                                               int pageNumber = 1)
        {
         
           CommonResponse commonResponse = new CommonResponse();   
           try
            {
                var userId = "";
                var jwtToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                var decodedToken = _jwtService.DecodeJwtToken(jwtToken);
                if (decodedToken != null)
                {
                    var userIdClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == "Id");
                    if (userIdClaim != null)
                    {
                        userId = userIdClaim.Value;
                        // lấy user đang đăng nhập
                        // Do something with user ID here

                    }
                    else throw new Exception("Internal Server Error.");
                }


                commonResponse.Status = 200;             
                commonResponse = _notificationService.FilterNotification(StartDate, Guid.Parse(userId), notificationType, notificationStatus, pageSize, pageNumber);
                return StatusCode(200, commonResponse);
            }
            catch
            {
                commonResponse.Status = 500;
                commonResponse.Message = "Internal Server Error";
                return StatusCode(500, commonResponse);

            }
        }

        [HttpPut]
        [Authorize]
        public IActionResult UpdateNotificationStatus(NotificationUpdateDTO notificationUpdateDTO)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                commonResponse = _notificationService.UpdateNotificationStatus(notificationUpdateDTO);
                return Ok(commonResponse);
            } catch
            {
                return StatusCode(500, commonResponse);
            }
        }

       
       
       

    }
}
