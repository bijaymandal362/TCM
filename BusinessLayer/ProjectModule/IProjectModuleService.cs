using Models.Core;
using Models.GridTableFilterModel;
using Models.GridTableProperty;
using Models.Import;
using Models.ProjectModule;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.ProjectModule
{
    public interface IProjectModuleService
    {

        Task<Result<List<ProjectModuleModel>>> GetProjectModuleListAsync(string projectSlug);
        Task<Result<List<ProjectMemberDeveloperListModel>>> GetProjectMemberDeveloperListAsync(string projectSlug);
        Task<Result<List<ProjectModuleDeveloperModelList>>> GetProjectModuleDeveloperListAsync();
        Task<Result<List<TestCaseModel>>> GetTestCaseListAsync();
        Task<Result<string>> AddProjectModuleAysnc(AddUpdateProjectModuleModel model);
        Task<Result<string>> UpdateProjectModuleAysnc(AddUpdateProjectModuleModel model);     
        Task<Result<string>> AddProjectModuleDeveloperAsync(AddProjectModuleDeveloperModel model);
        Task<Result<string>> UpdateProjectModuleDeveloperAsync(UpdateProjectModuleListDeveloperModel model);     
        Task<Result<string>> DeleteProjectModuleAsync(int projectModuleId);
        Task<Result<string>> DeleteTestCaseAsync(int testCaseId);

        Task<Result<string>> DeleteProjectModuleDeveloperAsync(int projectModuleDeveloperId);
        Task<Result<GetTestCaseDetailListModel>> GetTestCaseDetailListById(int testCaseId);
        Task<Result<GetDeveloperDetailModel>> GetDeveloperDetailByFunctionId(int functionId);

        Task<Result<PagedResponsePersonModel<List<ProjectModuleModel>>>> GetProjectModuleListFilterByModuleAsync(FilterModel filter, string projectSlug);


        //Import and  Export TestCase
        Task<Result<string>> ImportProjectModuleTestCaseAsync(ImportProjectModuleModel projectModule);
        Task<byte[]> DownloadTestByFunctionIdAsync(int functionId);

        Task<byte[]> DownloadTestCaseAsync(string projectSlug);


        //ordering TreeView Structure

        Task<Result<string>> DragDropProjectModuleFunctionTestCaseAsync(DragDropTestCaseDetail model);

        Task<Result<string>> DeleteImportTestCaseAsync(int projectModuleId);
    }
}
