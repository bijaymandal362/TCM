import { useState } from "react";
import {
  List,
  Skeleton,
  Avatar,
  Tag,
  PageHeader,
  Button,
  Divider,
  Input,
  Menu,
  Col,
  Row,
} from "antd";
import {
  TableOutlined,
  BookOutlined,
  PlayCircleOutlined,
} from "@ant-design/icons";
import { useDebounce } from "use-debounce";
import {
  useGetProjectsListQuery,
  useGetStarredProjectsListQuery,
} from "../../store/services/main/projects";
import moment from "moment";
import CheckPermission from "../../hoc/checkPermission";
import useUpdateEffect from "../../util/custom-hooks/useUpdateEffect";
import { Link } from "react-router-dom";

const ProjectList = () => {
  // States
  const [selectedKey, setSelectedKeys] = useState("0");
  const [search, setSearch] = useState("");
  const [searchStarredProjects, setSearchStarredProjects] = useState("");
  const [page, setPage] = useState(1);
  const [starredProjectsPage, setStarredProjectsPage] = useState(1);
  const [pageSize, setPageSize] = useState<number>(20);
  const [starredProjectsPageSize, setStarredProjectsPageSize] =
    useState<number>(20);
  const [notFound, setNotFound] = useState(false);
  const [notFoundStarredProjects, setNotFoundStarredProjects] = useState(false);

  const [debouncedSearch] = useDebounce(search, 500);
  const [debouncedSearchStarredProjects] = useDebounce(
    searchStarredProjects,
    500
  );

  const handleClick = (e: any) => {
    setSelectedKeys(e.key);
  };

  const {
    data: projectList,
    isLoading,
    isFetching,
    error,
  } = useGetProjectsListQuery(
    {
      PageNumber: page,
      SearchValue: debouncedSearch,
      PageSize: pageSize,
    },
    {
      // refetchOnFocus: true,
      refetchOnReconnect: true,
      refetchOnMountOrArgChange: true,
    }
  );

  const {
    data: starredProjectList,
    isLoading: isStarredProjectListLoading,
    isFetching: isStarredProjectListFetching,
    error: isStarredProjectListError,
  } = useGetStarredProjectsListQuery(
    {
      PageNumber: starredProjectsPage,
      SearchValue: debouncedSearchStarredProjects,
      PageSize: starredProjectsPageSize,
    },
    {
      // refetchOnFocus: true,
      refetchOnReconnect: true,
      refetchOnMountOrArgChange: true,
      // Need to show the total count of starred projects so, commenting below code
      // skip: selectedKey === "0",
    }
  );

  // To show no found UI, as API sends you 404 error when no data found
  useUpdateEffect(() => {
    if (!isFetching) {
      if ((error as any)?.status === 404) {
        if (!notFound) setNotFound(true);
      } else {
        if (notFound) setNotFound(false);
      }
    }
  }, [isFetching]);

  // To show no found UI, as API sends you 404 error when no data found
  useUpdateEffect(() => {
    if (!isStarredProjectListFetching) {
      if ((isStarredProjectListError as any)?.status === 404) {
        if (!notFoundStarredProjects) setNotFoundStarredProjects(true);
      } else {
        if (notFoundStarredProjects) setNotFoundStarredProjects(false);
      }
    }
  }, [isStarredProjectListFetching]);

  return (
    <div>
      <div className="w-75 mx-auto ">
        <PageHeader
          title="Projects"
          className="py-0"
          extra={[
            <CheckPermission key="newproject" slug="project.create">
              <Button key="1" type="primary">
                <a href="/new-project">New Project</a>
              </Button>
            </CheckPermission>,
          ]}
        />
        <Divider className="mt-1 mb-0" />
        <CheckPermission slug="project.read">
          <List
            header={
              <Row className="d-flex justify-content-between mx-1">
                <Col
                  lg={15}
                  md={15}
                  xs={24}
                  className="scrollable__horizontal mb-2"
                >
                  <Menu
                    mode="horizontal"
                    onClick={handleClick}
                    style={{
                      background: "none",
                      borderBottom: "none",
                      fontWeight: "bold",
                    }}
                    selectedKeys={[selectedKey]}
                  >
                    <Menu.Item key="0">
                      Your Projects &nbsp;
                      {!isLoading ? (
                        <Tag>{notFound ? 0 : projectList?.totalRecords}</Tag>
                      ) : (
                        <Tag>0</Tag>
                      )}
                    </Menu.Item>
                    <Menu.Item key="1">
                      Starred Projects &nbsp;
                      {!isStarredProjectListLoading ? (
                        <Tag>
                          {notFoundStarredProjects
                            ? 0
                            : starredProjectList?.totalRecords}
                        </Tag>
                      ) : (
                        <Tag>0</Tag>
                      )}
                    </Menu.Item>
                  </Menu>
                </Col>

                <Col lg={6} md={6} xs={24} className="my-auto mx-3">
                  <Input
                    placeholder="Filter By Name..."
                    value={selectedKey === "0" ? search : searchStarredProjects}
                    onChange={(e) => {
                      if (page !== 1) {
                        if (selectedKey === "0") setPage(() => 1);
                        else setStarredProjectsPage(() => 1);
                      }

                      if (selectedKey === "0") {
                        setSearch(e.target.value);
                      } else {
                        setSearchStarredProjects(e.target.value);
                      }
                    }}
                  />
                </Col>
              </Row>
            }
            loading={isLoading || isStarredProjectListLoading}
            size="large"
            itemLayout="horizontal"
            pagination={{
              onChange: (newPage, newPageSize) => {
                if (selectedKey === "0") {
                  setPage(newPage);
                  setPageSize(newPageSize as number);
                } else {
                  setStarredProjectsPage(newPage);
                  setStarredProjectsPageSize(newPageSize as number);
                }
                window.scrollTo(0, 0);
              },
              pageSize:
                (selectedKey === "0"
                  ? projectList?.pageSize
                  : starredProjectList?.pageSize) || 20,
              total:
                selectedKey === "0"
                  ? projectList?.totalRecords
                  : starredProjectList?.totalRecords,
              showSizeChanger: true,
            }}
            dataSource={
              selectedKey === "0"
                ? notFound
                  ? []
                  : projectList?.data
                : notFoundStarredProjects
                ? []
                : starredProjectList?.data
            }
            renderItem={(item, index: number) => (
              <List.Item
                key={index}
                actions={[
                  <Col
                    lg={24}
                    xs={0}
                    style={{
                      minWidth: 175,
                      textAlign: "right",
                    }}
                  >
                    Updated {moment(item.date).startOf("minute").fromNow()}
                  </Col>,
                ]}
              >
                <Skeleton avatar title={false} loading={false} active>
                  <List.Item.Meta
                    avatar={
                      <Avatar
                        size="large"
                        alt={item.projectName}
                        className="mx-0"
                      >
                        {item.projectName?.charAt(0)?.toUpperCase()}
                      </Avatar>
                    }
                    title={
                      <Row>
                        <Col lg={12} xs={24}>
                          <Link
                            className="project-name"
                            to={`/project/${item.projectSlug?.replace(
                              / /g,
                              "-"
                            )}`}
                          >
                            {item.projectName}
                          </Link>

                          {item.projectRole && (
                            <Tag style={{ marginLeft: 8 }}>
                              {" "}
                              {item.projectRole}
                            </Tag>
                          )}
                        </Col>
                      </Row>
                    }
                    description={
                      <Row>
                        <Col lg={24} xs={0}>
                          {item.projectDescription}
                        </Col>
                        <Col lg={0} xs={24}>
                          <p className="text-white">
                            <BookOutlined /> {item.testCaseCount}{" "}
                            &nbsp;&nbsp;&nbsp;
                            <TableOutlined /> {item.testPlanCount}{" "}
                            &nbsp;&nbsp;&nbsp;
                            <PlayCircleOutlined /> {item.testRunCount}
                          </p>
                        </Col>
                        <Col></Col>
                      </Row>
                    }
                  />
                  <Row>
                    <Col
                      lg={24}
                      xs={0}
                      style={{
                        minWidth: 225,
                        textAlign: "left",
                        marginLeft: 75,
                      }}
                    >
                      <BookOutlined /> {item.testCaseCount} &nbsp;&nbsp;&nbsp;
                      <TableOutlined /> {item.testPlanCount} &nbsp;&nbsp;&nbsp;
                      <PlayCircleOutlined /> {item.testRunCount}
                    </Col>
                  </Row>
                </Skeleton>
              </List.Item>
            )}
          />
        </CheckPermission>
      </div>
    </div>
  );
};

export default ProjectList;
