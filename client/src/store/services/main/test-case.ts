import { mainServerApi } from "../mainServer";
import { addAuthHeader } from "../../../util/auth.util";
import { formatTreeResponse } from "../../../util/tree.util";

export const testCaseApi = mainServerApi.injectEndpoints({
  endpoints: (build) => ({
    getTestCases: build.query<
      any,
      { projectSlug: string; searchValue: string }
    >({
      query: (data) => ({
        method: "POST",
        url: `ProjectModule/GetProjectModuleListFilterByModule/${data.projectSlug}`,
        headers: {
          ...addAuthHeader(),
        },
        params: {
          SearchValue: data.searchValue,
        },
      }),
      transformResponse: (res: any) => formatTreeResponse(res.data),
    }),
    // This API is deprecated & will be removed in the future.
    deleteTestCaseStepDetail: build.mutation<any, number>({
      query: (id) => ({
        url: `ProjectModule/DeleteTestCaseStepDetail/${id}`,
        method: "DELETE",
        responseHandler: (response) => response.text(),
        headers: {
          ...addAuthHeader(),
        },
      }),
    }),
  }),
  overrideExisting: false,
});

export const { useGetTestCasesQuery, useDeleteTestCaseStepDetailMutation } =
  testCaseApi;
