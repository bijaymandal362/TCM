using Models.Constant.Authorization;
using Models.Constant.ListItem;
using System.Collections.Generic;
using System.Linq;

namespace Data.MenuRoleSeed
{
	public static class ProjectRolePermission
	{
		public static List<ProjectRolePermissionModel> projectRolePermission;
		static ProjectRolePermission()
		{
			projectRolePermission = new List<ProjectRolePermissionModel>
			{
               // For ProjectRole Owner
               new ProjectRolePermissionModel{ ProjectRoleSystemName = ProjectRoleListItem.Owner, ProjectRolePermissionSlug = ProjectRoleSlug.ViewUser },
			   new ProjectRolePermissionModel{ ProjectRoleSystemName = ProjectRoleListItem.Owner, ProjectRolePermissionSlug = ProjectRoleSlug.CreateUser },
			   new ProjectRolePermissionModel{ ProjectRoleSystemName = ProjectRoleListItem.Owner, ProjectRolePermissionSlug = ProjectRoleSlug.UpdateUser },
			   new ProjectRolePermissionModel{ ProjectRoleSystemName = ProjectRoleListItem.Owner, ProjectRolePermissionSlug = ProjectRoleSlug.DeleteUser },

			   new ProjectRolePermissionModel{ ProjectRoleSystemName = ProjectRoleListItem.Owner, ProjectRolePermissionSlug = ProjectRoleSlug.ViewProjectMember },
			   new ProjectRolePermissionModel{ ProjectRoleSystemName = ProjectRoleListItem.Owner, ProjectRolePermissionSlug = ProjectRoleSlug.CreateProjectMember },
			   new ProjectRolePermissionModel{ ProjectRoleSystemName = ProjectRoleListItem.Owner, ProjectRolePermissionSlug = ProjectRoleSlug.UpdateProjectMember },
			   new ProjectRolePermissionModel{ ProjectRoleSystemName = ProjectRoleListItem.Owner, ProjectRolePermissionSlug = ProjectRoleSlug.DeleteProjectMember },

			   new ProjectRolePermissionModel{ ProjectRoleSystemName = ProjectRoleListItem.Owner, ProjectRolePermissionSlug = ProjectRoleSlug.ViewProjectModule },
			   new ProjectRolePermissionModel{ ProjectRoleSystemName = ProjectRoleListItem.Owner, ProjectRolePermissionSlug = ProjectRoleSlug.CreateProjectModule },
			   new ProjectRolePermissionModel{ ProjectRoleSystemName = ProjectRoleListItem.Owner, ProjectRolePermissionSlug = ProjectRoleSlug.UpdateProjectModule },
			   new ProjectRolePermissionModel{ ProjectRoleSystemName = ProjectRoleListItem.Owner, ProjectRolePermissionSlug = ProjectRoleSlug.DeleteProjectModule },

			   new ProjectRolePermissionModel{ ProjectRoleSystemName = ProjectRoleListItem.Owner, ProjectRolePermissionSlug = ProjectRoleSlug.ViewProjectModuleDeveloper },
			   new ProjectRolePermissionModel{ ProjectRoleSystemName = ProjectRoleListItem.Owner, ProjectRolePermissionSlug = ProjectRoleSlug.CreateProjectModuleDeveloper },
			   new ProjectRolePermissionModel{ ProjectRoleSystemName = ProjectRoleListItem.Owner, ProjectRolePermissionSlug = ProjectRoleSlug.UpdateProjectModuleDeveloper },
			   new ProjectRolePermissionModel{ ProjectRoleSystemName = ProjectRoleListItem.Owner, ProjectRolePermissionSlug = ProjectRoleSlug.DeleteProjectModuleDeveloper },


			   new ProjectRolePermissionModel{ ProjectRoleSystemName = ProjectRoleListItem.Owner, ProjectRolePermissionSlug = ProjectRoleSlug.ViewTestPlan },
			   new ProjectRolePermissionModel{ ProjectRoleSystemName = ProjectRoleListItem.Owner, ProjectRolePermissionSlug = ProjectRoleSlug.CreateTestPlan },
			   new ProjectRolePermissionModel{ ProjectRoleSystemName = ProjectRoleListItem.Owner, ProjectRolePermissionSlug = ProjectRoleSlug.UpdateTestPlan },
			   new ProjectRolePermissionModel{ ProjectRoleSystemName = ProjectRoleListItem.Owner, ProjectRolePermissionSlug = ProjectRoleSlug.DeleteTestPlan },

               // For ProjectRole Maintainer
               new ProjectRolePermissionModel{ ProjectRoleSystemName = ProjectRoleListItem.Maintainer, ProjectRolePermissionSlug = ProjectRoleSlug.ViewProjectMember },
			   new ProjectRolePermissionModel{ ProjectRoleSystemName = ProjectRoleListItem.Maintainer, ProjectRolePermissionSlug = ProjectRoleSlug.CreateProjectMember },
			   new ProjectRolePermissionModel{ ProjectRoleSystemName = ProjectRoleListItem.Maintainer, ProjectRolePermissionSlug = ProjectRoleSlug.UpdateProjectMember },
			   new ProjectRolePermissionModel{ ProjectRoleSystemName = ProjectRoleListItem.Maintainer, ProjectRolePermissionSlug = ProjectRoleSlug.DeleteProjectMember },

			   new ProjectRolePermissionModel{ ProjectRoleSystemName = ProjectRoleListItem.Maintainer, ProjectRolePermissionSlug = ProjectRoleSlug.ViewProjectModule },
			   new ProjectRolePermissionModel{ ProjectRoleSystemName = ProjectRoleListItem.Maintainer, ProjectRolePermissionSlug = ProjectRoleSlug.CreateProjectModule },
			   new ProjectRolePermissionModel{ ProjectRoleSystemName = ProjectRoleListItem.Maintainer, ProjectRolePermissionSlug = ProjectRoleSlug.UpdateProjectModule },
			   new ProjectRolePermissionModel{ ProjectRoleSystemName = ProjectRoleListItem.Maintainer, ProjectRolePermissionSlug = ProjectRoleSlug.DeleteProjectModule },

			   new ProjectRolePermissionModel{ ProjectRoleSystemName = ProjectRoleListItem.Maintainer, ProjectRolePermissionSlug = ProjectRoleSlug.ViewProjectModuleDeveloper },
			   new ProjectRolePermissionModel{ ProjectRoleSystemName = ProjectRoleListItem.Maintainer, ProjectRolePermissionSlug = ProjectRoleSlug.CreateProjectModuleDeveloper },
			   new ProjectRolePermissionModel{ ProjectRoleSystemName = ProjectRoleListItem.Maintainer, ProjectRolePermissionSlug = ProjectRoleSlug.UpdateProjectModuleDeveloper },
			   new ProjectRolePermissionModel{ ProjectRoleSystemName = ProjectRoleListItem.Maintainer, ProjectRolePermissionSlug = ProjectRoleSlug.DeleteProjectModuleDeveloper },

			   new ProjectRolePermissionModel{ ProjectRoleSystemName = ProjectRoleListItem.Maintainer, ProjectRolePermissionSlug = ProjectRoleSlug.ViewTestPlan },
			   new ProjectRolePermissionModel{ ProjectRoleSystemName = ProjectRoleListItem.Maintainer, ProjectRolePermissionSlug = ProjectRoleSlug.CreateTestPlan },
			   new ProjectRolePermissionModel{ ProjectRoleSystemName = ProjectRoleListItem.Maintainer, ProjectRolePermissionSlug = ProjectRoleSlug.UpdateTestPlan },
			   new ProjectRolePermissionModel{ ProjectRoleSystemName = ProjectRoleListItem.Maintainer, ProjectRolePermissionSlug = ProjectRoleSlug.DeleteTestPlan },
			  


               // For ProjectRole Member
               new ProjectRolePermissionModel{ ProjectRoleSystemName = ProjectRoleListItem.Member, ProjectRolePermissionSlug = ProjectRoleSlug.ViewProjectMember },

			   new ProjectRolePermissionModel{ ProjectRoleSystemName = ProjectRoleListItem.Member, ProjectRolePermissionSlug = ProjectRoleSlug.ViewProjectModule },
			   new ProjectRolePermissionModel{ ProjectRoleSystemName = ProjectRoleListItem.Member, ProjectRolePermissionSlug = ProjectRoleSlug.CreateProjectModule },
			   new ProjectRolePermissionModel{ ProjectRoleSystemName = ProjectRoleListItem.Member, ProjectRolePermissionSlug = ProjectRoleSlug.UpdateProjectModule },
			   new ProjectRolePermissionModel{ ProjectRoleSystemName = ProjectRoleListItem.Member, ProjectRolePermissionSlug = ProjectRoleSlug.DeleteProjectModule },

			   new ProjectRolePermissionModel{ ProjectRoleSystemName = ProjectRoleListItem.Member, ProjectRolePermissionSlug = ProjectRoleSlug.ViewProjectModuleDeveloper },
			   new ProjectRolePermissionModel{ ProjectRoleSystemName = ProjectRoleListItem.Member, ProjectRolePermissionSlug = ProjectRoleSlug.CreateProjectModuleDeveloper },
			   new ProjectRolePermissionModel{ ProjectRoleSystemName = ProjectRoleListItem.Member, ProjectRolePermissionSlug = ProjectRoleSlug.UpdateProjectModuleDeveloper },
			   new ProjectRolePermissionModel{ ProjectRoleSystemName = ProjectRoleListItem.Member, ProjectRolePermissionSlug = ProjectRoleSlug.DeleteProjectModuleDeveloper },

			   new ProjectRolePermissionModel{ ProjectRoleSystemName = ProjectRoleListItem.Member, ProjectRolePermissionSlug = ProjectRoleSlug.ViewTestPlan },


			   new ProjectRolePermissionModel{ ProjectRoleSystemName = ProjectRoleListItem.Member, ProjectRolePermissionSlug = ProjectRoleSlug.CreateTestPlan },
			   new ProjectRolePermissionModel{ ProjectRoleSystemName = ProjectRoleListItem.Member, ProjectRolePermissionSlug = ProjectRoleSlug.UpdateTestPlan },
			   new ProjectRolePermissionModel{ ProjectRoleSystemName = ProjectRoleListItem.Member, ProjectRolePermissionSlug = ProjectRoleSlug.DeleteTestPlan },


			};
		}

		public static List<string> GetProjectRolePermission(string projectRoleSystemName)
		{
			return projectRolePermission.Where(x => x.ProjectRoleSystemName == projectRoleSystemName).Select(x => x.ProjectRolePermissionSlug).ToList();
		}

	}

	public class ProjectRolePermissionModel
	{
		public string ProjectRoleSystemName { get; set; }
		public string ProjectRolePermissionSlug { get; set; }
	}
}
