import React, { useState } from "react";
import { useHistory, useRouteMatch } from "react-router";
import { useDebounce } from "use-debounce";
import {
  Button,
  Divider,
  Input,
  message,
  Popconfirm,
  Progress,
  Space,
  Table,
  Tooltip,
} from "antd";
import moment from "moment";
import "moment-duration-format";
import { DeleteOutlined, EditOutlined } from "@ant-design/icons";
import { Link } from "react-router-dom";
import {
  useDeleteTestRunMutation,
  useGetTestRunListsQuery,
} from "../../store/services/main/test-run";
import { collapseSidebar } from "../../store/features/projectSlice";
import { useAppDispatch } from "../../store/reduxHooks";
import { Status } from "../../interfaces";
import useUpdateEffect from "../../util/custom-hooks/useUpdateEffect";
import CustomStatusBar from "../../components/CustomStatusBar";

const TestRun: React.FC = (): JSX.Element => {
  const route = useRouteMatch<{ projectSlug: string }>();
  const history = useHistory();
  const dispatch = useAppDispatch();

  // States
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState<number>(25);
  const [search, setSearch] = useState("");
  const [notFound, setNotFound] = useState(false);

  const [debouncedSearch] = useDebounce(search, 500);

  const {
    data: getTestRunListData,
    isLoading,
    isFetching,
    error,
  } = useGetTestRunListsQuery(
    {
      projectSlug: route.params.projectSlug,
      PageNumber: page,
      SearchValue: debouncedSearch,
      PageSize: pageSize,
    },
    {
      // refetchOnFocus: true,
      refetchOnMountOrArgChange: true,
    }
  );

  const [deleteTestRun] = useDeleteTestRunMutation();

  useUpdateEffect(() => {
    if (!isFetching) {
      if ((error as any)?.status === 404) {
        if (!notFound) setNotFound(true);
      } else {
        if (notFound) setNotFound(false);
      }
    }
  }, [isFetching]);

  const columns = [
    {
      title: "Title",
      dataIndex: "title",
      key: "title",
      render: (text: string, record: any) => (
        <Link
          to={`/project/${route.params.projectSlug}/test-runs/dashboard/${record.testRunId}`}
          className="test-run-link"
          onClick={() => dispatch(collapseSidebar())}
        >
          {text.length > 30 ? `${text.substring(0, 30)}...` : text}
        </Link>
      ),
    },
    {
      title: "Environment",
      dataIndex: "environment",
      key: "environment",
    },
    {
      title: "Time",
      dataIndex: "time",
      key: "time",
      render: (text: null | string) => (
        <span>
          {text
            ? moment
                .utc(moment.duration(text, "seconds").asMilliseconds())
                .format("HH:mm:ss")
            : "00:00:00"}
        </span>
      ),
    },
    {
      title: "Status",
      key: "status",
      dataIndex: "status",
      render: (text: any, record: any) => {
        let totalPassed = 0,
          totalFailed = 0,
          totalPending = 0,
          totalBlocked = 0;

        record.status.forEach((item: Status) => {
          if (item.status === "Passed") totalPassed = item.statusCount;
          else if (item.status === "Failed") totalFailed = item.statusCount;
          else if (item.status === "Pending") totalPending = item.statusCount;
          else if (item.status === "Blocked") totalBlocked = item.statusCount;
        });

        const total = totalPassed + totalFailed + totalPending + totalBlocked;

        return (
          <CustomStatusBar
            counts={[totalPending, totalPassed, totalFailed, totalBlocked]}
            total={total}
          />
        );
      },
    },
    {
      title: "Actions",
      key: "actions",
      render: (text: any, record: any) => (
        <Space size="middle">
          <Tooltip title="Edit">
            <EditOutlined
              className="table-action"
              onClick={() =>
                history.push(
                  `/project/${route.params.projectSlug}/test-runs/edit/${record.testRunId}`
                )
              }
            />
          </Tooltip>
          <Popconfirm
            title="Are you sure to delete?"
            okText="Yes"
            cancelText="No"
            onConfirm={() =>
              deleteTestRun(record.testRunId)
                .unwrap()
                .then(() =>
                  message.success("Test Run deleted successfully.", 3)
                )
                .catch(() => message.error("Unable to delete Test Run.", 3))
            }
          >
            <DeleteOutlined className="table-action" />
          </Popconfirm>
        </Space>
      ),
    },
  ];

  return (
    <div className="test-run-home">
      <Button type="primary">
        <a href={`/project/${route.params.projectSlug}/test-runs/create`}>
          Start New Test Run
        </a>
      </Button>
      <Input
        placeholder="Search for test runs"
        onChange={(e) => setSearch(e.target.value)}
      />
      <Divider />
      <Table
        rowKey={(record) => record.testRunId}
        columns={columns}
        dataSource={!notFound ? (getTestRunListData as any)?.data : []}
        loading={isLoading}
        pagination={{
          onChange: (page, pagesize) => {
            setPage(page);
            setPageSize(pagesize as number);
          },
          pageSize: pageSize,
          total: getTestRunListData?.totalRecords,
        }}
      />
    </div>
  );
};

export default TestRun;
