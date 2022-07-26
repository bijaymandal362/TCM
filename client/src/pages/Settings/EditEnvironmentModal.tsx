import React, { useState } from "react";
import { Form, Button, Input, Typography, message } from "antd";
import { updateEnvironment } from "../../store/actions/environmentActionCreators";

const EditEnvironmentModal = ({ data, onOk, getEnvironments }: any) => {
  const [environmentName, setEnvironmentName] = useState(data.environmentName);
  const [environmentUrl, setEnvironmentUrl] = useState(data.url);

  const [isEditLoading, setIsEditLoading] = useState(false);

  const handleSubmit = async () => {
    const updatedEnvironmentData = {
      environmentId: data.environmentId,
      environmentName: environmentName,
      url: environmentUrl,
      projectSlug: data.projectSlug,
    };
    setIsEditLoading(true);
    const [response, error] = await updateEnvironment(updatedEnvironmentData);
    setIsEditLoading(false);
    if (response) {
      onOk();
      message.success(response.data);
      getEnvironments();
    }
    if (error) {
      message.error("Unable to Update environment !");
    }
  };

  const onFinishFailed = () => {};

  return (
    <>
      <Form
        name="basic"
        // initialValues={{ remember: true }}
        onFinish={handleSubmit}
        onFinishFailed={onFinishFailed}
        autoComplete="off"
      >
        <Form.Item
          // label="Environment"
          initialValue={environmentName}
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
          <Button type="primary" htmlType="submit" loading={isEditLoading}>
            Update
          </Button>
        </Form.Item>
      </Form>
    </>
  );
};
export default EditEnvironmentModal;
