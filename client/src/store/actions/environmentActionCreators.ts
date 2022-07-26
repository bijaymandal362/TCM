import axiosInstance from "../../axios/axios";
import { setHeaders } from "../../util/localStorage.util";

export const getEnvironment = async (
  params: any,
  projectSlug: string
): Promise<[any, any]> => {
  setHeaders();
  try {
    const response = await axiosInstance({
      method: "POST",
      url: `TestRun/GetEnvironmentList/${projectSlug}`,
      params: {
        PageNumber: params.page,
        SearchValue: params.search,
        PageSize: 25,
      },
    });
    return [response, null];
  } catch (err) {
    return [null, err];
  }
};

export const AddEnvironment = async (data: any): Promise<[any, any]> => {
  setHeaders();
  try {
    const response = await axiosInstance({
      method: "POST",
      url: "/TestRun/AddEnvironment",
      data,
    });
    return [response, null];
  } catch (err) {
    return [null, err];
  }
};

export const deleteEnvironment = async (id: any): Promise<[any, any]> => {
  setHeaders();

  try {
    const response = await axiosInstance({
      method: "DELETE",
      url: `/TestRun/DeleteEnvironment/${id}`,
    });
    return [response, null];
  } catch (err) {
    return [null, err];
  }
};

export const updateEnvironment = async (data: any): Promise<[any, any]> => {
  setHeaders();
  try {
    const response = await axiosInstance({
      method: "PUT",
      url: "TestRun/UpdateEnvironment",
      data,
    });
    return [response, null];
  } catch (err) {
    return [null, err];
  }
};
