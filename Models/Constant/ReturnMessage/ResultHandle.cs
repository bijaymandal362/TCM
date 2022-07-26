namespace Models.Constant.ReturnMessage
{
    public class ReturnMessage
    {
        //EventOutPut
        public const string SavedSuccessfully = "Saved Successfully";
        public const string UpdatedSuccessfully = "Updated Successfully";
        public const string DeletedSuccessfully = "Deleted Successfully";

        //ExceptionHanding
        //Project
        public const string FailedToAddProject = "Failed to add the project";
        public const string FailedToUpdateProject = "Failed to update the project";
        public const string FailedToDeleteProject = "Failed to delete the project";
        public const string FailedToShowTestRun = "Failed to show  the testrun";

        //ProjectMember
        public const string FailedToAddProjectMember = "Failed to add the projectmember";
        public const string FailedToGetTestCaseWithTestRunIdAndTestPlanId = "Failed to get testcase with testplanid";

        public const string FailedToFetchData = "Failed to fetch data";
        public const string FailedToUpdateProjectMember = "Failed to update the projectmember";
        public const string FailedToDeleteProjectMember = "Failed to delete the projectmember";
        public const string ProjectMemberIdNotFound = "ProjectMemberId not found";
        public const string PersonAlreadyExist = "Project member already added!";
        public const string UserAddedSuccessfully = "User added successfully";
        public const string UserUpdatedSuccessfully = "User updated successfully";
        public const string UserDeletedSuccessfully = "User deleted successfully";

        //
        public const string UnauthorizedUser = "Unauthorized User";

        //
        public const string ProjectNameCannotBeAdmin = "ProjectName cannot be admin";
        public const string ProjectNameisalreadyexistsPleasespecifyauniquename = "ProjectName is already exists, Please specify a unique name.";
        public const string ProjectNameCannotBeEmpty = "ProjectName cannot be empty";

        

        //User
        public const string FailedToUpdateUser = "Failed to update the user";
        public const string UserCountIsZero = "User count is zero";


        public const string FailedToRefreshTestplan = "Failed to refresh testplan";


        //ProjectModule
        public const string FailedToAddProjectModule = "Failed to add the projectmodule";
        public const string FailedToUpdateProjectModule = "Failed to update the projectmodule";
        public const string FailedToUpdateTestRunTestCaseHistory = "Failed to update the testruntestcasehistory";
        public const string FailedToAddTestCase = "Failed to add the testcase";
        public const string FailedToAddProjectModuleDeveloper = "Failed to add the projectmoduledeveloper";
        public const string FailedToUpdateProjectModuleDeveloper = "Failed to update the projectmoduledeveloper";
        public const string FailedToUpdateTestCase = "Failed to update the testcase";
        public const string FailedToDeleteTestCaseStepDetail = "Failed to delete the testcasestepdetail";
        public const string FailedToDeleteProjectModule = "Failed to delete project module";
        public const string FailedToDeleteProjectModuleFunction = "Failed to delete projectmodule function";
        public const string FailedToDeleteProjectModuleTestCase = "Failed to delete projectmodule testcase";
        public const string FailedToDeleteTestCase = "Failed to delete the testcase";
        public const string FailedToDeleteProjectModuleDeveloper = "Failed to delete project module developer";
        public const string ProjectModuleTypeIsNotFound = "ProjectModuleType not found";
        public const string TestCaseDetailId = "TestCaseDetailId not found";
        public const string FailedToImportTestCase = "Failed to import test case ";
        public const string FailedToDownloadTestCase = "Failed to download test case";
        public const string PleaseCheckTestCaseType = "Please check testcase type";
        public const string ModuleCanOnlyBeDragDropToModule = "Module can only be drag and drop to module";
        public const string FileFormateNotSupportedPleaseSelectExcelFileOnly = "File formate not supported, Please select excel file only";
        public const string FailedToUploadExcelStepValueIsAlphanumeric = "Failed to upload excel, Step value is alphanumeric";
        public const string TestCaseDetailIdNotFound = "Test case detail id not found";
        public const string FunctionIdNotFound = "Function id not found";
        public const string ProjectModuleIdNotFound = "ProjectModuleId not found";
        public const string TestCaseStepDetail = "TestCaseStepDetail not found";
        public const string TestCaseNeverBeZeroInTestPlanException = "There most be atlest one TestCase for TestPlan";
        public const string TestPlanNameCannotBeEmpty = "TestPlanName cannot be empty";
        public const string ModuleNameCannotBeEmpty = "TestCaseRepository Name cannot be empty";
        public const string TestCaseDataCannotBeEmpty = "Either Pre-Condition Or Expected Result cannot be empty";
        public const string TestCaseStepDetailDataCannotBeEmpty = "Either  Step Description Or Expected Result  cannot be empty";
        public const string FileNotUploadException = "Please upload file";
        public const string FunctionContainsTestCases = "Failed to delete function, it contains testcases";
        public const string ModuleContainsFunction = "Failed to delete module, it contains functions";
        public const string ModuleContainsNestedModule = "Failed to delete module, it contains nested module";
        public const string ProjectModuleTypeIsNotValid = "ProjectModuleType is not valid";
        public const string ProjectModuleNameAlreadyExist = "Module Name already exists, please enter a unique name";
        public const string ProjectFunctionNameAlreadyExist = "Function Name already exists, please enter a unique name";
        public const string ProjectTestCaseNameAlreadyExist = "TestCase Name already exists, please enter a unique name";
        public const string ModuleDeletedSuccessfully = "Module Deleted Successfully";
        public const string FunctionDeletedSuccessfully = "Function Deleted Successfully";
        public const string TestCaseDeletedSuccessfully = "TestCase Deleted Successfully"; 
        public const string TestCaseDoesnotContainsStep = "TestCase does not contains step"; 
        public const string TestCaseImportedSuccessfully = "TestCase Imported Succesfully."; 
        public const string ImproperFormat = "Improper Format."; 


        //
        public const string ProjectSlugDoesNotExists = "ProjectSlug doesnot exists";
        public const string ProjectModuleProjectSlugIdNotFound = "ProjectSlug or projectmoduleId not found";
        public const string TestRunIdOrTestCaseIdDoesnotExists = "TestRunId or TestCaseId doesnot Exists";
        public const string EnvironmentIdDoesnotExists = "EnvironmentId doesnot Exists";
        public const string FailedToUploadExcelHeaderColumnExceeded = "Failed to upload excel, number of header column exceeded."; 

        //
        public const string FailedToUpdatePersonPersonalization = "Failed to update person personalization";
     
        public const string FailedToUpdateEnvironment = "Failed to update environment";

        public const string RefreshSuccessfully = "Refresh Successfully";
        public const string RefreshSuccessfullyButThereIsNothingToAdd = "Refresh successfully but there is nothing to add new test case";



        ///TestCase
        public const string FailedToAddTestPlan = "Failed to add the testplan";
        public const string FailedToUpdateTestPlan = "Failed to update the testplan";
        public const string TestPlanTypeListItemIdNotFound = "TestPlanTypeListItemId not found";
        public const string FailedToDragOrDropTestPlan = "Failed to drag or drop testplan";
        public const string FailedToDeleteTestPlanTestCase = "Failed to delete the testplantestcase";
        public const string FailedToDeleteTestPlan = "Failed to delete the testplan";
        public const string TestPlanIdNotFound = "TestPlanId not found";
        public const string TestPlanTypeListItemIdNotFoundForTestPlan = "TestPlanTypeListItemId not found for testplan";
        public const string TestPlanTestCaseCountZeroException = "Please add testcase for adding testplan";
        public const string TestPlanIdCannotBeDeleted = "TestPlanId is used in testrun module";
        

        //TestRun
        public const string FailedToAddTestRun = "Failed to add the testrun";
        public const string FailedToUpdateTestRun = "Failed to update the testrun";
        public const string TestRunIdDoesnotExists = "TestRunId doesnot exists";
        public const string TestRunTestCaseHistoryIdDoesnotExists = "TestRunTestCaseHistoryId doesnot exists";
        public const string TestRunPlanIdDoesnotExists = "TestRunPlanId doesnot exists";
        public const string FailedToDeleteTestRun = "Failed to delete testrun";
        public const string UserSuccessfullyAssignedToTestCases = "User successfully assigned to Test Cases";
        public const string UserFailedAssignedToTestCases = "User failed to assign to Test Cases";
        public const string UserFailedUnAssignedToTestCases = "User failed to unassign to Test Cases";
        public const string UserSuccessfullyUnAssignedToTestCases = "User successfully unassigned from Test Cases";
        public const string TestPlanTestCaseIdDoesnotExists = "TestPlanTestCase id does not exists";
        public const string TestCaseIdNotFound = "TestCaseId Not Found";
        public const string NoDataFound = "Not Data Found";
        public const string FailedToRetestTestCase = "Failed To Retest TestCase"; 
        public const string PersonIdIsNotAssignToThisProjectException = "You are not assigned to this project as projectmember";
        public const string RetestSuccessfully = "Retest Successfully";
        public const string EnvironmentIdUsedInTestRun = "Failed to delete, Environment is used in the test run";

        //Environmnet
        public const string FailedToDeleteEnvironment = "Failed to delete the environment";
        public const string FailedToAddEnvironment = "Failed to add environment";
        public const string TestRunTitleCannotBeEmpty = "TestRunTitle cannot be empty";
        public const string EnvironmentNameShoulNotBeNullOrOnlySpace = "EnvironmentName should not empty or only space";
        public const string EnvironmentNameAlreadyExist = "Environment name already exists, please enter a unique name.";
        public const string EnvironmentAddedSuccessfully = "Environment added successfully";
        public const string EnvironmentUpdatedSuccessfully = "Environment updated successfully";
        public const string EnvironmentDeletedSuccessfully = "Environment deleted successfully";


        //ProjectStarred
        public const string FailedToAddProjectStarred = "Failed to add the project starred";
        public const string FailedToUpdateProjectStarred = "Failed to update the project starred";
        public const string FailedToDeleteProjectStarred = "Failed to delete the project starred";

        //TestPlan

        public const string TestPlanNameAlreadyExist = "Test Plan name already used";
        public const string FolderNameAlreadyExist = "Folder name already used";
        public const string DuplicateTestCaseFound = "Duplicate testcase found.";
        public const string DuplicateModuleFound = "Duplicate module found.";
        public const string DuplicateFunctionFound = "Duplicate function found.";
        

    }
}
