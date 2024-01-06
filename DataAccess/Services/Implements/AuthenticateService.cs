using BusinessObject.DTOs.User;
using BusinessObject.Models;
using DataAccess.Repositories;
using DataAccess.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services.Implements
{
    public class AuthenticateService : IAuthenticateService
    {
        private readonly IUserRepository userRepository;
        private readonly IJwtService jwtService;

        public AuthenticateService(IUserRepository userRepository, IJwtService jwtService)
        {
            this.userRepository = userRepository;
            this.jwtService = jwtService;
     
        }

        public async Task<string> Authenticate(LoginDTO loginDTO)
        {
            User account = await userRepository.FindAccountByEmail(loginDTO.UserName);
            bool checkPassword = false;
            if (account == null )
            {
                return null;
            }
            else
            {

                checkPassword = PasswordHasher.Verify(loginDTO.Password, account.Password);
                if (!checkPassword)
                {
                    return null;
                }

            }
            if(account.Status == BusinessObject.Enums.UserStatus.UNVERIFIED)
            {
                return "Unverify";
            }
            else if (account.Status == BusinessObject.Enums.UserStatus.INACTIVE)
            {
                return "Inactive";
            }

            //if (account.Status == BussinessObject.Status.AccountStatus.INACTIVE)
            //{
            //    throw new TaskCanceledException("Tài khoản đã bị khóa");
            //}

            string role = null;
            if (account.IsAdmin)
            {
                role = "Admin";
            }
            else
            {
                await userRepository.UpdateLastLoginDate(account);
                role = "User";
            }
           

            string token = jwtService.GenerateJwtToken(account, role);
            return token;
        }

        // Dành cho đăng nhập bằng google không cần mật khẩu

        public async Task<string> AuthenticateByGoogleOauth2(string email)
        {
            User account = await userRepository.FindAccountByEmail(email);
            if (account == null)
            {
                return null;
            }
            else
            {
                //if (account.Status == BussinessObject.Status.AccountStatus.INACTIVE)
                //{
                //    throw new TaskCanceledException("Tài khoản đã bị khóa");
                //}
                if (account.Status == BusinessObject.Enums.UserStatus.UNVERIFIED)
                {
                  
                   
                    return "Unverify";
                }
                else if (account.Status == BusinessObject.Enums.UserStatus.INACTIVE)
                {
                    return "Inactive";
                }
                string role = null;
                if (account.IsAdmin)
                {
                    role = "Admin"; 
                } else
                {
                    await userRepository.UpdateLastLoginDate(account);
                    role = "User";
                }

                string token = jwtService.GenerateJwtToken(account, role);
                return token;

            }

        }
    }
}
