using Microsoft.AspNetCore.Http;
using Microsoft.VisualBasic;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Models.TestRun
{
	public class TestRunModel
	{
		public int TestRunId { get; set; }
		public int TestRunPlanId { get; set; }
		public int TestRunTestCaseHistoryId { get; set; }
		public List<int> TestPlanId { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public string ProjectSlug { get; set; }
		public int? EnvironmentId { get; set; }
		public int? DefaultAssigneeProjectMemberId { get; set; }
	}

	public class UpdateTestRunModel
	{
		public int TestRunId { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public int? EnvironmentId { get; set; }

	}

	public class AssignUserToTestCaseModel
	{
		public int? AssigneProjectMemberId { get; set; }
		public List<int> TestRunTestCaseHistoryId { get; set; }

	}

	public class DeleteMultipleTestCaseModel
	{
		public int TestRunId { get; set; }
		public List<TestPlanList> TestPlan { get; set; }




	}

	public class TestPlanList
	{
		public int TestPlanId { get; set; }

		public List<int> TestCaseId { get; set; }
	}

	public class RetestMultipleTestCaseModel
	{
		public int TestRunId { get; set; }
		public List<TestPlanList> TestPlan { get; set; }

	}
	public class UnAssignUserToTestCaseModel
	{
		public int? AssigneProjectMemberId { get; set; }
		public List<int> TestRunTestCaseHistoryId { get; set; }

	}

	public class TestPlanByProjectSlugModel
	{
		public int TestPlanId { get; set; }
		public string TestPlanName { get; set; }
	}

	public class ProjectMemberByProjectSlugModel
	{
		public int ProjectMemberId { get; set; }
		public string Name { get; set; }
		public string UserName { get; set; }
	}

	public class EnvironmentModel
	{
		public int EnvironmentId { get; set; }
		public string EnvironmentName { get; set; }
		public string? URL { get; set; }
		public string ProjectSlug { get; set; }

	}


	public class TestRunListModel
	{
		public int TestRunId { get; set; }
		public string Title { get; set; }
		public int? EnvironmentId { get; set; }
		public string Environment { get; set; }
		public List<TestRunStatus> Status { get; set; }
		public int StatusId { get; set; }
		public string StatusName { get; set; }
		public int? Time { get; set; }
		public int TestRunHistoryId { get; set; }
		public int ProjectModuleId { get; set; }
		public int TestPlanId { get; set; }
		public DateTimeOffset? InsertDate { get; set; }

	}
	public class TestRunStatus
	{

		public int StatusId { get; set; }
		public int StatusCount { get; set; }
		public string Status { get; set; }
	}

	public class GetAllTestRunListModel
	{
		public int TestRunId { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public List<TestRunStatus> Status { get; set; }
		public string CompletionRate { get; set; }
		public string StartedBy { get; set; }
		public int? TimeSpent { get; set; }
		public string StatusName { get; set; }

		public int ProjectModuleId { get; set; }
		public DateTimeOffset InsertDate { get; set; }



	}
	public class TestRunTestCasesModel
	{
		public int TestPlanId { get; set; }
		public string TestPlanTitle { get; set; }
		public List<TestCases> TestCases { get; set; }

	}
	public class TestRunTestPlanByTestRunIdModel
	{
		public int TestPlanId { get; set; }
		public string TestPlanName { get; set; }
		public int TestRunId { get; set; }
	}

	public class TestCases
	{
		public int ProjectModuleId { get; set; }
		public string TestCaseTitle { get; set; }
		public string Assignee { get; set; }
		public int? TimeSpent { get; set; }
		public string Results { get; set; }
		public DateTimeOffset InsertDate { get; set; }
		public int TestRunTestCaseHistoryId { get; set; }
		public int TestPlanTestCaseId { get; set; }
		public int TestRunHistoryId { get; set; }
		public int TestPlanId { get; set; }

	}

	public class TestRunTeamStatsModel
	{
		public int Id { get; set; }
		public int? TimeSpent { get; set; }
		public UserModel User { get; set; }
		public List<TestRunStatus> Status { get; set; }

	}
	public class UserModel
	{
		public int UserId { get; set; }
		public string Image { get; set; }
		public string Name { get; set; }
		public string Username { get; set; }
		public int Role { get; set; }
		public string RoleName { get; set; }
		public string Email { get; set; }

		public int? DefaultAssignee { get; set; }
		public string DefaultAssigneeName { get; set; }



	}

	public class TestCaseResultsDataModel
	{
		public int Id { get; set; }
		public string TestCaseTitle { get; set; }
		public DateTimeOffset UpdateDate { get; set; }
		public List<TestCaseResult> Results { get; set; }
	}
	public class TestCaseResult
	{
		public int UserId { get; set; }
		public string User { get; set; }
		public int? TimeSpent { get; set; }
		public string Status { get; set; }
		public DateTimeOffset? FinishTime { get; set; }
		public string Comment { get; set; }
		public int? TestRunTestCaseHistoryDocumentId { get; set; }
		public int? DocumentId { get; set; }

		public string FileName { get; set; }
		public int TestRunTestCaseHistoryId { get; set; }
		public DateTimeOffset UpdateDate { get; set; }
		public List<TestCaseStepsResult> StepsToReproduce { get; set; }

	}

	public class TestCaseStepsResult
	{
		public int TestRunTestCaseStepHistoryId { get; set; }
		public int TestCaseStepResultId { get; set; }
		public int? DocumentId { get; set; }
		public string Step { get; set; }
		public string ExpectedResult { get; set; }
		public string Status { get; set; }
		public string FileName { get; set; }
		public string Comment { get; set; }

	}

	public class TestRunTestCaseModel
	{
		public int TestRunId { get; set; }
		public int TestPlanId { get; set; }
		public string TestPlanName { get; set; }
		public int? PassedCount { get; set; }
		public int? FailedCount { get; set; }
		public int? BlockedCount { get; set; }
		public int? PendingCount { get; set; }

	}
	public class TestRunStatusModle
	{

		public int ProjectModuleId { get; set; }
		public int? Status { get; set; }
		public string? StatusListItemSystemName { get; set; }
	}
	public class TestRunTestCaseHistoryStatusModel
	{
		public int TestRunId { get; set; }
		public int TestPlanId { get; set; }
		public int ProjectModuleId { get; set; }
		public DateTimeOffset UpdateDate { get; set; }
		public string TestPlanName { get; set; }
		public int? Status { get; set; }
		public string? StatusListItemSystemName { get; set; }
		public int? TimeSpent { get; set; }
		public int TestRunTestCaseHistoryId { get; set; }

	}

	public class TimeSpentModel
	{
		public int TestRunId { get; set; }
		public int TestPlanId { get; set; }
		public int? TotalTimeSpent { get; set; }
		public int TimeSpent { get; set; }
		public DateTimeOffset UpdateDate { get; set; }
		public int ProjectModuleId { get; set; }
		public int TestRunTestCaseHistoryId { get; set; }
		public string Status { get; set; }

	}
	public class TestRunTestCaseListModel
	{
		public int TestRunId { get; set; }
		public int TestPlanId { get; set; }
		public int TestRunPlanDetailId { get; set; }
		public string TestPlanDetailJson { get; set; }
		public List<TestPlanJson> TestPlanDetailJsons { get; set; }
	}

	public class TestRunTestCaseDetailListModel
	{
		public int TestRunId { get; set; }
		public int TestPlanId { get; set; }
		public int TestRunPlanDetailId { get; set; }
		public string TestCaseDetailJson { get; set; }
		public List<TestPlanTestCaseJson> TestCaseDetailJsons { get; set; }

	}

	public class TestRunTestCaseFilterModel
	{
		public int TestRunId { get; set; }
		public int TestRunTestCaseHistoryId { get; set; }
		public int TestPlanId { get; set; }
		public int ProjectModuleId { get; set; }
		public string TestCaseName { get; set; }
		public int? TotalTimeSpent { get; set; }
		public int? AssigneeListItemId { get; set; }
		public int TestRunTestCaseStatusListItemId { get; set; }
		public string Assignee { get; set; }
		public string TestRunTestCaseStatusListItemSystemName { get; set; }
		public DateTimeOffset InsertDate { get; set; }

	}

	public class TestCaseStepResult
	{
		public int TestRunTestCaseStepHistoryId { get; set; }
		public int? TestRunTestCaseHistoryDocumentId { get; set; }
		public int? StepHistoryDocumentId { get; set; }
		public int TestCaseStepDetailId { get; set; }
		public int Step { get; set; }
		public string StepDescription { get; set; }
		public string FileName { get; set; }
		public string Extension { get; set; }
		public string ExpectedResult { get; set; }
		public string Status { get; set; }
		public string Comment { get; set; }
	}

	public class GetTestRunEdit
	{
		public int TestRunId { get; set; }
		public string Title { get; set; }
		public string? Environment { get; set; }
		public int? EnvironmentId { get; set; }
		public int? AssigneeId { get; set; }
		public string Assignee { get; set; }
		public string Description { get; set; }
		public List<int> TestPlanId { get; set; }
	}

	public class TestRunTestCaseHistory
	{
		public int TestRunHistoryId { get; set; }
		public string TestRunName { get; set; }

		public DateTimeOffset UpdateDate { get; set; }
		public int? TimeSpent { get; set; }
		public int TestRunStatusListItemId { get; set; }
		public string Status { get; set; }
	}
	public class TestRunTestCaseWizard
	{
		public int TestRunTestCaseHistoryId { get; set; }
		public int TestCaseDetail { get; set; }

		public string TestCaseName { get; set; }
		public string TestCaseScenario { get; set; }
		public string PreConditions { get; set; }
		public string ExpectedResult { get; set; }
		public string Environment { get; set; }
		public int? EnvironmentId { get; set; }
		public int TestRunStatusListItemId { get; set; }
		public string Status { get; set; }
		public List<TestCaseStepsResult> StepsToReproduce { get; set; }
		public List<TestRunTestCaseHistory> TestCaseRunHistory { get; set; }
	}
	public class TestRunTestCaseWizardModel
	{
		public int TestRunTestCaseHistoryId { get; set; }
		public int TestPlanId { get; set; }
		public int TestCaseDetail { get; set; }
		public string FileName { get; set; }
		public string Extension { get; set; }
		public int? TestRunTestCaseHistoryDocumentId { get; set; }
		public int? HistoryDocumentId { get; set; }
		public string TestCaseName { get; set; }
		public string TestCaseScenario { get; set; }
		public string PreConditions { get; set; }
		public string ExpectedResult { get; set; }
		public string Environment { get; set; }
		public int? EnvironmentId { get; set; }
		public int TestRunStatusListItemId { get; set; }
		public string Status { get; set; }
		public string Comment { get; set; }
		public List<TestCaseStepResult> StepsToReproduce { get; set; }

	}

	public class UpdateTestRunTestCaseHistoryWizard
	{
		public int TestRunTestCaseHistoryId { get; set; }

		public int? TimeSpent { get; set; }
		public int TestRunStatusListItemId { get; set; }
		public int DocumentId { get; set; }
		public string FileName { get; set; }
		public string Extension { get; set; }
		public IFormFile files { get; set; }
		public int TestRunTestCaseHistoryDocumentId { get; set; }
		public string Comment { get; set; }
	}
	public class GetAllEnvironmentListModel
	{
		public int EnvironmentId { get; set; }
		public string EnvironmentName { get; set; }
		public string? URL { get; set; }
		public DateTimeOffset UpdateDate { get; set; }
	}
	public class GetEnvironmentListModel
	{
		public int EnvironmentId { get; set; }
		public string EnvironmentName { get; set; }

	}

	public class TestRunTestCaseExportModel
	{
		public int SN { get; set; }
		public string TestCaseName { get; set; }
		public int TestPlanId { get; set; }
		public int TestCaseId { get; set; }

		public string TestPlanName { get; set; }
		public string Status { get; set; }


	}


	public class TestRunTestCaseExportTestResultCountModel
	{

		public string TestPlanNameForCount { get; set; }
		public int TestPlanId { get; set; }

		public int TotalPassedCount { get; set; }
		public int TotalFailedCount { get; set; }
		public int TotalPendingCount { get; set; }
		public int TotalBlockCount { get; set; }

	}
	public class FunctionModuleModel
	{
		public int? FunctionId { get; set; }
		public string FunctionName { get; set; }
		public int? TotalCountTestCaseByTestRunId { get; set; }
	}

	public class TestRunTestCaseCountPercentageModel
	{
		public decimal PassedPercentage { get; set; }
		public decimal FailedPercentage { get; set; }
		public decimal PendingPercentage { get; set; }
		public decimal BlockedPercentage { get; set; }

	}

	public class TestRunExcelModel
	{
		public string TestCaseManagementSystem { get; set; }
		public string ProjectName { get; set; }
		public string Environment { get; set; }
		public string Description { get; set; }
		public string? QA { get; set; }
		public int? EnvironmentId { get; set; }
	}
	public class TestRunExcelOrPdf
	{
		public int TestRunId { get; set; }
		public string? PDF { get; set; }
		public string? Excel { get; set; }
	}

	public class FunctionNameWithTestCaseDetailModel
	{
		public int? FunctionId { get; set; }
		public int TestPlanId { get; set; }
		public string TestPlanName { get; set; }

		public string FunctionName { get; set; }
		public List<TestCaseDetailModel> TestCaseDetail { get; set; }
	}

	public class FunctionNameModel
	{
		public int? FunctionId { get; set; }
		public string FunctionName { get; set; }

	}
	public class TestCaseDetailModel
	{
		public int? ProjectModuleId { get; set; }
		public int TestRunTestCaseHistoryId { get; set; }
		public int? ParentProjectModuleId { get; set; }
		public int TestPlanId { get; set; }
		public string TestCaseName { get; set; }
		public int TestCaseDetailId { get; set; }
		public string PreConditon { get; set; }
		public string ExceptedResult { get; set; }
		public string Status { get; set; }
		public string Remarks { get; set; }
	}
	public class AddUpdateTestRunTestCaseStepHistoryWizard
	{

		public int TestRunTestCaseStepHistoryId { get; set; }
		public int? TimeSpent { get; set; }
		public int TestRunStatusListItemId { get; set; }
		public int TestRunStatusListItemIdForNewHistory { get; set; }
		public int DocumentId { get; set; }
		public string FileName { get; set; }
		public string Extension { get; set; }
		public IFormFile files { get; set; }
		public int TestRunTestCaseHistoryDocumentId { get; set; }
		public string Comment { get; set; }
	}

	public class TestPlanJson
	{
		public int TestRunPlanId { get; set; }
		public int TestPlanId { get; set; }
		public int? ParentTestPlanId { get; set; }
		public string TestPlanName { get; set; }
		public int TestPlanTypeListItemId { get; set; }
		public string TestPlanTypeListItemName { get; set; }
		public string TestPlanTitle { get; set; }
		public string TestPlanDescription { get; set; }
		public int ProjectId { get; set; }
		public string ProjectName { get; set; }
		public string ProjectSlug { get; set; }
		public int ProjectMarketListItemId { get; set; }
		public string ProjectMarketListItemName { get; set; }
		public string ProjectDescription { get; set; }
		public int? EnvironmentId { get; set; }
		public string EnvironemntName { get; set; }
		public int ProjectMemberId { get; set; }

		public int? DefaultAssigneeProjectMemberId { get; set; }
		public int TestRunId { get; set; }
		public string TestRunName { get; set; }
		public string TestRunDescription { get; set; }
		public int InsertPersonId { get; set; }
		public int UpdatedPersonId { get; set; }
		public DateTimeOffset InsertDate { get; set; }
		public DateTimeOffset UpdateDate { get; set; }
	}

	public class TestPlanTestCaseJson
	{
		public int TestPlanTestCaseId { get; set; }
		public int ProjectModuleId { get; set; }
		public int? ParentProjectModuleId { get; set; }
		public string? FunctionName { get; set; }
		public string ProjectModuleName { get; set; }
		public int ProjectModuleListItemId { get; set; }
		public DateTimeOffset OrderDate { get; set; }
		public string ProjectModuleListItemName { get; set; }
		public string ProjectModuleDescription { get; set; }
		public int ProjectId { get; set; }
		public string ProjectName { get; set; }
		public string ProjectSlug { get; set; }
		public string ProjectDescription { get; set; }
		public int TestPlanId { get; set; }
		public int? ParentTestPlanId { get; set; }
		public string TestPlanName { get; set; }
		public int TestPlanTypeListItemId { get; set; }
		public string TestPlanTypeListItemName { get; set; }
		public string TestPlanTitle { get; set; }
		public string TestPlanDescription { get; set; }
		public int TestRunId { get; set; }
		public string TestRunName { get; set; }
		public int InsertPersonId { get; set; }
		public int UpdatedPersonId { get; set; }
		public DateTimeOffset InsertDate { get; set; }
		public DateTimeOffset UpdateDate { get; set; }
		public int TestCaseDetailId { get; set; }
		public string PreCondition { get; set; }
		public string ExpectedResult { get; set; }
		public int TestCaseListItemId { get; set; }
		public string TestCaseListItemName { get; set; }


	}
	public class TestRunTestRunHistoryModel
	{
		public int TestRunId { get; set; }
		public int TestRunHistoryId { get; set; }
		public string TestRunName { get; set; }
	}
	public class TestCaseStepDetailsJson
	{
		public int TestCaseStepDetailId { get; set; }

		public int StepNumber { get; set; }
		public string StepDescription { get; set; }

		public string ExpectedResult { get; set; }
		public int TestCaseListItemId { get; set; }

		public string TestCaseListItemName { get; set; }
		public int ProjectModuleId { get; set; }
		public int ProjectId { get; set; }
		public int? ParentProjectModuleId { get; set; }
		public string ProjectModuleName { get; set; }
		public int ProjectModuleListItemId { get; set; }
		public DateTimeOffset OrderDate { get; set; }
		public string ProjectModuleListItemName { get; set; }
		public string ProjectModuleDescription { get; set; }
		public int InsertPersonId { get; set; }
		public DateTimeOffset InsertDate { get; set; }
		public int UpdatePersonId { get; set; }
		public DateTimeOffset UpdateDate { get; set; }

		public int TestRunTestCaseStepHistoryId { get; set; }

		public int TestRunTestCaseHistoryId { get; set; }
		public int TestPlanId { get; set; }
		public string TestCaseStepStatusName { get; set; }


	}

	public class TestRunTestCaseStepStatusCount
	{
		public int TestRunStatusListItemIdForNewHistory { get; set; }
	}

	public class PdfExcelTestRunTestCaseReportModel
	{
		public TestRunExcelModel TestRunExcelModelTitle { get; set; }
		public List<TestRunTestCaseExportModel> TestRunTestCaseExportModelForExcelReport { get; set; }
		public List<FunctionModuleModel> RedarDiagram { get; set; }
		public TestRunTestCaseCountPercentageModel PieChart { get; set; }
		public List<TestRunTestCaseExportTestResultCountModel> BarChart { get; set; }
		public List<FunctionNameWithTestCaseDetailModel> FunctionTestCase { get; set; }
	}
	public class PdfTestRunTestCaseReportModel
	{
		public TestRunExcelModel TestRunExcelModelTitle { get; set; }
		public List<FunctionModuleModel> RedarDiagram { get; set; }
		public TestRunTestCaseCountPercentageModel PieChart { get; set; }
		public List<TestRunTestCaseExportTestResultCountModel> BarChart { get; set; }
		public List<FunctionNameWithTestCaseDetailModel> FunctionTestCase { get; set; }
	}

	public class TestRunPlanDetailByTestRunIdTestPlanIdModel
	{
		public int TestRunId { get; set; }
		public string TestRunName { get; set; }
		public int TestRunHistoryId { get; set; }
		public int TestPlanId { get; set; }
		public int TestRunPlanId { get; set; }
		public int TestRunPlanDetailId { get; set; }
		public string TestPlanDetailJson { get; set; }
		public string TestCaseDetailJson { get; set; }
		public string TestCaseStepDetailJson { get; set; }
		public List<TestPlanJson> TestPlanDetailJsons { get; set; }
		public List<TestPlanTestCaseJson> TestCaseDetailJsons { get; set; }
		public List<TestCaseStepDetailsJson> TestCaseStepDetailJsons { get; set; }


	}
	public class TestCaseDetailAsyncModel
	{
		public int TestCaseDetailId { get; set; }
		public int ProjectModuleId { get; set; }
		public string PreCondition { get; set; }
		public string ExpectedResult { get; set; }
		public string TestCaseListItemName { get; set; }
		public int TestCaseListItemId { get; set; }
	}

	public class TestCaseDetail
	{
		public int ProjectModuleId { get; set; }

		public int TestCaseListItemId { get; set; }
		public string TestCaseListItemName { get; set; }
		public int TestCaseDetailId { get; set; }
		public string PreCondition { get; set; }
		public string ExpectedResult { get; set; }

	}
	public class TestPlanTestCaseModel
	{
		public int ProjectModuleId { get; set; }
		public int ProjectModuleListItemId { get; set; }
		public string ProjectModuleListItemName { get; set; }
		public int TestPlanTypeListItemId { get; set; }
		public string TestPlanTypeListItemName { get; set; }
		public int TestPlanId { get; set; }

	}
	public class TestPlanStatusModel
	{
		public int ProjectModuleId { get; set; }
		public DateTimeOffset InsertDate { get; set; }
		public int TestRunStatusListItemId { get; set; }
		public int TestRunTestCaseHistoryId { get; set; }
		public string TestRunStatusName { get; set; }
		public int TestPlanId { get; set; }

	}

	public class TestPlanStatusFilterMode
	{
		public int TestPlanId { get; set; }
		public List<TestCaseHistoryListModel> TestCaseHistoryList { get; set; }
	}

	public class TestCaseHistoryListModel
	{
		public int TestPlanId { get; set; }
		public int PassedCount { get; set; }
		public int PendingCount { get; set; }
		public int FailedCount { get; set; }
		public int BlockedCount { get; set; }
		public string TestPlanName { get; set; }
		public int TestRunTestCaseHistoryId { get; set; }
		public int ProjectModuleId { get; set; }
		public int TestRunStatusListItemId { get; set; }
		public string TestRunStatusName { get; set; }
	}

	public class TestPlanDetailJsonModel
	{
		public int TestPlanId { get; set; }
		public string TestPlanDetail { get; set; }
		public List<TestPlanJson> TestPlanDetailJson { get; set; }
	}

	public class TestRunPlanDetailFunctionTestCaseModel
	{
		public string TestCaseDetail { get; set; }
		public List<TestPlanTestCaseJson> TestCaseDetailJson { get; set; }
	}

	public class FunctionTestCaseCountModel
	{
		public int? ParentModuleId { get; set; }
		public int? ParentProjectModuleId { get; set; }
		public int Count { get; set; }
		public string TestCaseName { get; set; }
	}

	public class FunctionNameTestCaseCountResultModel
	{
		public int TestPlanId { get; set; }
		public List<int> TestCaseId { get; set; }
		public List<TestPlanJson> TestPlanDetailJson { get; set; }
		public List<TestPlanTestCaseJson> TestCaseDetailJson { get; set; }
	}
	public class FunctionNameTestCaseCountModel
	{
		public int TestRunHistoryId { get; set; }
		public int TestRunTestCaseHistoryId { get; set; }
		public int TestRunId { get; set; }
		public int TestRunPlanId { get; set; }
		public int TestPlanId { get; set; }
		public int TestCaseId { get; set; }
		public int ProjectModuleId { get; set; }
		public int? ParentProjectModuleId { get; set; }
		public int TestRunStatusListItemId { get; set; }
		public int? TimeSpent { get; set; }
		public string TestCaseDetailStatus { get; set; }
		public string TestRunTestCaseStatus { get; set; }
		public string TestCaseDetail { get; set; }
		public string TestRunNameTitle { get; set; }
		public DateTimeOffset InsertDate { get; set; }

	}

	public class FunctionNameTestCaseJsonModel
	{
		public int TestPlanId { get; set; }
		public int ProjectModuleId { get; set; }
		public int TestRunTestCaseHistoryId { get; set; }
		public string TestPlanDetail { get; set; }
		public DateTimeOffset UpdateDate { get; set; }
		public string TestCaseDetail { get; set; }
		public List<TestPlanJson> TestPlanDetailJson { get; set; }
		public List<TestPlanTestCaseJson> TestCaseDetailJson { get; set; }
	}
	public class TestCaseStatusListModel
	{
		public int ProjectModuleId { get; set; }
		public DateTimeOffset InsertDate { get; set; }
		public int TestRunId { get; set; }
		public int TestPlanId { get; set; }
		public string TestCaseStatus { get; set; }
	}

	public class TestRunTestCaseStatusResultModel
	{
		public int TestPlanId { get; set; }
		public int ProjectModuleId { get; set; }
		public int TestRunTestCaseHistoryId { get; set; }
		public List<TestPlanJson> TestPlanDetailJson { get; set; }
		public List<TestPlanTestCaseJson> TestCaseDetailJson { get; set; }
	}

	public class TestCaseStepsUpdateModel
	{
		public int ProjectModuleId { get; set; }
		public int TestRunTestCaseHistoryId { get; set; }
		public int TestCaseStepDetailId { get; set; }
		public int TestPlanId { get; set; }
		public int TestRunTestCaseStepHistoryId { get; set; }
		public DateTimeOffset? StartTime { get; set; }
		public DateTimeOffset? EndTime { get; set; }
		public int? TotalTimeSpent { get; set; }
		public int TestRunStatusListItemId { get; set; }

	}
	public class DocumentFileDownloadModel
	{
		public int DocumentId { get; set; }
		public byte[] File { get; set; }
		public string Extension { get; set; }
		public string FileName { get; set; }
		public string ContentType { get; set; }

	}

	public class DocumentFileModel
	{
		public int DocumentId { get; set; }
		public string Extension { get; set; }
		public DateTimeOffset UpdateDate { get; set; }
		public string FileName { get; set; }


	}

	public class GetTestRunTestCaseDetailJsonModel
	{
		public int TestRunId { get; set; }
		public int TestPlanId { get; set; }
		public int TestRunPlanDetailId { get; set; }
		public string TestPlanDetailJson { get; set; }
		public string TestCaseDetailJson { get; set; }
		public string TestCaseStepDetailJson { get; set; }
		public List<TestPlanJson> TestPlanDetailJsons { get; set; }
		public List<TestPlanTestCaseJson> TestCaseDetailJsons { get; set; }
		public List<TestCaseStepDetailsJson> TestCaseStepDetailJsons { get; set; }

	}

	public class GetTestRunTestCaseWizardDetail
	{
		public int TestRunId { get; set; }
		public int TestPlanId { get; set; }
		public int TestCaseId { get; set; }
		public int TestCaseDetailId { get; set; }
		public int TestRunHistoryId { get; set; }
		public int TestRunTestCaseStepHistoryStatusId { get; set; }
		public int TestCaseStepDetailId { get; set; }
		public int ProjectModuleId { get; set; }
		public int TestRunTestCaseHistoryId { get; set; }
		public int TestRunTestCaseStepHistoryId { get; set; }
		public int? EnvironmentId { get; set; }
		public int TestRunStatusListItemId { get; set; }
		public int? TimeSpent { get; set; }
		public DateTimeOffset InsertDate { get; set; }
		public DateTimeOffset UpdateDate { get; set; }
		public DateTimeOffset UpdateDateStep { get; set; }
		public string Environment { get; set; }
		public string TestRunTestCaseStatus { get; set; }
		public string TestRunTestCaseStepHistoryStatus { get; set; }
		public string TestRunNameTitle { get; set; }


	}

	public class GetTestRunTestCaseWizardDetailResult
	{
		public int TestRunId { get; set; }
		public int TestPlanId { get; set; }
		public int? TestRunTestCaseDoucmentId { get; set; }
		public List<int?> TestRunTestCaseStepDoucmentId { get; set; }
		public int TestCaseId { get; set; }
		public int TestCaseDetailId { get; set; }
		public List<int> TestCaseStepDetailId { get; set; }
		public int ProjectModuleId { get; set; }
		public int TestRunTestCaseHistoryId { get; set; }
		public List<int> TestRunTestCaseStepHistoryId { get; set; }
		public int? EnvironmentId { get; set; }
		public int TestRunStatusListItemId { get; set; }
		public int? TimeSpent { get; set; }
		public string Environment { get; set; }
		public string TestRunTestCaseStatus { get; set; }
		public string TestRunTestCaseStepHistoryStatus { get; set; }
	}

	public class TestRunTestCaseHistoryModelWizard
	{
		public int TestRunTestCaseHistoryId { get; set; }
		public int TestRunTestCaseStepHistoryId { get; set; }
		public int TestCaseStepDetailId { get; set; }
		public string TestRunTestCaseHistoryStatus { get; set; }
	}

	public class DocumentTestRunTestCaseModel
	{
		public int? DocumentId { get; set; }
		public int TestRunTestCaseHistoryId { get; set; }

	}

	public class DocumentTestRunTestStepCaseModel
	{
		public int? DocumentId { get; set; }
		public int TestRunTestCaseStepHistoryId { get; set; }

	}

	public class TestRunTestCaseWithTestCaseIdModel
	{
		public int TestRunTestCaseHistoryId { get; set; }
		public int? TimeSpent { get; set; }
		public int ProjectModuleId { get; set; }
		public int TestRunStatusListItemId { get; set; }
		public string TestRunName { get; set; }
		public string Status { get; set; }
		public DateTimeOffset UpdateDate { get; set; }

	}

	public class TestRunTestPlanWithTestPlanIdModel
	{
		public int TestPlanId { get; set; }
		public int TestRunId { get; set; }
		public int TestRunTestCaseHistoryId { get; set; }
		public int PassedCount { get; set; }
		public int FailedCount { get; set; }
		public int BlockedCount { get; set; }
		public int PendingCount { get; set; }
		public int? TimeSpent { get; set; }
		public string TestRunName { get; set; }
		public string TestPlanName { get; set; }
		public DateTimeOffset UpdateDate { get; set; }


	}
}
