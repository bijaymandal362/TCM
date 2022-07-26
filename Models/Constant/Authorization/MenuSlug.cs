namespace Models.Constant.Authorization
{
    public class MenuSlug
    {
        //Menu Slug is unique

        //Project
        public const string ViewProject = "project.read";
        public const string CreateProject = "project.create";
        public const string UpdateProject = "project.update";
        public const string DeleteProject = "project.delete";

        //User
        public const string ViewUser = "user.read";
        public const string UpdateUser = "user.update";
        public const string CreateUser = "user.create";
        public const string DeleteUser = "user.delete";

        //ProjectMember
        public const string ViewProjectMember = "projectmember.read";
        public const string AddProjectMember = "projectmember.add";
        public const string UpdateProjectMember = "projectmember.update";
        public const string DeleteProjectMember = "projectmember.delete";
        

    }
}
