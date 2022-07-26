using Entities;
using Models.Core;
using Models.GridTableFilterModel;
using Models.GridTableProperty;
using Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.User
{
	public interface IUserService
	{
		Task<Result<List<UserListModel>>> GetUserListAsync();
		Task<Result<PagedResponseModel<List<UserModel>>>> GetUserListFilterByNameAsync(PaginationFilterModel filter);

		Task<Result<PagedResponseModel<List<UserModel>>>> GetUserListFilterByRoleAsync(PaginationFilterModel filter, int? roleId);

		Task<Result<List<UserRoleListModel>>> GetRoleListAsync();

		Task<Result<string>> UpdateUserAsync(UserUpdateModel model);
		Task<Result<UserDetailModel>> GetUserDetailByIdAsync(int userId);



	}
}
