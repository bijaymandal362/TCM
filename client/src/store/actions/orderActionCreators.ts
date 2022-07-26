// import { IAdminData } from "./../../interfaces/admin.interface";
// import { IHttp, IHttpResponse } from "./../../interfaces/http.interface";
import axiosInstance from "../../axios/axios";
import { getUserFromStorage } from "../../util/localStorage.util";
// import * as adminActions from "./adminActions";
const storageInfo = getUserFromStorage();

const header = {
  headers: {
    Authorization: `Bearer ${storageInfo?JSON.parse(storageInfo).data.token:""}` //the token is a variable which holds the token
  }
}

export const getAllOrder = async (
  data: any
): Promise<[any, any]> => {
  try {
    const response = await axiosInstance.get(`/Orders/List?PageNumber=${data.PageNumber}&PageSize=${data.PageSize}&Search=${data.Search}&ShipStartDate=${data.ShipStartDate}&ShipEnddate=${data.ShipEnddate}&OrderBy=${data.OrderBy}&OrderDirection=${data.OrderDirection}`, header);
    return [response.data.data, null];
  } catch (err) {
    return [null, err];
  }
};

export const getAllAdminOrder = async (
  data: any
): Promise<[any, any]> => {
  try {
    const response = await axiosInstance.get(`/Orders/List-for-admin?PageNumber=${data.PageNumber}&PageSize=${data.PageSize}&Search=${data.Search}&ShipStartDate=${data.ShipStartDate}&ShipEnddate=${data.ShipEnddate}&OrderBy=${data.OrderBy}&OrderDirection=${data.OrderDirection}`, header);
    return [response.data.data, null];
  } catch (err) {
    return [null, err];
  }
};

