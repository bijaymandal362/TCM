using System;
using System.Threading.Tasks;
using Hangfire;

namespace API.Jobs
{
    public class TestJob : IJob
    {
        public async Task Run(IJobCancellationToken token)
        {
            Console.WriteLine("This is a test job2");
            await Task.FromResult(0);
        }
    }
}