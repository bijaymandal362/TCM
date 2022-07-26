import axiosInstance from "../../axios/axios";
import { setHeaders } from "../../util/localStorage.util";

export const getDeveloperLists = async (): Promise<[any, any]> => {
  setHeaders();
  try {
    const response = await axiosInstance({
      method: "GET",
      url: "/ProjectModule/GetProjectModuleDeveloperList",
    });

    return [response, null];
  } catch (err) {
    return [null, err];
  }
};

export const getTestCaseList = async (): Promise<[any, any]> => {
  setHeaders();
  try {
    const response = await axiosInstance({
      method: "GET",
      url: "/ProjectModule/GetTestCaseList",
    });

    return [response, null];
  } catch (err) {
    return [null, err];
  }
};

export const getTestCaseDetails = async (id: any): Promise<[any, any]> => {
  setHeaders();

  try {
    const response = await axiosInstance({
      method: "GET",
      url: `/ProjectModule/GetTestCaseDetailbyId/${id}`,
    });
    return [response, null];
  } catch (err) {
    return [null, err];
  }
};

export const createModules = async (data: any): Promise<[any, any]> => {
  setHeaders();

  try {
    const response = await axiosInstance({
      method: "POST",
      url: "/ProjectModule/AddProjectModule",
      data,
    });
    return [response, null];
  } catch (err) {
    return [null, err];
  }
};

export const createTestCase = async (data: any): Promise<[any, any]> => {
  setHeaders();

  try {
    const response = await axiosInstance({
      method: "POST",
      url: "/ProjectModule/AddTestCaseDetail",
      data,
    });
    return [response, null];
  } catch (err) {
    return [null, err];
  }
};

export const updateModules = async (data: any): Promise<[any, any]> => {
  setHeaders();

  try {
    const response = await axiosInstance({
      method: "PUT",
      //change url based upon types
      url: "/ProjectModule/UpdateProjectModule",
      data,
    });
    return [response, null];
  } catch (err) {
    return [null, err];
  }
};

export const updateDevelopers = async (data: any): Promise<[any, any]> => {
  setHeaders();

  try {
    const response = await axiosInstance({
      method: "PUT",
      //change url based upon types
      url: "/ProjectModule/UpdateProjectModuleDeveloper",
      data,
    });
    return [response, null];
  } catch (err) {
    return [null, err];
  }
};

export const addDevelopers = async (data: any): Promise<[any, any]> => {
  setHeaders();

  try {
    const response = await axiosInstance({
      method: "POST",
      url: "/ProjectModule/AddProjectModuleDeveloper",
      data,
    });
    return [response, null];
  } catch (err) {
    return [null, err];
  }
};

export const removeDevelopers = async (id: any): Promise<[any, any]> => {
  setHeaders();

  try {
    const response = await axiosInstance({
      method: "DELETE",
      url: `/ProjectModule/DeleteProjectModuleDeveloper/${id}`,
    });
    return [response, null];
  } catch (err) {
    return [null, err];
  }
};

export const getFunctionDetails = async (id: any): Promise<[any, any]> => {
  setHeaders();

  try {
    const response = await axiosInstance({
      method: "GET",
      url: `/ProjectModule/GetDeveloperDetailByFunctionId/${id}`,
    });
    return [response, null];
  } catch (err) {
    return [null, err];
  }
};

export const deleteModules = async (id: any): Promise<[any, any]> => {
  setHeaders();

  try {
    const response = await axiosInstance({
      method: "PUT",
      //change url based upon types
      url: `/ProjectModule/DeleteProjectModule/${id}`,
    });
    return [response, null];
  } catch (err) {
    return [null, err];
  }
};

export const downloadTestCases = async (id: any): Promise<[any, any]> => {
  setHeaders();

  try {
    const response = await axiosInstance({
      method: "GET",
      responseType: "blob",
      url: `/ProjectModule/DownloadTestByFunctionId/${id}`,
    });
    return [response, null];
  } catch (err) {
    return [null, err];
  }
};

export const downloadAllTestCases = async (slug: any): Promise<[any, any]> => {
  setHeaders();

  try {
    const response = await axiosInstance({
      method: "GET",
      responseType: "blob",
      url: `/ProjectModule/DownloadTestCase/${slug}`,
    });
    return [response, null];
  } catch (err) {
    return [null, err];
  }
};

export const uploadTestCases = async (data: any): Promise<[any, any]> => {
  setHeaders();

  try {
    const response = await axiosInstance({
      method: "POST",
      //change url based upon types
      url: `/ProjectModule/ImportProjectModuleTestCase`,
      data,
    });
    return [response, null];
  } catch (err) {
    return [null, err];
  }
};

export const dragAndDropTree = async (data: any): Promise<[any, any]> => {
  setHeaders();

  try {
    const response = await axiosInstance({
      method: "PUT",
      //change url based upon types
      url: `/ProjectModule/DragDropTestCaseDetail`,
      data,
    });
    return [response, null];
  } catch (err) {
    return [null, err];
  }
};
