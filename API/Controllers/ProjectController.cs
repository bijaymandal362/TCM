using API.Attributes;
using BusinessLayer.Project;
using Microsoft.AspNetCore.Mvc;
using Models.Constant.Authorization;
using Models.GridTableFilterModel;
using Models.Project;
using System.Threading.Tasks;

namespace API.Controllers
{

    public class ProjectController : BaseApiController
    {

        private readonly IProjectService _iProjectService;

        public ProjectController(IProjectService iProjectService)
        {
            _iProjectService = iProjectService;

        }

        [HttpGet("GetProjectList")]
        [Permission(MenuSlug.ViewProject)]
        public async Task<IActionResult> GetProjectList()
        {
            return HandleResult(await _iProjectService.GetProjectListAsync());
        }

        [HttpPost("GetProjectListFilterByModule")]
        [Permission(MenuSlug.ViewProject)]
        public async Task<IActionResult> GetProjectListFilterByModule([FromQuery] PaginationFilterModel filter)
        {
            return HandleResult(await _iProjectService.GetProjectListFilterByModuleAsync(filter));
        }

        [HttpGet]
        [Route("GetProjectByProjectId/{projectId}")]
        [Permission(MenuSlug.ViewProject)]
        public async Task<IActionResult> GetProjectByProjectId(int projectId)
        {
            return HandleResult(await _iProjectService.GetProjectByProjectIdAsync(projectId));
        }

        [HttpPost]
        [Route("AddProject")]
        [Permission(MenuSlug.CreateProject)]
        public async Task<IActionResult> AddProject([FromBody] ProjectModel projectModel)
        {
            return HandleResult(await _iProjectService.AddProjectAsync(projectModel));
        }

        [HttpPut]
        [Route("UpdateProject")]
        [Permission(MenuSlug.UpdateProject)]
        public async Task<IActionResult> UpdateProject([FromBody] ProjectModel projectModel)
        {
            return HandleResult(await _iProjectService.UpdateProjectAsync(projectModel));
        }

        [HttpDelete]
        [Route("DeleteProject/{projectId}")]
        [Permission(MenuSlug.DeleteProject)]
        public async Task<IActionResult> DeleteProject(int projectId)
        {
            return HandleResult(await _iProjectService.DeleteProjectAsync(projectId));
        }


        [HttpGet]
        [Route("GetRolePermission/{projectSlug}")]
        [Permission(MenuSlug.ViewProject)]
        public async Task<IActionResult> GetRolePermission(string projectSlug)
        {
            return HandleResult(await _iProjectService.GetProjectRolePermissionAsync(projectSlug));
        }

        [HttpPost("GetAllProjectListFilterByModule")]
        public async Task<IActionResult> GetAllProjectListFilterByModule([FromQuery] PaginationFilterModel filter)
        { 
            return HandleResult(await _iProjectService.GetAllProjectListFilterByModuleAsync(filter));
        }

    }
}
