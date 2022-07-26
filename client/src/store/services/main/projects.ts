import { mainServerApi } from "../mainServer";
import { addAuthHeader } from "../../../util/auth.util";
import {
  CreateNewProjectBody,
  GetProjectsParams,
  Project,
} from "../../../interfaces";
import { Pagination } from "../../../interfaces";

const projectsApi = mainServerApi.injectEndpoints({
  endpoints: (build) => ({
    getProjectsList: build.query<Pagination<Project>, GetProjectsParams>({
      query: (params) => ({
        url: "Project/GetProjectListFilterByModule",
        method: "POST",
        params,
        headers: {
          ...addAuthHeader(),
        },
      }),
      providesTags: ["Projects"],
    }),
    getStarredProjectsList: build.query<Pagination<Project>, GetProjectsParams>(
      {
        query: (params) => ({
          url: "ProjectStarred/GetProjectStarredListFilterByModule",
          method: "POST",
          params,
          headers: {
            ...addAuthHeader(),
          },
        }),
      }
    ),
    createNewProject: build.mutation<any, CreateNewProjectBody>({
      query: (body) => ({
        url: "Project/AddProject",
        method: "POST",
        body,
        responseHandler: (response) => response.text(),
        headers: {
          ...addAuthHeader(),
        },
      }),
      invalidatesTags: ["Projects"],
    }),
    getProjectsPermissionList: build.query<string[], string>({
      query: (projectSlug) => ({
        url: `Project/GetRolePermission/${projectSlug}`,
        headers: {
          ...addAuthHeader(),
        },
      }),
    }),
  }),
  overrideExisting: false,
});

export const {
  useGetProjectsListQuery,
  useGetStarredProjectsListQuery,
  useCreateNewProjectMutation,
  useGetProjectsPermissionListQuery,
} = projectsApi;
