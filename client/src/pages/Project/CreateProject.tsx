import {
  Breadcrumb,
  Button,
  Col,
  DatePicker,
  Divider,
  Form,
  Input,
  message,
  Row,
  Select,
} from "antd";
import { FolderOpenOutlined } from "@ant-design/icons";
import TextArea from "antd/lib/input/TextArea";
import { useHistory } from "react-router";
import { getDropdownLists } from "../../store/actions/projectActionCreators";
import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { useCreateNewProjectMutation } from "../../store/services/main/projects";

const { Option } = Select;
const CreateProject = () => {
  const [form] = Form.useForm();
  const [marketList, setMarketList] = useState([]);
  const history = useHistory();

  const [createNewProject, { isLoading: isCreateNewProjectLoading }] =
    useCreateNewProjectMutation();

  const handleFormSubmit = async (values: any) => {
    const data = {
      projectName: values.project_name.trim(),
      startDate: values.start_date._d.toISOString(),
      projectMarketListItemId: values.market,
      projectDescription: values.project_description,
    };

    createNewProject(data)
      .unwrap()
      .then(() => {
        message.success("Project created successfully.");
        history.push("/");
      })
      .catch((err) => {
        if (err.data.toLowerCase().startsWith("projectname")) {
          form.setFields([
            {
              name: "project_name",
              errors: ["Project name already exists."],
            },
          ]);
        } else {
          message.error(err.data, 3);
        }
      });
  };

  const getMarketLists = async () => {
    const [response, error] = await getDropdownLists("ProjectMarket");

    if (response) {
      setMarketList(response.data);
    }
    if (error) {
      message.error("Failed to list the markets.");
    }
  };

  useEffect(() => {
    getMarketLists();
  }, []);

  return (
    <div>
      <Row className="mt-4 p-4">
        <Col lg={6} xs={24} className="mx-auto">
          <FolderOpenOutlined
            style={{
              fontSize: 150,
            }}
          />
          <h4>Create Blank Project</h4>
          <p>
            Create a blank project to house your files, plan your work, and
            collaborate on code, among other things.
          </p>
        </Col>
        <Col lg={14} xs={24} className="p-2 mt-2">
          <Row>
            <Col lg={18} xs={24}>
              <Breadcrumb>
                <Breadcrumb.Item>New Project</Breadcrumb.Item>
              </Breadcrumb>
              <Divider className="mb-0 mt-3" />
            </Col>
          </Row>

          <Form
            className="mt-4"
            layout="vertical"
            onFinish={handleFormSubmit}
            form={form}
          >
            <Row gutter={24} className="mb-2">
              <Col lg={16} xs={24} className="gutter-row">
                <Form.Item
                  label="Project Name"
                  name="project_name"
                  rules={[
                    { required: true, message: "Project Name is required." },
                    {
                      whitespace: true,
                      message: "Project Name cannot be empty.",
                    },
                    {
                      pattern: /^[a-zA-Z0-9_-\s]+$/,
                      message:
                        "Project Name cannot contain special characters.",
                    },
                    {
                      max: 49,
                      message:
                        "Project Name should be less than 50 characters.",
                    },
                  ]}
                >
                  <Input placeholder="My awesome project" />
                </Form.Item>
              </Col>
            </Row>
            <Row gutter={24} className="mb-2">
              <Col lg={8} xs={24} className="gutter-row">
                <Form.Item
                  label="Start Date"
                  name="start_date"
                  rules={[
                    { required: true, message: "Start Date is required." },
                  ]}
                >
                  <DatePicker className="w-100" onChange={() => {}} />
                </Form.Item>
              </Col>

              <Col lg={8} xs={24} className="gutter-row">
                <Form.Item
                  label="Market"
                  name="market"
                  rules={[{ required: true, message: "Market is required." }]}
                >
                  <Select placeholder="Select Market">
                    {marketList.map((item: any, index: number) => {
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

            <Row>
              <Col lg={16} xs={24}>
                <Form.Item
                  label="Project Description (Optional)"
                  name="project_description"
                >
                  <TextArea showCount maxLength={500} />
                </Form.Item>
              </Col>
            </Row>

            <Row className="mt-4">
              <Col lg={8} xs={12}>
                <Form.Item>
                  <Button
                    loading={isCreateNewProjectLoading}
                    type="primary"
                    htmlType="submit"
                  >
                    Create Project
                  </Button>
                </Form.Item>
              </Col>
              <Col lg={8} xs={12} className="d-flex justify-content-end">
                <Form.Item>
                  <Button
                    type="default"
                    onClick={() => {
                      history.push("/");
                    }}
                  >
                    Cancel
                  </Button>
                </Form.Item>
              </Col>
            </Row>
          </Form>
        </Col>
      </Row>
    </div>
  );
};

export default CreateProject;
