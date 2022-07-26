import { ReactNode } from "react";
import { Route } from "react-router";
import { IRouteItem } from "../interfaces";
import { getPermissions } from "../util/auth.util";
import { lazy } from "./lazy";

export const mainRoutesWithoutSidenavList: IRouteItem[] = [
  {
    name: "Projects",
    path: "/",
    exact: true,
    LazyComponent: lazy(() => import("../pages/Project/Project")),
  },

  {
    name: "New Project",
    path: "/new-project",
    LazyComponent: lazy(() => import("../pages/Project/CreateProject")),
    exact: true,
    permission: "project.create",
  },
  {
    name: "Preferences",
    path: "/profile/preferences",
    LazyComponent: lazy(() => import("../pages/Profile/preferences")),
    exact: true,
  },
];

export const mainRoutesList: IRouteItem[] = [
  {
    name: "Dashboard",
    path: "/admin",
    permission: "user.update",
    exact: true,
    LazyComponent: lazy(() => import("../pages/Dashboard/AdminDashboard")),
  },
  {
    name: "Projects List",
    path: "/admin/projects",
    permission: "user.update",
    LazyComponent: lazy(() => import("../pages/Project/ProjectsDashboard")),
    exact: true,
  },
  {
    name: "User List",
    path: "/admin/users",
    permission: "user.update",
    exact: true,
    LazyComponent: lazy(() => import("../pages/User/UserList")),
  },
  {
    name: "Edit User",
    path: "/admin/users/:userId",
    permission: "user.update",
    exact: true,
    LazyComponent: lazy(() => import("../pages/User/EditUser")),
  },
  {
    name: "Project Dashboard",
    path: "/project/:projectSlug",
    LazyComponent: lazy(() => import("../pages/Project/ProjectDashboard")),
    exact: true,
  },
  {
    name: "Requirement Management",
    path: "/project/:projectSlug/requirement-management",
    LazyComponent: lazy(() => import("../pages/RequirementManagement")),
  },
  {
    name: "Test Case",
    path: "/project/:projectSlug/test-case",
    LazyComponent: lazy(() => import("../pages/TestCase")),
  },
  {
    name: "Test Plans",
    path: "/project/:projectSlug/test-plans",
    LazyComponent: lazy(() => import("../pages/TestPlan")),
  },
  {
    name: "Test Runs",
    path: "/project/:projectSlug/test-runs",
    LazyComponent: lazy(() => import("../pages/TestRun")),
  },
  {
    name: "Create Test Run",
    path: "/project/:projectSlug/test-runs/create",
    LazyComponent: lazy(() => import("../pages/TestRun/AddEditTestRun")),
  },
  {
    name: "Edit Test Run",
    path: "/project/:projectSlug/test-runs/edit/:testRunId",
    LazyComponent: lazy(() => import("../pages/TestRun/AddEditTestRun")),
  },
  {
    name: "Test Run Dashboard",
    path: "/project/:projectSlug/test-runs/dashboard/:testRunId",
    LazyComponent: lazy(() => import("../pages/TestRun/Dashboard")),
  },
  {
    name: "Project Members",
    path: "/project/:projectSlug/settings/members",
    LazyComponent: lazy(() => import("../pages/ProjectMembers")),
    exact: true,
  },
  {
    name: "Environment",
    path: "/project/:projectSlug/settings/environment",
    LazyComponent: lazy(() => import("../pages/Settings")),
    exact: true,
  },
];

const routerDom: ReactNode[] = [];
const parseRoutes = (routeList: IRouteItem[]) => {
  routeList.forEach((route, Key) => {
    return routerDom.push(
      <Route
        key={Key}
        exact
        {...route}
        children={(props) => <route.LazyComponent {...props} route={route} />}
      />
    );
  });
  return routerDom;
};

export const mainRoutes = parseRoutes(mainRoutesList);

export const mainRoutesWithoutSidenav = parseRoutes(
  mainRoutesWithoutSidenavList
);
