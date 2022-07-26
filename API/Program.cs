using System;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Data.MenuRoleSeed;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Npgsql;

namespace API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {


            try
            {
                var host = CreateHostBuilder(args).Build();
                using var scope = host.Services.CreateScope();
                var services = scope.ServiceProvider;
                var configuration = services.GetService<IConfiguration>();
                var seq = configuration.GetValue<string>("SeqUrl");
                Log.Logger = new Serilog.LoggerConfiguration()
                           .MinimumLevel.Information()
                           .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information)
                           .WriteTo.File(new CompactJsonFormatter(), "Logs/logs.log", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: LogEventLevel.Warning)
                           .WriteTo.Seq(seq,LogEventLevel.Information)
                           .CreateLogger();
                Log.Information("Starting web host");
               
                try
                {
                    var context = services.GetRequiredService<DataContext>();
                    if (context.Database.GetPendingMigrations().Any())
                    {
                        await context.Database.MigrateAsync();
                        await context.Database.OpenConnectionAsync();
                        ((NpgsqlConnection)context.Database.GetDbConnection()).ReloadTypes();
                    }
                    await Seed.SeedData(context);
                    await MenuData.SeedMenu(context);
                    await MenuPermission.SeedPermissionsForRole(context);

                    var auditContext = services.GetRequiredService<AuditDataContext>();
                    await auditContext.Database.MigrateAsync();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occured during migration");
                    throw;

                }
                await host.RunAsync();
            }
            catch (Exception ex)
            {

                Log.Fatal(ex, "Host terminated unexpectedly");

            }
            finally
            {
                Log.CloseAndFlush();
            }

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                }).UseSerilog()
                  .ConfigureAppConfiguration(configurationBuilder => { configurationBuilder.AddEnvironmentVariables(); });


    }
}
