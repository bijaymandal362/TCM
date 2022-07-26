using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace API.Filters
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        private readonly IConfiguration _config;
        private static readonly string HangFireCookieName = "HangFireCookie";
      
        public HangfireAuthorizationFilter(IConfiguration config)
        {
            _config = config;
        }

        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            string key = string.Empty;
            bool setCookie = false;

            if (httpContext.Request.Query.ContainsKey("key"))
            {
                key = httpContext.Request.Query["key"].FirstOrDefault();
                setCookie = true;
            }
            else
            {
                key = httpContext.Request.Cookies[HangFireCookieName];
            }
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }
            if (key != _config["Hangfire:Key"])
            {
                return false;
            }

            if (setCookie)
            {
                httpContext.Response.Cookies.Append(HangFireCookieName,
                key,
                new CookieOptions()
                {
                    HttpOnly=true,
                    Secure=true,
                    Expires = DateTime.Now.AddMinutes(Convert.ToDouble(_config["Hangfire:ExpiresInMinutes"]))
                });
            }


            return true;


        }
    }
}