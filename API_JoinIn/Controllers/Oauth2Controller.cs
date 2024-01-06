using API_JoinIn.Utils.Email;
using BusinessObject.DTOs.Common;
using BusinessObject.DTOs.Google;
using BusinessObject.DTOs.User;
using BusinessObject.Enums;
using BusinessObject.Models;
using DataAccess.Repositories.Implements;
using DataAccess.Services;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text.Json;
using System.Transactions;

namespace API_JoinIn.Controllers
{
    [ApiController]
    [Route("oauth2")]
    public class Oauth2Controller : ControllerBase
    {

        private readonly IConfiguration _configuration;
        private readonly IUserService userService;
        private readonly IEmailService emailService;
        private readonly IAuthenticateService authenticateService;

        public Oauth2Controller(IConfiguration configuration, IUserService userService, IEmailService emailService, IAuthenticateService authenticateService)
        {
            _configuration = configuration;
            this.userService = userService;
            this.emailService = emailService;
            this.authenticateService = authenticateService;
        }

        ///<summary>
        ///Login by google
        ///</summary>
        [HttpGet]
        [Route("google-sign-in")]
        public IActionResult Index()
        {

            var clientId = _configuration["Authentication:Google:ClientId"];
            var clientSecret = _configuration["Authentication:Google:ClientSecret"];
            var authenLink = _configuration["Authentication:Google:authenLink"];
            var redirectUrl = _configuration["Authentication:Google:redirect_uri"];
            // Redirect đến Google để xác thực đăng nhập
            var redirectAuthenLink = authenLink +
                "?client_id=" + clientId +
                "&redirect_uri=" + redirectUrl +
                "&clientSecret=" + clientSecret +
                "&response_type=code" +
                "&scope=email%20profile";

            return Ok(redirectAuthenLink);
        }

        [HttpPost("call-back")]
        public async Task<IActionResult> GoogleResponse(GoogleSignInDTO googleSignInDTO)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
               //string code = HttpContext.Request.Query["code"];
               // var clientId = _configuration["Authentication:Google:ClientId"];
               // var clientSecret = _configuration["Authentication:Google:ClientSecret"];
               // string redirectUri = _configuration["Authentication:Google:redirect_uri"]; 
            

                // Yêu cầu thông tin truy cập từ Google
                var client = new HttpClient();
                //var content = new FormUrlEncodedContent(new[]
                //{
                //    new KeyValuePair<string, string>("code", code),
                //    new KeyValuePair<string, string>("client_id", clientId),
                //    new KeyValuePair<string, string>("client_secret", clientSecret),
                //    new KeyValuePair<string, string>("redirect_uri", redirectUri),
                //    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                //});
                //var tokenResponse = await client.PostAsync("https://oauth2.googleapis.com/token", content);
                //var tokenContent = await tokenResponse.Content.ReadAsStringAsync();
                //var tokenData = JsonSerializer.Deserialize<GoogleTokenResponse>(tokenContent);

                // Yêu cầu thông tin người dùng từ Google
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", googleSignInDTO.googleToken);
                var userResponse = await client.GetAsync("https://www.googleapis.com/oauth2/v2/userinfo");
                var userContent = await userResponse.Content.ReadAsStringAsync();
                var userData = JsonSerializer.Deserialize<GoogleUserResponse>(userContent);

                // check duplicated email
                var account = await userService.FindUserByEmail(userData.Email);
                if (account != null) { 

                    // nếu user đã tồn tại, tạo token
                    var res = await  authenticateService.AuthenticateByGoogleOauth2(userData.Email);

                    if (res == "Unverify")
                    {
                        commonResponse.Status = 406;
                        account.Token = Guid.NewGuid().ToString("N").Substring(0, 16);
                        await userService.UpdateUser(account);
                        commonResponse.Message = "Unverify user.";
                        string baseUrl = _configuration["BaseUrl"];
                        commonResponse.Data = $"/profile/initialization?token={account.Token}"; 
                        return Ok(commonResponse);

                    }
                    else if (res == "Inactive")
                    {
                        commonResponse.Status = 403;
                        commonResponse.Message = "User has been banned.";
                        return StatusCode(403, commonResponse);
                    }
                    else
                    {
                      
                        commonResponse.Data =  res;
                        commonResponse.Status = 200;
                        return (Ok(commonResponse));
                    }
                  }

                else
                {
                    // Sử dụng thông tin người dùng ở đây

                    User user = new User();
                    user.Email = userData.Email;
                    user.FullName = userData.Name;
                    user.Password = "";
                    user.BirthDay = new DateTime(1975, 4, 30);
                    user.Gender = true;
                    user.Description = "";
                    user.OtherContact = "";
                    user.Skill = "";
                    user.Avatar = userData.Picture;
                    user.Theme = "";
                    user.Status = UserStatus.UNVERIFIED;
                    user.IsAdmin = false;
                    // tạo token để verify
                    user.Token = Guid.NewGuid().ToString("N").Substring(0, 16);

                    using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {

                        var userInput = userService.AddUser(user);

                        if (userInput == null) throw new Exception();

                        //  bool isEmailVerified = userData.VerifiedEmail;
                        //string pictureUrl = userData.Picture;
                        //  string locale = userData.Locale;
                        //  string hd = userData.Hd;


                        //tạo token
                        // khi người dùng đăng nhập bằng token vừa tạo => đẩy người dùng về trang điền thông tin

                        string baseUrl = _configuration["BaseUrl"];
                        commonResponse.Status = 406;  
                        commonResponse.Message = "Unverify user.";
                        commonResponse.Data = $"/profile/initialization?token={user.Token}";
                        scope.Complete(); // commit transaction
                        return Ok(commonResponse);



                    }
                }
            }
            catch (Exception e)
            {
                commonResponse.Data = "Internal Server Error.";
                commonResponse.Status = 500;
                return StatusCode(StatusCodes.Status500InternalServerError, commonResponse);
            }
        }
    }
}
