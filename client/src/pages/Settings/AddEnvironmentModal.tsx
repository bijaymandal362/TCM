import { Input, message, Typography } from "antd";
import React, { useEffect, useState } from "react";
import { Form, Button, Checkbox } from "antd";
import { AddEnvironment } from "../../store/actions/environmentActionCreators";

const AddEnvironmentModal = (props: any) => {
  const [environmentName, setEnvironmentName] = useState("");
  const [environmentUrl, setEnvironmentUrl] = useState("");

  const [isAddEnvironmentLoading, setIsAddEnvironmentLoading] = useState(false);

  const handleSubmit = async (values: any) => {
    const environmentData = {
      environmentId: 0,
      environmentName: environmentName,
      url: environmentUrl,
      projectSlug: props.projectSlug,
    };
    setIsAddEnvironmentLoading(true);
    const [response, error] = await AddEnvironment(environmentData);
    setIsAddEnvironmentLoading(false);
    if (response) {
      // form.resetFields();
      props.onOk();
      message.success(response.data);
      props.getEnvironments();
    }
    if (error) {
      message.error(error.response.data, 3);
    }
  };

  const onFinishFailed = (errorInfo: any) => {
    console.log("Failed:", errorInfo);
  };

  const [form] = Form.useForm();

  // useEffect(() => getEnvironments(), []);

  return (
    <div>
      <Form
        name="basic"
        initialValues={{ remember: true }}
        onFinish={handleSubmit}
        onFinishFailed={onFinishFailed}
        autoComplete="off"
        form={form}
      >
        <Form.Item
          name="environment"
          rules={[
            {
              required: true,
              message: "Please input your environment name!",
              whitespace: true,
            },
            {
              max: 255,
              message: "Environment Name should be less than 255 characters.",
            },
          ]}
        >
          <Typography.Text strong>
            &#42; Environment Name
            <Input
              value={environmentName}
              onChange={(e) => setEnvironmentName(e.target.value)}
            />
          </Typography.Text>
        </Form.Item>

        <Form.Item
          name="url"
          rules={[
            {
              max: 255,
              message: "URL should be less than 255 characters.",
            },
          ]}
        >
          <Typography.Text strong>
            URL
            <Input
              value={environmentUrl}
              onChange={(e) => setEnvironmentUrl(e.target.value)}
            />
          </Typography.Text>
        </Form.Item>

        <Form.Item className="mt-2 mb-0">
          <Button
            type="primary"
            htmlType="submit"
            loading={isAddEnvironmentLoading}
          >
            Add
          </Button>
        </Form.Item>
      </Form>
    </div>
  );
};

export default AddEnvironmentModal;
