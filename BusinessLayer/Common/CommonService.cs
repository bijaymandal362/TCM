using Data;
using Data.MenuRoleSeed;
using Data.Migrations.core;

using Infrastructure;

using Microsoft.EntityFrameworkCore;

using Models.Common;
using Models.Constant.ListItem;
using Models.Core;
using Models.Enum;
using Models.Project;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLayer.Common
{
	public class CommonService : ICommonService
	{
		private readonly DataContext _context;
		private readonly IPersonAccessor _iPersonAccessor;

		public CommonService(DataContext context, IPersonAccessor iPersonAccessor)
		{
			_context = context;
			_iPersonAccessor = iPersonAccessor;
		}
		public async Task<Result<List<ListItemModel>>> GetListItemByListItemCategorySystemName(string listItemCategoryName)
		{
			var listItems = await (from lic in _context.ListItemCategory
								   join li in _context.ListItem on lic.ListItemCategoryId equals li.ListItemCategoryId
								   where lic.ListItemCategorySystemName == listItemCategoryName
								   select new ListItemModel
								   {
									   ListItemId = li.ListItemId,
									   ListItemName = li.ListItemName,
									   ListItemSystemName = li.ListItemSystemName
								   }).ToListAsync();

			if (listItems.Any())
			{
				return Result<List<ListItemModel>>.Success(listItems);
			}
			else
			{
				return Result<List<ListItemModel>>.Success(null);
			}
		}
		public async Task<ListItemModel> GetListItemDetailByListItemSystemName(string listItemSystemName)
		{
			var listItem = await (from li in _context.ListItem
								  where li.ListItemSystemName == listItemSystemName
								  select new ListItemModel
								  {
									  ListItemId = li.ListItemId,
									  ListItemName = li.ListItemName,
									  ListItemSystemName = li.ListItemSystemName
								  }).FirstOrDefaultAsync();
			return listItem;
		}

		public async Task<Result<bool>> IsMember(int projectId)
		{
			var currentPersonId = _iPersonAccessor.GetPersonId();
			var member = await _context.ProjectMember.Include(x => x.ProjectRoleListItem)
				  .Where(x => x.PersonId == currentPersonId && x.ProjectId == projectId
				  && x.ProjectRoleListItem.ListItemSystemName == ListItem.Member.ToString()).AnyAsync();


			if (member)
			{
				return Result<bool>.Success(true);
			}
			else
			{
				return Result<bool>.Success(false);
			}
		}

		public async Task<Result<bool>> ISOwnerOrMaintainer(int projectId)
		{

			var currentPersonId = (_iPersonAccessor.GetPersonId());

			var ownerOrMaintainer = await _context.ProjectMember.Include(x => x.ProjectRoleListItem)
				   .Where(x => x.PersonId == currentPersonId && x.ProjectId == projectId
				   && (x.ProjectRoleListItem.ListItemSystemName == nameof(ListItem.Owner)
				   || x.ProjectRoleListItem.ListItemSystemName == nameof(ListItem.Maintainer))
					).AnyAsync();
			if (ownerOrMaintainer)
			{
				return Result<bool>.Success(true);
			}
			else
			{
				return Result<bool>.Success(false);
			}
		}

		public async Task<Result<bool>> ISOwnerOrMaintainerOrMember(int projectId)
		{
			var currentPersonId = (_iPersonAccessor.GetPersonId());
			var ownerOrMaintainerorMember = await _context.ProjectMember.Include(x => x.ProjectRoleListItem)
				   .Where(x => x.PersonId == currentPersonId && x.ProjectId == projectId
					&& (x.ProjectRoleListItem.ListItemSystemName == ListItem.Owner.ToString()
					|| x.ProjectRoleListItem.ListItemSystemName == ListItem.Maintainer.ToString()
					|| x.ProjectRoleListItem.ListItemSystemName == ListItem.Member.ToString())
				   ).AnyAsync();
			if (ownerOrMaintainerorMember)
			{
				return Result<bool>.Success(true);
			}
			else
			{
				return Result<bool>.Success(false);
			}
		}

		public async Task<bool> CheckProjectPermission(string projectSlug, string projectRolePermissionSlug)
		{
			var currentPersonId = _iPersonAccessor.GetPersonId();
			bool isUserRoleAdmin = await IsUserRoleAdmin(currentPersonId);
			if (isUserRoleAdmin)
			{
				// UserRole "Admin" has every permission of projectRole "Owner"
				return ProjectRolePermission.GetProjectRolePermission(ProjectRoleListItem.Owner).Contains(projectRolePermissionSlug);
			}
			else
			{
				var projectRole = await (from project in _context.Project
										 join projectMember in _context.ProjectMember
										 on project.ProjectId equals projectMember.ProjectId
										 join listItem in _context.ListItem
										 on projectMember.ProjectRoleListItemId equals listItem.ListItemId
										 where project.ProjectSlug == projectSlug && projectMember.PersonId == currentPersonId
										 select listItem.ListItemSystemName
									).FirstOrDefaultAsync();
				if (string.IsNullOrEmpty(projectRole))
				{
					return false;
				}
				else
				{
					return ProjectRolePermission.GetProjectRolePermission(projectRole).Contains(projectRolePermissionSlug);
				}
			}
		}

		#region Helper
		private async Task<bool> IsUserRoleAdmin(int currentPersonId)
		{
			return await (from person in _context.Person
						  join personRole in _context.ListItem
						  on person.UserRoleListItemId equals personRole.ListItemId
						  where person.PersonId == currentPersonId && personRole.ListItemSystemName == UserRoleListItem.Admin
						  select person.PersonId).AnyAsync();
		}
		#endregion
		public async Task<int> GetProjectIdFromProjectSlug(string projectSlug)
		{
			var projectId = await (from project in _context.Project
								   where project.ProjectSlug == projectSlug
								   select project.ProjectId).FirstOrDefaultAsync();
			if (projectId > 0)
			{
				return projectId;
			}
			else
			{
				return 0;
			}
		}

		public async Task<Result<ProjectListModel>> GetProjectNameFromProjectSlug(string projectSlug)
		{
			var currentPersonId = _iPersonAccessor.GetPersonId();

			var projectId = await _context.Project.Where(x => x.ProjectSlug == projectSlug).FirstOrDefaultAsync();

			bool projectStarred = await (from ps in _context.ProjectStarred 
										join per in _context.Person on ps.PersonId equals per.PersonId
										 join p in _context.Project on ps.ProjectId equals p.ProjectId
										 where ps.ProjectId == projectId.ProjectId && ps.PersonId == currentPersonId
										 select ps.ProjectStarredId).AnyAsync();



			var project = await (from p in _context.Project												
								 where p.ProjectSlug == projectSlug
								 select new ProjectListModel
								 {
									 ProjectId = p.ProjectId,
									 ProjectSlug = p.ProjectSlug,
									 ProjectName = p.ProjectName,
									 ProjectMarketId = p.ProjectMarketListItemId,
									 ProjectMarketName = p.ProjectMarketListItem.ListItemSystemName,
								     IsStarredProject = projectStarred
								 }).FirstOrDefaultAsync();
			if (project != null)
			{
				return Result<ProjectListModel>.Success(project);
			}
			else
			{
				return Result<ProjectListModel>.Success(null);
			}
		}
		
	}
}