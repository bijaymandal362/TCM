import { DeleteOutlined, RollbackOutlined } from "@ant-design/icons";
import {
  Card,
  Col,
  Row,
  Form,
  Typography,
  Select,
  Button,
  Table,
  Space,
  Divider,
  Tabs,
  Tag,
  message,
  Breadcrumb,
  Spin,
} from "antd";
import React, { useEffect, useState } from "react";
import { getDropdownLists } from "../../store/actions/projectActionCreators";
import {
  AddProject,
  deleteProject,
  getSearchPerson,
  getUser,
  updateProject,
} from "../../store/actions/projectMembersActionCreators";
import moment from "moment";
import CustomModal from "../../components/customModal";
import { useDebounce } from "use-debounce/lib";
import CheckProjectPermission from "../../hoc/checkProjectPermission";
import { useGetProjectsPermissionListQuery } from "../../store/services/main/projects";
import { useGetProjectNameFromProjectSlugQuery } from "../../store/services/main/project-dashboard";

export const ProjectMembers = (props: any) => {
  const [rolesList, setRolesList] = useState<any>([]);
  const [usersList, setUsersList] = useState<any>([]);
  const [userLoading, setUserLoading] = useState(false);
  const [addProjectloading, setAddProjectLoading] = useState(false);
  const [usersListDetails, setUsersListDetails] = useState<any>(null);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [search] = useState("");
  const [visible, setVisible] = useState(false);
  const [options, setOptions] = useState([]);
  const [filter, setFilter] = useState<any>(null);
  const [filterDebounce] = useDebounce(filter, 500);
  const [projectRoleId, setProjectRoleId] = useState<any>(null);
  const [confirmLoading, setConfirmLoading] = useState(false);
  const [isFetchingUserList, setIsFetchingUserList] = useState(false);
  const [form] = Form.useForm();

  const getProjectsPermissionList = useGetProjectsPermissionListQuery(
    props?.match?.params?.projectSlug
  );

  const getProjectNameFromProjectSlug = useGetProjectNameFromProjectSlugQuery(
    props?.match?.params?.projectSlug,
    {
      refetchOnMountOrArgChange: true,
    }
  );

  const columns: any = [
    {
      title: "SN",
      dataIndex: "key",
      key: "key",
      responsive: ["xs", "sm", "md", "xl"],
    },
    {
      title: "Account",
      dataIndex: "personName",
      key: "personName",
      responsive: ["xs", "sm", "md", "xl"],
    },
    {
      title: "Access granted",
      key: "action2",
      render: (value: any) => {
        return (
          <div>
            {moment(value?.inviteDate).fromNow()}

            {value?.insertedPersonName && <span className="m-2">by</span>}
            {value?.insertedPersonName && <p> {value?.insertedPersonName}</p>}
          </div>
        );
      },
      responsive: ["xs", "sm", "md", "xl"],
    },

    {
      title: "Max role",
      key: "action1",
      render: (value: any) => {
        if (value?.projectRole === "Owner") {
          return <Tag>{value?.projectRole}</Tag>;
        } else {
          return (
            <CheckProjectPermission slug="projectrole.projectmember.update">
              <Select
                className="w-50"
                placeholder="Select Role"
                defaultValue={value?.projectRoleListItemId}
                onChange={(role) => {
                  changeRoles({
                    projectMemberId: value.projectMemberId,
                    projectSlug: value.projectSlug,
                    personId: [value.personId],
                    projectRoleListItemId: role,
                  });
                }}
              >
                {rolesList.map((item: any, index: number) => {
                  return (
                    <Select.Option key={index} value={item?.listItemId}>
                      {item?.listItemName}
                    </Select.Option>
                  );
                })}
              </Select>
            </CheckProjectPermission>
          );
        }
      },
      responsive: ["xs", "sm", "md", "xl"],
    },
    {
      title: "Action",
      key: "action",
      render: (value: any) =>
        value?.projectRole !== "Owner" ? (
          <CheckProjectPermission slug="projectrole.projectmember.delete">
            <Space size="middle">
              <>
                <DeleteOutlined
                  onClick={() => {
                    setProjectRoleId(value?.projectMemberId);
                    setVisible(true);
                  }}
                />
              </>
            </Space>
          </CheckProjectPermission>
        ) : (
          <Space size="middle">
            <>
              <RollbackOutlined />
            </>
          </Space>
        ),
      responsive: ["xs", "sm", "md", "xl"],
    },
  ];

  const changeRoles = async (values: any) => {
    setUserLoading(true);
    const [response, error] = await updateProject(values);
    setUserLoading(false);
    if (response) {
      message.success(response.data);
      getProjectsPermissionList.refetch();
      getUsers();
    }
    if (error) {
      message.error("Failed to change roles!");
    }
  };

  const handleSubmit = async (values: any) => {
    setAddProjectLoading(true);

    const projectData = {
      projectMemberId: 0,
      projectSlug: props.match.params.projectSlug,
      personId: values.email.map((item: any) => {
        return item.value;
      }),
      projectRoleListItemId: values.role,
    };

    const [response, error] = await AddProject(projectData);
    setAddProjectLoading(false);
    if (response) {
      form.resetFields();
      message.success(response.data);
      getUsers();
    }
    if (error) {
      message.error(error.response.data, 3);
    }
  };

  const { TabPane } = Tabs;

  async function fetchUserList(value: any) {
    setOptions([]);
    setIsFetchingUserList(true);
    const [response, error] = await getSearchPerson(value);
    if (response) {
      setOptions(
        response.data.data.map((item: any) => {
          return {
            label: item?.name,
            value: item?.personId,
          };
        })
      );
    }

    setIsFetchingUserList(false);
  }

  const getProjectRoles = async () => {
    const [response, error] = await getDropdownLists("ProjectRole");
    if (response) {
      setRolesList(
        response.data.filter((item: any) => item.listItemName !== "Owner")
      );
    }

    if (error) {
      message.error("Failed to display roles");
    }
  };

  const getUsers = async () => {
    setUsersList([]);
    setUserLoading(true);
    const [response, error] = await getUser({
      projectId: props.match.params.projectSlug,
      page,
      search,
      pageSize,
    });
    setUserLoading(false);
    if (response) {
      setUsersList(
        response.data.data.map((item: any, index: any) => {
          return {
            ...item,
            key:
              index +
              response.data.pageSize * (response.data.pageNumber - 1) +
              1,
          };
        })
      );
      setUsersListDetails(response.data);
    }

    if (error) {
      // message.error("Failed to display roles");
    }
  };

  useEffect(() => {
    getProjectRoles();
  }, []);

  useEffect(() => {
    fetchUserList(filterDebounce);
  }, [filterDebounce]);

  useEffect(() => {
    if (page || search || pageSize) {
      getUsers();
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [page, search, pageSize]);

  return (
    <>
      <CustomModal
        visible={visible}
        title="Delete User !"
        modalContent="Are you sure want delete User ?"
        okText="Yes"
        cancelText="No"
        confirmLoading={confirmLoading}
        handleOk={async () => {
          setConfirmLoading(true);
          const [response, error] = await deleteProject(projectRoleId);

          setConfirmLoading(false);
          if (response) {
            setVisible(false);
            message.success(response.data);
            getUsers();
          }

          if (error) {
            message.error("Unable to delete users !");
          }
        }}
        handleCancel={() => {
          setProjectRoleId(null);
          setVisible(false);
        }}
      />
      <Row className="mx-auto">
        <Col xs={23} lg={20}>
          <Breadcrumb>
            <Breadcrumb.Item>
              {getProjectNameFromProjectSlug.isLoading ? (
                <Spin />
              ) : (
                getProjectNameFromProjectSlug.data?.projectName
              )}
            </Breadcrumb.Item>
            <Breadcrumb.Item>Members</Breadcrumb.Item>
          </Breadcrumb>
        </Col>

        <Divider />

        <Col xs={23} lg={20}>
          <Typography.Title level={3}>Project Members</Typography.Title>
        </Col>
        <CheckProjectPermission slug="projectrole.projectmember.create">
          {" "}
          <Col xs={23} lg={20}>
            <Card
              bordered
              style={{
                background: "none",
              }}
            >
              <Form onFinish={handleSubmit} form={form}>
                <Row gutter={24}>
                  <Col xs={24} lg={24} className="gutter-row">
                    <label>Email Address</label>
                    <Form.Item
                      className="mt-2"
                      name="email"
                      rules={[{ required: true, message: "Email is Required" }]}
                    >
                      <Select
                        mode="multiple"
                        labelInValue
                        filterOption={false}
                        onSearch={(value) => {
                          setFilter(value);
                        }}
                        onBlur={() => {
                          fetchUserList("");
                        }}
                        options={options}
                        loading={isFetchingUserList}
                      />
                    </Form.Item>
                  </Col>

                  <Col xs={24} lg={24} className="gutter-row">
                    <label>Choose a role permission</label>
                    <Form.Item
                      name="role"
                      className="mt-2"
                      rules={[{ required: true, message: "Role is Required" }]}
                    >
                      <Select placeholder="Select Role">
                        {rolesList.map((item: any, index: number) => {
                          return (
                            <Select.Option key={index} value={item?.listItemId}>
                              {item?.listItemName}
                            </Select.Option>
                          );
                        })}
                      </Select>
                    </Form.Item>
                  </Col>

                  <Col lg={18} className="mt-2">
                    <Button htmlType="submit" loading={addProjectloading}>
                      Invite
                    </Button>
                  </Col>
                </Row>
              </Form>
            </Card>
          </Col>
        </CheckProjectPermission>

        <CheckProjectPermission slug="projectrole.projectmember.read">
          <Col xs={23} lg={20} className="mt-4">
            <Tabs defaultActiveKey="1">
              <TabPane
                tab={
                  <span>
                    Members
                    <Tag className="m-2">
                      {usersListDetails?.totalRecords
                        ? usersListDetails?.totalRecords
                        : 0}
                    </Tag>
                  </span>
                }
                key="1"
              >
                <Divider />
                <Col lg={24} sm={0} xs={0} md={24}>
                  <Table
                    columns={columns}
                    className="table table-responsive"
                    dataSource={usersList}
                    loading={userLoading}
                    pagination={{
                      onChange: (newPage, newPageSize) => {
                        setPage(newPage);
                        setPageSize(newPageSize!);
                      },
                      pageSize: usersListDetails?.pageSize,
                      total: usersListDetails?.totalRecords,
                      showSizeChanger: true,
                    }}
                  />
                </Col>

                <Col lg={0} xs={24} sm={24} md={0}>
                  <Card title="Members List" className="w-100">
                    {usersList.map((item: any, index: number) => {
                      return (
                        <React.Fragment key={index}>
                          <Card
                            type="inner"
                            title="Account"
                            extra={
                              <p style={{ marginTop: 8 }}>{item.personName}</p>
                            }
                          >
                            <Row>
                              <Col
                                xs={24}
                                sm={24}
                                lg={0}
                                md={0}
                                className="d-flex w-100 justify-content-between"
                              >
                                <Typography>Access Granted</Typography>

                                <div>
                                  {moment(
                                    item?.inviteDate,
                                    "YYYYMMDD"
                                  ).fromNow()}

                                  {item?.insertedPersonName && (
                                    <span className="m-2">by</span>
                                  )}
                                  {item?.insertedPersonName && (
                                    <p> {item?.insertedPersonName}</p>
                                  )}
                                </div>
                              </Col>
                              <Divider />

                              <Col
                                xs={24}
                                sm={24}
                                lg={0}
                                md={0}
                                className="d-flex w-100 justify-content-between"
                              >
                                <Typography>Max Role</Typography>

                                {item?.projectRole === "Owner" ? (
                                  <Tag>{item?.projectRole}</Tag>
                                ) : (
                                  <Select
                                    className="w-50"
                                    placeholder="Select Role"
                                    defaultValue={item?.projectRoleListItemId}
                                    onChange={(role) => {
                                      changeRoles({
                                        projectMemberId: item.projectMemberId,
                                        projectSlug: item.projectSlug,
                                        personId: [item.personId],
                                        projectRoleListItemId: role,
                                      });
                                    }}
                                  >
                                    {rolesList.map(
                                      (item: any, index: number) => {
                                        return (
                                          <Select.Option
                                            key={index}
                                            value={item?.listItemId}
                                          >
                                            {item?.listItemName}
                                          </Select.Option>
                                        );
                                      }
                                    )}
                                  </Select>
                                )}
                              </Col>

                              <Divider />
                              {/* acceess granted */}

                              <Col
                                xs={24}
                                sm={24}
                                lg={0}
                                md={0}
                                className="d-flex w-100 justify-content-between p-0"
                              >
                                <Typography>Access Granted</Typography>
                                <Typography>
                                  {moment(
                                    item.inviteDate,
                                    "YYYYMMDD"
                                  ).fromNow()}

                                  {item.insertedPersonName && (
                                    <span className="m-2">by</span>
                                  )}
                                  {item.insertedPersonName && (
                                    <p> {item.insertedPersonName}</p>
                                  )}
                                </Typography>
                              </Col>
                              <Divider />

                              {/* actions */}

                              <Col
                                xs={24}
                                sm={24}
                                lg={0}
                                md={0}
                                className="d-flex w-100 justify-content-between p-0"
                              >
                                <Typography>Actions</Typography>

                                {item?.projectRole !== "Owner" ? (
                                  <CheckProjectPermission slug="projectmember.delete">
                                    <Space size="middle">
                                      <>
                                        <DeleteOutlined
                                          onClick={() => {
                                            setProjectRoleId(
                                              item?.projectMemberId
                                            );
                                            setVisible(true);
                                          }}
                                        />
                                      </>
                                    </Space>
                                  </CheckProjectPermission>
                                ) : (
                                  <Space size="middle">
                                    <>
                                      <RollbackOutlined />
                                    </>
                                  </Space>
                                )}
                              </Col>
                            </Row>
                          </Card>
                          <br />
                          <br />
                        </React.Fragment>
                      );
                    })}
                  </Card>
                </Col>
              </TabPane>
            </Tabs>
          </Col>
        </CheckProjectPermission>
      </Row>
    </>
  );
};

export default ProjectMembers;
