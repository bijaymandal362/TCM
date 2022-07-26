using System;

namespace Models.User
{
    public class UserModel
	{
		public int UserId { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public int ProjectCount { get; set; }

		public DateTimeOffset CreatedOn { get; set; }

		public DateTimeOffset LastActivity { get; set; }

		public int UserRoleListItemId { get; set; }
		public string RoleName { get; set; }
	}

	public class UserRoleListModel
	{
		public int RoleId { get; set; }
		public string Name { get; set; }

		public int UserCount { get; set; }

				
	}

	public class UserListModel
	{
		public int UserId { get; set; }
		public string Name { get; set; }

		public int ProjectCount { get; set; }

		public DateTimeOffset CreatedOn { get; set; }

		public DateTimeOffset LastActivity { get; set; }

		public string UserRoleName { get; set; }
		
		public int UserRoleListItemId { get; set; }

		public string UserMarket { get; set; }

		public int UserMarketId { get; set; }
	}
	public class UserUpdateModel
	{
		public int UserId { get; set; }
		public int UserRoleListItemId { get; set; }

		public int UserMarketListItemId { get; set; }
	}
	public class UserDetailModel
	{

		public int UserId { get; set; }
		public string Name { get; set; }

		public string Email { get; set; }

		public string Username { get; set; }

		public string Role { get; set; }
		public int RoleId { get; set; }

		public string Market { get; set; }
		public int MarketId { get; set; }



	}
}
