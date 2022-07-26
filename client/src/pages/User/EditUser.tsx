import {
  Breadcrumb,
  Button,
  Col,
  Divider,
  Form,
  Input,
  message,
  Row,
  Select,
} from "antd";

import { useHistory, useRouteMatch } from "react-router";
import {
  updateUser,
  getUserDetail,
} from "../../store/actions/userActionCreators";
import { useEffect, useState } from "react";
import { getDropdownLists } from "../../store/actions/projectActionCreators";

const { Option } = Select;

export const EditUser = () => {
  const [form] = Form.useForm();
  const [role, setRole] = useState(0);
  const [loading, setLoading] = useState(false);
  const history = useHistory();
  const route = useRouteMatch<{ userId: string }>();
  const [marketList, setMarketList] = useState([]);
  const initialFormValues = {
    name: "",
    username: "",
    email: "",
    role: "",
    market: "",
  };
  const [roleList, setRoleList] = useState([]);
  const userId = route.params?.userId || 0;

  const handleFormValueChange = (changedValues: any) => {
    const fieldName = Object.keys(changedValues)[0];

    if (fieldName === "role") {
      const value = changedValues[fieldName];
      if (value !== "28") {
        form.setFieldsValue({ market: "" });
      }
      setRole(value);
    }
  };

  const handleFormSubmit = async (values: any) => {
    setLoading(true);
    // console.log(values);
    const data: { [key: string]: any } = {
      userId: +userId,
      userRoleListItemId: values.role,
    };

    if (values.market) {
      data.userMarketListItemId = values.market;
    }

    const [response, error] = await updateUser(data);
    setLoading(false);
    if (response) {
      history.push("/admin/users");
      message.success("User updated sucessfully.");
    }
    if (error) {
      message.error("Unable to update user.");
    }
  };

  const getMarketLists = async () => {
    const [response, error] = await getDropdownLists("ProjectMarket");

    if (response) {
      setMarketList(response.data);
    }
    if (error) {
      message.error("Failed to list market");
    }
  };

  const getRoleList = async () => {
    const [response, error] = await getDropdownLists("UserRole");

    if (response) {
      const data = response.data;
      data.map((item: any) => ({
        value: item.listItemId,
        label: item.listItemName,
      }));
      setRoleList(data);
    }
    if (error) {
      setRoleList([]);
      message.error("Failed to list role");
    }
  };

  const loadUser = async () => {
    const [response, error] = await getUserDetail(userId);

    if (response) {
      const data = response?.data;
      const { name, username, email, roleId: role, marketId: market } = data;
      form.setFieldsValue({ name, username, email, role, market });
      if (market) {
        setRole(role);
      }
    }
    if (error) {
      if (error.status === 404) {
        // history.push("/admin/users")
      }
      message.error("Failed to load user");
    }
  };

  useEffect(() => {
    getRoleList();
    getMarketLists();
    loadUser();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  return (
    <Row className="p-4">
      <Col span="24" className="p-2">
        <Breadcrumb>
          <Breadcrumb.Item>User settings</Breadcrumb.Item>
          <Breadcrumb.Item>
            <a href={`/admin/users/${1}`}>Edit Profile</a>
          </Breadcrumb.Item>
        </Breadcrumb>
        <Divider />

        <Form
          layout="vertical"
          onFinish={handleFormSubmit}
          form={form}
          initialValues={initialFormValues}
          onValuesChange={handleFormValueChange}
        >
          <Row gutter={24}>
            <Col lg={16} xs={24} className="gutter-row">
              <Form.Item
                label="Full Name"
                name="name"
                rules={[{ required: true, message: "Full Name is Required" }]}
              >
                <Input size="large" placeholder="Ram Sharma" disabled />
              </Form.Item>
            </Col>
          </Row>
          <Row gutter={24}>
            <Col lg={16} xs={24} className="gutter-row">
              <Form.Item
                label="Username"
                name="username"
                rules={[{ required: true, message: "Username is Required" }]}
              >
                <Input size="large" placeholder="ram234" disabled />
              </Form.Item>
            </Col>
          </Row>
          <Row gutter={24}>
            <Col lg={16} xs={24} className="gutter-row">
              <Form.Item
                label="Email"
                name="email"
                rules={[{ required: true, message: "Email is Required" }]}
              >
                <Input
                  size="large"
                  type="email"
                  placeholder="hello@gmail.com"
                  disabled
                />
              </Form.Item>
            </Col>
          </Row>
          <Row gutter={24}>
            <Col lg={16} xs={24} className="gutter-row">
              <Form.Item
                label="Role"
                name="role"
                rules={[{ required: true, message: "Role is Required" }]}
              >
                <Select size="large" placeholder="Select Role">
                  {roleList.map((item: any, index: number) => {
                    return (
                      <Option key={index} value={item?.listItemId}>
                        {item?.listItemName}
                      </Option>
                    );
                  })}
                </Select>
              </Form.Item>
            </Col>
          </Row>
          <Row gutter={24}>
            <Col lg={16} xs={24} className="gutter-row">
              {role === 28 && (
                <Form.Item
                  label="Market"
                  name="market"
                  rules={[{ required: true, message: "Market is Required" }]}
                >
                  <Select size="large" placeholder="Select Market">
                    {marketList.map((item: any, index: number) => {
                      return (
                        <Option key={index} value={item?.listItemId}>
                          {item?.listItemName}
                        </Option>
                      );
                    })}
                  </Select>
                </Form.Item>
              )}
            </Col>
          </Row>

          <Row className="mt-4">
            <Col lg={8} xs={12}>
              <Form.Item>
                <Button
                  size="middle"
                  loading={loading}
                  type="primary"
                  htmlType="submit"
                >
                  Update User
                </Button>
              </Form.Item>
            </Col>
            <Col lg={8} xs={12} className="d-flex justify-content-end">
              <Form.Item>
                <Button
                  type="default"
                  size="middle"
                  onClick={() => {
                    history.push("/admin/users");
                  }}
                >
                  Go Back
                </Button>
              </Form.Item>
            </Col>
          </Row>
        </Form>
      </Col>
    </Row>
  );
};

export default EditUser;
