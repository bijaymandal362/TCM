using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Models.Dashboard
{
    public class DashboardModel
    {
        public string TestCase { get; set; }
        public int TestCaseCount { get; set; }

        public string TestPlan { get; set; }
        public int TestPlanCount { get; set; }

        public string TestRun { get; set; }
        public int TestRunCount { get; set; }
    }

    public class TestRunStatusCountModel
    {
        public int TestRunId { get; set; }
        public string TestRunName { get; set; }
        public string Pending { get; set; }
        public int PendingCount { get; set; }
        public string Passed { get; set; }
        public int PassedCount { get; set; }
        public string Failed { get; set; }
        public int FailedCount { get; set; }
        public string Blocked { get; set; }
        public int BlockedCount { get; set; }
    }

    public class TestRunListModels
    {
        public int TestRunId { get; set; }
        public string TestRunName { get; set; }
    }

    public class TestCaseRepositoryListModel
    {
        public int ProjectModuleId { get; set; }
        public int? ParentProjectModuleId { get; set; }
        public int ProjectId { get; set; }
        public string ModuleName { get; set; }
        public int ProjectModuleListItemId { get; set; }
        public string ProjectModuleType { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }
        public DateTimeOffset OrderDate { get; set; }
        public List<TestCaseRepositoryListModel> ChildModule { get; set; }
    }


    public class ProjectModuleListCountModel
    {
        public int ProjectModuleId { get; set; }
        public bool IsDelete { get; set; }
        public int? ParentProjectModuleId { get; set; }
        public string ModuleName { get; set; }
        public string ProjectModuleTypeListItemName { get; set; }
        public int ProjectId { get; set; }
        public int ProjectModuleTypeListItemId { get; set; }
    }
    public class FunctionTestCaseListCountModel
    {
        public int ProjectModuleId { get; set; }
        public string ModuleName { get; set; }
        public int? ParentProjectModuleId { get; set; }

        public int ProjectId { get; set; }

        public int TestCaseCount { get; set; }
    }

    public class TestCaseListDetail
    {
        public int TestCaseId { get; set; }
        public int? ParentProjectModuleId { get; set; }

        public string Function { get; set; }
        public string TestCaseName { get; set; }

        public string TestCaseScenario { get; set; }
        public string ExpectedResult { get; set; }

        public int TestCaseFailedCount { get; set; }
    }

    public class TestCaseCountModel
    {
        [JsonIgnore]
        public int Day { get; set; }

        [JsonIgnore]
        public DateTimeOffset DateTime { get; set; }

        public string Date { get; set; } 

        public int TestCaseCountPerDay { get; set; }
    }
}