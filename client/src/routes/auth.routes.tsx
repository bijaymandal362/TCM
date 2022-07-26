import { ReactNode } from "react";
import { Route } from "react-router";
import { IRouteItem } from "../interfaces";
import { lazy } from "./lazy";

export const authRouteList: IRouteItem[] = [
  {
    name: "Login",
    path: "/login",
    LazyComponent: lazy(() => import("../pages/Login/Login")),
    exact: true,
  },
  {
    name: "Page not found",
    path: "/pagenotfound",
    LazyComponent: lazy(() => import("../pages/Error/Pagenotfound")),
  },
  {
    name: "Pdf",
    path: "/pdf/:testRunId",
    LazyComponent: lazy(() => import("../pages/Pdf")),
    exact: true,
  },
];

const routerDom: ReactNode[] = [];
const parseRoutes = (routeList: IRouteItem[]) => {
  routeList.forEach((route, Key) =>
    routerDom.push(
      <Route
        key={Key}
        {...route}
        children={(props) => <route.LazyComponent {...props} route={route} />}
      />
    )
  );
  return routerDom;
};

export const authRoutes = parseRoutes(authRouteList);
