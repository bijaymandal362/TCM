import axiosInstance from "../../axios/axios";
import { setHeaders } from "../../util/localStorage.util";

export const getUser = async (param: any): Promise<[any, any]> => {
  setHeaders();
  try {
    const response = await axiosInstance({
      method: "POST",
      url: `/ProjectMember/GetProjectMemberListFilterByProjectId/${param.projectId}`,
      params: {
        PageNumber: param.page,
        PageSize: param.pageSize,
        SearchValue: param.search,
      },
    });
    return [response, null];
  } catch (err) {
    return [null, err];
  }
};

export const getDevelopers = async (param: any): Promise<[any, any]> => {
  setHeaders();
  try {
    const response = await axiosInstance({
      method: "POST",
      url: `/ProjectMember/GetProjectMemberListFilterByProjectId/${param.projectId}`,
    });
    return [response, null];
  } catch (err) {
    return [null, err];
  }
};

export const getPersons = async (): Promise<[any, any]> => {
  setHeaders();
  try {
    const response = await axiosInstance({
      method: "GET",
      url: "/Person/GetPersonList",
    });
    return [response, null];
  } catch (err) {
    return [null, err];
  }
};

export const getSearchPerson = async (search: any): Promise<[any, any]> => {
  setHeaders();
  let newParams: any = {};
  if (search) {
    newParams["SearchValue"] = search;
  }
  try {
    const response = await axiosInstance({
      method: "POST",
      url: "/Person/GetPersonListFilterByName",
      params: newParams,
    });
    return [response, null];
  } catch (err) {
    return [null, err];
  }
};

export const AddProject = async (data: any): Promise<[any, any]> => {
  setHeaders();

  try {
    const response = await axiosInstance({
      method: "POST",
      url: "/ProjectMember/AddProjectMember",
      data,
    });
    return [response, null];
  } catch (err) {
    return [null, err];
  }
};

export const updateProject = async (data: any): Promise<[any, any]> => {
  setHeaders();

  try {
    const response = await axiosInstance({
      method: "PUT",
      url: "/ProjectMember/updateProjectMember",
      data,
    });
    return [response, null];
  } catch (err) {
    return [null, err];
  }
};

export const deleteProject = async (id: any): Promise<[any, any]> => {
  setHeaders();

  try {
    const response = await axiosInstance({
      method: "DELETE",
      url: `/ProjectMember/DeleteProjectMember/${id}`,
    });
    return [response, null];
  } catch (err) {
    return [null, err];
  }
};
