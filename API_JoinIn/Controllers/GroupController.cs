using BusinessObject.DTOs;
using BusinessObject.DTOs.Common;
using BusinessObject.Enums;
using BusinessObject.Models;
using DataAccess.Security;
using DataAccess.Services;
using DataAccess.Services.Implements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;

namespace API_JoinIn.Controllers
{
    [ApiController]
    [Authorize]
    [Route("groups")]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;
        private readonly IGroupMajorService _groupMajorService;
        private readonly IJwtService _jwtService;
        private readonly IMajorService _majorService;
        private readonly IMemberService _memberService;

        public GroupController(IGroupService groupService, IGroupMajorService groupMajorService, IJwtService jwtService, IMajorService majorService, IMemberService memberService)
        {
            _groupService = groupService;
            _groupMajorService = groupMajorService;
            _jwtService = jwtService;
            _majorService = majorService;
            _memberService = memberService;
        }



        ///<summary>
        ///User create group
        ///</summary>
        [HttpPost]
        public CommonResponse CreateGroup(GroupDTOForCreating groupDTOForCreating)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                Guid createrId = Guid.Empty;
                var jwtToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                var decodedToken = _jwtService.DecodeJwtToken(jwtToken);
                if (decodedToken != null)
                {
                    var userIdClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == "Id");
                    if (userIdClaim != null)
                    {
                        createrId = Guid.Parse(userIdClaim.Value);
                    }
                    else throw new Exception("Internal server error");
                }
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    Guid rs = _groupService.CreateGroup(createrId, groupDTOForCreating);
                    response.Data = rs;
                    response.Message = "Create group success.";
                    response.Status = StatusCodes.Status200OK;
                    scope.Complete();
                    return response;
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Status = StatusCodes.Status500InternalServerError;
                return response;
            }
        }

        ///<summary>
        ///Leader update group's information
        ///</summary>
        [HttpPut]
        public CommonResponse UpdateGroup(GroupDTOForUpdating groupDTOForUpdating)
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
                    Guid rs = _groupService.UpdateGroup(userId, groupDTOForUpdating);
                    response.Data = rs;
                    response.Message = "Update group success.";
                    response.Status = StatusCodes.Status200OK;
                    scope.Complete();
                    return response;
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Status = StatusCodes.Status500InternalServerError;
                return response;
            }
        }

        ///<summary>
        ///User search groups to apply application by name and majors
        ///</summary>
        ///<param name="majorIdsString">The string of major's Ids separated by ","</param>
        [HttpGet]
        [Route("search-to-apply")]
        public IActionResult FilterGroupsToApply(string? name, string? majorIdsString, int? pageSize, int? page, string? orderBy, string? value)
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
                List<Guid>? majorIds = majorIdsString?.Split(',').Select(a => Guid.Parse(a)).ToList();
                CommonResponse commonResponse = _groupService.FilterGroupsToApply(userId, majorIds, name, pageSize, page, orderBy, value);
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
        ///User search groups by all, owned or joined as member
        ///</summary>
        ///<param name="type">Type of groups, leave null to get all, "Owned" to get owned groups, "Joined" to get joined group as member</param>
        [HttpGet]
        public IActionResult FilterGroups(string? name, string? type, int? pageSize, int? page, string? orderBy, string? value)
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
                CommonResponse commonResponse = _groupService.FilterGroups(userId, name, type, pageSize, page, orderBy, value);
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
        ///Member get joined group's detail information
        ///</summary>
        [HttpGet]
        [Route("{groupId}")]
        public IActionResult GetGroupDetail(Guid groupId)
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
                response.Data = _groupService.GetDetailByIdAndUserId(groupId, userId);
                response.Status = StatusCodes.Status200OK; ;
                response.Message = "Get group's detail success.";
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
        ///Get role of currently logged member in group
        ///</summary>
        [HttpGet]
        [Route("{groupId}/role")]
        public IActionResult GetRoleInGroup(Guid groupId)
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
                MemberRole? role = _memberService.GetRoleInThisGroup(userId, groupId);
                response.Data = role == null ? null : Enum.GetName(typeof(MemberRole), role);
                response.Status = StatusCodes.Status200OK; ;
                response.Message = "Get role success.";
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
        ///Get all majors of all members in group
        ///</summary>
        [HttpGet]
        [Route("{groupId}/majors-of-members")]
        public IActionResult FilterMajorsForGroupViewForLeader(Guid groupId, string? name, int? pageSize, int? page, string? orderBy, string? value)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                commonResponse = _majorService.FilterMajorsForGroupViewForLeader(groupId, name, pageSize, page, orderBy, value);
                return new OkObjectResult(commonResponse);
            }
            catch (Exception ex)
            {
                commonResponse.Message = ex.Message;
                commonResponse.Status = StatusCodes.Status500InternalServerError;
                return Ok(commonResponse);
            }
        }

        ///<summary>
        ///Get all majors of waiting applications
        ///</summary>
        [HttpGet]
        [Route("{groupId}/majors-of-waiting-applications")]
        public IActionResult FilterMajorsForApplicationViewForLeader(Guid groupId, string? name, int? pageSize, int? page, string? orderBy, string? value)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                commonResponse = _majorService.FilterMajorsForApplicationViewForLeader(groupId, name, pageSize, page, orderBy, value);
                return new OkObjectResult(commonResponse);
            }
            catch (Exception ex)
            {
                commonResponse.Message = ex.Message;
                commonResponse.Status = StatusCodes.Status500InternalServerError;
                return Ok(commonResponse);
            }
        }

        
        ///<summary>
        ///User view recruting information of a group
        ///</summary>
        [HttpGet]
        [Route("{groupId}/recruiting-information")]
        public CommonResponse GetRecruitingGroupMajors(Guid groupId)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                response.Data = _groupMajorService.GetRecruitingGroupMajorsByGroupId(groupId);
                response.Message = "Get recruiting information success.";
                response.Status = StatusCodes.Status200OK;
                return response;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Status = StatusCodes.Status500InternalServerError;
                return response;
            }
        }

        ///<summary>
        ///User view recruting information of a group
        ///</summary>
        [HttpDelete]
        [Route("{groupId}")]
        public IActionResult DeleteGroup(Guid groupId)
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
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    _groupService.DeleteGroup(userId, groupId);
                    response.Status = StatusCodes.Status200OK; ;
                    response.Message = "Delete group success.";
                    scope.Complete();
                    return new OkObjectResult(response);
                }
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
