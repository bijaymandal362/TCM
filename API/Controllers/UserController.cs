using API.Attributes;
using BusinessLayer.User;
using Microsoft.AspNetCore.Mvc;
using Models.Constant.Authorization;
using Models.GridTableFilterModel;
using Models.User;
using System.Threading.Tasks;

namespace API.Controllers
{


    public class UserController : BaseApiController
	{
		private readonly IUserService _iuserService;

		public UserController(IUserService iuserService)
		{
			_iuserService = iuserService;
		}

		
		[HttpGet("GetUserList")]
		[Permission(MenuSlug.ViewUser)]
		public async Task<IActionResult> GetUserList()
		{
			return HandleResult(await _iuserService.GetUserListAsync());
		}

		[HttpGet("GetUserDetail/{userId}")]
		[Permission(MenuSlug.ViewUser)]
		public async Task<IActionResult> GetUserDetail(int userId)
		{
			return HandleResult(await _iuserService.GetUserDetailByIdAsync(userId));
		}


		[HttpPost("GetUserListFilterByName")]
		[Permission(MenuSlug.ViewUser)]
		public async Task<IActionResult> GetUserListFilterByName([FromQuery] PaginationFilterModel filter)
		{
			return HandleResult(await _iuserService.GetUserListFilterByNameAsync(filter));
		}

		[HttpGet("GetUserListFilterByRole")]
		[Permission(MenuSlug.ViewUser)]
		public async Task<IActionResult> GetUserListFilterByRole([FromQuery] PaginationFilterModel filter, [FromQuery] int? roleId)
		{
			return HandleResult(await _iuserService.GetUserListFilterByRoleAsync(filter, roleId));
		}


		[HttpGet("GetRoleList")]
		[Permission(MenuSlug.ViewUser)]
		public async Task<IActionResult> GetRoleList()
		{
			return HandleResult(await _iuserService.GetRoleListAsync());
		}

		[HttpPut]
		[Route("UpdateUser")]
		[Permission(MenuSlug.UpdateUser)]
		public async Task<IActionResult> UpdateUser([FromBody]UserUpdateModel userModel)
		{
			return HandleResult(await _iuserService.UpdateUserAsync(userModel));

		}
	}
}
