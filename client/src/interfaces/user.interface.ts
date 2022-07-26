export interface UserDetail {
  name: string;
  username: string;
  email: string;
  role: string;
  market: string;
}

export interface UserDetailResponse extends UserDetail {
  id: number;
}

export interface UserTypeMetaInterface {
  roleId: number;
  name: string;
  userCount: number;
}

export interface UserListItem {
  userId: number;
  name: string;
  email: string;
  projectCount: number;
  createdOn: string;
  lastActivity: string;
  userRoleListItemId: number;
  roleName: string;
}

export interface GetUsersListByRoleParams {
  PageNumber: number;
  SearchValue: string;
  PageSize: number;
  roleId: string;
}
