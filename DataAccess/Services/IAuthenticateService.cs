using BusinessObject.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services
{
    public interface IAuthenticateService
    {
        public Task<string> Authenticate(LoginDTO loginDTO);

        public Task<string> AuthenticateByGoogleOauth2(string email);
    }
}
