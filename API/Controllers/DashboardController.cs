using BusinessLayer.Dashboard;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Controllers
{
    public class DashboardController : BaseApiController
    {
        private readonly IDashboardService _iDashboardService;

        public DashboardController(IDashboardService iDashboardService)
        {
            _iDashboardService = iDashboardService;
        }

        [HttpGet]
        [Route("GetTestCaseTestPlanTestRunCount/{projectSlug}")]
        public async Task<IActionResult> GetTestCaseTestPlanTestRunCount(string projectSlug)
        {
            return HandleResult(await _iDashboardService.GetTestCaseTestPlanTestRunCountAsync(projectSlug));
        }

        [HttpGet]
        [Route("GetTestRunStatusCountById/{testRunId}")]
        public async Task<IActionResult> GetTestRunStatusCountByTestRunId(int testRunId)
        {
            return HandleResult(await _iDashboardService.GetTestRunStatusCountByTestRunIdAsync(testRunId));
        }

        [HttpGet]
        [Route("GetTestRunList/{projectSlug}")]
        public async Task<IActionResult> GetTestRunList(string projectSlug)
        {
            return HandleResult(await _iDashboardService.GetTestRunListAsync(projectSlug));
        }

        [HttpGet]
        [Route("GetTestCaseRepositoryList/{projectSlug}")]
        public async Task<IActionResult> GetTestCaseRepositoryList(string projectSlug)
        {
            return HandleResult(await _iDashboardService.GetTestCaseRepositoryListAsync(projectSlug));
        }

        [HttpGet]
        [Route("GetFunctionTestCaseListCount/{projectSlug}/{projectModuleId}")]
        public async Task<IActionResult> GetFunctionTestCaseListCount(string projectSlug, int projectModuleId)
        {
            return HandleResult(await _iDashboardService.GetFunctionTestCaseListCountAsync(projectSlug, projectModuleId));
        }

        [HttpGet]
        [Route("GetDefaultFunctionTestCaseListCountByProjectSlug/{projectSlug}")]
        public async Task<IActionResult> GetDefaultFunctionTestCaseListCountByProjectSlug(string projectSlug)
        {
            return HandleResult(await _iDashboardService.GetDefaultFunctionTestCaseListCountByProjectSlugAsync(projectSlug));
        }

        [HttpGet]
        [Route("GetTestCaseListDetailStatusCount/{projectSlug}")]
        public async Task<IActionResult> GetTestCaseListDetailStatusCount(string projectSlug)
        {
            return HandleResult(await _iDashboardService.GetTestCaseListDetailStatusCountAsync(projectSlug));
        }

        [HttpGet]
        [Route("GetTestCaseTestPlanTestRunCount")]
        public async Task<IActionResult> GetTestCaseTestPlanTestRunCount()
        {
            return HandleResult(await _iDashboardService.GetTestCaseTestPlanTestRunCountAsync());
        }

        [HttpGet]
        [Route("GetProjectCount")]
        public async Task<IActionResult> GetProjectCount()
        {
            return HandleResult(await _iDashboardService.GetProjectCountAsync());
        }

        [HttpGet]
        [Route("GetUserCount")]
        public async Task<IActionResult> GetUserCount()
        {
            return HandleResult(await _iDashboardService.GetUserCountAsync());
        }

        [HttpGet]
        [Route("GetTestCaseCountFromLastMonth")]
        public async Task<IActionResult> GetTestCaseCountFromLastMonth()  
        {
            return HandleResult(await _iDashboardService.GetTestCaseCountFromLastMonthAsync());
        }
    }
}