using API.Attributes;

using BusinessLayer.ProjectStarred;

using Microsoft.AspNetCore.Mvc;

using Models.Constant.Authorization;
using Models.GridTableFilterModel;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
	public class ProjectStarredController : BaseApiController
    {

        private readonly IProjectStarredService _iProjectStarredService;

        public ProjectStarredController(IProjectStarredService iProjectStarredService)
        {
            _iProjectStarredService = iProjectStarredService;

        }

        [HttpPost("GetProjectStarredListFilterByModule")]
        [Permission(MenuSlug.ViewProject)]
        public async Task<IActionResult> GetProjectStarredListFilterByModule([FromQuery] PaginationFilterModel filter)
        {
            return HandleResult(await _iProjectStarredService.GetProjectStarredListFilterByModuleAsync(filter));
        }



        [HttpPost]
        [Route("AssignProjectToProjectStarred/{projectSlug}")]
      
        public async Task<IActionResult> AssignProjectToProjectStarred(string projectSlug)
        {
            return HandleResult(await _iProjectStarredService.AssignProjectToProjectStarredAsync(projectSlug));
        }

        [HttpDelete]
        [Route("UnAssignProjectToProjectStarred/{projectSlug}")]
       
        public async Task<IActionResult> UnAssignProjectToProjectStarred(string projectSlug)
        {
            return HandleResult(await _iProjectStarredService.UnAssignProjectToProjectStarredAsync(projectSlug));
        }
    }
}
