import { addAuthHeader } from "../../../util/auth.util";
import { mainServerApi } from "../mainServer";
import {
  Projects,
  ProjectsGetParams,
} from "../../../interfaces/admin.interface";
import { GetProjectsParams, Pagination } from "../../../interfaces";

const adminDashboardApi = mainServerApi.injectEndpoints({
  endpoints: (build) => ({
    getUsersCount: build.query<number, void>({
      query: () => ({
        url: "Dashboard/GetUserCount",
        headers: {
          ...addAuthHeader(),
        },
      }),
    }),
    getProjectsCount: build.query<number, void>({
      query: () => ({
        url: "Dashboard/GetProjectCount",
        headers: {
          ...addAuthHeader(),
        },
      }),
    }),
    getTotalTestCaseTestPlanTestRunCount: build.query<any, void>({
      query: () => ({
        url: "Dashboard/GetTestCaseTestPlanTestRunCount",
        headers: {
          ...addAuthHeader(),
        },
      }),
    }),
    getTestCaseCountFromLastMonth: build.query<any, void>({
      query: () => ({
        url: "Dashboard/GetTestCaseCountFromLastMonth",
        headers: {
          ...addAuthHeader(),
        },
      }),
    }),
    getProjects: build.query<Pagination<Projects>, ProjectsGetParams>({
      query: (params) => ({
        url: `/project/GetAllProjectListFilterByModule`,
        method: "POST",
        params,
        headers: {
          ...addAuthHeader(),
        },
      }),
    }),
  }),
});

export const {
  useGetUsersCountQuery,
  useGetProjectsCountQuery,
  useGetTotalTestCaseTestPlanTestRunCountQuery,
  useGetTestCaseCountFromLastMonthQuery,
  useLazyGetProjectsQuery,
} = adminDashboardApi;
