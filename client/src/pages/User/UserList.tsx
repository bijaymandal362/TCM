import { useState } from "react";
import {
  Tag,
  PageHeader,
  Divider,
  Menu,
  Col,
  Row,
  Input,
  Select,
  Table,
  Avatar,
  Space,
} from "antd";
import { useDebounce } from "use-debounce";
import {
  EditOutlined,
  SearchOutlined,
  SettingOutlined,
} from "@ant-design/icons";
import { UserListItem } from "../../interfaces";
import { Link } from "react-router-dom";
import moment from "moment";
import CheckPermission from "../../hoc/checkPermission";
import useBreakpoint from "antd/lib/grid/hooks/useBreakpoint";
import {
  useGetUsersRoleListQuery,
  useGetUsersListByRoleQuery,
} from "../../store/services/main/users";
import useUpdateEffect from "../../util/custom-hooks/useUpdateEffect";

const UserList = () => {
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [sortBy, setSortBy] = useState("name");
  const [currentTab, setCurrentTab] = useState<string>("25");
  const [search, setSearch] = useState("");
  const [filter] = useDebounce(search, 1000);
  const breakpoint = useBreakpoint();

  // States
  const [notFound, setNotFound] = useState(false);

  const { data: usersRoleList } = useGetUsersRoleListQuery(undefined, {
    // refetchOnFocus: true,
    refetchOnMountOrArgChange: true,
  });

  const {
    data: usersListByRole,
    isLoading: isUsersListByRoleLoading,
    isFetching,
    error,
  } = useGetUsersListByRoleQuery(
    {
      PageNumber: page,
      SearchValue: filter,
      PageSize: pageSize,
      roleId: currentTab,
    },
    {
      // refetchOnFocus: true,
      refetchOnMountOrArgChange: true,
    }
  );

  useUpdateEffect(() => {
    if (!isFetching) {
      if ((error as any)?.status === 404) {
        if (!notFound) setNotFound(true);
      } else {
        if (notFound) setNotFound(false);
      }
    }
  }, [isFetching]);

  const renderUser = (text: string, record: UserListItem) => {
    const email = record?.email || "email not set";
    return (
      <div className="d-flex justify-content-left align-items-center">
        <Avatar size="large" alt={text}>
          {text?.charAt(0)}
        </Avatar>
        <div>
          <span>
            <b>{text}</b>
          </span>
          <br />
          <span className="text-wrap">{email}</span>
        </div>
      </div>
    );
  };
  const renderCreatedOn = (text: string) => (
    <span>{text ? moment(text).local().format("YY/MM/DD hh:mm") : ""}</span>
  );
  const renderActions = (text: string, record: UserListItem) => (
    <Space size="middle">
      <CheckPermission slug="user.update">
        <Link to={`/admin/users/${record.userId}`}>
          <EditOutlined />
        </Link>
      </CheckPermission>

      <Link to="">
        <SettingOutlined />
      </Link>
    </Space>
  );

  const tableColumns: any = [
    {
      title: "Responsive View",
      key: "name",
      dataIndex: "name",
      render: (text: string, record: UserListItem) => {
        return (
          <>
            <table>
              <tbody>
                <tr>
                  <th>User</th>

                  <td colSpan={2}>{renderUser(text, record)}</td>
                </tr>
                <tr>
                  <th>Projects </th>{" "}
                  <td className="ps-3">{record.projectCount}</td>
                </tr>
                <tr>
                  <th>Created On</th>
                  <td className="ps-3">{renderCreatedOn(record.createdOn)} </td>
                </tr>
                <tr>
                  <th>Actions</th>
                  <td className="ps-3">
                    {renderActions(record.name, record)}{" "}
                  </td>
                </tr>
              </tbody>
            </table>
          </>
        );
      },
      className: "d-table-cell d-sm-none",
    },
    {
      title: "Name",
      key: "name",
      dataIndex: "name",
      render: renderUser,
      responsive: ["sm"],
    },
    {
      title: "Projects",
      key: "projectCount",
      dataIndex: "projectCount",
      responsive: ["sm"],
    },
    {
      title: "Created On",
      key: "createdOn",
      dataIndex: "createdOn",
      render: renderCreatedOn,
      responsive: ["sm"],
    },
    {
      title: "Action",
      key: "name",
      render: renderActions,
      responsive: ["sm"],
    },
  ];

  const sortByOptions = [{ label: "Name", value: "name" }];

  const handleTabChange = (e: any) => {
    setCurrentTab(e.key);
  };

  return (
    <div className="user-list-page mx-auto">
      <PageHeader
        title="Users"
        className="py-0 px-3"
        extra={
          [
            // <Button
            //   key="1"
            //   type="primary"
            //   onClick={() => {
            //     history.push("/admin/users/");
            //   }}
            // >
            //   New User
            // </Button>,
          ]
        }
      />
      <div>
        <Divider className="my-2" />
      </div>
      <Row className="d-flex justify-content-between">
        <Col span={24} className="scrollable__horizontal mb-3">
          <Menu
            mode="horizontal"
            onClick={handleTabChange}
            style={{
              background: "none",
              borderBottom: "none",
              fontWeight: "bold",
            }}
            overflowedIndicator=""
            selectedKeys={currentTab ? [currentTab] : []}
          >
            {usersRoleList?.map((item) => {
              return (
                <Menu.Item key={item.roleId}>
                  {item.name} &nbsp;
                  <Tag>{item.userCount}</Tag>
                </Menu.Item>
              );
            })}
          </Menu>
        </Col>
      </Row>

      <Row className="d-flex justify-content-between">
        <Col lg={16} md={16} sm={24} xs={24} className="pe-0 pe-md-2">
          <Input
            size="large"
            prefix={<SearchOutlined style={{ color: "inherit" }} />}
            placeholder="Filter By Name..."
            value={search}
            onChange={(e) => {
              setSearch(e.target.value);
            }}
          />
        </Col>

        <Col
          span={8}
          xs={24}
          md={8}
          className="d-flex align-items-center justify-content-center mt-2 mt-md-auto  "
        >
          <span className="me-2 text-no-wrap">Sort By:</span>
          <Select
            size="large"
            value={sortBy}
            options={sortByOptions}
            onSelect={(val) => setSortBy(val)}
            className="w-100"
          />
        </Col>
      </Row>
      <Row className="mt-4">
        <Col span="24">
          <Table
            bordered
            className="scrollable__horizontal"
            columns={tableColumns}
            showHeader={breakpoint.sm}
            dataSource={notFound ? [] : usersListByRole?.data}
            loading={isUsersListByRoleLoading}
            pagination={{
              onChange: (page, pagesize) => {
                setPage(page);
                setPageSize(pagesize as number);
              },
              pageSize: pageSize,
              total: usersListByRole?.totalRecords,
              showSizeChanger: true,
              defaultPageSize: 10,
              pageSizeOptions: ["10", "20"],
            }}
          />
        </Col>
      </Row>
    </div>
  );
};

export default UserList;
