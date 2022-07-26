// import { ILoginData } from "./../../interfaces/auth.interface";
// import { IHttp, IHttpResponse } from "./../../interfaces/http.interface";
import { message } from "antd";
import axios from "axios";
import axiosInstance from "../../axios/axios";
import { clearLocalStorage } from "../../util/localStorage.util";
import * as authActions from "./authActions";

export const toogleDrawer = (toggleStatus: boolean) => {
  return {
    type: authActions.TOGGLE_DRAWER,
    payload: toggleStatus,
  };
};
export const checkLogin = (isAuth: boolean) => {
  // some aync logic
  return {
    type: authActions.CHECK_TOKEN,
    isAuth: isAuth,
  };
};

export const expireToken = (history: any, timer: any) => {
  // some aync logic
  setTimeout(() => {
    clearLocalStorage();
    history.push("/login");
  }, timer);
};

export const login = async (data: any): Promise<[any, any]> => {
  try {
    const response: any = await axiosInstance.post("/admin/authenticate", data);
    // localStorage.setItem("token", response.token);
    return [response, null];
  } catch (err) {
    return [null, err];
  }
};

export const customerLogin = async (
  data: any,
  setLoading: (bool: boolean) => void
): Promise<[any, any]> => {
  try {
    const response = await axios.post(
      `${process.env.REACT_APP_BASE_URL}/Account/Login`,
      data
    );
    return [response, null];
  } catch (err: any) {
    if (err.message === "Network Error") {
      message.error("Server is currently taken down for maintenance.", 5);
      setLoading(false);
      return [null, null];
    }
    return [null, err.response?.data];
  }
};

export const signup = async (data: any) => {
  try {
    const response = await axiosInstance.post("/customer/register", data);
    return [response, null];
  } catch (err) {
    return [null, err];
  }
};

export const checkCustomerExist = async (data: any) => {
  try {
    const response = await axiosInstance.post(
      "/customer/checkcustomerifexists",
      data
    );
    return [response, null];
  } catch (err) {
    return [null, err];
  }
};

export const resetPassword = async (data: any) => {
  try {
    const response = await axiosInstance.post("/auth/reset-password", data);
    return [response, null];
  } catch (err) {
    return [null, err];
  }
};

export const confirmUserEmail = async (data: any) => {
  try {
    const response = await axiosInstance.post("/auth/confirm-email", data);
    return [response, null];
  } catch (err) {
    return [null, err];
  }
};

export const forgotPassword = async (data: any) => {
  try {
    const response = await axiosInstance.post("/auth/forgot-password", data);
    return [response, null];
  } catch (err) {
    return [null, err];
  }
};

export const resendLink = async (email: any) => {
  try {
    const response = await axiosInstance.get(
      `/auth/resend-verification-email/${email}`
    );
    return [response, null];
  } catch (err) {
    return [null, err];
  }
};
