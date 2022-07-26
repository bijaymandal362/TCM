using System;
using System.Text;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace API.Extensions
{
    public static class AuthenticationServiceExtension
    {
        public static IServiceCollection AddAuthenticationServices(this IServiceCollection services, IConfiguration config)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
          
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opt =>
            {    
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime=true,
                    ClockSkew=TimeSpan.Zero
                };
            });
            services.AddScoped<TokenService>();
            services.AddHttpClient<AuthenticationService>(client =>
            {
                client.BaseAddress = new Uri(config["AuthServer:host"]);
            });
            services.AddHttpContextAccessor();
            return services;
        }
    }
}