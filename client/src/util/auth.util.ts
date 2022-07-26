import { getUserFromStorage } from "./localStorage.util";
import { decodeToken } from "react-jwt";
export const isUserLoggedIn = () => {
  if (!!getUserFromStorage()) {
    return true;
  }
  return false;
};

export const parseUser = () => {
  const user = getUserFromStorage();

  try {
    return user ? JSON.parse(user) : {};
  } catch (err) {}
};

export const getToken = () => {
  return parseUser()?.token;
};

export const getAppearance = (token: string) => {
  const decodedToken = decodeToken(token) as { [key: string]: any };
  return decodedToken ? decodedToken?.theme : null;
};

export const getPermissions = (): any => {
  const user = parseUser();
  const decodedToken = decodeToken(user?.token) as { [key: string]: any };
  return decodedToken ? JSON.parse(decodedToken?.permission) : null;
};

export const getRoleId = (): number => {
  const user = parseUser();
  const decodedToken = decodeToken(user?.token) as { [key: string]: any };
  return decodedToken ? JSON.parse(decodedToken?.roleId) : null;
};

export const getExpiryDate = (): number => {
  const user = parseUser();
  const decodedToken = decodeToken(user?.token) as { [key: string]: any };
  return decodedToken ? JSON.parse(decodedToken?.exp) : null;
};

// export const getProjectPermissionMap=():any=>{
//   return {
//     'owner':['project.create','project.read'],
//     'maintainer':['project.delete']
//   }
// }

const projectPermissionMap: any = {
  owner: ["project.create", "project.read"],
  maintainer: ["project.delete"],
};

export const getProjectPermission = (role: string): any => {
  return projectPermissionMap?.role;
};

export const addAuthHeader = () => ({ authorization: `Bearer ${getToken()}` });
