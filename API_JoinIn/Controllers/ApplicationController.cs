using API_JoinIn.Utils.Email;
using API_JoinIn.Utils.Email.Impl;
using API_JoinIn.Utils.Notification;
using API_JoinIn.Utils.Notification.Implements;
using BusinessObject.DTOs;
using BusinessObject.DTOs.Common;
using BusinessObject.DTOs.Email;
using BusinessObject.Enums;
using BusinessObject.Models;
using DataAccess.Security;
using DataAccess.Services;
using DataAccess.Services.Implements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;
using System.Xml.Linq;

namespace API_JoinIn.Controllers
{
    [ApiController]
    [Route("applications")]
    public class ApplicationController : ControllerBase
    {
        private readonly IApplicationService _applicationService;
        private readonly IJwtService _jwtService;
        private readonly IEmailService emailService;
        private readonly IUserService userService;
        private readonly IConfiguration configuration;
        private readonly IMemberService memberService;
        private readonly IGroupService groupService;
        private readonly IMajorService majorService;
        private readonly IHubContext<NotificationSignalSender> _hubContext;
        private readonly INotificationService _notificationService;

        public ApplicationController(IApplicationService applicationService,
                                    IJwtService jwtService,
                                    IEmailService emailService,
                                    IUserService userService,
                                    IConfiguration configuration,
                                    IMemberService memberService,
                                    IGroupService groupService,
                                    IMajorService majorService,
                                    IHubContext<NotificationSignalSender> hubContext,
                                    INotificationService notificationService

            )
        {
            _applicationService = applicationService;
            _jwtService = jwtService;
            this.emailService = emailService;
            this.userService = userService;
            this.configuration = configuration;           
            this.memberService = memberService;
            this.groupService = groupService;
            this.majorService = majorService;
            this._notificationService= notificationService;
            this._hubContext = hubContext;
        }

        ///<summary>
        ///User send application to a group
        ///</summary>
        [HttpPost]
        [Authorize]
        [Route("send-application")]
        public   IActionResult SendApplication(SentApplicationDTO sentApplicationDTO)
        {
            CommonResponse response = new CommonResponse();
            try
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
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    Guid newApplicationId = _applicationService.CreateApplication(userId, sentApplicationDTO);
                    if (newApplicationId != Guid.Empty)
                    {
                        response.Data = newApplicationId;
                        response.Message = "Create application success";
                        response.Status = StatusCodes.Status200OK;


                        string applicationLink = configuration["BaseUrl"] + configuration["ApplicationUrlLink"];
                        var user = userService.FindUserById(userId);// lấy user đã tạo appication
                        var leader = memberService.GetGroupLeader(sentApplicationDTO.GroupId);
                        var group = groupService.GetGroupByGuid(sentApplicationDTO.GroupId);
                        emailService.SendEmailNotificationWhenHasNewApplication(leader.User.Email, user.FullName, leader.User.FullName, group.Name, user.Email);
                        string applicationMessage = NotificationMessage.BuildNewApplicationMessage(user.FullName, group.Name);
                        NotificationDTO notificationDTO = new NotificationDTO();
                        notificationDTO.link = applicationLink + newApplicationId; 
                        notificationDTO.message = applicationMessage;
                        notificationDTO.CreatedDate = DateTime.Now;
                        notificationDTO.UserId =  leader.UserId;
                        notificationDTO.Image = group.Avatar == null ? "" : group.Avatar;
                        notificationDTO.Status = NotificationStatus.NOT_SEEN_YET;
                        notificationDTO.Type = NotificationType.NEW_APPLY;
                        notificationDTO.Name = "Notification of " + leader.User.FullName;


                        string notification = JsonConvert.SerializeObject(notificationDTO);
                       var n =  _notificationService.AddNotification(
                         new Notification
                         {
                             Name = "Notification of " + leader.User.FullName,
                             Content = notification,
                             Image = notificationDTO.Image,
                             Status = notificationDTO.Status,
                             Type = notificationDTO.Type,
                             UserId = notificationDTO.UserId,
                             CreatedDate = DateTime.Now,

                         }
                         );
                        notificationDTO.Id = n.Id;
                        notification = JsonConvert.SerializeObject(notificationDTO);
                        _hubContext.Clients.All.SendAsync(leader.UserId.ToString(), notification);

                        scope.Complete();
                    }
                         
                    else throw new Exception("Create application failed");
                  

                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Status = StatusCodes.Status500InternalServerError;
                return Ok(response);
            }
        }

