using BusinessObject.DTOs.User;
using BusinessObject.Models;
using DataAccess.Services;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;
using DataAccess.Security;
using API_JoinIn.Utils.Email;
using Microsoft.AspNetCore.Authorization;
using BusinessObject.DTOs.Common;
using BusinessObject.Enums;
using System.IO;
using Google.Cloud.Storage.V1;
using System.Text;
using Google.Apis.Auth.OAuth2;
using Firebase.Storage;
using API_JoinIn.Utils.Firebase;
using Newtonsoft.Json;
using DataAccess.Services.Implements;
using Org.BouncyCastle.Asn1.Ocsp;
using System;

namespace API_JoinIn.Controllers
{
    [ApiController]
    [Route("users")]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService userService;
        private readonly IMajorService majorService;
        private readonly IEmailService emailService;
        private readonly IJwtService jwtService;
        private readonly IUserMajorService userMajorService;
        private readonly IFirebaseStorageService firebaseStorageService;
        private readonly IPlatformUserCountService _platformUserCountService;

        public UserController(IConfiguration configuration, IUserService userService, IMajorService majorService, IEmailService emailService, IJwtService jwtService, IUserMajorService userMajorService, IFirebaseStorageService firebaseStorageService, IPlatformUserCountService platformUserCountService)
        {
            _configuration = configuration;
            this.userService = userService;
            this.majorService = majorService;
            this.emailService = emailService;
            this.jwtService = jwtService;
            this.userMajorService = userMajorService;
            this.firebaseStorageService = firebaseStorageService;
            _platformUserCountService = platformUserCountService;
        }

        ///<summary>
        ///User create an unverified account
        ///</summary>
        [HttpPost]
        [Route("/register")]
        public async Task<IActionResult> Register(RegisterDTO registerDTO, [FromQuery]string? Platform)
        {
            CommonResponse commonResponse = new CommonResponse();

            if (await userService.CheckDuplicatedEmail(registerDTO.Email))
            {
                commonResponse.Message = "Email already exist.";
                commonResponse.Status = 400;
                return BadRequest(commonResponse);
            }

            User user = new User();
            user.Email = registerDTO.Email;
            user.FullName = "";
            user.Password = registerDTO.Password;
            user.BirthDay = new DateTime(1975, 4, 30);
            user.Gender = true;
            user.Description = "";
            user.OtherContact = "";
            user.Skill = "";
            user.Avatar = "";
            user.Theme = "";
            user.Status = UserStatus.UNVERIFIED;
            user.IsAdmin = false;
            //set up verify token

            var token = Guid.NewGuid().ToString("N").Substring(0, 16);
            user.Token = token;
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    userService.AddUser(user);

                }
                catch
                {
                    commonResponse.Message = "Internal server error.";
                    commonResponse = new CommonResponse();
                    return StatusCode(StatusCodes.Status500InternalServerError, commonResponse);
                }

