import { Layout } from "antd";
import { ReactNode, useEffect, useState } from "react";
import { useAppSelector, useAppDispatch } from "../store/reduxHooks";
import { Redirect, Route, Switch } from "react-router";
import { mainRoutes, mainRoutesList } from "../routes/main.routes";
import {
  getProjectPermissions,
  toggleDrawer,
} from "../store/features/projectSlice";
import { isUserLoggedIn } from "../util/auth.util";
import Header from "./header";
import Navdrawer from "./navdrawer";
import Sidenav from "./sidenav";
import { useGetProjectsPermissionListQuery } from "../store/services/main/projects";
import { IRouteItem } from "../interfaces";

const { Content } = Layout;

function MainLayout(props: any) {
  const dispatch = useAppDispatch();
  const isSidebarCollapsed = useAppSelector(
    (state) => state.project.isSidebarCollapsed
  );

  const { data, error } = useGetProjectsPermissionListQuery(
    props?.match?.params?.projectSlug,
    { skip: !props?.match?.params?.projectSlug }
  );

  const newData: any = data;
  if (newData) {
    dispatch(getProjectPermissions(newData));
    localStorage.setItem("slugs", JSON.stringify(newData));
  }

  const [colorTheme, setColorTheme] = useState<boolean>(false);

  const drawerVisible = useAppSelector((state) => state.project.drawerVisible);

  const onClose = () => {
    dispatch(toggleDrawer());
  };
  const hideDrawer = () => {
    dispatch(toggleDrawer());
  };

  const handleThemeChange = () => {
    setColorTheme(!colorTheme);
  };

  useEffect(() => {
    if (colorTheme) {
      document.body.classList.add("dark");
      document.body.classList.remove("light");
    } else {
      document.body.classList.add("light");
      document.body.classList.remove("dark");
    }
  }, [colorTheme]);

  if (!isUserLoggedIn()) {
    return <Redirect to="/login" />;
  }
  if (error || data?.length === 0) {
    return <Redirect to="/pagenotfound" />;
  }

  const routerDom: ReactNode[] = [];
  const ProjectparseRoutes = (routeList: IRouteItem[]) => {
    routeList.forEach((route, Key) => {
      if (
        !route.permission ||
        (route.permission && data?.includes(route.permission))
      ) {
        return routerDom.push(
          <Route
            key={Key}
            {...route}
            children={(props) => (
              <route.LazyComponent {...props} route={route} />
            )}
          />
        );
      }
    });
    return routerDom;
  };

  return (
    <Layout>
      <Header changeTheme={handleThemeChange} onClose={onClose} />
      <Sidenav sidebarColllapse={isSidebarCollapsed} />
      <Navdrawer
        onClose={onClose}
        visible={drawerVisible}
        hideDrawer={hideDrawer}
      />
      <Content
        className={`page__wrapper ${isSidebarCollapsed ? "expanded" : ""}`}
      >
        <Switch>{mainRoutes}</Switch>
      </Content>
    </Layout>
  );
}

export default MainLayout;
