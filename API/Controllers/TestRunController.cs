using BusinessLayer.TestRun;
using Microsoft.AspNetCore.Mvc;
using Models.GridTableFilterModel;
using Models.TestRun;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestRunController : BaseApiController
    {
        private readonly ITestRunService _iTestRunService;
        
        public TestRunController(ITestRunService iTestRunService)
        {
            _iTestRunService = iTestRunService;           
        }

        [HttpPost]
        [Route("AddTestRun")]
        public async Task<IActionResult> AddTestRun([FromBody] TestRunModel model)
        {
            return HandleResult(await _iTestRunService.AddTestRunAsync(model));
        }

        [HttpPut]
        [Route("UpdateTestRun")]
        public async Task<IActionResult> UpdateTestRun([FromBody] UpdateTestRunModel model)
        {
            return HandleResult(await _iTestRunService.UpdateTestRunAsync(model));
        }

        [HttpDelete]
        [Route("DeleteTestRun/{testRunId}")]
        public async Task<IActionResult> DeleteTestRun(int testRunId)
        {
            return HandleResult(await _iTestRunService.DeleteTestRunAsync(testRunId));
        }

        [HttpPut]
        [Route("AssignUserToTestCase")]
        public async Task<IActionResult> AssignUserToTestCase([FromBody] AssignUserToTestCaseModel model)
        {
            return HandleResult(await _iTestRunService.AssignUserToTestCaseAsync(model));
        }

        [HttpPut]
        [Route("UnAssignUserToTestCase")]
        public async Task<IActionResult> UnAssignUserToTestCase([FromBody] UnAssignUserToTestCaseModel model)
        {
            return HandleResult(await _iTestRunService.UnAssignUserToTestCaseAsync(model));
        }

        [HttpGet]
        [Route("GetTestPlanbyProjectSlug/{projectSlug}")]
        public async Task<IActionResult> GetTestPlanbyProjectSlug([FromQuery] FilterModel filter, string projectSlug)
        {
            return HandleResult(await _iTestRunService.GetTestPlanbyProjectSlugAsync(filter, projectSlug));
        }

        [HttpGet]
        [Route("GetProjectMemberListBySlug/{projectSlug}")]
        public async Task<IActionResult> GetProjectMemberListBySlugAsync([FromQuery] FilterModel filter, string projectSlug)
        {
            return HandleResult(await _iTestRunService.GetProjectMemberListBySlugAsync(filter, projectSlug));
        }

        [HttpPost("GetTestRunList/{projectSlug}")]
        public async Task<IActionResult> GetTestRunList([FromQuery] PaginationFilterModel filter, string projectSlug)
        {
            return HandleResult(await _iTestRunService.GetTestRunListAsync(filter, projectSlug));
        } 

        [HttpGet("GetTestRunListbyId/{testRunId}")]
        public async Task<IActionResult> GetTestRunListbyId(int testRunId)
        {
            return HandleResult(await _iTestRunService.GetTestRunListbyIdAsync(testRunId));
        }

        [HttpGet("GetTestRunTestCases/{testRunId}")]
        public async Task<IActionResult> GetTestRunTestCases([FromQuery] FilterModel filter, int testRunId)
        {
            return HandleResult(await _iTestRunService.GetTestRunTestCasesModelAsync(filter, testRunId));
        }


        [HttpGet("GetTestRunTeamStats/{testRunId}")]
        public async Task<IActionResult> GetTestRunTeamStats(int testRunId)
        {
            return HandleResult(await _iTestRunService.GetTestRunTeamStatsModelAsync(testRunId));
        }

        [HttpGet("GetTestCaseResultsData/{testRunId}/{testCaseId}/{testPlanId}")]
        public async Task<IActionResult> GetTestCaseResultsData(int testRunId, int testCaseId, int testPlanId)
        {
            return HandleResult(await _iTestRunService.GetTestCaseResultsDataModelAsync(testRunId, testCaseId, testPlanId));
        }


        [HttpGet("GetEditTestRunById/{testRunId}")]
        public async Task<IActionResult> GetEditTestRunById(int testRunId)
        {
            return HandleResult(await _iTestRunService.GetEditTestRunByIdAsync(testRunId));
        }

        [HttpGet("GetTestRunTestCaseWizard/{testRunId}/{testCaseId}/{testPlanId}")]
        public async Task<IActionResult> GetTestRunTestCaseWizard(int testRunId, int testCaseId, int testPlanId)
        {
            return HandleResult(await _iTestRunService.GetTestRunTestCaseWizardAsync(testRunId, testCaseId, testPlanId));
        }


        [HttpPut]
        [Route("UpdateTestRunTestCaseHistoryWizardAsync")]
        public async Task<IActionResult> UpdateTestRunTestCaseHistoryWizard([FromForm] UpdateTestRunTestCaseHistoryWizard model)
        {
            return HandleResult(await _iTestRunService.UpdateTestRunTestCaseHistoryWizardAsync(model));
        }

        [HttpPut]
        [Route("AddUpdateTestRunTestCaseStepHistoryWizardAsync")]
        public async Task<IActionResult> AddUpdateTestRunTestCaseStepHistoryWizard([FromForm] AddUpdateTestRunTestCaseStepHistoryWizard model)
        {
            return HandleResult(await _iTestRunService.AddUpdateTestRunTestCaseStepHistoryWizardAsync(model));
        }

       
        //EnvironmentCrud
        [HttpPost]
        [Route("AddEnvironment")]
        public async Task<IActionResult> AddEnvironment([FromBody] EnvironmentModel model)
        {
            return HandleResult(await _iTestRunService.AddEnvironmentAsync(model));
        }

        [HttpPut]
        [Route("UpdateEnvironment")]
        public async Task<IActionResult> UpdateEnvironment([FromBody] EnvironmentModel model)
        {
            return HandleResult(await _iTestRunService.UpdateEnvironmentAsync(model));
        }

        [HttpDelete]
        [Route("DeleteEnvironment/{environmentId}")]
        public async Task<IActionResult> DeleteEnvironment(int environmentId)
        {
            return HandleResult(await _iTestRunService.DeleteEnvironmentAsync(environmentId));
        }

        [HttpPost("GetEnvironmentList/{projectSlug}")]
        public async Task<IActionResult> GetEnvironmentList([FromQuery] PaginationFilterModel filter, string projectSlug)
        {
            return HandleResult(await _iTestRunService.GetEnvironmentListAsync(filter, projectSlug));
        }

        [HttpGet("GetEnvironmentById/{environmentId}")]
        public async Task<IActionResult> GetEnvironmentById(int environmentId)
        {
            return HandleResult(await _iTestRunService.GetEnvironmentByIdAsync(environmentId));
        }
       
        [HttpGet]
        [Route("ExportTestRunTestCaseByTestRunId/{testRunId}")]
        public async Task<IActionResult> ExportTestRunTestCaseByTestRunId(int testRunId)
        {
            var excel = await _iTestRunService.ExportTestRunTestCaseByTestRunIdAsync(testRunId);
            return File(excel, "application/ms-excel", "TestRunTestCaseByTestRunId.xlsx");

        } 
        
        [HttpGet]
        [Route("GeneratePdfReportForTestRunTestCaseByTestRunId/{testRunId}")]
        public async Task<IActionResult> GeneratePdfReportForTestRunTestCaseByTestRunId(int testRunId)
        {
            return HandleResult(await _iTestRunService.GeneratePdfReportForTestRunTestCaseByTestRunIdAsync(testRunId));
        }


        [HttpDelete]
        [Route("DeleteTestRunTestPlanTestCaseId/{projectModuleId}/{testPlanId}/{testRunId}")]
        public async Task<IActionResult> DeleteTestRunTestPlanTestCaseId(int projectModuleId, int testPlanId, int testRunId)
        {
            return HandleResult(await _iTestRunService.DeleteTestRunTestPlanTestCaseIdAsync(projectModuleId, testPlanId, testRunId));
        }


        [HttpDelete]
        [Route("DeleteMultipleTestCaseId")]
        public async Task<IActionResult> DeleteMultipleTestCaseId([FromBody] DeleteMultipleTestCaseModel model)
        {
            return HandleResult(await _iTestRunService.DeleteMultipleTestCaseIdAsync(model));
        }


        [HttpPut]
        [Route("RetestTestPlanTestCaseId")]
        public async Task<IActionResult> RetestTestPlanTestCaseId([FromBody]RetestMultipleTestCaseModel retestModel)
        {
            return HandleResult(await _iTestRunService.RetestTestPlanTestCaseIdAsync(retestModel));
        }


        [HttpGet("GetAllEnvironmentList/{projectSlug}")]
        public async Task<IActionResult> GetAllEnvironmentList(string projectSlug)
        {
            return HandleResult(await _iTestRunService.GetAllEnvironmentListAsync(projectSlug));
        }

        [HttpPost]
        [Route("RefreshTestPlan/{testRunId}/{testPlanId}")]
        public async Task<IActionResult> RefreshTestPlan(int testRunId , int testPlanId)
        {
            return HandleResult(await _iTestRunService.RefreshTestPlanAsync(testRunId, testPlanId));
        }

        [HttpGet]
        [Route("DownloadTestRunWizardStausFile/{documentId}")]
        public async Task<IActionResult> DownloadTestRunWizardStausFile(int documentId)
        {
            var fileTestCaseWizard = await _iTestRunService.DownloadTestRunWizardStausFileAsync(documentId);  
            return File(fileTestCaseWizard.File, $"{fileTestCaseWizard.ContentType}", $"{fileTestCaseWizard.FileName}");
           
        }

        [HttpPost("GetTestRunTestCaseWizard/{testCaseId}")]
        public async Task<IActionResult> GetTestRunTestCaseHistoryWizard([FromQuery] PaginationFilterModel filter, int testCaseId)
        {
            return HandleResult(await _iTestRunService.GetTestRunTestCaseHistoryWizardAsync(filter, testCaseId));
        } 
        
        [HttpPost("GetTestRunTestPlanList/{testPlanId}")]
        public async Task<IActionResult> GetTestRunTestPlanList([FromQuery] PaginationFilterModel filter, int testPlanId)
        {
            return HandleResult(await _iTestRunService.GetTestRunTestPlanListAsync(filter, testPlanId));
        }

        [HttpPost("GetTestRunTestPlanByTestRunId/{testRunId}")]
        public async Task<IActionResult> GetTestRunTestPlanByTestRunId([FromQuery] PaginationFilterModel filter, int testRunId)
        {
            return HandleResult(await _iTestRunService.GetTestRunTestPlanByTestRunIdAsync(filter, testRunId));
        }
        
        [HttpPost("GetTestRunTestCaseDetails/{testRunId}/{testPlanId}/{projectMemeberId}")]
        public async Task<IActionResult> GetTestRunTestCaseDetails([FromQuery] PaginationFilterModel filter, int testRunId , int testPlanId, int? projectMemeberId)
        {
            return HandleResult(await _iTestRunService.GetTestRunTestCaseDetailsAsync(filter, testRunId, testPlanId, projectMemeberId));
        }
    }
}
