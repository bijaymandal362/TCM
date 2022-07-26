import { ArrowLeftOutlined } from "@ant-design/icons";
import { Button, Drawer, Menu, Typography } from "antd";
import SubMenu from "antd/lib/menu/SubMenu";
import { Link, useLocation } from "react-router-dom";
import { useRouteMatch } from "react-router";
import { NavLinks } from "../interfaces";
import { getUserFromStorage } from "../util/localStorage.util";
import { adminNavList, projectNavList } from "./sideNavList";
import { getPermissions } from "../util/auth.util";

interface IProps {
  hideDrawer: () => void;
  visible: boolean;
  onClose: () => void;
}

function Navdrawer({ visible, hideDrawer, onClose }: IProps) {
  const location = useLocation();
  const route = useRouteMatch<{ projectSlug: string }>();

  const projectSlug = route.params.projectSlug;

  const permissions = getPermissions();

  const slugs: any = localStorage.getItem("slugs");

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

  return (
    <Drawer
      placement="left"
      closable={false}
      onClose={onClose}
      visible={visible}
      zIndex={9}
    >
      <div className="drawer__header">
        {/* <div>
                        <img src={Logo} alt="" height="50" />
                    </div> */}
        <Typography.Text strong>TCMS</Typography.Text>
        <Button onClick={hideDrawer} className="drawer__trigger">
          <ArrowLeftOutlined />
        </Button>
      </div>

      <Menu
        mode={"inline"}
        defaultOpenKeys={["/" + location.pathname.split("/")[1]]}
        defaultSelectedKeys={[location.pathname]}
        theme="light"
      >
        debugger;
        {composeNavLinks().map((navLink: NavLinks) => {
          let storage = getUserFromStorage();
          if (storage) {
            let isAdmin = JSON.parse(storage).isAdmin;
            if (!isAdmin) {
              if (
                navLink.title === "Admin Management" ||
                navLink.title === "User Management"
              ) {
                return null;
              }
            }
          }

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
            <Menu.Item key={navLink.href} icon={navLink.icon}>
              <Link to={navLink.href}>{navLink.title}</Link>
            </Menu.Item>
          );
        })}
      </Menu>
    </Drawer>
  );
}

export default Navdrawer;
