using Microsoft.EntityFrameworkCore;
using Models.Constant.ListItem;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data.MenuRoleSeed
{
    public class MenuPermission
    {
        public async static Task SeedPermissionsForRole(DataContext dbContext)
        {
            await SeedAdminPermissions(dbContext);
            await SeedProjectLeadPermissions(dbContext);
            await SeedSeniorQAPermissions(dbContext);
            await SeedOnsitePermissions(dbContext);
            await SeedUserMemberPermissions(dbContext);
        }

        private static async Task SeedAdminPermissions(DataContext dbContext)
        {
            var roleMenuPermission = await (from userRole in dbContext.ListItem
                                            join menuPermission in dbContext.MenuPermission
                                            on userRole.ListItemId equals menuPermission.RoleId into menu
                                            from menuPermission in menu.DefaultIfEmpty()
                                            where userRole.ListItemSystemName == UserRoleListItem.Admin
                                            select new { RoleId = userRole.ListItemId, MenuPermission = menuPermission }).ToListAsync();
            var roleClaims = roleMenuPermission.Where(x => x.MenuPermission != null).Select(x => x.MenuPermission).ToList();
            var roleId = roleMenuPermission.Select(x => x.RoleId).FirstOrDefault();

            var claims = new List<Entities.MenuPermission>
            {
                new Entities.MenuPermission(roleId, 1),
                new Entities.MenuPermission(roleId, 2),
                new Entities.MenuPermission(roleId, 3),
                new Entities.MenuPermission(roleId, 4),
                new Entities.MenuPermission(roleId, 5),
                new Entities.MenuPermission(roleId, 6),
                new Entities.MenuPermission(roleId, 7),
                new Entities.MenuPermission(roleId, 8),
                new Entities.MenuPermission(roleId, 9),
                new Entities.MenuPermission(roleId, 10),
                new Entities.MenuPermission(roleId, 11),
                new Entities.MenuPermission(roleId, 12)
            };
            dbContext.MenuPermission.RemoveRange(roleClaims.Where(c => !claims.Select(x => x.MenuId).Contains(c.MenuId)));
            await dbContext.MenuPermission.AddRangeAsync(claims.Where(c => !roleClaims.Select(x => x.MenuId).Contains(c.MenuId)));
            await dbContext.SaveChangesAsync();
        }
        private static async Task SeedProjectLeadPermissions(DataContext dbContext)
        {
            var roleMenuPermission = await (from userRole in dbContext.ListItem
                                            join menuPermission in dbContext.MenuPermission
                                            on userRole.ListItemId equals menuPermission.RoleId into menu
                                            from menuPermission in menu.DefaultIfEmpty()
                                            where userRole.ListItemSystemName == UserRoleListItem.ProjectLead
                                            select new { RoleId = userRole.ListItemId, MenuPermission = menuPermission }).ToListAsync();
            var roleClaims = roleMenuPermission.Where(x => x.MenuPermission != null).Select(x => x.MenuPermission).ToList();
            var roleId = roleMenuPermission.Select(x => x.RoleId).FirstOrDefault();

            var claims = new List<Entities.MenuPermission>
            {
                new Entities.MenuPermission(roleId, 1),
                new Entities.MenuPermission(roleId, 2),
                new Entities.MenuPermission(roleId, 5),
                new Entities.MenuPermission(roleId, 7),
                new Entities.MenuPermission(roleId, 9),
                new Entities.MenuPermission(roleId, 10)
            };
            dbContext.MenuPermission.RemoveRange(roleClaims.Where(c => !claims.Select(x => x.MenuId).Contains(c.MenuId)));
            await dbContext.MenuPermission.AddRangeAsync(claims.Where(c => !roleClaims.Select(x => x.MenuId).Contains(c.MenuId)));
            await dbContext.SaveChangesAsync();
        }
        private static async Task SeedSeniorQAPermissions(DataContext dbContext)
        {
            var roleMenuPermission = await (from userRole in dbContext.ListItem
                                            join menuPermission in dbContext.MenuPermission
                                            on userRole.ListItemId equals menuPermission.RoleId into menu
                                            from menuPermission in menu.DefaultIfEmpty()
                                            where userRole.ListItemSystemName == UserRoleListItem.SeniorQA
                                            select new { RoleId = userRole.ListItemId, MenuPermission = menuPermission }).ToListAsync();
            var roleClaims = roleMenuPermission.Where(x => x.MenuPermission != null).Select(x => x.MenuPermission).ToList();
            var roleId = roleMenuPermission.Select(x => x.RoleId).FirstOrDefault();

            var claims = new List<Entities.MenuPermission>
            {
                new Entities.MenuPermission(roleId, 1),
                new Entities.MenuPermission(roleId, 2),
                new Entities.MenuPermission(roleId, 5),
                new Entities.MenuPermission(roleId, 7),
                new Entities.MenuPermission(roleId, 9),
                new Entities.MenuPermission(roleId, 10),
            };
            dbContext.MenuPermission.RemoveRange(roleClaims.Where(c => !claims.Select(x => x.MenuId).Contains(c.MenuId)));
            await dbContext.MenuPermission.AddRangeAsync(claims.Where(c => !roleClaims.Select(x => x.MenuId).Contains(c.MenuId)));
            await dbContext.SaveChangesAsync();
        }
        private static async Task SeedOnsitePermissions(DataContext dbContext)
        {
            var roleMenuPermission = await (from userRole in dbContext.ListItem
                                            join menuPermission in dbContext.MenuPermission
                                            on userRole.ListItemId equals menuPermission.RoleId into menu
                                            from menuPermission in menu.DefaultIfEmpty()
                                            where userRole.ListItemSystemName == UserRoleListItem.Onsite
                                            select new { RoleId = userRole.ListItemId, MenuPermission = menuPermission }).ToListAsync();
            var roleClaims = roleMenuPermission.Where(x => x.MenuPermission != null).Select(x => x.MenuPermission).ToList();
            var roleId = roleMenuPermission.Select(x => x.RoleId).FirstOrDefault();

            var claims = new List<Entities.MenuPermission>
            {
                new Entities.MenuPermission(roleId, 1),
                new Entities.MenuPermission(roleId, 5)
            };
            dbContext.MenuPermission.RemoveRange(roleClaims.Where(c => !claims.Select(x => x.MenuId).Contains(c.MenuId)));
            await dbContext.MenuPermission.AddRangeAsync(claims.Where(c => !roleClaims.Select(x => x.MenuId).Contains(c.MenuId)));
            await dbContext.SaveChangesAsync();
        }
        private static async Task SeedUserMemberPermissions(DataContext dbContext)
        {
            var roleMenuPermission = await (from userRole in dbContext.ListItem
                                            join menuPermission in dbContext.MenuPermission
                                            on userRole.ListItemId equals menuPermission.RoleId into menu
                                            from menuPermission in menu.DefaultIfEmpty()
                                            where userRole.ListItemSystemName == UserRoleListItem.UserMember
                                            select new { RoleId = userRole.ListItemId, MenuPermission = menuPermission }).ToListAsync();
            var roleClaims = roleMenuPermission.Where(x => x.MenuPermission != null).Select(x => x.MenuPermission).ToList();
            var roleId = roleMenuPermission.Select(x => x.RoleId).FirstOrDefault();

            var claims = new List<Entities.MenuPermission>
            {
                new Entities.MenuPermission(roleId, 1),
                new Entities.MenuPermission(roleId, 5),
                new Entities.MenuPermission(roleId, 9)
            };
            dbContext.MenuPermission.RemoveRange(roleClaims.Where(c => !claims.Select(x => x.MenuId).Contains(c.MenuId)));
            await dbContext.MenuPermission.AddRangeAsync(claims.Where(c => !roleClaims.Select(x => x.MenuId).Contains(c.MenuId)));
            await dbContext.SaveChangesAsync();
        }
    }
}
