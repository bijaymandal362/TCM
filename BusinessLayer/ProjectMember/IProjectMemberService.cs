using Models.Core;
using Models.GridTableFilterModel;
using Models.GridTableProperty;
using Models.PersonProjectRole;
using Models.ProjectMember;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.ProjectMember
{
    public interface IProjectMemberService
    {
        Task<Result<PagedResponseProjectMemberModel<List<ProjectMemberInviteModel>>>> GetProjectMemberListFilterByProjectIdAsync(PaginationFilterModel filter,string projectSlug);
        Task<Result<List<ProjectMemberInviteModel>>> GetProjectMemberListAllAsync();

        Task<Result<string>> AddProjectMemberAsync(ProjectMemberModel model);
        Task<Result<string>> UpdateProjectMemberAsync(ProjectMemberModel model);
        Task<Result<string>> DeleteProjectMemberAsync(int projectMemberId);

        Task<Result<List<ProjectMemberModelList>>> GetProjectMemberListBySlugAsync(string projectSlug);
        


    }
}
