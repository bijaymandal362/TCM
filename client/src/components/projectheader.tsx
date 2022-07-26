import { Layout, Menu } from "antd";
import Avatar from "antd/lib/avatar/avatar";
import Logo from "../assets/images/logo.png";

const { Header } = Layout;
const ProjectHeader = () => {
  return (
    <Header className="py-0 h-25">
      <div className="d-flex justify-content-between align-items-center ">
        <Avatar size="small" src={Logo} />

        <Menu>
          <Menu.Item key="0">
            <Avatar
              size="small"
              src="https://st.depositphotos.com/2101611/3925/v/600/depositphotos_39258143-stock-illustration-businessman-avatar-profile-picture.jpg"
            />
          </Menu.Item>
        </Menu>
      </div>
    </Header>
  );
};

export default ProjectHeader;
