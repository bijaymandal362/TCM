import {
  IDefaultFunctionTestCaseListCountByProjectSlug,
  IFunctionTestCaseListCount,
  ITestCaseListDetailStatusCountData,
  ITestCaseRepositoryList,
  ITestCaseTestPlanTestRunCount,
  ITestRunList,
  ITestRunStatusCountById,
} from "../../../interfaces/project-dashboard.interface";
import { addAuthHeader } from "../../../util/auth.util";
import { mainServerApi } from "../mainServer";

export const projectDashboardApi = mainServerApi.injectEndpoints({
  endpoints: (build) => ({
    getTestCaseTestPlanTestRunCount: build.query<
      ITestCaseTestPlanTestRunCount,
      any
    >({
      query: (projectSlug) => ({
        url: `Dashboard/GetTestCaseTestPlanTestRunCount/${projectSlug}`,
        headers: {
          ...addAuthHeader(),
        },
      }),
      providesTags: ["ProjectDashboard"],
    }),
    getTestCaseListDetailStatusCount: build.query<
      ITestCaseListDetailStatusCountData[],
      any
    >({
      query: (projectSlug) => ({
        url: `Dashboard/GetTestCaseListDetailStatusCount/${projectSlug}`,
        headers: {
          ...addAuthHeader(),
        },
      }),
      keepUnusedDataFor: 0,
    }),
    getTestCaseRepositoryList: build.query<ITestCaseRepositoryList[], any>({
      query: (projectSlug) => ({
        url: `Dashboard/GetTestCaseRepositoryList/${projectSlug}`,
        headers: {
          ...addAuthHeader(),
        },
      }),
    }),
    getFunctionTestCaseListCount: build.query<
      IFunctionTestCaseListCount[],
      { projectSlug: string; projectModuleId: string }
    >({
      query: ({ projectSlug, projectModuleId }) => ({
        url: `Dashboard/GetFunctionTestCaseListCount/${projectSlug}/${projectModuleId}`,
        headers: {
          ...addAuthHeader(),
        },
      }),
    }),

    getDefaultFunctionTestCaseListCountByProjectSlug: build.query<
      IDefaultFunctionTestCaseListCountByProjectSlug[],
      any
    >({
      query: (projectSlug) => ({
        url: `Dashboard/GetDefaultFunctionTestCaseListCountByProjectSlug/${projectSlug}`,
        headers: {
          ...addAuthHeader(),
        },
      }),
    }),
    //api 2

    getTestRunList: build.query<ITestRunList[], any>({
      query: (projectSlug) => ({
        url: `Dashboard/GetTestRunList/${projectSlug}`,
        headers: {
          ...addAuthHeader(),
        },
      }),
      keepUnusedDataFor: 0,
    }),
    getTestRunStatusCountById: build.query<ITestRunStatusCountById, any>({
      query: (testRunId) => ({
        url: `Dashboard/GetTestRunStatusCountById/${testRunId}`,
        headers: {
          ...addAuthHeader(),
        },
      }),
      keepUnusedDataFor: 0,
    }),

    getProjectNameFromProjectSlug: build.query<any, string>({
      query: (projectSlug) => ({
        url: `Common/GetProjectNameFromProjectSlug/${projectSlug}`,
        headers: {
          ...addAuthHeader(),
        },
      }),
    }),

    starProject: build.mutation<any, string>({
      query: (projectSlug) => ({
        url: `ProjectStarred/AssignProjectToProjectStarred/${projectSlug}`,
        method: "POST",
        responseHandler: (response) => response.text(),
        headers: {
          ...addAuthHeader(),
        },
      }),
    }),

    unStarProject: build.mutation<any, string>({
      query: (projectSlug) => ({
        url: `ProjectStarred/UnAssignProjectToProjectStarred/${projectSlug}`,
        method: "DELETE",
        responseHandler: (response) => response.text(),
        headers: {
          ...addAuthHeader(),
        },
      }),
    }),
  }),
});

export const {
  useGetTestCaseTestPlanTestRunCountQuery,
  useGetTestCaseListDetailStatusCountQuery,
  useGetTestCaseRepositoryListQuery,
  useGetFunctionTestCaseListCountQuery,
  useGetDefaultFunctionTestCaseListCountByProjectSlugQuery,
  useGetTestRunListQuery,
  useGetTestRunStatusCountByIdQuery,
  useGetProjectNameFromProjectSlugQuery,
  useStarProjectMutation,
  useUnStarProjectMutation,
} = projectDashboardApi;
