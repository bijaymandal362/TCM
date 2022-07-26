using API.Attributes;
using BusinessLayer.ProjectMember;
using Microsoft.AspNetCore.Mvc;
using Models.Constant.Authorization;
using Models.GridTableFilterModel;
using Models.ProjectMember;
using System.Threading.Tasks;

namespace API.Controllers
{

    public class ProjectMemberController : BaseApiController
    {

        private readonly IProjectMemberService _iProjectMemberService;

        public ProjectMemberController(IProjectMemberService iProjectMemberService)
        {
            _iProjectMemberService = iProjectMemberService;
        }

        [HttpPost]
        [Route("AddProjectMember")]       
        public async Task<IActionResult> AddProjectMember([FromBody] ProjectMemberModel model)
        {
            return HandleResult(await _iProjectMemberService.AddProjectMemberAsync(model));
        }

        [HttpPost]
        [Route("GetProjectMemberListFilterByProjectId/{projectSlug}")] 
        public async Task<IActionResult> GetProjectMemberListFilterByProjectId([FromQuery] PaginationFilterModel filter, string projectSlug)
        {
            return HandleResult(await _iProjectMemberService.GetProjectMemberListFilterByProjectIdAsync(filter, projectSlug));
        }


        [HttpGet]
        [Route("GetProjectMemberListBySlug/{projectSlug}")]
        public async Task<IActionResult> GetProjectMemberListBySlug(string projectSlug)
        {
            return HandleResult(await _iProjectMemberService.GetProjectMemberListBySlugAsync(projectSlug));
        }

        [HttpGet]
        [Route("GetProjectMemberListAll")]       
        public async Task<IActionResult> GetProjectMemberListAll()
        {
            return HandleResult(await _iProjectMemberService.GetProjectMemberListAllAsync());
        }

        [HttpDelete]
        [Route("DeleteProjectMember/{projectMemberId}")]       
        public async Task<IActionResult> DeleteProjectMember(int projectMemberId)
        {
            return HandleResult(await _iProjectMemberService.DeleteProjectMemberAsync(projectMemberId));
        }

        [HttpPut]
        [Route("UpdateProjectMember")]  
        public async Task<IActionResult> UpdateProjectMember([FromBody] ProjectMemberModel projectModel)
        {
            return HandleResult(await _iProjectMemberService.UpdateProjectMemberAsync(projectModel));
        }
    }
}
