using API.Extensions;
using API.Filters;
using API.Middleware;
using API.Services;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace API
{
    public class Startup
    {
        private readonly IConfiguration _config;
        public Startup(IConfiguration config)
        {

            _config = config;

        }


        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers()
                .AddNewtonsoftJson();
            services.AddApplicationServices(_config);
            services.AddAuthenticationServices(_config);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseMiddleware<ExceptionMiddleware>();
            app.UseXContentTypeOptions();
            app.UseReferrerPolicy(opt => opt.NoReferrer());
            app.UseXXssProtection(opt => opt.EnabledWithBlockMode());
            app.UseXfo(opt => opt.Deny());
            app.UseCsp(opt => opt
                   .BlockAllMixedContent()
                   .FormActions(s => s.Self())
                   .FrameAncestors(s => s.Self())
               );
            if (!env.IsDevelopment())
            {
                app.UseHsts(o => o.MaxAge(days: 365)) ;
            }
          
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("Permissions-Policy", "accelerometer=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), payment=(), usb=()");
                await next();
            });
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
            }


            app.UseRouting();
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseCors("CorsPolicy");
            app.UseAuthentication();
            app.UseAuthorization();

        

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireAuthorizationFilter(_config) }
            });
            
            HangfireJobRegisterService.RegisterJobs();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapFallbackToController("Index", "Fallback");
            });

        }
    }
}
