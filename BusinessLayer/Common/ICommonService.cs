using System.Collections.Generic;
using System.Threading.Tasks;
using Models.Common;
using Models.Core;
using Models.Project;

namespace BusinessLayer.Common
{
    public interface ICommonService
    {
        Task<Result<List<ListItemModel>>> GetListItemByListItemCategorySystemName(string listItemCategoryName);

        Task<ListItemModel> GetListItemDetailByListItemSystemName(string listItemSystemName);

        Task<Result<bool>> ISOwnerOrMaintainer(int projectId);
        Task<Result<bool>> ISOwnerOrMaintainerOrMember(int projectId);
        Task<Result<bool>> IsMember(int projectId);
        Task<bool> CheckProjectPermission(string projectSlug, string projectRolePermissionSlug);
        Task<int> GetProjectIdFromProjectSlug(string projectSlug);

        Task<Result<ProjectListModel>> GetProjectNameFromProjectSlug(string projectSlug);
    }
}