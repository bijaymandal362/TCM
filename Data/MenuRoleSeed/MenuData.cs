using Entities;
using Models.Constant.Authorization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data.MenuRoleSeed
{
    public class MenuData
    {
        public static async Task SeedMenu(DataContext context)
        {
            var permissions = new List<Menu> {
                
                //Project
                new Menu(1, MenuName.ViewProject, MenuSlug.ViewProject),
                new Menu(2, MenuName.CreateProject, MenuSlug.CreateProject),
                new Menu(3, MenuName.UpdateProject, MenuSlug.UpdateProject),
                new Menu(4, MenuName.DeleteProject, MenuSlug.DeleteProject),


                //User
                new Menu(5, MenuName.ViewUser, MenuSlug.ViewUser),
                new Menu(6, MenuName.UpdateUser, MenuSlug.UpdateUser),
                new Menu(7, MenuName.CreateUser, MenuSlug.CreateUser),
                new Menu(8, MenuName.DeleteUser, MenuSlug.DeleteUser),
  

				//ProjectMember
				new Menu(9, MenuName.ViewProjectMember,MenuSlug.ViewProjectMember),
                new Menu(10, MenuName.AddProjectMember,MenuSlug.AddProjectMember),
                new Menu(11, MenuName.UpdateProjectMember,MenuSlug.UpdateProjectMember),
                new Menu(12, MenuName.DeleteProjectMember,MenuSlug.DeleteProjectMember)
            };

            var existingPermissions = context.Menu.ToList();
            //var existingSlugs = existingPermissions.Select(s => s.MenuSlug);
            //var newPermissions = permissions.Where(p => !existingSlugs.Contains(p.MenuSlug));

            context.Menu.RemoveRange(existingPermissions);
            await context.Menu.AddRangeAsync(permissions);
            await context.SaveChangesAsync();
        }
    }
}
