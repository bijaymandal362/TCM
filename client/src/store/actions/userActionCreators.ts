import axiosInstance from "../../axios/axios";
import { setHeaders } from "../../util/localStorage.util";

export const updateUser = async (data: any): Promise<[any, any]> => {
  setHeaders();
  try {
    const response = await axiosInstance({
      url: "/User/UpdateUser",
      method: "PUT",
      data: data,
    });
    return [response, null];
  } catch (err) {
    return [null, err];
  }
};

export const getUserDetail = async (
  user: string | number
): Promise<[any, any]> => {
  setHeaders();
  try {
    const requestUrl = `/User/GetUserDetail/${user}`;
    const response = await axiosInstance({
      url: requestUrl,
      method: "GET",
    });
    return [response, null];
  } catch (err) {
    return [null, err];
  }
};
