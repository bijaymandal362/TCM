import { mainServerApi } from "../mainServer";
import { addAuthHeader } from "../../../util/auth.util";
import {
  IAddUpdateTestPlanParams,
  IDragDropTestPlanBody,
  IFolderAndTestPlan,
  ITestPlanTestCases,
  ITestPlanTestCasesParams,
  ITestRunTestPlanList,
  Pagination,
} from "../../../interfaces";
import { Draft } from "immer";

export const testPlanApi = mainServerApi.injectEndpoints({
  endpoints: (build) => ({
    getFolderAndTestPlans: build.query<
      { data: IFolderAndTestPlan[] },
      { projectSlug: string; searchValue: string }
    >({
      query: (data) => ({
        url: `TestPlan/GetTestPlanList/${data.projectSlug}`,
        headers: {
          ...addAuthHeader(),
        },
        params: {
          SearchValue: data.searchValue,
        },
      }),
      providesTags: ["TestPlan"],
    }),
    addFolderOrTestPlan: build.mutation<any, IAddUpdateTestPlanParams>({
      query: (body) => ({
        url: `TestPlan/AddTestPlan`,
        method: "POST",
        body,
        responseHandler: (response) => response.text(),
        headers: {
          ...addAuthHeader(),
        },
      }),
      invalidatesTags: ["TestPlan"],
    }),
    updateFolderOrTestPlan: build.mutation<any, IAddUpdateTestPlanParams>({
      query: (body) => ({
        url: `TestPlan/UpdateTestPlan`,
        method: "PUT",
        body,
        responseHandler: (response) => response.text(),
        headers: {
          ...addAuthHeader(),
        },
      }),
      invalidatesTags: ["TestPlan"],
    }),
    deleteFolderOrTestPlan: build.mutation<any, number>({
      query: (testPlanId) => ({
        url: `TestPlan/DeleteTestPlan/${testPlanId}`,
        method: "PUT",
        responseHandler: (response) => response.text(),
        headers: {
          ...addAuthHeader(),
        },
      }),
      invalidatesTags: ["TestPlan"],
    }),
    getTestPlanTestCases: build.query<
      Pagination<ITestPlanTestCases>,
      ITestPlanTestCasesParams
    >({
      query: (params) => ({
        url: `TestPlan/GetTestPlanTestCaseByTestPlanId/${params.testPlanId}`,
        params: {
          pageNumber: params.pageNumber,
          pageSize: params.pageSize,
          searchValue: params.searchValue,
        },
        headers: {
          ...addAuthHeader(),
        },
      }),
      providesTags: ["TestPlanTestCase"],
    }),
    dragAndDropTestPlan: build.mutation<any, IDragDropTestPlanBody>({
      query: (body) => ({
        url: `TestPlan/DragDropTestPlan`,
        method: "PUT",
        body,
        responseHandler: (response) => response.text(),
        headers: {
          ...addAuthHeader(),
        },
      }),
      // async onQueryStarted(body, { dispatch, queryFulfilled }) {
      //   const patchResult = dispatch(
      //     testPlanApi.util.updateQueryData(
      //       "getFolderAndTestPlans",
      //       body.projectSlug,
      //       (draft: any): any => {
      //         console.log(body.dragDropOrderingView, "body here");
      //         // draft.push(body.dragDropOrderingView);
      //         Object.assign(draft, body.dragDropOrderingView);
      //       }
      //     )
      //   );
      //   try {
      //     await queryFulfilled;
      //   } catch {
      //     patchResult.undo();
      //   }
      // },
      invalidatesTags: ["TestPlan"],
    }),
    getTestRunTestPlanList: build.query<
      Pagination<ITestRunTestPlanList>,
      ITestPlanTestCasesParams
    >({
      query: (params) => ({
        url: `TestRun/GetTestRunTestPlanList/${params.testPlanId}`,
        method: "POST",
        params: {
          pageNumber: params.pageNumber,
          pageSize: params.pageSize,
          searchValue: params.searchValue,
        },
        headers: {
          ...addAuthHeader(),
        },
      }),
    }),
  }),
  overrideExisting: false,
});

export const {
  useGetFolderAndTestPlansQuery,
  useAddFolderOrTestPlanMutation,
  useUpdateFolderOrTestPlanMutation,
  useDeleteFolderOrTestPlanMutation,
  useGetTestPlanTestCasesQuery,
  useDragAndDropTestPlanMutation,
  useGetTestRunTestPlanListQuery,
} = testPlanApi;
