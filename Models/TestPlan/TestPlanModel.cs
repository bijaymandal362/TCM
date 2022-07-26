using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.TestPlan
{

    public class AddUpdateTestPlanModel
    {
        public int TestPlanId { get; set; }
        public int? ParentTestPlanId { get; set; }
        public bool IsDeleted { get; set; }
        public string TestPlanName { get; set; }
        public DateTimeOffset OrderDate { get; set; }
        public string Title { get; set; }
        public string ProjectSlug { get; set; }
        public string Description { get; set; }
     
        public int TestPlanIdForTestPlanTestCase { get; set; }
        public int TestPlanTypeListItemId { get; set; }
        public string TestPlanType { get; set; }
#nullable enable
        public List<int>? testPlanTestCaseId { get; set; }
        public List<int>? ProjectModuleId { get; set; }
        #nullable disable
    }


    public class TestPlanListModel
    {
        public int TestPlanId { get; set; }
        public int? ParentTestPlanId { get; set; }
        public string TestPlanName { get; set; }
        public DateTimeOffset OrderDate { get; set; }
        public string Title { get; set; }
        public int ProjectId { get; set; }
        public string ProjectSlug { get; set; }
        public string Description { get; set; }  
        public int TestPlanTypeListItemId { get; set; }
        public string TestPlanType { get; set; }

        public List<TestPlanListModel> TestPlanChildModule { get; set; }

    }

    public class DragDropTestPlanModel
    {
        public int TestPlanId { get; set; }
        public int? ParentTestPlanId { get; set; }
        public int? DragDropTestPlanId { get; set; }     
        public string ProjectSlug { get; set; }  
        public int TestPlanTypeListItemId { get; set; }
        public List<DragDropOrderingTestPlanModel> DragDropOrderingView { get; set; }
    }
    public class DragDropOrderingTestPlanModel
    {
        public int TestPlanId { get; set; }
        public int? ParentTestPlanId { get; set; }
        public string TestPlanName { get; set; }
        public DateTimeOffset OrderDate { get; set; }
        public string Title { get; set; }
        public int ProjectId { get; set; }
        public string ProjectSlug { get; set; }
        public string Description { get; set; }
        public int TestPlanTypeListItemId { get; set; }
        public List<DragDropOrderingTestPlanModel> TestPlanChildModule { get; set; }
    }

    public class TestPlanTestCaseModel 
    {
        public int TestPlanTestCaseId { get; set; }      
        public string TestPlanName { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public int ProjectModuleId { get; set; }
        public string TestCaseName { get; set; }
        public string Scenario { get; set; }
        public string ExpectedResult { get; set; }
        public string Author { get; set; }


    }


}