                string baseUrlVerify = _configuration["BaseVerifyLink"];
                var queryString = $"{token}";
                var url = $"{baseUrlVerify}{queryString}?utm_source={Platform}";
                await emailService.SendConfirmationEmail(user.Email, url);
                scope.Complete();
                // success
                commonResponse.Message = "Register successfully, please check your email.";
                commonResponse.Status = 200;
                return Ok(commonResponse);
            }
        }

        ///<summary>
        ///System send verification email
        ///</summary>
        [HttpGet("send-email-verification")]
        public async Task<IActionResult> SendEmailVerification(String email, String ?Platform)
        {
            try
            {
                // Gửi email chứa link xác nhận đến địa chỉ email đã được chỉ định
                // (thực hiện bằng cách sử dụng thư viện gửi email của bạn)
                //var configuration = new ConfigurationBuilder()
                //    .SetBasePath(Directory.GetCurrentDirectory())
                //    .AddJsonFile("appsettings.json")
                //    .Build();
                string baseUrlVerify = _configuration["BaseVerifyLink"];
                CommonResponse commonResponse = new CommonResponse();

                User user = await userService.FindUserByEmail(email);
                if (user != null && user.Email == email)
                {

                    using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        var verifyToken = Guid.NewGuid().ToString("N").Substring(0, 16);
                        user.Token = verifyToken;
                        var rs = await userService.UpdateUser(user);
                        var queryString = $"{verifyToken}";
                        var url = $"{baseUrlVerify}{queryString}?utm_source={Platform}";
                        await emailService.SendConfirmationEmail(user.Email, url);
                        commonResponse.Message = "Email verification were sent successfully.";
                        commonResponse.Status = 200;
                        scope.Complete();
                        return Ok(commonResponse);

                    }
                }

                else
                {
                    commonResponse.Message = "Email not found.";
                    commonResponse.Status = 400;
                    return BadRequest(commonResponse);

                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        ///<summary>
        ///API link that sent to user's mail for verify user
        ///</summary>
        [HttpGet("vertified-email/{token}")]
        public async Task<IActionResult> IsEmailVerified(string token)
        {
            CommonResponse commonResponse = new CommonResponse();
            User user;


            user = await userService.FindUserByToken(token);

            if (user == null)
            {
                commonResponse.Message = "Unauthorize";
                commonResponse.Status = StatusCodes.Status401Unauthorized;
                return Unauthorized(commonResponse);
            }


            if (user.Token == token)
            {

                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        // tạo verify token mới
                        var verifyToken = Guid.NewGuid().ToString("N").Substring(0, 16);
                        user.Token = verifyToken;
                        await userService.UpdateUser(user);
                        scope.Complete();
                        commonResponse.Message = "Verify email successfully.";
                        commonResponse.Data = verifyToken;
                        // redirect to frontend
                        return Ok(commonResponse);

                    }
                    catch
                    {
                        commonResponse.Message = "Internal Server Error.";
                        return StatusCode(StatusCodes.Status500InternalServerError, commonResponse);

                    }
                    scope.Complete();
                }
            }
            else
            {
                commonResponse.Message = "Unauthorize.";
                commonResponse.Status = StatusCodes.Status401Unauthorized;
                return Unauthorized(commonResponse);
            }


        }


        ///<summary>
        ///User update profile
        ///</summary>
        [Authorize]
        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile(UserRequestDTO userRequestDTO)
        {
            CommonResponse commonResponse = new CommonResponse();
            List<UserMajor> majorsUser = new List<UserMajor>();
            string userId = "";
            try
            {
                var jwtToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                var decodedToken = jwtService.DecodeJwtToken(jwtToken);
                if (decodedToken != null)
                {
                    var userIdClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == "Id");
                    if (userIdClaim != null)
                    {
                        userId = userIdClaim.Value;
                        // Do something with user ID here
                    }
                    else throw new Exception("Internal Server Error.");
                }
                //rollback khi có lỗi xảy ra
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var user = await userService.FindUserByGuid(Guid.Parse(userId));
                    user.FullName = userRequestDTO.FullName;
                    user.BirthDay = userRequestDTO.BirthDay;
                    user.Gender = userRequestDTO.Gender;
                    user.Description = userRequestDTO.Description;
                    user.OtherContact = userRequestDTO.OtherContact;
                    user.Skill = userRequestDTO.Skill;
                    user.Avatar = userRequestDTO.Avatar;
                    user.Theme = userRequestDTO.Theme;
                    user.Status = UserStatus.ACTIVE;
                    user.Phone = userRequestDTO.PhoneNumber;
                    user.IsAdmin = false;


                    // check danh sách major
                    foreach (Guid id in userRequestDTO.MajorIdList)
                    {
                        var tmp = majorService.FindMajorById(id);
                        if (tmp != null)
                        {
                            await userMajorService.UpdateUserMajor(id, user.Id);

                        }
                        else throw new Exception("Your major is invalid.");
                    }

                    bool check = await userService.UpdateUser(user);

                    if (user == null || !check) throw new Exception();


                    scope.Complete(); // commit transaction
                }

                commonResponse.Status = 200;
                commonResponse.Message = "Update user profile successfully.";

                // Quay về trang login
                return Ok(commonResponse);

            }
            catch
            {
                commonResponse.Data = "Internal Server Error.";
                commonResponse.Status = 500;
                return StatusCode(StatusCodes.Status500InternalServerError, commonResponse);
            }
        }

        ///<summary>
        ///User recovery password
        ///</summary>
        [HttpGet("reset-password")]
        public async Task<IActionResult> SendEmailResetPassword(String email)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                // Gửi email chứa link xác nhận đến địa chỉ email đã được chỉ định
                // (thực hiện bằng cách sử dụng thư viện gửi email của bạn)

                string baseUrlVerify = _configuration["BaseResetPasswordLink"];


                User user = await userService.FindUserByEmail(email);
                if (user != null && user.Email == email)
                {
                  
                    using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        var verifyToken = Guid.NewGuid().ToString("N").Substring(0, 16);
                        user.Token = verifyToken;
                        var rs = await userService.UpdateUser(user);
                        var queryString = $"{verifyToken}";
                        var url = $"{baseUrlVerify}/{queryString}";
                        await emailService.SendRecoveryPasswordEmail(user.Email, url);
                        commonResponse.Message = "Email were sent successfully.";
                        commonResponse.Status = 200;
                        scope.Complete();
                        return Ok(commonResponse);

                    }
                }

                else
                {
                    commonResponse.Message = "Email not found.";
                    commonResponse.Status = 400;
                    return BadRequest(commonResponse);

                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có

                commonResponse.Data = "Internal server error.";
                commonResponse.Status = 500;
                return StatusCode(StatusCodes.Status500InternalServerError, commonResponse);
            }
        }

        ///<summary>
        ///Upload image
        ///</summary>
        [HttpPost("upload/image")]
        public async Task<IActionResult> UploadAvatar(List<IFormFile> files)
        {
            IFormFile file = files[0];
            var imageName = "";
            CommonResponse commonResponse = new CommonResponse();

            if (file == null || file.Length == 0)
            {
                commonResponse.Status = 400;
                commonResponse.Message = "Invalid file.";
                return BadRequest(commonResponse);
            }
            // Check if the file has a valid extension
            var validExtensions = new string[] { ".jpg", ".jpeg", ".png", ".gif" }; // Add more extensions if needed
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!validExtensions.Contains(extension))
            {
                commonResponse.Status = 400;
                commonResponse.Message = "Invalid file.";
                return BadRequest(commonResponse);
            }
            // update user photo and theme
            try
            {
                using (var stream = file.OpenReadStream())
                {
                    // Tạo tên tệp hình ảnh duy nhất
                    imageName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    // Gọi phương thức tải lên từ FirebaseStorageService
                    string linkImg = await firebaseStorageService.UploadImageToFirebase(stream, imageName);
                    commonResponse.Data = linkImg;
                    commonResponse.Status = 200;
                    commonResponse.Message = "Upload image success";
                    return Ok(commonResponse);

                }
              
                
            }
            catch
            {
                commonResponse.Status = 500;
                commonResponse.Message = "Internal Server Error.";
                return StatusCode(StatusCodes.Status500InternalServerError, commonResponse);
            }
        }

        ///<summary>
        ///User get profile's information
        ///</summary>
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfileForUserByID(string userId)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                User user = await userService.FindUserByGuid(Guid.Parse(userId));
                if (user == null)
                {
                    commonResponse.Status = 400;
                    commonResponse.Message = "User is not found.";
                    return BadRequest(commonResponse);
                }
                UserProfileResponseDTO userProfileResponse = new UserProfileResponseDTO();
                userProfileResponse.FullName = user.FullName;
                userProfileResponse.Avatar = user.Avatar;
                userProfileResponse.BirthDay = user.BirthDay;
                userProfileResponse.Gender = user.Gender;
                userProfileResponse.Email = user.Email;
                userProfileResponse.Description = user.Description;
                userProfileResponse.Skill = user.Skill;
                userProfileResponse.Id = user.Id;
                userProfileResponse.Theme = user.Theme;
                userProfileResponse.majors = user.UserMajors.Select(u => new Major { Id = u.Major.Id, Name = u.Major.Name, ShortName = u.Major.ShortName }).ToList();
                commonResponse.Status = 200;
                commonResponse.Data = userProfileResponse;
                return Ok(commonResponse);

            }
            catch
            {
                commonResponse.Status = 500;
                commonResponse.Message = "Internal Server Error.";
                return StatusCode(StatusCodes.Status500InternalServerError, commonResponse);

            }

        }

        ///<summary>
        ///User complete profile's information first time after registered
        ///</summary>
        [HttpPut("complete-profile")]
        public async Task<IActionResult> CompleteUserInformation(UserRequestDTO userRequestDTO, string verifyToken)
        {
            CommonResponse commonResponse = new CommonResponse();
            User user;
            try
            {
                user = await userService.FindUserByToken(verifyToken);

                if (user == null) {
                    commonResponse.Status = 401;
                    commonResponse.Message = "Verify token is incorrect.";
                    return Unauthorized(commonResponse);
                }
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {

                    user.FullName = userRequestDTO.FullName;
                    user.BirthDay = userRequestDTO.BirthDay;
                    user.Gender = userRequestDTO.Gender;
                    user.Description = userRequestDTO.Description;
                    user.OtherContact = userRequestDTO.OtherContact;
                    user.Skill = userRequestDTO.Skill;
                    user.Avatar = userRequestDTO.Avatar;
                    user.Theme = userRequestDTO.Theme;
                    user.Status = UserStatus.ACTIVE;
                    user.Phone = userRequestDTO.PhoneNumber;
                    user.IsAdmin = false;
                    // update major
                    // check danh sách major
                    foreach (Guid id in userRequestDTO.MajorIdList)
                    {
                        var tmp = majorService.FindMajorById(id);
                        if (tmp != null)
                        {
                            await userMajorService.UpdateUserMajor(id, user.Id);

                        }
                        else
                        {
                            commonResponse.Status = 400;
                            commonResponse.Message = "Major input is invalid";
                            return BadRequest(commonResponse);
                        }
                    }

                    bool check = await userService.UpdateUser(user);

                    if (user == null || !check) throw new Exception();
                    string platform = "unknown";
                    switch (userRequestDTO.PlatForm)
                    {
                        case PlatForm.FACEBOOK:
                            platform = "facebook";
                            break;
                        case PlatForm.TIKTOK:
                            platform = "tiktok";
                            break;
                        default:
                            platform = "unknown";
                            break;

                    }
                    _platformUserCountService.IncreaseUserCountByPlatformName(platform);
                    scope.Complete();
                    commonResponse.Status = 200;
                    var acccesToken = jwtService.GenerateJwtToken(user,"User");
                    commonResponse.Data = acccesToken;
                    commonResponse.Message = "Update user profile successfully.";

                    // increase user in table UserFlatform
                   
                    return Ok(commonResponse);
                }
            } catch
            {
                commonResponse.Message = "Internal Server Error.";
                return StatusCode(StatusCodes.Status500InternalServerError, commonResponse);
            }
        }

        ///<summary>
        ///System send verification code to user's email, user for change password
        ///</summary>
        [Authorize]
        [HttpGet("send-verifyCode")]
        public async Task<IActionResult> SendVerificationCode()
        {
            string userId = "";
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                var jwtToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                var decodedToken = jwtService.DecodeJwtToken(jwtToken);
                if (decodedToken != null)
                {
                    var userIdClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == "Id");
                    if (userIdClaim != null)
                    {
                        userId = userIdClaim.Value;
                        // Do something with user ID here
                    }
                    else throw new Exception("Internal Server Error.");

                    User user = await userService.FindUserByGuid(Guid.Parse(userId));

                    using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        Random random = new Random();
                        var verifyCode = random.Next(100000, 999999).ToString();
                        user.VerifyCode = verifyCode;
                        var rs = await userService.UpdateUser(user);
                        await emailService.SendVerifyCode(user.Email,verifyCode);
                        commonResponse.Message = "Verification code were send to your email.";
                        commonResponse.Status = 200;
                        scope.Complete();
                        return Ok(commonResponse);

                    }
                }

                else
                {
                    commonResponse.Message = "Email not found.";
                    commonResponse.Status = 400;
                    return BadRequest(commonResponse);

                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có

                commonResponse.Data = "Internal server error.";
                commonResponse.Status = 500;
                return StatusCode(StatusCodes.Status500InternalServerError, commonResponse);
            }
        }

        ///<summary>
        ///User update password using verification code
        ///</summary>
        [Authorize]
        [HttpPut("update-password")]
        public async Task<IActionResult> UpdatePassword(PasswordUpdateDTO passwordUpdateDTO)
        {

            CommonResponse commonResponse = new CommonResponse();

            string userId = "";
            try
            {
                var jwtToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                var decodedToken = jwtService.DecodeJwtToken(jwtToken);
                if (decodedToken != null)
                {
                    var userIdClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == "Id");
                    if (userIdClaim != null)
                    {
                        userId = userIdClaim.Value;
                        // Do something with user ID here
                    }
                    else throw new Exception("Internal Server Error.");
                }
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var user = await userService.FindUserByGuid(Guid.Parse(userId));

                    if(user.VerifyCode != passwordUpdateDTO.verifyCode)
                    {
                        commonResponse.Message = "Verify code is incorrect.";
                        commonResponse.Status = 400;
                        return BadRequest(commonResponse);
                    }
                    user.Password = PasswordHasher.Hash(passwordUpdateDTO.password);
                    await userService.UpdateUser(user);
                    scope.Complete();
                    commonResponse.Status = 200;
                    commonResponse.Message = "Change password successfully.";
                    return Ok(commonResponse);

                }
            }
            catch
            {
                commonResponse.Status = 500;
                commonResponse.Message = "Internal Server Error.";
                return StatusCode(StatusCodes.Status500InternalServerError, commonResponse);
            }


        }


        [HttpPut("reset-password")]
        public async Task<IActionResult> ResetPassword(PasswordResetDTO passwordResetDTO )
        {

            CommonResponse commonResponse = new CommonResponse();

            string userId = "";
            try
            {
                
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var user = await userService.FindUserByToken(passwordResetDTO.verifyToken);



                    if (user == null)
                    {
                        commonResponse.Message = "Incorrect token";
                        commonResponse.Status = 401;
                        return Unauthorized(commonResponse);
                    }
                    user.Password = PasswordHasher.Hash(passwordResetDTO.password);
                    await userService.UpdateUser(user);
                    scope.Complete();
                    commonResponse.Status = 200;
                    commonResponse.Message = "Change password successfully.";
                    return Ok(commonResponse);

                }
            }
            catch
            {
                commonResponse.Status = 500;
                commonResponse.Message = "Internal Server Error.";
                return StatusCode(StatusCodes.Status500InternalServerError, commonResponse);
            }


        }


        ///<summary>
        ///Get current user login profile
        ///</summary>
        [Authorize]
        [HttpGet("user/profile")]
        public async Task<IActionResult> GetProfileLoginUser()
        {
            var userId = "";
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                var jwtToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                var decodedToken = jwtService.DecodeJwtToken(jwtToken);
                if (decodedToken != null)
                {
                    var userIdClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == "Id");
                    if (userIdClaim != null)
                    {
                        userId = userIdClaim.Value;
                        // Do something with user ID here
                    }
                    else throw new Exception("Internal Server Error.");
                }
                else throw new Exception("Internal Server Error.");
                User user = await userService.FindUserByGuid(Guid.Parse(userId));
                if (user == null)
                {
                    commonResponse.Status = 400;
                    commonResponse.Message = "User is not found.";
                    return BadRequest(commonResponse);
                }
                UserProfileResponseDTO userProfileResponse = new UserProfileResponseDTO();
                userProfileResponse.FullName = user.FullName;
                userProfileResponse.Avatar = user.Avatar;
                userProfileResponse.BirthDay = user.BirthDay;
                userProfileResponse.Gender = user.Gender;
                userProfileResponse.Email = user.Email;
                userProfileResponse.Description = user.Description;
                userProfileResponse.Phone = user.Phone;
                userProfileResponse.OtherContact = user.OtherContact;
                userProfileResponse.Skill = user.Skill;
                userProfileResponse.Id = user.Id;
                userProfileResponse.Theme = user.Theme;
                userProfileResponse.majors = user.UserMajors.Select(u => new Major { Id = u.Major.Id, Name = u.Major.Name, ShortName = u.Major.ShortName }).ToList();
                commonResponse.Status = 200;
                commonResponse.Data = userProfileResponse;
                return Ok(commonResponse);

            }
            catch
            {
                commonResponse.Status = 500;
                commonResponse.Message = "Internal Server Error.";
                return StatusCode(StatusCodes.Status500InternalServerError, commonResponse);

            }

        }


        ///<summary>
        ///Get user list by email
        ///</summary>
        //[Authorize]
        [HttpGet("")]
        public async Task<IActionResult> GetUsersByEmail(string? email="",int pageSize=10, int currentPage=1)
        {
            CommonResponse commonResponse = new CommonResponse();   
            try
            {
                List<UserProfileResponseDTO> tmp = await userService.findUserByEmail(email,pageSize,currentPage);

                if (tmp == null || tmp.Count == 0)
                {
                   
                    commonResponse.Message = "User not found.";
                    commonResponse.Status = 200;
                    return Ok(commonResponse);
                }
                commonResponse.Status = 200;
                commonResponse.Data = tmp;
                return Ok(commonResponse);
            } catch
            {
                commonResponse.Message = "Internal server error";
                commonResponse.Status = 500;
                return StatusCode(500,commonResponse);
            }
        }

        ///<summary>
        ///Get users and transactions as dashboard in weeks
        ///</summary>
        [Authorize(Roles = "Admin")]
        [HttpGet("dashboard")]
        public IActionResult GetDashBoardInformation()
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                commonResponse.Data = userService.GetDashBoardInformation();
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


        ///<summary>
        ///Create sample user, dont need to reggister or complete profile, delete it when submit project
        ///</summary>
        ///<param name="Email">For the email that you want to create account </param>
        [HttpPost("Create-sample-account/{Email}")]
        public async Task<IActionResult> CreateSampleAccount(String Email, UserRequestDTO userRequestDTO)
        {
           
            CommonResponse commonResponse = new CommonResponse();
            try
            {


                if (await userService.CheckDuplicatedEmail(Email))

                {
                    commonResponse.Message = "Email already exist.";
                    commonResponse.Status = 400;
                    return BadRequest(commonResponse);
                }
                User user = new User();
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {

                    user.FullName = userRequestDTO.FullName;
                    user.BirthDay = userRequestDTO.BirthDay;
                    user.Gender = userRequestDTO.Gender;
                    user.Description = userRequestDTO.Description;
                    user.OtherContact = userRequestDTO.OtherContact;
                    user.Skill = userRequestDTO.Skill;
                    user.Avatar = userRequestDTO.Avatar;
                    user.Theme = userRequestDTO.Theme;
                    user.Status = UserStatus.ACTIVE;
                    user.Phone = userRequestDTO.PhoneNumber;
                    user.IsAdmin = false;
                    user.Password = "1234567890a";
                    user.Email = Email;
                    User check =  userService.AddUser(user);

                    if (user == null || check == null) throw new Exception();
                    // update major
                    // check danh sách major
                    foreach (Guid id in userRequestDTO.MajorIdList)
                    {
                        var tmp = majorService.FindMajorById(id);
                        if (tmp != null)
                        {
                            await userMajorService.UpdateUserMajor(id, user.Id);

                        }
                        else
                        {
                            commonResponse.Status = 400;
                            commonResponse.Message = "Major input is invalid";
                            return BadRequest(commonResponse);
                        }
                    }

                  
                    scope.Complete();

                    commonResponse.Message = "Create succesfully";
                    commonResponse.Status = 200;
                    return Ok(commonResponse);
                }
            }
            catch
            {
                commonResponse.Status = 500;
                commonResponse.Message = "Internal Server Error.";
                return StatusCode(StatusCodes.Status500InternalServerError, commonResponse);

            }
        }
        [Authorize(Roles ="Admin")]       
        
        [HttpGet("admin/user")]
        public async Task<IActionResult> FilterUserForAdmin(string? email="", string? name="", int pageSize=10, int pageNumber=1)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                commonResponse = await userService.FilterUserForAdmin(email, name,pageSize,pageNumber);
                return Ok(commonResponse);

            } catch
            {
                commonResponse.Status = 500;
                commonResponse.Message = "Internal server error.";
                return StatusCode(500, commonResponse);
            }

        }

        [Authorize (Roles = "Admin")]
        [HttpPut("admin/user/ban")]
        public async Task<IActionResult> BanUser(Guid userId)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                User user = await userService.FindUserByGuid(userId);
                if (user == null) {
                    commonResponse.Status = 400;
                    commonResponse.Message = "User not found.";
                    return StatusCode(400, commonResponse);
                }
                user.Status = UserStatus.INACTIVE;
                var rs = userService.UpdateUser(user);
                if (rs == null) throw new Exception();

                commonResponse.Status = 200;
                commonResponse.Message = "Ban user successfully.";
                return StatusCode(200, commonResponse);

            } catch
            {
                commonResponse.Message = "Internal server error.";
                return StatusCode(500, commonResponse); 
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("admin/user/unban")]
        public async Task<IActionResult> UnbanUser(Guid userId)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                User user = await userService.FindUserByGuid(userId);
                if (user == null)
                {
                    commonResponse.Status = 400;
                    commonResponse.Message = "User not found.";
                    return StatusCode(400, commonResponse);
                }
                user.Status = UserStatus.ACTIVE;
                var rs = userService.UpdateUser(user);
                if (rs == null) throw new Exception();

                commonResponse.Status = 200;
                commonResponse.Message = "Unban user successfully.";
                return StatusCode(200, commonResponse);

            }
            catch
            {
                commonResponse.Message = "Internal server error";
                return StatusCode(500, commonResponse);
            }
        }
    }

    }
