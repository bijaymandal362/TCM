import { ILoginData } from "./../interfaces/auth.interface";
import { LocalStorageKeys, ThemeList } from "../enum/enum";
import axiosInstance from "../axios/axios";
import { getToken } from "./auth.util";

export const getUserFromStorage = (): any => {
  let user = localStorage.getItem(LocalStorageKeys.USER);

  return user;
};

export const clearLocalStorage = () => {
  localStorage.removeItem(LocalStorageKeys.USER);
};

export const addAuthToLocalStorage = (user: ILoginData) => {
  localStorage.setItem(LocalStorageKeys.USER, JSON.stringify(user));
};

export const getTheme = (): ThemeList | null => {
  return localStorage.getItem(LocalStorageKeys.THEME) as ThemeList;
};

export const setTheme = (theme: string) => {
  localStorage.setItem(LocalStorageKeys.THEME, theme);
};

export const setAccount = (account: string) => {
  localStorage.setItem(LocalStorageKeys.ACCOUNT, account);
};

export const getAccount = (): string | null => {
  return localStorage.getItem(LocalStorageKeys.ACCOUNT);
};

export const getProjectId = (): any => {
  return localStorage.getItem(LocalStorageKeys.PROJECT);
};

export const setHeaders = () => {
  if (getUserFromStorage()) {
    axiosInstance.defaults.headers.common[
      "Authorization"
    ] = `Bearer ${getToken()}`;
  } else {
    delete axiosInstance.defaults.headers.common["Authorization"];
  }
};
