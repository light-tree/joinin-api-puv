using BusinessObject.DTOs.Common;
using BusinessObject.DTOs.User;
using DataAccess.Security;
using DataAccess.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace API_JoinIn.Controllers
{
    [ApiController]
    [Route("auth")]
    public class LoginController : ControllerBase
    {
        private readonly IAuthenticateService authenticateService;
        


        public LoginController(IUserService userService, IAuthenticateService authenticateService)
        {
            this.authenticateService = authenticateService;
         

        }

        ///<summary>
        ///Login basic by email and password
        ///</summary>
        [HttpPost]
        [Route("/authenticate")]
        public async Task<IActionResult> Authenticate(LoginDTO loginDTO)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {

                var res =  await authenticateService.Authenticate(loginDTO);
                if (res == null)
                {
                    commonResponse.Message = "Incorrect email or password.";
                    return Unauthorized(commonResponse);

                } else if(res == "Unverify")
                {
                    commonResponse.Status = 406;
                    commonResponse.Message = "Unverify user.";
                    return StatusCode(406,commonResponse);
                }
                else if (res == "Inactive")
                {
                    commonResponse.Status = 403;
                    commonResponse.Message = "User has been banned.";
                    return StatusCode(403, commonResponse);
                }
                else
                {
                    commonResponse.Status = 200;
                    commonResponse.Data =  res;
                    return Ok(commonResponse);

                }
            }
            catch (TaskCanceledException ex)
            {

                commonResponse.Message = "Internal Server Error.";
                return StatusCode(StatusCodes.Status400BadRequest, commonResponse);
            }

            catch (Exception ex)
            {
                commonResponse.Message = "Internal Server Error.";
                return StatusCode(StatusCodes.Status500InternalServerError, commonResponse);
            }
        }

      
    }
}
