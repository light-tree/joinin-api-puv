using BusinessObject.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DataAccess.Security
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _config;

        public JwtService(IConfiguration config)
        {
            _config = config;
        }
        public string GenerateJwtToken(User user, string role)
        {

            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var secretKey = _config["JwtConfig:SecretKey"];
            var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
            var issuer = _config["JwtConfig:Issuer"];



            var tokenDescription = new SecurityTokenDescriptor
            {
                Issuer = issuer,
                Audience = issuer,
                Subject = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name,user.FullName),

                   
                    new Claim("Id", user.Id.ToString()),


                    new Claim("TokenId", Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Role, role )

                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha512Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescription);

            return jwtTokenHandler.WriteToken(token);
        }
        public  ClaimsPrincipal DecodeJwtToken(string jwtToken)
        {
            var secretKey = _config["JwtConfig:SecretKey"];
            var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
                ValidateIssuer = false,
                ValidateAudience = false
            };

            try
            {
                var claimsPrincipal = tokenHandler.ValidateToken(jwtToken, validationParameters, out var validatedToken);
                return claimsPrincipal;
            }
            catch (Exception e)
            {
                // Handle token validation errors here
                return null;
            }
        }

    }
}
