using BusinessLayer.Account;
using BusinessLayer.Common;
using BusinessLayer.PersonUser;
using BusinessLayer.Project;
using BusinessLayer.User;
using BusinessLayer.ProjectMember;
using Data;
using Hangfire;
using Hangfire.Storage.SQLite;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using BusinessLayer.UserProfile;
using BusinessLayer.ProjectModule;
using BusinessLayer.TestPlan;
using BusinessLayer.TestRun;
using BusinessLayer.Dashboard;
using BusinessLayer.ProjectStarred;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<DataContext>(opt =>
            {
                opt.UseNpgsql(config.GetConnectionString("DefaultConnection"));
            });

            services.AddDbContext<AuditDataContext>(opt =>
            {
                opt.UseNpgsql(config.GetConnectionString("AuditDefaultConnection"));
            });

            services.AddHangfire(opt=>
            {
                opt.UseSQLiteStorage(config.GetConnectionString("HangfireConnection"));
            });

           

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
            });
            services.AddCors(opt =>
             {
                opt.AddPolicy("CorsPolicy", policy =>
                 {
                     policy.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin();
                 });
            });

            services.AddHangfireServer(o=>{
                o.WorkerCount=1;
            });
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ICommonService, CommonService>();
            services.AddScoped<IPersonAccessor, PersonAccessor>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<IPersonService, PersonService>(); 
            services.AddScoped<IProjectMemberService, ProjectMemberService>();
            services.AddScoped<IUserService, UserService>(); 
            services.AddScoped<IUserProfileService, UserProfileService>();
            services.AddScoped<IProjectModuleService, ProjectModuleService>();
            services.AddScoped<ITestPlanService, TestPlanService>();
            services.AddScoped<ITestRunService, TestRunService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IProjectStarredService, ProjectStarredService>();
            return services;
        }
    }
}