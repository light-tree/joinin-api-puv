using BusinessObject.DTOs;
using BusinessObject.DTOs.Common;
using BusinessObject.Models;
using DataAccess.Security;
using DataAccess.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;

namespace API_JoinIn.Controllers
{
    [ApiController]
    [Authorize]
    [Route("group-majors")]
    public class GroupMajorController : ControllerBase
    {
        private readonly IGroupMajorService _groupMajorService;
        private readonly IJwtService _jwtService;

        public GroupMajorController(IGroupMajorService groupMajorService, IJwtService jwtService)
        {
            _groupMajorService = groupMajorService;
            _jwtService = jwtService;
        }

        ///<summary>
        ///Leader recruit members/apply new recruitment by majors and number of members needed for each major
        ///</summary>
        [HttpPut]
        public CommonResponse RecruitingGroupMajors(GroupMajorsDTOForRecruiting groupMajorsDTOForRecruiting)
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
                    Guid rs = _groupMajorService.CreateGroupMajors(userId, groupMajorsDTOForRecruiting);
                    response.Data = rs;
                    response.Message = "Update recruiting success.";
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
    }
}
