using Models.Core;
using Models.GridTableFilterModel;
using Models.GridTableProperty;
using Models.ProjectStarred;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ProjectStarred
{
	public interface IProjectStarredService 
	{
		
		Task<Result<PagedResponseModel<List<ProjectStarredModel>>>> GetProjectStarredListFilterByModuleAsync(PaginationFilterModel filter);

		Task<Result<string>> AssignProjectToProjectStarredAsync(string projectSlug);
		Task<Result<string>> UnAssignProjectToProjectStarredAsync(string projectSlug);
	}
}
