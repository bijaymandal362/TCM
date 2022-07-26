import {
  EyeInvisibleOutlined,
  EyeTwoTone,
  LockOutlined,
  UserOutlined,
} from "@ant-design/icons";
import {
  Button,
  Form,
  Input,
  Checkbox,
  Row,
  Col,
  Typography,
  message,
} from "antd";
import { useState, useRef, useEffect } from "react";
import { Redirect, useHistory } from "react-router-dom";
import Logo from "../../assets/images/logo.png";
import {
  customerLogin,
  // expireToken,
} from "../../store/actions/authActionCreators";
import { getAppearance, isUserLoggedIn } from "../../util/auth.util";
import { addAuthToLocalStorage, setTheme } from "../../util/localStorage.util";
import { ThemeList } from "../../enum/enum";
// import { useJwt } from "react-jwt";
// import moment from "moment";

const Login = () => {
  const [form] = Form.useForm();
  const passwordInput = useRef(null);

  const [loading, setLoading] = useState<boolean>(false);
  // const history = useHistory();

  useEffect(() => {
    if (passwordInput && passwordInput.current) {
      // @ts-ignore: Object is possibly 'null'.
      passwordInput.current.focus();
    }
  }, [passwordInput]);

  const onFinish = async (values: any) => {
    setLoading(true);
    const [response, err] = await customerLogin(
      {
        userName: values.username,
        password: values.password,
      },
      (bool) => {
        setLoading(bool);
      }
    );
    if (response) {
      response.data["isAdmin"] = false;

      setTheme(
        getAppearance(response.data?.token) === "Dark Mode"
          ? ThemeList.DARK
          : ThemeList.Light
      );
      addAuthToLocalStorage(response.data);

      // const users: any = getPermissions();
      // expireToken(history, moment().unix() - users.exp);
      // console.log(moment().unix());
      // console.log(users.exp);

      setLoading(false);
      window.location.pathname = "/"; // to prevent some edge cases, needed to use it
    }
    if (err) {
      if (err.status === 401) {
        message.error("Invalid username or password.", 3);
      } else {
        message.error("Something went wrong.", 3);
      }
      setLoading(false);
    }
  };

  if (isUserLoggedIn()) {
    return <Redirect to="/" />;
  }

  return (
    <div className="authentication__body">
      <Col span={20} className="authentication__wrapper mt-5">
        <Row className="desc__wrapper" gutter={{ xs: 8, sm: 16, md: 24 }}>
          <Col
            span={12}
            xs={24}
            sm={24}
            md={24}
            lg={14}
            className="authentication__description"
          >
            <img
              src={Logo}
              alt="TestMink"
              className="authentication__main__logo"
            />

            <Typography.Title level={3} className="w__100 text__center mt-3">
              A Complete Testing Platform
            </Typography.Title>
          </Col>

          <Col
            span={12}
            xs={24}
            sm={24}
            md={24}
            lg={10}
            className="authentication__content login mt-2"
          >
            <div>
              <div className="form__wrapper">
                <Form
                  layout="vertical"
                  form={form}
                  onFinish={onFinish}
                  requiredMark={false}
                >
                  <Form.Item
                    name="username"
                    label="Username or Email"
                    rules={[
                      {
                        required: true,
                        message: "Username or Email is required",
                      },
                    ]}
                  >
                    <Input ref={passwordInput} prefix={<UserOutlined />} />
                  </Form.Item>
                  <Form.Item
                    name="password"
                    label="Password"
                    rules={[
                      {
                        required: true,
                        message: "Password is required",
                      },
                    ]}
                  >
                    <Input.Password
                      prefix={<LockOutlined />}
                      iconRender={(visible) =>
                        visible ? <EyeTwoTone /> : <EyeInvisibleOutlined />
                      }
                    />
                  </Form.Item>
                  <Form.Item name="remember" valuePropName="checked">
                    <Checkbox className="checkbox-container">
                      Remember me
                    </Checkbox>
                  </Form.Item>
                  <Form.Item>
                    <Button
                      loading={loading}
                      className="login__btn w__100"
                      htmlType="submit"
                    >
                      Sign In
                    </Button>
                  </Form.Item>
                </Form>
              </div>
            </div>
          </Col>
        </Row>
      </Col>
    </div>
  );
};

export default Login;