        ///<summary>
        ///Group's leader confirm sent application of user as APPROVED ("status": 1) or DISAPPROVED ("status": 2)
        ///</summary>
        [HttpPut]
        [Authorize]
        [Route("confirm-application")]
        public IActionResult ConfirmApplication(ConfirmedApplicationDTO confirmedApplicationDTO)
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
                    Guid? applicationId = _applicationService.ConfirmApplication(userId, confirmedApplicationDTO);
                    if (applicationId != null) {
                        response.Data = applicationId;
                        response.Message = "Confirm application success";
                        response.Status = StatusCodes.Status200OK;

                        var tmp = _applicationService.GetApplication((Guid)applicationId);
                        var user = userService.FindUserById(tmp.UserId); // get user in appilication
                        var leader = userService.FindUserById(userId);


                        NotificationDTO notificationDTO = new NotificationDTO();
                        notificationDTO.link = "";
                        string notification = "";

                        switch (tmp.Status)
                        {
                            case ApplicationStatus.APPROVED:
                                string ApproveMessage = NotificationMessage.BuildApproveApplication(tmp.Group.Name);
                               
                                notificationDTO.message = ApproveMessage;
                                notificationDTO.CreatedDate = DateTime.Now;
                                notificationDTO.UserId = tmp.User.Id;
                                notificationDTO.Image = tmp.Group.Avatar == null ? "" : tmp.Group.Avatar;
                                notificationDTO.Status = NotificationStatus.NOT_SEEN_YET;
                                notificationDTO.Type = NotificationType.APPROVE_APPLICATION;
                                notificationDTO.CreatedDate = tmp.CreatedDate;
                                notificationDTO.Name = "Notification of " + tmp.User.FullName;

                                notification = JsonConvert.SerializeObject(notificationDTO);

                                var approveNotification = _notificationService.AddNotification(
                                 new Notification
                                 {
                                     Name = "Notification of " + tmp.User.FullName,
                                     Content = notification,
                                     Image = notificationDTO.Image,
                                     Status = notificationDTO.Status,
                                     Type = notificationDTO.Type,
                                     UserId = notificationDTO.UserId,
                                     CreatedDate = notificationDTO.CreatedDate
                                 }
                                 );
                                notificationDTO.Id = approveNotification.Id;
                                notification = JsonConvert.SerializeObject(notificationDTO);
                              
                                 _hubContext.Clients.All.SendAsync(tmp.User.Id.ToString(), notification);
                                emailService.SendEmailNotificationToJoinGroup(user.Email, user.FullName, leader.FullName, tmp.Group.Name, true);
                                
                                
                                break;
                            case ApplicationStatus.DISAPPROVED:
                                string DisApproveMessage = NotificationMessage.BuildDisApproveApplication(tmp.Group.Name);

                                notificationDTO.CreatedDate = DateTime.Now;
                                notificationDTO.UserId = tmp.User.Id;
                                notificationDTO.Image = tmp.Group.Avatar == null ? "" : tmp.Group.Avatar;
                                notificationDTO.Status = NotificationStatus.NOT_SEEN_YET;
                                notificationDTO.Type = NotificationType.DISAPPROVE_APPLICATION;
                                notificationDTO.CreatedDate = DateTime.Now;
                                notificationDTO.Name = "Notification of " + tmp.User.FullName;

                                notification = JsonConvert.SerializeObject(notificationDTO);

                                var DisapproveNotification = _notificationService.AddNotification(
                                 new Notification
                                 {
                                     Name = "Notification of " + tmp.User.FullName,
                                     Content = notification,
                                     Image = notificationDTO.Image,
                                     Status = notificationDTO.Status,
                                     Type = notificationDTO.Type,
                                     UserId = notificationDTO.UserId,
                                     CreatedDate = notificationDTO.CreatedDate
                                 }
                                 );
                                notificationDTO.Id = DisapproveNotification.Id;
                                notification = JsonConvert.SerializeObject(notificationDTO);
                                var disapproveNotificationObject = JsonConvert.SerializeObject(notification);
                                _hubContext.Clients.All.SendAsync(tmp.User.Id.ToString(), disapproveNotificationObject);
                                emailService.SendEmailNotificationToJoinGroup(user.Email, user.FullName, leader.FullName, tmp.Group.Name, false);
                                break;

                        }
                        scope.Complete();
                        return Ok(response);


                    }
                    else throw new Exception("Confirm application failed");


                   
                    
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Status = StatusCodes.Status500InternalServerError;
                return Ok(response);
            }
        }

        ///<summary>
        ///Group's leader send invitations to users
        ///</summary>
        [HttpPost]
        [Authorize]
        [Route("send-invitation")]
        public async Task<IActionResult> SendInvitation(SentInvitationsDTO sentInvitationsDTO)
        {
            CommonResponse response = new CommonResponse();
            Guid userId = Guid.Empty;
            try
            {
                
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
                var sender = userService.FindUserByGuid(userId).Result;
              
              
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {


                    List<InvitedMemberDTO> listInvitedEmail  = await _applicationService.CreateInvitations(userId, sentInvitationsDTO);
                    if (listInvitedEmail.Count == sentInvitationsDTO.UserIds.Count)
                    {
                        await emailService.SendEmailInovationToListUser(listInvitedEmail);
                       
                        foreach (var tmp in listInvitedEmail)
                        {
                            string applicationMessage = NotificationMessage.BuildNewInvitationMessage(tmp.groupName);
                            NotificationDTO notificationDTO = new NotificationDTO();
                            notificationDTO.link = tmp.inovationLink;
                            notificationDTO.message = applicationMessage;
                            notificationDTO.UserId = tmp.userId;
                            notificationDTO.Status = NotificationStatus.NOT_SEEN_YET;
                            notificationDTO.Type = NotificationType.NEW_INVITE;
                            notificationDTO.Image = tmp.groupLogo == null ? "" : tmp.groupLogo;
                            notificationDTO.Name = "Notification of " + tmp.senderName;
                            notificationDTO.CreatedDate = DateTime.Now;
                            string notification = JsonConvert.SerializeObject(notificationDTO);
                       
                           var n = _notificationService.AddNotification(
                             new Notification
                             {
                                 Name = "Notification of " +  tmp.senderName,
                                 Content = notification,
                                 Image = tmp.groupLogo == null ? "" : tmp.groupLogo,
                                 Status = NotificationStatus.NOT_SEEN_YET,
                                 Type = NotificationType.NEW_INVITE,
                                 UserId = tmp.userId,
                                 CreatedDate = notificationDTO.CreatedDate
                              
                             }
                             );
                            notificationDTO.Id = n.Id;
                            notification = JsonConvert.SerializeObject(notificationDTO);

                            await _hubContext.Clients.All.SendAsync(tmp.userId.ToString(), notification);
                        }
                        scope.Complete();
                    }
                    
                    else throw new Exception("Create invitations failed");

                    response.Message = "Create invitations success";
                    response.Status = StatusCodes.Status200OK;
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Status = StatusCodes.Status500InternalServerError;
                return Ok(response);
            }
        }

        ///<summary>
        ///User confirm sent invitation of group as APPROVED ("status": 1) or DISAPPROVED ("status": 2)
        ///</summary>
        [HttpGet]
        [Route("confirm-invitation")]
        public IActionResult ConfirmInvitation(Guid invitationId, ApplicationStatus status)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    Guid? applicationId = _applicationService.ConfirmInvitation(new ConfirmedApplicationDTO
                    {
                        ApplicationId = invitationId,
                        Status = status
                    });
                    if (applicationId != null) scope.Complete();
                    else throw new Exception("Confirm invitation failed");
                    response.Data = applicationId;
                    response.Message = "Confirm invitation success";
                    response.Status = StatusCodes.Status200OK;
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Status = StatusCodes.Status500InternalServerError;
                return Ok(response);
            }
        }

        ///<summary>
        ///Group's leader search sent applications of users by user's name and applied majors
        ///</summary>
        ///<param name="majorIdsString">The string of major's Ids separated by ","</param>
        [HttpGet]
        public IActionResult FilterApplications(Guid groupId, string? name, string? majorIdsString, int? pageSize, int? page, string? orderBy, string? value)
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
                List<Guid>? majorIds = majorIdsString?.Split(',').Select(a => Guid.Parse(a)).ToList();
                CommonResponse commonResponse = _applicationService.FilterApplications(userId, groupId, majorIds, name, pageSize, page, orderBy, value);
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
        ///Invited user get invitaion's information before confirm
        ///</summary>
        [HttpGet]
        [Route("{applicationId}")]
        public IActionResult GetApplicationsById(Guid applicationId)
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
            CommonResponse response = new CommonResponse();
            try
            {
                response.Data = _applicationService.GetInvitationDetailByIdAndUserId(applicationId, userId);
                response.Status = StatusCodes.Status200OK;
                response.Message = "Get application's detail success.";
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
