import {
  CloseOutlined,
  LogoutOutlined,
  SearchOutlined,
  SettingOutlined,
  SmileOutlined,
  EditOutlined,
  UserOutlined,
  ToolOutlined,
  // DownOutlined,
} from "@ant-design/icons";
import { Button, Dropdown, Input, Menu } from "antd";
import { Header } from "antd/lib/layout/layout";
import React, { useEffect, useState } from "react";
import { Link, useHistory } from "react-router-dom";
import { useLocation } from "react-router";
import Logo from "../assets/images/logo.png";
// import Profile from "../assets/images/logo.png";
import { clearLocalStorage } from "../util/localStorage.util";
import CheckPermission from "../hoc/checkPermission";
import { useGetUserProfileQuery } from "../store/services/main/profile";
import { getExpiryDate } from "../util/auth.util";
import useUpdateEffect from "../util/custom-hooks/useUpdateEffect";

const Navbar = (props: any) => {
  const history = useHistory();
  const location = useLocation();

  const { data: profileDetails, error: profileDetailsError } =
    useGetUserProfileQuery();

  const [showResponsiveSearch, setShowResponsiveSearch] =
    useState<boolean>(false);

  const handleResponsiveSearch = () => {
    setShowResponsiveSearch(true);
  };

  const handleResponsiveSearchClose = () => {
    setShowResponsiveSearch(false);
  };

  const signOutHandler = (
    e: React.MouseEvent<HTMLAnchorElement, MouseEvent>
  ) => {
    e.preventDefault();
    clearLocalStorage();
    history.push("/login");
  };

  // If token expires & user is logged in, logout user (actually bottom one is enough)
  useEffect(() => {
    if (getExpiryDate() * 1000 < Date.now()) {
      clearLocalStorage();
      history.push("/login");
    }
  }, []);

  // if user is unauthorized, redirect to login page
  useUpdateEffect(() => {
    if (profileDetailsError && (profileDetailsError as any)?.status === 401) {
      clearLocalStorage();
      history.push("/login");
    }
  }, [profileDetailsError]);

  // const menuNotification = (
  //   <Menu className="notification__dropdown">
  //     <Menu.Item>
  //       <div className="notification__header">
  //         <h4>Notification</h4>
  //         <Button type="text" className="clear__btn">
  //           Clearn
  //         </Button>
  //       </div>
  //     </Menu.Item>
  //     <Menu.Item>
  //       <div className="notification__list">
  //         <Avatar size={40}>
  //           <img src={Profile} alt="" />
  //         </Avatar>
  //         <div className="comment__section">
  //           <span className="commentor__name">Erin Gonzales </span>
  //           <span className="comment__text">has comment on your board</span>
  //         </div>
  //         <span className="comment__time">7:57PM</span>
  //       </div>
  //     </Menu.Item>
  //     <Menu.Item>
  //       <div className="notification__list">
  //         <Avatar size={40}>
  //           <img src={Profile} alt="" />
  //         </Avatar>
  //         <div className="comment__section">
  //           <span className="commentor__name">Erin Gonzales </span>
  //           <span className="comment__text">has comment on your board</span>
  //         </div>
  //         <span className="comment__time">7:57PM</span>
  //       </div>
  //     </Menu.Item>
  //     <Menu.Item>
  //       <div className="notification__list">
  //         <Avatar size={40}>
  //           <img src={Profile} alt="" />
  //         </Avatar>
  //         <div className="comment__section">
  //           <span className="commentor__name">Erin Gonzales </span>
  //           <span className="comment__text">has comment on your board</span>
  //         </div>
  //         <span className="comment__time">7:57PM</span>
  //       </div>
  //     </Menu.Item>
  //     <Menu.Item>
  //       <div className="notification__list">
  //         <Avatar size={40}>
  //           <img src={Profile} alt="" />
  //         </Avatar>
  //         <div className="comment__section">
  //           <span className="commentor__name">Erin Gonzales </span>
  //           <span className="comment__text">has comment on your board</span>
  //         </div>
  //         <span className="comment__time">7:57PM</span>
  //       </div>
  //     </Menu.Item>
  //   </Menu>
  // );

  // const menuLanguage = (
  //   <Menu>
  //     <Menu.Item>
  //       <a
  //         target="_blank"
  //         rel="noopener noreferrer"
  //         href="https://www.antgroup.com"
  //       >
  //         English
  //       </a>
  //     </Menu.Item>
  //     <Menu.Item>
  //       <a
  //         target="_blank"
  //         rel="noopener noreferrer"
  //         href="https://www.aliyun.com"
  //       >
  //         Japanese
  //       </a>
  //     </Menu.Item>
  //     <Menu.Item>
  //       <a
  //         target="_blank"
  //         rel="noopener noreferrer"
  //         href="https://www.luohanacademy.com"
  //       >
  //         Korean
  //       </a>
  //     </Menu.Item>
  //   </Menu>
  // );

  const menuProfile = (
    <Menu className="profile__dropdown">
      <Menu.Item
        key="1"
        className="user-profile-name"
        style={{
          pointerEvents: "none",
        }}
      >
        <p>{profileDetails?.name || "Your Name will appear here."}</p>
        <p>{`@${profileDetails?.userName || "username will appear here."}`}</p>
      </Menu.Item>
      {/* <Menu.Item key="2">
        <Link to="#" className="flex-menu-item">
          <span className="link__icons">
            <SmileOutlined />
          </span>
          Set status
        </Link>
      </Menu.Item>
      <Menu.Item key="3">
        <Link to="#" className="flex-menu-item">
          <span className="link__icons">
            <EditOutlined />
          </span>
          Edit profile
        </Link>
      </Menu.Item> */}
      <Menu.Item key="4">
        <Link to="/profile/preferences" className="flex-menu-item">
          <span className="link__icons">
            <SettingOutlined />
          </span>
          Preferences
        </Link>
      </Menu.Item>
      <Menu.Item key="5" className="sign-out-btn">
        <Link to="#" onClick={signOutHandler} className="flex-menu-item">
          <span className="link__icons">
            <LogoutOutlined />
          </span>
          Sign out
        </Link>
      </Menu.Item>
    </Menu>
  );

  return (
    <Header className="m-0 p-0 page__header">
      <Link className="logo px-3" to="/">
        <img src={Logo} alt="Logo" height="50px" />
      </Link>
      <ul className="navigation__left">
        <li>
          <Menu
            mode="horizontal"
            className="d-flex align-items-center justify-content-center"
            defaultSelectedKeys={[location.pathname]}
          >
            <CheckPermission slug="adminuser.read">
              <Menu.Item
                key="/admin/users"
                className="d-flex align-items-center"
                icon={<SettingOutlined />}
              >
                <Link to="/admin/users">Settings</Link>
              </Menu.Item>
            </CheckPermission>
          </Menu>
        </li>
      </ul>
      <div className="header__main">
        <div
          className={`responsive__search ${showResponsiveSearch ? "show" : ""}`}
        >
          <div className="search__wrapper">
            <Button>
              <SearchOutlined />
            </Button>
            <Input placeholder="Basic usage" />
          </div>
          <Button
            onClick={handleResponsiveSearchClose}
            className="btn__close__search"
          >
            <CloseOutlined />
          </Button>
        </div>

        <ul className="navigation__right">
          <li className="display__lg">
            <Button
              className="btn__search__sm"
              onClick={handleResponsiveSearch}
            >
              <SearchOutlined />
            </Button>
          </li>
          {/* <li>
            <Dropdown
              overlay={menuNotification}
              placement="bottomRight"
              arrow
              trigger={["click"]}
            >
              <Button className="btn__notification">
                <BellOutlined />
                <Badge count={5} />
              </Button>
            </Dropdown>
          </li> */}
          <CheckPermission slug="adminuser.read" roleId={25}>
            <li className="admin-route-link">
              <Link to="/admin">
                <ToolOutlined />
              </Link>
            </li>
          </CheckPermission>
          <li>
            <Dropdown
              overlay={menuProfile}
              placement="bottomRight"
              arrow
              trigger={["click"]}
            >
              <Button className="btn__profile">
                <span className="profile__image">
                  {profileDetails?.profileImage ? (
                    <img
                      src={profileDetails?.profileImage}
                      alt={profileDetails?.name}
                    />
                  ) : (
                    <UserOutlined />
                  )}
                  {/* <DownOutlined
                    style={{ fontSize: "12px", marginLeft: "4px" }}
                  /> */}
                </span>
              </Button>
            </Dropdown>
          </li>
        </ul>
      </div>
    </Header>
  );
};

export default Navbar;
