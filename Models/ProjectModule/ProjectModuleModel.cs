using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ProjectModule
{
    public class ProjectModuleModel
    {
        public int ProjectModuleId { get; set; }
        public int? ParentProjectModuleId { get; set; }
        public int ProjectId { get; set; }
        public string ModuleName { get; set; }
        public DateTimeOffset OrderDate { get; set; }

        public bool IsDeleted { get; set; }
        public int ProjectModuleListItemId { get; set; }
        public string ProjectModuleType { get; set; }
        public string Description { get; set; }
        public List<ProjectModuleModel> ChildModule { get; set; }
        public List<int> DeveloperListId { get; set; }

        public string ExpectedResult { get; set; }

    }

    public class ProjectModuleDeveloperListModel
    {
        public int PersonId { get; set; }
        public int ProjectModuleId { get; set; }
        public int ProjectModuleDeveloperId { get; set; }


    }
    public class AddUpdateProjectModuleModel
    {
        public int ProjectModuleId { get; set; }
        public int? ParentProjectModuleId { get; set; }
        public string ProjectSlug { get; set; }
        public string ModuleName { get; set; }
        public string ProjectModuleType { get; set; }
        public string Description { get; set; }

        public int TestCaseDetailId { get; set; }
        public string PreCondition { get; set; }
        public string ExpectedResult { get; set; }
        public int TestCaseListItemId { get; set; }
        public List<int> DeleteTestCaseStepDetailId { get; set; }
        public List<TestCaseStepDetailModel> TestCaseStepDetailModel { get; set; }

    }
    
   

    public class TestCaseStepDetailModel
    {
        public int TestCaseStepDetailId { get; set; }
        public int TestCaseStepDetailProjectModuleId { get; set; }
        public string ExpectedResultTestStep { get; set; }
        public int StepNumber { get; set; }
        public string StepDescription { get; set; }
        

    }
    public class GetDeveloperDetailModel
    {
        public int FunctionId { get; set; }
        public int ProjectModuleDeveloperId { get; set; }
        public int ProjectModuleId { get; set; }


        public List<ProjectModuleDeveloperFunctionModel> ProjectModuleDeveloperFunctionModel { get; set; }
    }

    public class ProjectModuleDeveloperFunctionModel
    {
        public int ProjectModuleDeveloperId { get; set; }
        public int ProjectMemberId { get; set; }
        public string Member { get; set; }
        public bool IsDisabled { get; set; }

    }

    public class TestCaseModel
    {
        public int TestCaseDetailId { get; set; }
        public int ProjectModuleId { get; set; }
        public string PreCondition { get; set; }
        public string ExpectedResult { get; set; }
        public string TestCases { get; set; }

    }
    public class AddProjectModuleDeveloperModel
    {
        public int ProjectModuleDeveloperId { get; set; }
        public int ProjectModuleId { get; set; }
        public List<int> ProjectMemberId { get; set; }
        public bool IsDisabled { get; set; }

   
    }

    public class UpdateProjectModuleDeveloperModel
    {
        public int ProjectModuleDeveloperId { get; set; }
        public int ProjectModuleId { get; set; }
        public int ProjectMemberId { get; set; }
        public bool IsDisabled { get; set; }

     

    }
    public class UpdateProjectModuleListDeveloperModel
    {
        public List<UpdateProjectModuleDeveloperModel> UpdateProjectModuleDeveloperModel { get; set; }
      
    }

    public class ProjectModuleDeveloperModelList
    {
        public int ProjectModuleDeveloperId { get; set; }
        public int ProjectModuleId { get; set; }
        public int ProjectMemberId { get; set; }
        public string Member { get; set; }
        public bool IsDisabled { get; set; }



    }

    public class GetTestCaseDetailListModel
    {
        public int TestCaseDetailId { get; set; }

        public int ParentProjectModuleId { get; set; }
        public string PreCondition { get; set; }
        public string ExpectedResult { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public int TestCaseListItemId { get; set; }
        
        public List<TestCaseStepDetailModel> TestCaseStepDetailModel { get; set; }

    }

    public class ProjectMemberDeveloperListModel
    {
        public int ProjectMemberId { get; set; }
        public int PersonId { get; set; }
        public string ProjectSlug { get; set; }
        public string Developer { get; set; }

    }

    public class TestCaseViewModel
    {
        public string TestCaseName { get; set; }
        public string TestScenario { get; set; }
        public string Type { get; set; }
        public int Steps { get; set; }
        public string Description { get; set; }
        public string ExpectedResult { get; set; }
    }

    public class DragDropTestCaseDetail
    {
        public int ProjectModuleId { get; set; }       
        public string ProjectSlug { get; set; }
        public string ModuleName { get; set; }
        public string ProjectModuleType { get; set; }
        public string Description { get; set; }     
        public int? DragDropParentProjectModuleId { get; set; }
        public List<DragDropProjectModuleModel> DragDropProjectModuleModel { get; set; }

    }
    public class DragDropProjectModuleModel
    {
        public int ProjectModuleId { get; set; }
        public int? ParentProjectModuleId { get; set; }
        public string ProjectSlug { get; set; }
        public string ModuleName { get; set; }
        public string ProjectModuleType { get; set; }
        public string Description { get; set; }
        public DateTimeOffset OrderDate { get; set; }
        public List<DragDropProjectModuleModel> ChildModule { get; set; }

    }

    public class FunctionTestCaseModel
	{
		public int FunctionId { get; set; }

        public string FunctionName { get; set; }

        public List<TestCaseViewModel> TestCaseViewModel { get; set; }
    }

    public class TestCaseViewModelForExcel
    {
        public int FunctionId { get; set; }
        public string FunctionName { get; set; }
        public string TestCaseName { get; set; }
        public string TestScenario { get; set; }
        public string Type { get; set; }
        public int? Steps { get; set; }
        public string Description { get; set; }
        public string ExpectedResult { get; set; }
        public DateTimeOffset OrderDate { get; set; }
        public int ProjectModuleId { get; set; }
	}
    public class DownloadTestCaseModel
    {
        public int ProjectId { get; set; }
        public string ProjectSlug { get; set; }
    
	}
    
    public class DownloadTestCaseModelByFunctionId
    {
        public int ProjectId { get; set; }
        public int FunctionId { get; set; }
        public string ProjectSlug { get; set; }
        public string ModuleName { get; set; }
    
	}
	

    

   

}
