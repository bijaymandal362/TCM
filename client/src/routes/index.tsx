import { Route, Switch } from "react-router-dom";
import MainLayout from "../components/mainLayout";
import MainLayoutWithoutSidebar from "../components/mainLayoutWithoutSidebar";
import { authRoutes } from "./auth.routes";
import {
  mainRoutesList,
  mainRoutesWithoutSidenav,
  mainRoutesWithoutSidenavList,
} from "./main.routes";
import { Redirect } from "react-router";
import { getPermissions } from "../util/auth.util";
import { IRouteItem } from "../interfaces";

function Routes() {
  const permissions = getPermissions();
  const filterPermittedPaths = (routes: IRouteItem[]) => {
    return routes.filter(
      (route) =>
        !route.permission ||
        (route.permission && permissions?.includes(route.permission))
    );
  };

  let slugs: any = localStorage?.getItem("slugs");

  const filterProjectPermittedPaths = (routes: any) => {
    return routes.filter(
      (route: any) =>
        !route.permission ||
        (route.permission && JSON.parse(slugs)?.includes(route.permission)) ||
        (route.permission && permissions?.includes(route.permission))
    );
  };

  return (
    <Switch>
      {authRoutes}

      {filterPermittedPaths(mainRoutesWithoutSidenavList).map((item) => {
        return (
          <Route
            key={item.name}
            exact
            path={item.path}
            component={MainLayoutWithoutSidebar}
          />
        );
      })}

      {filterProjectPermittedPaths(mainRoutesList).map((item: any) => {
        return (
          <Route
            key={item.name}
            exact
            path={item.path}
            component={MainLayout}
          />
        );
      })}

      <Redirect to="/pagenotfound" />
    </Switch>
  );
}

export default Routes;
