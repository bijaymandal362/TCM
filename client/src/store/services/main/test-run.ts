import { mainServerApi } from "../mainServer";
import { addAuthHeader } from "../../../util/auth.util";
import {
  AddTestRunBody,
  UpdateTestRunBody,
  AssignAndUnAssignUserBody,
  GetEditTestRun,
  GetTestRunListParams,
  Pagination,
  TestRunData,
  TestRunList,
  TestRunTeamStats,
  TestRunTestPlans,
  GetTestRunTestCaseParams,
  TestRunTestCase,
  TestPlan,
  ProjectMember,
  TestRunResults,
  DeleteTestCaseParams,
  EnvironmentList,
  DeleteAndRetestMultipleTestCaseParams,
  getTestRunTestPlanByTestRunIdParams,
  getTestRunTestCaseDetailsParams,
  TestRunTestPlansByTestRunId,
  TestRunTestCaseDetails,
} from "../../../interfaces";

export const testRunApi = mainServerApi.injectEndpoints({
  endpoints: (build) => ({
    getTestRunLists: build.query<Pagination<TestRunList>, GetTestRunListParams>(
      {
        query: (params) => ({
          url: `TestRun/GetTestRunList/${params.projectSlug}`,
          method: "POST",
          params: {
            perPage: params.PageSize,
            page: params.PageNumber,
            searchvalue: params.SearchValue,
          },
          headers: {
            ...addAuthHeader(),
          },
        }),
        providesTags: ["TestRun"],
      }
    ),
    getEditTestRunData: build.query<GetEditTestRun, number>({
      query: (id) => ({
        url: `TestRun/GetEditTestRunById/${id}`,
        headers: {
          ...addAuthHeader(),
        },
      }),
      providesTags: ["TestRunEditData"],
    }),
    addTestRun: build.mutation<any, AddTestRunBody>({
      query: (body) => ({
        url: `TestRun/AddTestRun`,
        method: "POST",
        headers: {
          ...addAuthHeader(),
        },
        body,
        responseHandler: (response) => response.text(),
      }),
      invalidatesTags: ["TestRun"],
    }),
    updateTestRun: build.mutation<any, UpdateTestRunBody>({
      query: (body) => ({
        url: `TestRun/UpdateTestRun`,
        method: "PUT",
        headers: {
          ...addAuthHeader(),
        },
        body,
        responseHandler: (response) => response.text(),
      }),
      invalidatesTags: ["TestRun", "TestRunEditData"],
    }),
    deleteTestRun: build.mutation<any, number>({
      query: (id) => ({
        url: `TestRun/DeleteTestRun/${id}`,
        method: "DELETE",
        headers: {
          ...addAuthHeader(),
        },
        responseHandler: (response) => response.text(),
      }),
      invalidatesTags: ["TestRun"],
    }),
    getTestRunTestPlans: build.query<
      { data: TestRunTestPlans[] },
      { testRunId: number; search: string }
    >({
      query: (data) => ({
        url: `TestRun/GetTestRunTestCases/${data.testRunId}`,
        params: {
          searchvalue: data.search,
        },
        headers: {
          ...addAuthHeader(),
        },
      }),
      providesTags: ["TestRunTestCase"],
    }),
    getTestRunTestPlanByTestRunId: build.query<
      Pagination<TestRunTestPlansByTestRunId>,
      getTestRunTestPlanByTestRunIdParams
    >({
      query: (data) => ({
        url: `TestRun/GetTestRunTestPlanByTestRunId/${data.testRunId}`,
        method: "POST",
        params: {
          SearchValue: data.searchValue,
          PageSize: data.pageSize,
          PageNumber: data.pageNumber,
        },
        headers: {
          ...addAuthHeader(),
        },
      }),
      providesTags: ["TestRunTestCase"],
    }),
    getTestRunTestCaseDetails: build.query<
      Pagination<TestRunTestCaseDetails>,
      getTestRunTestCaseDetailsParams
    >({
      query: (data) => ({
        url: `TestRun/GetTestRunTestCaseDetails/${data.testRunId}/${
          data.testPlanId
        }/${data.filters.assignee || 0}`,
        method: "POST",
        params: {
          SearchValue: data.searchValue,
          PageSize: data.pageSize,
          PageNumber: data.pageNumber,
        },
        headers: {
          ...addAuthHeader(),
        },
      }),
      providesTags: ["TestRunTestCase"],
    }),
    getTestRunTeamStats: build.query<TestRunTeamStats[], number>({
      query: (id) => ({
        url: `TestRun/GetTestRunTeamStats/${id}`,
        headers: {
          ...addAuthHeader(),
        },
      }),
    }),
    getTestRunData: build.query<TestRunData, number>({
      query: (id) => ({
        url: `TestRun/GetTestRunListbyId/${id}`,
        headers: {
          ...addAuthHeader(),
        },
      }),
      providesTags: ["TestRunList"],
    }),
    assignUserToTestCases: build.mutation<any, AssignAndUnAssignUserBody>({
      query: (body) => ({
        url: "TestRun/AssignUserToTestCase",
        headers: {
          ...addAuthHeader(),
        },
        method: "PUT",
        body,
        responseHandler: (response) => response.text(),
      }),
      invalidatesTags: ["TestRunTestCase"],
    }),
    unAssignUserFromTestCases: build.mutation<any, AssignAndUnAssignUserBody>({
      query: (body) => ({
        url: "TestRun/UnAssignUserToTestCase",
        headers: {
          ...addAuthHeader(),
        },
        method: "PUT",
        body,
        responseHandler: (response) => response.text(),
      }),
      invalidatesTags: ["TestRunTestCase"],
    }),
    getTestCaseResultsData: build.query<
      TestRunResults,
      { testRunId: number; testCaseId: number; testPlanId: number }
    >({
      query: (params) => ({
        url: `TestRun/GetTestCaseResultsData/${params.testRunId}/${params.testCaseId}/${params.testPlanId}`,
        headers: {
          ...addAuthHeader(),
        },
      }),
    }),
    getTestRunTestCaseDetail: build.query<
      TestRunTestCase,
      GetTestRunTestCaseParams
    >({
      query: (params) => ({
        url: `TestRun/GetTestRunTestCaseWizard/${params.testRunID}/${params.testCaseID}/${params.testPlanID}`,
        headers: {
          ...addAuthHeader(),
        },
      }),
      providesTags: ["TestRunWizard"],
    }),
    getAllEnvironmentList: build.query<EnvironmentList[], string>({
      query: (projectSlug) => ({
        url: `TestRun/GetAllEnvironmentList/${projectSlug}`,
        headers: {
          ...addAuthHeader(),
        },
      }),
    }),
    getTestPlansByProjectSlug: build.query<
      { data: TestPlan[] },
      { projectSlug: string; searchValue: string }
    >({
      query: (params) => ({
        url: `TestRun/GetTestPlanbyProjectSlug/${params.projectSlug}`,
        params: {
          searchValue: params.searchValue,
        },
        headers: {
          ...addAuthHeader(),
        },
      }),
    }),
    getProjectMemberList: build.query<{ data: ProjectMember[] }, string>({
      query: (projectSlug) => ({
        url: `TestRun/GetProjectMemberListBySlug/${projectSlug}`,
        headers: {
          ...addAuthHeader(),
        },
      }),
    }),
    updateTestCaseFromWizard: build.mutation<any, FormData>({
      query: (body) => ({
        url: "TestRun/UpdateTestRunTestCaseHistoryWizardAsync",
        headers: {
          ...addAuthHeader(),
        },
        method: "PUT",
        body,
        responseHandler: (response) => response.text(),
      }),
      invalidatesTags: ["TestRunWizard", "TestRunTestCase", "TestRunList"],
    }),
    updateTestCaseStepFromWizard: build.mutation<any, FormData>({
      query: (body) => ({
        url: "TestRun/AddUpdateTestRunTestCaseStepHistoryWizardAsync",
        headers: {
          ...addAuthHeader(),
        },
        method: "PUT",
        body,
        responseHandler: (response) => response.text(),
      }),
      invalidatesTags: ["TestRunWizard", "TestRunTestCase", "TestRunList"],
    }),
    deleteTestCaseOfTestPlanFromTestRun: build.mutation<
      any,
      DeleteTestCaseParams
    >({
      query: (params) => ({
        url: `TestRun/DeleteTestRunTestPlanTestCaseId/${params.projectModuleId}/${params.testPlanId}/${params.testRunId}`,
        headers: {
          ...addAuthHeader(),
        },
        method: "DELETE",
        responseHandler: (response) => response.text(),
      }),
      invalidatesTags: ["TestRunTestCase", "TestRunList"],
    }),
    deleteMultipleTestCaseOfTestPlanFromTestRun: build.mutation<
      any,
      DeleteAndRetestMultipleTestCaseParams
    >({
      query: (body) => ({
        url: "TestRun/DeleteMultipleTestCaseId",
        headers: {
          ...addAuthHeader(),
        },
        body,
        method: "DELETE",
        responseHandler: (response) => response.text(),
      }),
      invalidatesTags: ["TestRunList"],
    }),
    refreshTestPlan: build.mutation<
      any,
      { testRunId: number; testPlanId: number }
    >({
      query: (params) => ({
        url: `TestRun/RefreshTestPlan/${params.testRunId}/${params.testPlanId}`,
        headers: {
          ...addAuthHeader(),
        },
        method: "POST",
        responseHandler: (response) => response.text(),
      }),
      invalidatesTags: ["TestRunTestCase", "TestRunList"],
    }),
    retestTestPlanTestCase: build.mutation<
      any,
      DeleteAndRetestMultipleTestCaseParams
    >({
      query: (body) => ({
        url: "TestRun/RetestTestPlanTestCaseId",
        headers: {
          ...addAuthHeader(),
        },
        body,
        method: "PUT",
        responseHandler: (response) => response.text(),
      }),
      invalidatesTags: ["TestRunTestCase", "TestRunList"],
    }),
    downloadTestRunFile: build.mutation<any, string>({
      query: (documentId) => ({
        url: `TestRun/DownloadTestRunWizardStausFile/${documentId}`,
        headers: {
          ...addAuthHeader(),
        },
        method: "GET",
        responseHandler: (response) => response.blob(),
      }),
    }),
  }),
  overrideExisting: false,
});

export const {
  useGetTestRunListsQuery,
  useGetEditTestRunDataQuery,
  useAddTestRunMutation,
  useUpdateTestRunMutation,
  useDeleteTestRunMutation,
  useGetTestRunTestPlansQuery,
  useGetTestRunTestPlanByTestRunIdQuery,
  useLazyGetTestRunTestCaseDetailsQuery,
  useGetTestRunTeamStatsQuery,
  useGetTestRunDataQuery,
  useAssignUserToTestCasesMutation,
  useUnAssignUserFromTestCasesMutation,
  useGetTestCaseResultsDataQuery,
  useGetTestRunTestCaseDetailQuery,
  useGetAllEnvironmentListQuery,
  useGetTestPlansByProjectSlugQuery,
  useGetProjectMemberListQuery,
  useUpdateTestCaseFromWizardMutation,
  useUpdateTestCaseStepFromWizardMutation,
  useDeleteTestCaseOfTestPlanFromTestRunMutation,
  useDeleteMultipleTestCaseOfTestPlanFromTestRunMutation,
  useRefreshTestPlanMutation,
  useRetestTestPlanTestCaseMutation,
  useDownloadTestRunFileMutation,
} = testRunApi;
