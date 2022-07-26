import { mainServerApi } from "../mainServer";
import { addAuthHeader } from "../../../util/auth.util";
import {
  GetUsersListByRoleParams,
  Pagination,
  UserListItem,
  UserTypeMetaInterface,
} from "../../../interfaces";

const usersApi = mainServerApi.injectEndpoints({
  endpoints: (build) => ({
    getUsersRoleList: build.query<UserTypeMetaInterface[], void>({
      query: () => ({
        url: "User/GetRoleList",
        headers: {
          ...addAuthHeader(),
        },
      }),
    }),
    getUsersListByRole: build.query<
      Pagination<UserListItem>,
      GetUsersListByRoleParams
    >({
      query: (params) => ({
        url: "User/GetUserListFilterByRole",
        headers: {
          ...addAuthHeader(),
        },
        params,
      }),
    }),
  }),
  overrideExisting: false,
});

export const { useGetUsersRoleListQuery, useGetUsersListByRoleQuery } =
  usersApi;
