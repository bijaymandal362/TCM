using BusinessLayer.ProjectModule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.GridTableFilterModel;
using Models.Import;
using Models.ProjectModule;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
{
	public class ProjectModuleController : BaseApiController
	{
		private readonly IProjectModuleService _iprojectModuleService;

		public ProjectModuleController(IProjectModuleService iprojectModuleService)
		{
			_iprojectModuleService = iprojectModuleService;
		}

		[HttpGet("GetProjectModuleList/{projectSlug}")]
		public async Task<IActionResult> GetProjectModuleList(string projectSlug)
		{
			return HandleResult(await _iprojectModuleService.GetProjectModuleListAsync(projectSlug));
		}

		[HttpGet("GetProjectModuleDeveloperList")]
		public async Task<IActionResult> GetProjectModuleDeveloperList()
		{
			return HandleResult(await _iprojectModuleService.GetProjectModuleDeveloperListAsync());
		}

		[HttpGet("GetTestCaseList")]
		public async Task<IActionResult> GetTestCaseList()
		{
			return HandleResult(await _iprojectModuleService.GetTestCaseListAsync());
		}

		[HttpGet("GetProjectMemberDeveloperList/{projectSlug}")]
		public async Task<IActionResult> GetProjectMemberDeveloperList(string projectSlug)
		{
			return HandleResult(await _iprojectModuleService.GetProjectMemberDeveloperListAsync(projectSlug));
		}
		[HttpGet("GetTestCaseDetailbyId/{testCaseId}")]
		public async Task<IActionResult> GetTestCaseDetailbyId(int testCaseId)
		{
			return HandleResult(await _iprojectModuleService.GetTestCaseDetailListById(testCaseId));
		}

		[HttpGet("GetDeveloperDetailByFunctionId/{functionId}")]
		public async Task<IActionResult> GetDeveloperDetailByFunctionId(int functionId)
		{
			return HandleResult(await _iprojectModuleService.GetDeveloperDetailByFunctionId(functionId));
		}

		[HttpPost("AddProjectModule")]
		public async Task<IActionResult> AddProjectModule([FromBody] AddUpdateProjectModuleModel model)
		{
			return HandleResult(await _iprojectModuleService.AddProjectModuleAysnc(model));
		}

		[HttpPut("UpdateProjectModule")]
		public async Task<IActionResult> UpdteProjectModule([FromBody] AddUpdateProjectModuleModel model)
		{
			return HandleResult(await _iprojectModuleService.UpdateProjectModuleAysnc(model));
		}

		[HttpPost("AddProjectModuleDeveloper")]
		public async Task<IActionResult> AddProjectModuleDeveloper([FromBody] AddProjectModuleDeveloperModel model)
		{
			return HandleResult(await _iprojectModuleService.AddProjectModuleDeveloperAsync(model));
		}

		[HttpPut("UpdateProjectModuleDeveloper")]
		public async Task<IActionResult> UpdateProjectModuleDeveloper([FromBody] UpdateProjectModuleListDeveloperModel model)
		{
			return HandleResult(await _iprojectModuleService.UpdateProjectModuleDeveloperAsync(model));
		}


		[HttpPut("DeleteProjectModule/{projectModuleId}")]
		public async Task<IActionResult> DeleteProjectModule(int projectModuleId)
		{
			return HandleResult(await _iprojectModuleService.DeleteProjectModuleAsync(projectModuleId));
		}

		[HttpDelete("DeleteTestCase/{testCaseId}")]
		public async Task<IActionResult> DeleteTestCase(int testCaseId)
		{
			return HandleResult(await _iprojectModuleService.DeleteTestCaseAsync(testCaseId));
		}

		[HttpDelete("DeleteProjectModuleDeveloper/{projectModuleDeveloperId}")]
		public async Task<IActionResult> DeleteProjectModuleDeveloper(int projectModuleDeveloperId)
		{
			return HandleResult(await _iprojectModuleService.DeleteProjectModuleDeveloperAsync(projectModuleDeveloperId));
		}

		[HttpPost]
		[Route("ImportProjectModuleTestCase")]
		public async Task<IActionResult> ImportProjectModuleTestCase([FromForm] ImportProjectModuleModel projectModule)
		{
			return HandleResult(await _iprojectModuleService.ImportProjectModuleTestCaseAsync(projectModule));
		}

		[HttpGet]
		[Route("DownloadTestByFunctionId/{functionId}")]

		public async Task<IActionResult> DownloadTestByFunctionId(int functionId)
		{
			var excel =await  _iprojectModuleService.DownloadTestByFunctionIdAsync(functionId);
			return File(excel, "application/ms-excel", "TestCaseDetails.xlsx");
		}

		[HttpGet]
		[Route("DownloadTestCase/{projectSlug}")]

		public async Task<IActionResult> DownloadTestCase(string projectSlug)
		{
			var excel = await _iprojectModuleService.DownloadTestCaseAsync(projectSlug);
			return File(excel, "application/ms-excel", "TestCaseDetails.xlsx");
		}


		[HttpPut]
		[Route("DragDropTestCaseDetail")]
		public async Task<IActionResult> DragDropTestCaseDetail([FromBody] DragDropTestCaseDetail model)
		{
			return HandleResult(await _iprojectModuleService.DragDropProjectModuleFunctionTestCaseAsync(model));
		}


		[HttpPost]
		[Route("GetProjectModuleListFilterByModule/{projectSlug}")]
		public async Task<IActionResult> GetProjectModuleListFilterByModule([FromQuery] FilterModel filter,string projectSlug)
		{
			return HandleResult(await _iprojectModuleService.GetProjectModuleListFilterByModuleAsync(filter, projectSlug));
		}


		[HttpDelete("DeleteImportTestCase/{projectModuleId}")]
		public async Task<IActionResult> DeleteImportTestCase(int projectModuleId)
		{
			return HandleResult(await _iprojectModuleService.DeleteImportTestCaseAsync(projectModuleId));
		}
	}
}
