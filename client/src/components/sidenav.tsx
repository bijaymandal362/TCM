import { Menu, Spin, Typography } from "antd";
import Sider from "antd/lib/layout/Sider";
import SubMenu from "antd/lib/menu/SubMenu";
import { Link, useLocation } from "react-router-dom";
import { useHistory, useRouteMatch } from "react-router";
import { NavLinks } from "../interfaces";
import { adminNavList, projectNavList } from "./sideNavList";
import { useEffect, useState } from "react";
import { MenuFoldOutlined, MenuUnfoldOutlined } from "@ant-design/icons";
import { getPermissions } from "../util/auth.util";
import { useAppDispatch } from "../store/reduxHooks";
import { toggleSidebarCollapse } from "../store/features/projectSlice";
import { useGetProjectNameFromProjectSlugQuery } from "../store/services/main/project-dashboard";
interface IProps {
  sidebarColllapse: boolean;
}

function Sidenav({ sidebarColllapse }: IProps) {
  const location = useLocation();
  const history = useHistory();
  const route = useRouteMatch<{ projectSlug: string }>();
  const projectSlug = route.params.projectSlug;
  const permissions = getPermissions();
  const slugs: any = localStorage.getItem("slugs");
  const dispatch = useAppDispatch();

  // States
  const [isHidden, setIsHidden] = useState(false);
  const [mode, setMode] = useState<any>(
    sidebarColllapse ? "vertical" : "inline"
  );

  const getProjectNameFromProjectSlug =
    useGetProjectNameFromProjectSlugQuery(projectSlug);

  const filterPermittedNavs = (routes: NavLinks[]) => {
    return routes.filter(
      (route) =>
        !route.permission ||
        (route.permission && permissions?.includes(route.permission))
    );
  };

  const filterProjectPermittedNavs = (routes: NavLinks[]) => {
    return routes.filter(
      (route) =>
        !route.permission ||
        (route.permission && JSON.parse(slugs)?.includes(route.permission))
    );
  };

  const composeNavLinks = () => {
    if (route.path.startsWith("/admin"))
      return filterPermittedNavs(adminNavList(projectSlug));
    return filterProjectPermittedNavs(projectNavList(projectSlug));
  };

  useEffect(() => {
    setMode(sidebarColllapse ? "vertical" : "inline");
  }, [sidebarColllapse]);

  useEffect(() => {
    localStorage.setItem("projectName", projectSlug);
  }, []);

  return (
    <Sider
      trigger={
        <Menu>
          <Menu.Item
            key="1"
            onClick={() => dispatch(toggleSidebarCollapse())}
            className="btn__sider__trigger"
            icon={
              sidebarColllapse ? <MenuUnfoldOutlined /> : <MenuFoldOutlined />
            }
          >
            {!sidebarColllapse && "Collapse Sidebar"}
          </Menu.Item>
        </Menu>
      }
      collapsible
      collapsed={sidebarColllapse}
      breakpoint="lg"
      onBreakpoint={(broken) => {
        if (broken) return setIsHidden(true);
        return setIsHidden(false);
      }}
      hidden={isHidden}
    >
      {projectSlug && (
        <Typography.Title level={5} className="mt-1 mb-2">
          {getProjectNameFromProjectSlug.isLoading ? (
            <Spin />
          ) : (
            getProjectNameFromProjectSlug.data?.projectName
          )}
        </Typography.Title>
      )}
      <Menu
        mode={mode}
        defaultOpenKeys={["/" + location.pathname.split("/")[1]]}
        defaultSelectedKeys={[location.pathname]}
        theme="light"
      >
        {composeNavLinks().map((navLink: NavLinks) => {
          if (navLink.children) {
            return (
              <SubMenu
                icon={navLink.icon}
                key={navLink.href}
                title={navLink.title}
              >
                {navLink.children.map((childrenLink: NavLinks) => (
                  <Menu.Item key={childrenLink.href} icon={childrenLink.icon}>
                    <Link to={childrenLink.href}>{childrenLink.title}</Link>
                  </Menu.Item>
                ))}
              </SubMenu>
            );
          }
          return (
            <Menu.Item
              key={navLink.href}
              icon={navLink.icon}
              onClick={() => history.push(navLink.href)}
            >
              <Link to={navLink.href}>{navLink.title}</Link>
            </Menu.Item>
          );
        })}
      </Menu>
    </Sider>
  );
}

export default Sidenav;
