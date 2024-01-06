using BusinessObject.DTOs;
using BusinessObject.DTOs.Common;
using DataAccess.Security;
using DataAccess.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;

namespace API_JoinIn.Controllers
{
    [ApiController]
    [Route("user-majors")]
    public class UserMajorController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly IMajorService _majorService;

        public UserMajorController(IJwtService jwtService, IMajorService majorService)
        {
            _jwtService = jwtService;
            _majorService = majorService;
        }

        ///<summary>
        ///Get all majors in profile of user
        ///</summary>
        [HttpGet]
        public IActionResult FilterMajorsForApplicationViewForUser(string? name, int? pageSize, int? page, string? orderBy, string? value)
        {
            CommonResponse commonResponse = new CommonResponse();
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

                commonResponse = _majorService.FilterMajors(userId, name, pageSize, page, orderBy, value);
                return new OkObjectResult(commonResponse);
            }
            catch (Exception ex)
            {
                commonResponse.Message = ex.Message;
                commonResponse.Status = StatusCodes.Status500InternalServerError;
                return Ok(commonResponse);
            }
        }
    }
}
