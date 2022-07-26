using Models.Core;
using Models.GridTableFilterModel;
using Models.GridTableProperty;
using Models.TestRun;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.TestRun
{
    public interface ITestRunService
    {
        Task<Result<PagedResponseModel<List<TestRunListModel>>>> GetTestRunListAsync(PaginationFilterModel filter, string projectSlug);
        Task<Result<GetAllTestRunListModel>> GetTestRunListbyIdAsync(int testRunId);
        Task<Result<string>> AddTestRunAsync(TestRunModel model);
        Task<Result<string>> UpdateTestRunAsync(UpdateTestRunModel model);
        Task<Result<string>> DeleteTestRunAsync(int testRunId);
        Task<Result<PagedResponsePersonModel<List<TestPlanByProjectSlugModel>>>> GetTestPlanbyProjectSlugAsync(FilterModel filter ,string projectSlug);
        Task<Result<PagedResponsePersonModel<List<ProjectMemberByProjectSlugModel>>>> GetProjectMemberListBySlugAsync(FilterModel filter ,string projectSlug);
        Task<Result<string>> AssignUserToTestCaseAsync(AssignUserToTestCaseModel model);
        Task<Result<string>> UnAssignUserToTestCaseAsync(UnAssignUserToTestCaseModel model);
        Task<Result<PagedResponsePersonModel<List<TestRunTestCasesModel>>>> GetTestRunTestCasesModelAsync(FilterModel filter, int testRunId);
        Task<Result<PagedResponseModel<List<TestRunTestCaseModel>>>> GetTestRunTestPlanByTestRunIdAsync(PaginationFilterModel filter, int testRunId);
        Task<Result<PagedResponseModel<List<TestRunTestCaseFilterModel>>>> GetTestRunTestCaseDetailsAsync(PaginationFilterModel filter, int testRunId, int testPlanId, int? projectMemeberI);
        Task<Result<List<TestRunTeamStatsModel>>> GetTestRunTeamStatsModelAsync(int testRunId);
        Task<Result<TestCaseResultsDataModel>> GetTestCaseResultsDataModelAsync(int testRunId, int testCaseId, int testPlanId);
        Task<Result<GetTestRunEdit>> GetEditTestRunByIdAsync(int testRunId);
        Task<Result<TestRunTestCaseWizardModel>> GetTestRunTestCaseWizardAsync(int testRunId, int testCaseId, int testPlanId);
        Task<Result<PagedResponseModel<List<TestRunTestCaseWithTestCaseIdModel>>>> GetTestRunTestCaseHistoryWizardAsync(PaginationFilterModel filter, int testCaseId);
        Task<Result<string>> UpdateTestRunTestCaseHistoryWizardAsync(UpdateTestRunTestCaseHistoryWizard model);
        Task<Result<string>> AddUpdateTestRunTestCaseStepHistoryWizardAsync(AddUpdateTestRunTestCaseStepHistoryWizard model);
        Task<Result<string>> DeleteTestRunTestPlanTestCaseIdAsync(int projectModuleId, int testPlanId, int testRunId);
        Task<Result<string>> DeleteMultipleTestCaseIdAsync(DeleteMultipleTestCaseModel model);

        //EnvironmentCrud
        Task<Result<string>> AddEnvironmentAsync(EnvironmentModel model);
        Task<Result<string>> UpdateEnvironmentAsync(EnvironmentModel model);
        Task<Result<string>> DeleteEnvironmentAsync(int environmentId);
        Task<Result<List<GetEnvironmentListModel>>> GetAllEnvironmentListAsync(string projectSlug);
        Task<Result<GetAllEnvironmentListModel>> GetEnvironmentByIdAsync(int environmentId);
        Task<Result<PagedResponseModel<List<GetAllEnvironmentListModel>>>> GetEnvironmentListAsync(PaginationFilterModel filter, string projectSlug);

        //ExportTestRunTestCase
        Task<byte[]> ExportTestRunTestCaseByTestRunIdAsync(int testRunId);
        Task<Result<PdfTestRunTestCaseReportModel>> GeneratePdfReportForTestRunTestCaseByTestRunIdAsync(int testRunId);

        //Refresh
        Task<Result<string>> RefreshTestPlanAsync(int testRunId, int testPlanId);

        //Retest TestCase
        Task<Result<string>> RetestTestPlanTestCaseIdAsync(RetestMultipleTestCaseModel retestModel);
        Task<DocumentFileDownloadModel> DownloadTestRunWizardStausFileAsync(int documentId);
        Task<Result<PagedResponseModel<List<TestRunTestPlanWithTestPlanIdModel>>>> GetTestRunTestPlanListAsync(PaginationFilterModel filterModel, int testPlanId);
    }
}
