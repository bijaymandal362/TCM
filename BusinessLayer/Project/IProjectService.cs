using Models.Core;
using Models.GridTableFilterModel;
using Models.GridTableProperty;
using Models.Project;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Project
{
    public interface IProjectService
    {
      
        Task<Result<List<ListProjectModel>>> GetProjectListAsync();
        Task<Result<PagedResponseModel<List<GetAllListProjectModel>>>> GetProjectListFilterByModuleAsync(PaginationFilterModel filter);
        Task<Result<GetAllListProjectModel>> GetProjectByProjectIdAsync(int projectId);
        Task<Result<string>> AddProjectAsync(ProjectModel model);
        Task<Result<string>> UpdateProjectAsync(ProjectModel model);
        Task<Result<string>> DeleteProjectAsync(int projectId);
        Task<Result<List<string>>> GetProjectRolePermissionAsync(string projectSlug);

        Task<Result<PagedResponseModel<List<ProjectListViewModel>>>> GetAllProjectListFilterByModuleAsync(PaginationFilterModel filter); 
         

    }

}
