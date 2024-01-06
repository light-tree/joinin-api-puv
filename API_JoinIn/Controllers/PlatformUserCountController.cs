using BusinessObject.DTOs.Common;
using DataAccess.Services;
using DataAccess.Services.Implements;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace API_JoinIn.Controllers
{
    [ApiController]
    [Route("platform-user-counts")]
    public class PlatformUserCountController : ControllerBase
    {
        private readonly IPlatformUserCountService _platformUserCountService;

        public PlatformUserCountController(IPlatformUserCountService platformUserCountService)
        {
            _platformUserCountService = platformUserCountService;
        }

        ///<summary>
        ///Increase user count of the corresponding platform
        ///</summary>
        ///<param name="platformName">"facebook", "tiktok" or "unknown"</param>
        [HttpPut("{platformName}")]
        public IActionResult GetDashBoardInformation(string platformName)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                commonResponse.Data = _platformUserCountService.IncreaseUserCountByPlatformName(platformName);
                commonResponse.Status = 200;
                return Ok(commonResponse);
            }
            catch
            {
                commonResponse.Message = "Internal server error";
                commonResponse.Status = 500;
                return StatusCode(500, commonResponse);
            }
        }
    }
}
