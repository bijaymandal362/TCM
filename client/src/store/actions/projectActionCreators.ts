import axiosInstance from "../../axios/axios";
import { getProjectId, setHeaders } from "../../util/localStorage.util";

export const getProjectDetails = async (): Promise<[any, any]> => {
  setHeaders();
  try {
    const response = await axiosInstance({
      url: `/Project/GetProjectByProjectId/${getProjectId()}`,
      method: "GET",
    });

    return [response, null];
  } catch (err) {
    return [null, err];
  }
};

export const getDropdownLists = async (category: any): Promise<[any, any]> => {
  setHeaders();
  try {
    const response = await axiosInstance({
      url: `/Common/GetListItemByListItemCategorySystemName/${category}`,
      method: "GET",
    });
    return [response, null];
  } catch (err) {
    return [null, err];
  }
};
