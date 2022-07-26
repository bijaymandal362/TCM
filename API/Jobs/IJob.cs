using System.Threading.Tasks;
using Hangfire;

namespace API.Jobs
{
    public interface IJob
    {
        Task Run(IJobCancellationToken token);
    }
}