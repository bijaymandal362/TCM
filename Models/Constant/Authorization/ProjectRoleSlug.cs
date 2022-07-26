namespace Models.Constant.Authorization
{
    public class ProjectRoleSlug
    {
        //Project Role Slug is unique 

        //User
        public const string ViewUser = "projectrole.user.read";
        public const string UpdateUser = "projectrole.user.update";
        public const string CreateUser = "projectrole.user.create";
        public const string DeleteUser = "projectrole.user.delete";

        //ProjectMember
        public const string ViewProjectMember = "projectrole.projectmember.read";
        public const string CreateProjectMember = "projectrole.projectmember.create";
        public const string UpdateProjectMember = "projectrole.projectmember.update";
        public const string DeleteProjectMember = "projectrole.projectmember.delete";


        //ProjectModule
        public const string ViewProjectModule = "projectrole.projectmodule.read";
        public const string CreateProjectModule = "projectrole.projectmodule.create";
        public const string UpdateProjectModule = "projectrole.projectmodule.update";
        public const string DeleteProjectModule = "projectrole.projectmodule.delete";

        //ProjectModuleDeveloper
        public const string ViewProjectModuleDeveloper = "projectrole.projectmoduledeveloper.read";
        public const string CreateProjectModuleDeveloper = "projectrole.projectmoduledeveloper.create";
        public const string UpdateProjectModuleDeveloper = "projectrole.projectmoduledeveloper.update";
        public const string DeleteProjectModuleDeveloper = "projectrole.projectmoduledeveloper.delete";

        //TestCase
        public const string ViewTestCase = "projectrole.testcase.read";
        public const string CreateTestCase = "projectrole.testcase.create";
        public const string UpdateTestCase = "projectrole.testcase.update";
        public const string DeleteTestCase = "projectrole.testcase.delete";


        //TestPlan
        public const string ViewTestPlan = "projectrole.testplan.read";
        public const string CreateTestPlan = "projectrole.testplan.create";
        public const string UpdateTestPlan = "projectrole.testplan.update";
        public const string DeleteTestPlan = "projectrole.testplan.delete";
    }
}
