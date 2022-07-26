import { Layout } from "antd";
import { useEffect, useState } from "react";
import { Redirect, Switch } from "react-router";
import { mainRoutesWithoutSidenav } from "../routes/main.routes";
import { isUserLoggedIn } from "../util/auth.util";
import Header from "./header";

const { Content } = Layout;

function MainLayoutWithoutSidebar() {
  const [colorTheme, setColorTheme] = useState<boolean>(false);

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

  return (
    <Layout>
      <Header changeTheme={handleThemeChange} />
      <Content className={`page__wrapper nosidebar`}>
        <Switch>{mainRoutesWithoutSidenav}</Switch>
      </Content>
    </Layout>
  );
}

export default MainLayoutWithoutSidebar;
