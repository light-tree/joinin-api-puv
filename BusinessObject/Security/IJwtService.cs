using BusinessObject.Models;
using System.Security.Claims;

namespace DataAccess.Security
{
    public interface IJwtService
    {
        string GenerateJwtToken(User user, string role);
        public ClaimsPrincipal DecodeJwtToken(string jwtToken);
    }
}
