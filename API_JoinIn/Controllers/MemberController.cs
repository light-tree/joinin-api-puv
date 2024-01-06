using API_JoinIn.Utils.Email;
using API_JoinIn.Utils.Notification;
using API_JoinIn.Utils.Notification.Implements;
using BusinessObject.DTOs;
using BusinessObject.DTOs.Common;
using BusinessObject.Enums;
using BusinessObject.Models;
using DataAccess.Repositories;
using DataAccess.Security;
using DataAccess.Services;
using DataAccess.Services.Implements;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Diagnostics.Metrics;
using System.Threading.Tasks;
using System.Transactions;
using System.Xml.Linq;

namespace API_JoinIn.Controllers
{
    [ApiController]
    [Authorize]
    [Route("members")]
    public class MemberController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly IMemberService _memberService;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        private readonly IGroupRepository _groupRepository;
        private readonly IHubContext<NotificationSignalSender> _hubContext;
        private readonly IConfiguration _configuration;
        private readonly INotificationService _notificationService;


        public MemberController(IJwtService jwtService,
                                IMemberService memberService,
                                IUserService userService,
                                IEmailService emailService,
                                IGroupRepository groupRepository,
                                IHubContext<NotificationSignalSender> hubContext,
                                IConfiguration configuration,
                                INotificationService notificationService)
        {
            _jwtService = jwtService;
            _memberService = memberService;
            _userService = userService;
            _emailService = emailService;
            _groupRepository = groupRepository;
            _configuration= configuration;
            _hubContext= hubContext;
            _notificationService= notificationService;  
        }

        //[HttpGet]
        //public IActionResult FilterMembers(Guid groupId)
        //{
        //    CommonResponse response = new CommonResponse();
        //    try
        //    {
        //        Guid userId = Guid.Empty;
        //        var jwtToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        //        var decodedToken = _jwtService.DecodeJwtToken(jwtToken);
        //        if (decodedToken != null)
        //        {
        //            var userIdClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == "Id");
        //            if (userIdClaim != null)
        //            {
        //                userId = Guid.Parse(userIdClaim.Value);
        //                // Do something with user ID here
        //            }
        //            else throw new Exception("Internal server error");
        //        }

        //        CommonResponse commonResponse = _memberService.FilterMembers(userId, groupId, pageSize, page, orderBy, value);
        //        return new OkObjectResult(commonResponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Message = ex.Message;
        //        response.Status = StatusCodes.Status500InternalServerError;
        //        return Ok(response);
        //    }
        //}

