using Models.Core;
using Models.Dashboard;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Dashboard
{
    public interface IDashboardService
    {
        Task<Result<DashboardModel>> GetTestCaseTestPlanTestRunCountAsync(string projectSlug);

        Task<Result<TestRunStatusCountModel>> GetTestRunStatusCountByTestRunIdAsync(int testRunId);

        Task<Result<List<TestRunListModels>>> GetTestRunListAsync(string projectSlug);

        Task<Result<List<TestCaseRepositoryListModel>>> GetTestCaseRepositoryListAsync(string projectSlug);

        Task<Result<List<FunctionTestCaseListCountModel>>> GetFunctionTestCaseListCountAsync(string projectSlug, int projectModuleId);

        Task<Result<List<FunctionTestCaseListCountModel>>> GetDefaultFunctionTestCaseListCountByProjectSlugAsync(string projectSlug);

        Task<Result<List<TestCaseListDetail>>> GetTestCaseListDetailStatusCountAsync(string projectSlug);

        Task<Result<int>> GetProjectCountAsync();

        Task<Result<int>> GetUserCountAsync();

        Task<Result<DashboardModel>> GetTestCaseTestPlanTestRunCountAsync();

        Task<Result<List<TestCaseCountModel>>> GetTestCaseCountFromLastMonthAsync();
    }
}