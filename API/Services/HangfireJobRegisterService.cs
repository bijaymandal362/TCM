using API.Jobs;
using Hangfire;

namespace API.Services
{
    public class HangfireJobRegisterService
    {
        public static void RegisterJobs()
        {

            //RecurringJob.AddOrUpdate<TestJob>(nameof(TestJob),job => job.Run(JobCancellationToken.Null),"*/1  * * * *");

        }
    }
}