        ///<summary>
        ///Leader assign role for group's member
        ///</summary>
        [HttpPut("assign-role")]
        public  ActionResult AssignRoleToMember(RoleAssignDTO roleAssign) {

            var userId = "";
            CommonResponse commonResponse = new CommonResponse();
            try
            {
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
                    // check user login is the member is the leader
                    

                    else throw new Exception("Internal Server Error.");
                }

                var user = _memberService.findMemberByUserId(Guid.Parse(userId), roleAssign.GroupId);
               // check xem user đăng nhập có trong group không
                if(user == null)
                {
                    commonResponse.Status = 400;
                    commonResponse.Message = "You are not in group.";
                    return BadRequest(commonResponse);
                }
                // check xem user đang đăng nhập có phải leader của nhóm đó hay ko
                if (user.Role != MemberRole.LEADER)
                {
                    commonResponse.Status = 400;
                    commonResponse.Message = "Only leader in group can do this action.";
                    return BadRequest(commonResponse);
                }
                // check xem member được assign có ở trong group hay không
                var member = _memberService.findMemberByMemberId(roleAssign.MemberId, roleAssign.GroupId);
                if (member == null)
                {
                    commonResponse.Status = 400;
                    commonResponse.Message = "Member is not in this group.";
                    return BadRequest(commonResponse);
                }

                // check trường hợp leader tự gán quyền cho bản thân
                if (roleAssign.MemberId == user.Id)
                {
                    commonResponse.Status = 400;
                    commonResponse.Message = "You do not allow to assign role to yourselve.";
                    return BadRequest(commonResponse);
                }

                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                   var rs =  _memberService.updateMemberRole(roleAssign.MemberId,roleAssign.Role);

                   
                    if(rs == null) { throw new Exception(); }

                    // gán role leader cũ là member nếu role được truyền xuống là leader
                    if (rs.Role == MemberRole.LEADER)
                    {
                       
                         _memberService.updateMemberRole(user.Id,MemberRole.MEMBER);
                    }
                    var group = _groupRepository.FindById(roleAssign.GroupId);
                    string assignRoleMessage = NotificationMessage.BuildAssignedRoleToMember(roleAssign.Role.ToString(),group.Name);
                    NotificationDTO notificationDTO = new NotificationDTO();
                    notificationDTO.message = assignRoleMessage;
                    notificationDTO.UserId = rs.UserId;
                    notificationDTO.CreatedDate = DateTime.Now;
                    notificationDTO.Image = group.Avatar;
                    notificationDTO.Status = NotificationStatus.NOT_SEEN_YET;
                    notificationDTO.Type = NotificationType.ASSIGN_ROLE;
                    notificationDTO.Name = "Notification of " + rs.User.FullName;
                    string notification = JsonConvert.SerializeObject(notificationDTO);

                    var n = _notificationService.AddNotification(
                      new Notification
                      {
                          Name = "Notification of " + rs.User.FullName,
                          Content = notification,
                          Image = group.Avatar,
                          Status = NotificationStatus.NOT_SEEN_YET,
                          Type = NotificationType.ASSIGN_ROLE,
                          UserId = rs.UserId,
                          CreatedDate = notificationDTO.CreatedDate
                      }
                      );
                    notificationDTO.Id = n.Id;

                    notification = JsonConvert.SerializeObject(notificationDTO);
                    _hubContext.Clients.All.SendAsync(rs.UserId.ToString(), notification);
                    scope.Complete();
                }
                commonResponse.Message = "Change role successfully.";
                commonResponse.Status = 200;
                return StatusCode(200, commonResponse);


            } catch(Exception ex) {
                commonResponse.Message = "Internal server error.";
                commonResponse.Status = 500;
                return StatusCode(500, commonResponse);
            }
        }

        ///<summary>
        ///Get members' information of group
        ///</summary>
        [HttpGet]
        [Route("{groupId}")]
        public IActionResult GetMembersByGroupId(Guid groupId, string? name)
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
                response.Data = _memberService.GetMembersByGroupId(groupId, name, userId);
                response.Status = StatusCodes.Status200OK; ;
                response.Message = "Get group's members success.";
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                response.Status = StatusCodes.Status500InternalServerError;
                response.Message = ex.Message;
                return new OkObjectResult(response);
            }
        }


        ///<summary>
        ///Leader remove member from group
        ///</summary>  
        [HttpPut("leader/member")]
        public async Task<IActionResult> RemoveMember(RemoveMemberDTO removeMemberDTO)
        {
            var userId = "";
            CommonResponse commonResponse = new CommonResponse();
            try
            {
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
                    // check user login is the member is the leader


                    else throw new Exception("Internal Server Error.");
                }
              

                var leader = _memberService.findMemberByUserId(Guid.Parse(userId), removeMemberDTO.groupId);
                // check xem user đăng nhập có trong group không
                if (leader == null)
                {
                    commonResponse.Status = 400;
                    commonResponse.Message = "You are not in group.";
                    return BadRequest(commonResponse);
                }
                // check xem user đang đăng nhập có phải leader của nhóm đó hay ko
                if (leader.Role != MemberRole.LEADER)
                {
                    commonResponse.Status = 400;
                    commonResponse.Message = "Only leader in group can do this action.";
                    return BadRequest(commonResponse);
                }
                // check xem member bị xóa có ở trong group
                var member = _memberService.findMemberByMemberId(removeMemberDTO.MemberId, removeMemberDTO.groupId);
                if (member == null)
                {
                    commonResponse.Status = 400;
                    commonResponse.Message = "Member is not in this group.";
                    return BadRequest(commonResponse);
                }

                // check trường hợp leader tự remove  bản thân
                if (removeMemberDTO.MemberId == leader.Id)
                {
                    commonResponse.Status = 400;
                    commonResponse.Message = "You do not allow to remove yourselve.";
                    return BadRequest(commonResponse);
                }

                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var rs = _memberService.removeMemberFromGroup(member);
                    if (rs == null) { throw new Exception(); }
                

                // gửi email 
                var memberList = _memberService.GetMembersByGroupId(removeMemberDTO.groupId,null,Guid.Parse(userId));
                var group = _groupRepository.FindById(removeMemberDTO.groupId);
                if(memberList != null && memberList.Count > 0)
                    {
                        foreach(var tmp in memberList)
                        {
                            await _emailService.SendEmailNotificationWhenMemberOutGroup(tmp.User.Email,member.User.FullName,leader.User.FullName, group.Name);
                        }
                    }
                   
;                    await  _emailService.SendEmailNotificationRemoveMemberFromGroup(member.User.Email, member.User.FullName, leader.User.FullName,group.Name,removeMemberDTO.Description,leader.User.Email);
                    string groupLeavingMessage = NotificationMessage.BuildRemoveMemberMessage(group.Name);
                    NotificationDTO notificationDTO = new NotificationDTO();
                    notificationDTO.message = groupLeavingMessage;
                    notificationDTO.CreatedDate = DateTime.Now;
                    notificationDTO.UserId = member.UserId;
                    notificationDTO.Status = NotificationStatus.NOT_SEEN_YET;
                    notificationDTO.Type = NotificationType.MEMBER_REMOVING;
                    notificationDTO.link = "";
                    notificationDTO.Image = group.Avatar;
                    notificationDTO.Name = "Notification of " + member.User.FullName;
                    string notification = JsonConvert.SerializeObject(notificationDTO);
                     
                    var n =_notificationService.AddNotification(
                     new Notification
                     {
                         Name = "Notification of " + member.User.FullName,
                         Content = notification,
                         Image = group.Avatar,
                         Status = NotificationStatus.NOT_SEEN_YET,
                         Type = NotificationType.MEMBER_REMOVING,
                         UserId = member.User.Id,
                         CreatedDate = notificationDTO.CreatedDate
                     }
                     );
                    notificationDTO.Id = n.Id;
                    notification = JsonConvert.SerializeObject(notificationDTO);
                    await _hubContext.Clients.All.SendAsync(member.UserId.ToString(), notification);

                    scope.Complete();
                }
                commonResponse.Message = "Remove member successfully.";
                commonResponse.Status = 200;
                return StatusCode(200, commonResponse);


            }
            catch (Exception ex)
            {
                commonResponse.Message = "Internal server error.";
                commonResponse.Status = 500;
                return StatusCode(500, commonResponse);
            }
        
        }

        ///<summary>
        ///Member leave group
        ///</summary>
        [HttpPut]
        [Route("member")]
        public CommonResponse LeaveGroup(LeavingGroupDTO leavingGroupDTO)
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
                    }
                    else throw new Exception("Internal server error");
                }
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    _memberService.LeaveGroup(userId, leavingGroupDTO);
                    response.Message = "Leave group success.";
                    response.Status = StatusCodes.Status200OK;
                    scope.Complete();
   
                }
                var leader = _memberService.GetGroupLeader(leavingGroupDTO.GroupId);
                var memberList = _memberService.GetAllMemberOfGroup(leavingGroupDTO.GroupId);
                var group = _groupRepository.FindById(leavingGroupDTO.GroupId);
                var user = _userService.FindUserById(userId);
                if (memberList != null && memberList.Count > 0)
                {
                    foreach (var tmp in memberList)
                    {
                        if (tmp.UserId == userId)
                        {
                            continue;
                        }
                        string groupLeavingMessage = NotificationMessage.BuildGroupLeavingMessage(tmp.User.FullName,group.Name);
                        NotificationDTO notificationDTO = new NotificationDTO();
                        notificationDTO.message = groupLeavingMessage;
                        notificationDTO.link = "";
                        notificationDTO.Image = group.Avatar;
                        notificationDTO.Type = NotificationType.GROUP_LEAVING;
                        notificationDTO.Status = NotificationStatus.NOT_SEEN_YET;
                        notificationDTO.CreatedDate = DateTime.Now;
                        notificationDTO.UserId = tmp.User.Id;
                        notificationDTO.Name = "Notification of " + tmp.User.FullName;
                        string notification = JsonConvert.SerializeObject(notificationDTO);
                      
                       var n = _notificationService.AddNotification(
                         new Notification
                         {
                             Name = "Notification of " + tmp.User.FullName,
                             Content = notification,
                             Image = group.Avatar,
                             Status = NotificationStatus.NOT_SEEN_YET,
                             Type = NotificationType.GROUP_LEAVING,
                             UserId = tmp.User.Id,
                             CreatedDate = notificationDTO.CreatedDate
                         }
                         );
                      
                        if (tmp.UserId == leader.UserId)
                        {
                            _emailService.SendEmailNotificationMemberLeftGroupToLeader(leader.User.Email, user.FullName, leader.User.FullName, group.Name, leavingGroupDTO.Description);

                        }
                        else
                        {
                            _emailService.SendEmailNotificationWhenMemberOutGroup(tmp.User.Email, user.FullName, leader.User.FullName, group.Name);
                        }
                        notificationDTO.Id = n.Id;
                        notification = JsonConvert.SerializeObject(notificationDTO);
                        _hubContext.Clients.All.SendAsync(tmp.User.Id.ToString(), notification);
                    }

                }
              
                return response;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Status = StatusCodes.Status500InternalServerError;
                return response;
            }
        }
    }
}
