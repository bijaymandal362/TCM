using Models.Core;
using Models.GridTableFilterModel;
using Models.GridTableProperty;
using Models.TestPlan;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.TestPlan
{
    public interface ITestPlanService
    {
        Task<Result<string>> AddTestPlanAsync(AddUpdateTestPlanModel model);
        Task<Result<string>> UpdateTestPlanAsync(AddUpdateTestPlanModel model);
        Task<Result<PagedResponsePersonModel<List<TestPlanListModel>>>> GetTestPlanListAsync(FilterModel filter, string projectSlug); 
        
        Task<Result<string>> DeleteTestPlanAsync(int testPlanId);

        Task<Result<string>> DragDropTestPlanAsync(DragDropTestPlanModel model);
        Task<Result<PagedResponseModel<List<TestPlanTestCaseModel>>>> GetTestPlanTestCaseByTestPlanIdAsync(PaginationFilterModel filter, int testPlanId);



    }
}
