using API_JoinIn.Utils.Notification;
using API_JoinIn.Utils.Notification.Implements;
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
using Org.BouncyCastle.Asn1.Ocsp;
using System.Transactions;
using System.Xml.Linq;

namespace API_JoinIn.Controllers
{
    [ApiController]
    [Route("feedbacks")]
    [Authorize]
    public class FeedbackController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly IFeedbackService _feedbackService;
        private readonly IHubContext<NotificationSignalSender> _hubContext;
        private readonly IConfiguration _configuration;
        private readonly INotificationService _notificationService;
        private readonly IGroupService _groupService;
        private readonly IMemberService _memberService;


        public FeedbackController(IJwtService jwtService,
                                IFeedbackService feedbackService,
                                IHubContext<NotificationSignalSender> hubContext,
                                IConfiguration configuration,
                                INotificationService notificationService,
                                IGroupService groupService,
                                IMemberService memberService)
        {
            _jwtService = jwtService;
            _feedbackService = feedbackService;
            _hubContext = hubContext;
            _configuration = configuration;
            _notificationService = notificationService;
            _groupService = groupService;
            _memberService= memberService;
        }

        ///<summary>
        ///Member send feedback to leader or leader send feedback to members
        ///</summary>
        [HttpPost]
        public IActionResult FeedbackMember(SentFeedbackDTO sentFeedbackDTO)
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
                        }
                        else throw new Exception("Internal server error");
                    }

                    response.Data = _feedbackService.CreateFeedback(userId, sentFeedbackDTO);
                    response.Status = StatusCodes.Status200OK; ;
                    response.Message = "Feedback success.";
                    var member = _memberService.findMemberByMemberId(sentFeedbackDTO.MemberId, sentFeedbackDTO.GroupId);
                    string FeedbackMessage = NotificationMessage.BuildNewFeedbackMessage(sentFeedbackDTO.GroupId.ToString());
                    NotificationDTO notificationDTO = new NotificationDTO();
                    notificationDTO.message = FeedbackMessage;
                    notificationDTO.CreatedDate = DateTime.Now;
                    notificationDTO.UserId = member.User.Id;
                    notificationDTO.Image = _configuration["NotificationImgLink"];
                    notificationDTO.Status = NotificationStatus.NOT_SEEN_YET;
                    notificationDTO.Type = NotificationType.NEW_FEEDBACK;
                    notificationDTO.Name = "Notification of " + member.User.FullName;
                    string notification = JsonConvert.SerializeObject(notificationDTO);
               
                    var n =_notificationService.AddNotification(
                     new Notification
                     {
                         Name = "Notification of " + member.User.FullName,
                         Content = notification,
                         Image = _configuration["NotificationImgLink"],
                         Status = NotificationStatus.NOT_SEEN_YET,
                         Type = NotificationType.NEW_FEEDBACK,
                         UserId = member.User.Id,
                         CreatedDate = DateTime.Now
                     }
                     );
                    notificationDTO.Id = n.Id;
                     notification = JsonConvert.SerializeObject(notificationDTO);
                    _hubContext.Clients.All.SendAsync(member.UserId.ToString(), notification);
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
        ///User get feedback list of the choosed user
        ///</summary>
        ///<param name="userId">Leave null to get feedback list of currently logged user</param>
        [HttpGet]
        public IActionResult FilterFeedback(Guid? userId, int? pageSize, int? page, string? orderBy, string? value)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                Guid currentLoggeduserId = Guid.Empty;
                var jwtToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                var decodedToken = _jwtService.DecodeJwtToken(jwtToken);
                if (decodedToken != null)
                {
                    var userIdClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == "Id");
                    if (userIdClaim != null)
                    {
                        currentLoggeduserId = Guid.Parse(userIdClaim.Value);
                    }
                    else throw new Exception("Internal server error");
                }

                if (userId == null)
                    userId = currentLoggeduserId;
                CommonResponse commonResponse = _feedbackService.FilterFeedbacks(userId.Value, pageSize, page, orderBy, value);
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
        ///User get average rating from feedbacks of the choosed user
        ///</summary>
        ///<param name="userId">Leave null to get average rating from feedbacks of currently logged user</param>
        [HttpGet]
        [Route("average-rating")]
        public IActionResult GetAverageRating(Guid? userId)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                Guid currentLoggeduserId = Guid.Empty;
                var jwtToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                var decodedToken = _jwtService.DecodeJwtToken(jwtToken);
                if (decodedToken != null)
                {
                    var userIdClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == "Id");
                    if (userIdClaim != null)
                    {
                        currentLoggeduserId = Guid.Parse(userIdClaim.Value);
                    }
                    else throw new Exception("Internal server error");
                }

                if (userId == null)
                    userId = currentLoggeduserId;

                response.Data = _feedbackService.GetAverageRatingByUserId(userId.Value);
                response.Status = StatusCodes.Status200OK;
                response.Message = "Get average rating success";
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
