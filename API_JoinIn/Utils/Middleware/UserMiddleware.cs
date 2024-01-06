using BusinessObject.DTOs.Common;
using BusinessObject.Enums;
using BusinessObject.Migrations;
using BusinessObject.Models;
using DataAccess.Security;
using DataAccess.Services;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Net;
using System.Security.Claims;
using System.Text.Json;

namespace API_JoinIn.Utils.Middleware
{
    public class UserMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly IServiceProvider _serviceProvider;

        public UserMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
        {
            _next = next;
            _serviceProvider = serviceProvider;
        }

        public async System.Threading.Tasks.Task InvokeAsync(HttpContext context)
        {
            // Perform custom middleware logic here
            var userId = Guid.Empty;
            var authorizationHeader = context.Request.Headers["Authorization"].FirstOrDefault();

            using (var scope = _serviceProvider.CreateScope())
            {
                var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                var jwtService = scope.ServiceProvider.GetRequiredService<IJwtService>();

                if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer "))
                {
                    var token = authorizationHeader.Substring("Bearer ".Length);
                    var decodedToken = jwtService.DecodeJwtToken(token);
                    if (decodedToken != null)
                    {
                        var userIdClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == "Id");
                        if (userIdClaim != null)
                        {
                            userId = Guid.Parse(userIdClaim.Value);
                            // Do something with user ID here
                            var u = await userService.FindUserByGuid(userId);
                            if (u.Status == UserStatus.INACTIVE)
                            {
                                CommonResponse commonResponse = new CommonResponse();
                                commonResponse.Status = 403;
                                commonResponse.Message = "User has been banned.";
                                var json = JsonSerializer.Serialize(commonResponse);
                                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                                context.Response.ContentType = "application/json";
                                await context.Response.WriteAsync(json);
                                return;
                            }
                        }
                        else throw new Exception("Internal server error");
                    }
                }
            }

            await _next(context);
        }

    }

    public static class UserMiddlewareExtensions
    {
        public static IApplicationBuilder UseUserMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UserMiddleware>();
        }


    }
}
