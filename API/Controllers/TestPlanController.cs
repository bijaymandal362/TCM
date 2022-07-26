using BusinessLayer.TestPlan;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.GridTableFilterModel;
using Models.TestPlan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestPlanController : BaseApiController
    {
        private readonly ITestPlanService _iTestPlanService;

        public TestPlanController(ITestPlanService iTestPlanService)
        {
            _iTestPlanService = iTestPlanService;
        }

        [HttpPost("AddTestPlan")]
        public async Task<IActionResult> AddTestPlan(AddUpdateTestPlanModel model)
        {
            return HandleResult(await _iTestPlanService.AddTestPlanAsync(model));
        }
        
        [HttpPut("UpdateTestPlan")]
        public async Task<IActionResult> UpdateTestPlan(AddUpdateTestPlanModel model)
        {
            return HandleResult(await _iTestPlanService.UpdateTestPlanAsync(model));
        }
            
        [HttpGet("GetTestPlanList/{projectSlug}")]
        public async Task<IActionResult> GetTestPlanList([FromQuery] FilterModel filter, string projectSlug)
        {
            return HandleResult(await _iTestPlanService.GetTestPlanListAsync(filter, projectSlug));
        }


        [HttpPut("DeleteTestPlan/{testPlanId}")]
        public async Task<IActionResult> DeleteTestPlan(int testPlanId)
        {
            return HandleResult(await _iTestPlanService.DeleteTestPlanAsync(testPlanId));
        }

        
        [HttpPut("DragDropTestPlan")]
        public async Task<IActionResult> DragDropTestPlan(DragDropTestPlanModel model)
        {
            return HandleResult(await _iTestPlanService.DragDropTestPlanAsync(model));
        } 
        
        [HttpGet("GetTestPlanTestCaseByTestPlanId/{testPlanId}")]
        public async Task<IActionResult> GetTestPlanTestCaseByTestPlanId([FromQuery] PaginationFilterModel filter, int testPlanId)
        {
            return HandleResult(await _iTestPlanService.GetTestPlanTestCaseByTestPlanIdAsync(filter,testPlanId));
        }
    }
}
