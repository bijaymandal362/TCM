using System.Collections.Generic;

namespace Models.Constant.Authorization
{
    public static class PermissionCheckControllers
    {
        public static Dictionary<string, List<string>> PermissionCheckDetails;
        static PermissionCheckControllers()
        {
            PermissionCheckDetails = new Dictionary<string, List<string>>
            {
                {
                    Controllers.Project,
                    new List<string>
                    {
                        ActionMethod.GetProjectList,
                        ActionMethod.GetProjectListFilterByModule,
                        ActionMethod.GetProjectByProjectId,
                    }
                }
            };
        }
        public static bool ValidateControllerActionMethod(string controllerName, string actionName)
        {
            if (PermissionCheckDetails.ContainsKey(controllerName) && PermissionCheckDetails[controllerName].Contains(actionName))
                return true;

            return false;
        }
    }

    public class Controllers
    {
        public const string Project = "Project";
    }

    public class ActionMethod
    {
        //Project controller action methods
        public const string GetProjectList = "GetProjectList";
        public const string GetProjectListFilterByModule = "GetProjectListFilterByModule";
        public const string GetProjectByProjectId = "GetProjectByProjectId";
    }

}
