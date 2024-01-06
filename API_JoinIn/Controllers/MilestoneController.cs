using BusinessObject.DTOs;
using BusinessObject.DTOs.Common;
using DataAccess.Security;
using DataAccess.Services;
using Microsoft.AspNetCore.Mvc;

namespace API_JoinIn.Controllers
{
    [ApiController]
    [Route("milestones")]
    public class MilestoneController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly IMilestoneService _milestoneService;


        public MilestoneController(IJwtService jwtService, IMilestoneService milestoneService)
        {
            _jwtService = jwtService;
            _milestoneService = milestoneService;   
        }

        ///<summary>
        ///Get all milestone include current milestone's order of group
        ///</summary>
        [HttpGet]
        [Route("{groupId}")]
        public IActionResult GetMilestonesByGroupId(Guid groupId)
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
                response.Data = _milestoneService.GetMilestonesByGroupId(userId, groupId);
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
        ///Create milestone
        ///</summary>
        [HttpPost]
        public IActionResult CreateMilestione(MilestoneDTOForCreating milestone)
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
                response.Data = _milestoneService.CreateMilestone(milestone);
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
        ///Update milestone
        ///</summary>
        [HttpPut]
        public IActionResult UpdateMilestione(MilestoneDTOForUpdating milestone)
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
                response.Message = "Update Success";
                response.Data = _milestoneService.UpdateMilestone(milestone);
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
        ///Delete milestone
        ///</summary>
        [HttpDelete]
        public IActionResult DeleteMilestione(Guid id)
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
                response.Data = _milestoneService.DeleteMilestone(id);
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
        ///Update current milestone
        ///</summary>
        [HttpPut("updateCurrent")]
        public IActionResult UpdateCurentMilestone(MilestoneDTOForUpdatingCurrentOrder milestone)
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
                response.Message = "Update Success";
                response.Data = _milestoneService.UpdateCurrentMilestone(milestone, userId);
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
