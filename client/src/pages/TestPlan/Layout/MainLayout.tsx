import React, { useState } from "react";
import { Typography, Tabs, Divider, Table, Input } from "antd";
import {
  useGetTestPlanTestCasesQuery,
  useGetTestRunTestPlanListQuery,
} from "../../../store/services/main/test-plan";
import { useDebounce } from "use-debounce/lib";
import useUpdateEffect from "../../../util/custom-hooks/useUpdateEffect";
import moment from "moment";
import CustomStatusBar from "../../../components/CustomStatusBar";
import { useHistory, useParams } from "react-router";
import { Link } from "react-router-dom";

const columnsCases = [
  {
    title: "Test Case Name",
    dataIndex: "testCaseName",
    key: "testCaseName",
    render: (name: string) => <div style={{ maxWidth: 250 }}>{name}</div>,
  },
  {
    title: "Scenario",
    dataIndex: "scenario",
    key: "scenario",
    width: 350,
  },
  {
    title: "Expected Result",
    dataIndex: "expectedResult",
    key: "expectedResult",
    width: 450,
  },
  {
    title: "Author",
    dataIndex: "author",
    key: "author",
  },
];

interface MainLayoutProps {
  toggleEditing: () => void;
  selectedTestPlanData: any;
}

const MainLayout: React.FC<MainLayoutProps> = (props) => {
  // States
  const [pageCases, setPageCases] = useState(1);
  const [pageSizeCases, setPageSizeCases] = useState<number>(10);
  const [searchCases, setSearchCases] = useState("");
  const [notFoundCases, setNotFoundCases] = useState(false);
  const [pageRuns, setPageRuns] = useState(1);
  const [pageSizeRuns, setPageSizeRuns] = useState<number>(10);
  const [searchRuns, setSearchRuns] = useState("");
  const [notFoundRuns, setNotFoundRuns] = useState(false);

  const [debouncedSearchCases] = useDebounce(searchCases, 500);
  const [debouncedSearchRuns] = useDebounce(searchRuns, 500);
  const history = useHistory();

  let { projectSlug }: { projectSlug: string } = useParams();

  const {
    data: getTestPlanTestCasesData,
    isLoading: isGetTestPlanTestCasesLoading,
    isFetching: isGetTestPlanTestCasesFetching,
    isFetching,
    error,
  } = useGetTestPlanTestCasesQuery(
    {
      pageNumber: pageCases,
      pageSize: pageSizeCases,
      searchValue: debouncedSearchCases,
      testPlanId: props.selectedTestPlanData.key,
    },
    {
      refetchOnFocus: true,
      refetchOnMountOrArgChange: true,
    }
  );

  const getTestRunTestPlanList = useGetTestRunTestPlanListQuery(
    {
      pageNumber: pageRuns,
      pageSize: pageSizeRuns,
      searchValue: debouncedSearchRuns,
      testPlanId: props.selectedTestPlanData.key,
    },
    {
      refetchOnMountOrArgChange: true,
    }
  );

  const retrieveTestCases = () => {
    return getTestPlanTestCasesData?.data.map((testCase) => ({
      key: testCase.projectModuleId,
      testCaseName: testCase.testCaseName,
      scenario: testCase.scenario,
      expectedResult: testCase.expectedResult,
      author: testCase.author,
    }));
  };

  const retrieveTestRuns = () => {
    return getTestRunTestPlanList.data?.data.map((testRun) => ({
      key: Math.floor(Math.random() * 1000000000),
      testRunName: testRun.testRunName,
      timeSpent: moment
        .utc(moment.duration(testRun.timeSpent, "seconds").asMilliseconds())
        .format("HH:mm:ss"),
      passedCount: testRun.passedCount,
      failedCount: testRun.failedCount,
      pendingCount: testRun.pendingCount,
      blockedCount: testRun.blockedCount,
      testRunId: testRun.testRunId,
    }));
  };

  useUpdateEffect(() => {
    if (!isFetching) {
      if ((error as any)?.status === 404) {
        if (!notFoundCases) setNotFoundCases(true);
      } else {
        if (notFoundCases) setNotFoundCases(false);
      }
    }
  }, [isFetching]);

  useUpdateEffect(() => {
    if (!getTestRunTestPlanList.isFetching) {
      if ((getTestRunTestPlanList.error as any)?.status === 404) {
        if (!notFoundRuns) setNotFoundRuns(true);
      } else {
        if (notFoundRuns) setNotFoundRuns(false);
      }
    }
  }, [getTestRunTestPlanList.isFetching]);

  const columnsRuns = [
    {
      title: "Test Run Name",
      dataIndex: "testRunName",
      key: "testRunName",
      render: (_: any, record: any) => (
        <Link
          to={`/project/${projectSlug}/test-runs/dashboard/${record.testRunId}`}
          style={{ color: "white" }}
        >
          {record.testRunName}
        </Link>
      ),
    },
    {
      title: "Time Spent",
      dataIndex: "timeSpent",
      key: "timeSpent",
    },
    {
      title: "Status",
      dataIndex: "status",
      key: "status",
      render: (text: any, record: any) => {
        let totalPassed = 0,
          totalFailed = 0,
          totalPending = 0,
          totalBlocked = 0;

        if (record) {
          totalPassed = record.passedCount;
          totalFailed = record.failedCount;
          totalBlocked = record.blockedCount;
          totalPending = record.pendingCount;
        }

        const total = totalPassed + totalFailed + totalPending + totalBlocked;

        return (
          <CustomStatusBar
            counts={[totalPending, totalPassed, totalFailed, totalBlocked]}
            total={total}
          />
        );
      },
    },
  ];

  return (
    <div className="main-layout">
      <div className="header">
        <Typography.Title level={4}>
          {props.selectedTestPlanData.title || "No Title Provided"}
        </Typography.Title>
        <Divider className="mt-0 mb-3" />
        <Typography.Paragraph>
          {props.selectedTestPlanData.description ||
            "This Test Plan has no description."}
        </Typography.Paragraph>
      </div>

      <Tabs type="card">
        {/* Tab 1 Layout */}
        <Tabs.TabPane tab="Test Cases" key="1">
          <Input.Search
            placeholder="Search Test Case"
            className="mb-3"
            loading={isGetTestPlanTestCasesFetching}
            value={searchCases}
            onChange={(e) => {
              setSearchCases(e.target.value);
            }}
          />
          <Table
            columns={columnsCases}
            dataSource={notFoundCases ? [] : retrieveTestCases()}
            loading={isGetTestPlanTestCasesLoading}
            pagination={
              !getTestPlanTestCasesData?.totalPages
                ? false
                : {
                    onChange: (newPage, newPageSize) => {
                      setPageCases(newPage);
                      setPageSizeCases(newPageSize as number);
                    },
                    pageSize: getTestPlanTestCasesData?.pageSize,
                    total: getTestPlanTestCasesData?.totalRecords,
                  }
            }
          />
        </Tabs.TabPane>

        {/* Tab 2 Layout */}
        <Tabs.TabPane tab="Test Runs" key="2">
          <Input.Search
            placeholder="Search Test Runs"
            className="mb-3"
            loading={getTestRunTestPlanList.isFetching}
            value={searchRuns}
            onChange={(e) => {
              setSearchRuns(e.target.value);
            }}
          />
          <Table
            columns={columnsRuns}
            dataSource={notFoundRuns ? [] : retrieveTestRuns()}
            loading={getTestRunTestPlanList.isLoading}
            pagination={
              !getTestRunTestPlanList.data?.totalPages
                ? false
                : {
                    onChange: (newPage, newPageSize) => {
                      setPageRuns(newPage);
                      setPageSizeRuns(newPageSize as number);
                    },
                    pageSize: getTestRunTestPlanList.data?.pageSize,
                    total: getTestRunTestPlanList.data?.totalRecords,
                  }
            }
          />
        </Tabs.TabPane>
      </Tabs>
    </div>
  );
};

export default MainLayout;
