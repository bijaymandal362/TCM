import React, { lazy, Suspense, useRef, useState } from "react";
import { useHistory, useRouteMatch } from "react-router";
import { useDebounce } from "use-debounce";
import {
  Typography,
  Button,
  Row,
  Col,
  Space,
  Tabs,
  Input,
  Menu,
  Dropdown,
  Spin,
  message,
  Empty,
} from "antd";
import moment from "moment";
import "moment-duration-format";
import {
  ArrowLeftOutlined,
  CaretDownOutlined,
  DeleteOutlined,
  ExportOutlined,
  PlayCircleFilled,
  RetweetOutlined,
  // ShareAltOutlined,
  UserAddOutlined,
  UserDeleteOutlined,
} from "@ant-design/icons";
import TableLayout from "./components/TableLayout";
import TeamStats from "./components/TeamStats";
// import ShareReport from "./Modals/ShareReport";
import { Chart as ChartJS, ArcElement, Tooltip, Legend } from "chart.js";
import { Doughnut } from "react-chartjs-2";
import AssignMember from "./Modals/AssignMember";
import TestRunWizard from "./Modals/TestRunWizard";
import {
  useAssignUserToTestCasesMutation,
  useDeleteMultipleTestCaseOfTestPlanFromTestRunMutation,
  useGetProjectMemberListQuery,
  useGetTestRunDataQuery,
  useGetTestRunTeamStatsQuery,
  useGetTestRunTestPlanByTestRunIdQuery,
  useRetestTestPlanTestCaseMutation,
  useUnAssignUserFromTestCasesMutation,
} from "../../store/services/main/test-run";
import {
  getSelectedTestCasesIds,
  useRouterQuery,
} from "../../util/functions.utils";
import TestCaseStatusDetail from "./Modals/TestCaseStatusDetail";
// import { Status } from "../../interfaces";
import useUpdateEffect from "../../util/custom-hooks/useUpdateEffect";
import { TestPlanAndCaseIdsForDeleteAndRetest } from "../../interfaces";
import TableFilter, { FilterState } from "../../components/TableFilter";
const ExportTestRun = lazy(() => import("./Modals/ExportTestRun"));

ChartJS.register(ArcElement, Tooltip, Legend);

export const doughnutData = (statuses: Record<string, number>) => {
  return {
    labels: ["Pending", "Passed", "Failed", "Blocked"],
    datasets: [
      {
        label: "TestRun Statuses",
        data: [
          statuses.pending,
          statuses.passed,
          statuses.failed,
          statuses.blocked,
        ],
        backgroundColor: ["#6610f2", "#54a035", "#f14e71", "#e8a73e"],
        borderWidth: 0,
      },
    ],
  };
};

const doughnutOptions = {
  plugins: {
    legend: {
      display: false,
    },
  },
  layout: {
    padding: 25,
  },
};

const Dashboard: React.FC = (): JSX.Element => {
  const route = useRouteMatch<{ projectSlug: string; testRunId: string }>();
  const history = useHistory();
  const query = useRouterQuery();

  // States
  const [testCasesIds, setTestCasesIds] = useState<any>({});
  const [testPlanAndCaseIds, setTestPlanAndCaseIds] = useState<
    TestPlanAndCaseIdsForDeleteAndRetest[]
  >([]);
  const testCaseIdForAssign = useRef([]);
  const [isExportModalVisible, setIsExportModalVisible] = useState(false);
  // const [isShareModalVisible, setIsShareModalVisible] = useState(false);
  const [isDropdownVisible, setIsDropdownVisible] = useState({});
  const [isAssignModalVisible, setIsAssignModalVisible] = useState(false);
  const [isWizardModalVisible, setIsWizardModalVisible] = useState(false);
  const [
    isTestCaseStatusDetailModalVisible,
    setIsTestCaseStatusDetailModalVisible,
  ] = useState(() => !!query.get("testCaseId"));
  const [search, setSearch] = useState("");
  const [tabKey, setTabKey] = useState("1");
  const [notFound, setNotFound] = useState(false);
  const [currentFilters, setCurrentFilters] = useState<FilterState[]>([]);
  const [testCaseDelete, setTestCaseDelete] = useState<boolean>();

  const [debouncedSearch] = useDebounce(search, 500);

  const getTestRunTestPlanByTestRunId = useGetTestRunTestPlanByTestRunIdQuery(
    {
      testRunId: +route.params.testRunId,
      searchValue: "",
      pageNumber: 1,
      pageSize: 1000,
    },
    {
      refetchOnMountOrArgChange: true,
      skip: tabKey === "2",
    }
  );

  const {
    data: getTestRunTeamStatsData,
    isLoading: isGetTestRunTeamStatsLoading,
  } = useGetTestRunTeamStatsQuery(+route.params.testRunId, {
    // refetchOnFocus: true,
    refetchOnMountOrArgChange: true,
    skip: tabKey === "1",
  });

  const { data: getTestRunData, isLoading: isGetTestRunDataLoading } =
    useGetTestRunDataQuery(+route.params.testRunId, {
      // refetchOnFocus: true,
    });

  const { data: getProjectMemberListData } = useGetProjectMemberListQuery(
    route.params.projectSlug
  );

  const [assignUserToTestCases, { isLoading: isAssignUserToTestCasesLoading }] =
    useAssignUserToTestCasesMutation();
  const [unAssignUserFromTestCases] = useUnAssignUserFromTestCasesMutation();
  const [deleteMultipleTestCaseOfTestPlanFromTestRun] =
    useDeleteMultipleTestCaseOfTestPlanFromTestRunMutation();
  const [retestTestPlanTestCase] = useRetestTestPlanTestCaseMutation();

  const updateUserFromTestCase = (
    userId: number | undefined,
    testCasesIds: number[],
    operationAfterSuccess = () => {}
  ) => {
    if (userId !== undefined) {
      // Assigning User
      assignUserToTestCases({
        // testRunHistoryId: +route.params.testRunId,
        assigneProjectMemberId: userId,
        testRunTestCaseHistoryId: testCasesIds,
      })
        .unwrap()
        .then(operationAfterSuccess)
        .catch(() => message.error("Unable to assign user.", 3));
    } else {
      // UnAssigning User
      unAssignUserFromTestCases({
        assigneProjectMemberId: null,
        testRunTestCaseHistoryId: testCasesIds,
      })
        .unwrap()
        .then(operationAfterSuccess)
        .catch(() => message.error("Unable to unassign user.", 3));
    }
  };

  function handleMenuClick(e: any) {
    // console.log("click", e);

    if (e.key === "1") {
      retestTestPlanTestCase({
        testRunId: +route.params.testRunId,
        testPlan: testPlanAndCaseIds,
      })
        .unwrap()
        .then(() => {
          message.success("Retest test cases successfully.", 3);
          setTestPlanAndCaseIds([]);
        })
        .catch(() => message.error("Unable to retest test cases.", 3));
    }

    if (e.key === "2") {
      setIsAssignModalVisible(true);
    }

    if (e.key === "3") {
      updateUserFromTestCase(
        undefined,
        getSelectedTestCasesIds(testCasesIds),
        () => setTestCasesIds({})
      );
    }

    if (e.key === "4") {
      deleteMultipleTestCaseOfTestPlanFromTestRun({
        testRunId: +route.params.testRunId,
        testPlan: testPlanAndCaseIds,
      })
        .unwrap()
        .then(() => {
          setTestCaseDelete(true);
          setTestPlanAndCaseIds([]);
        })
        .catch(() => message.error("Unable to delete test cases.", 3));

      // console.log("DELETE MULTIPLE - testPlanAndCaseIds", testPlanAndCaseIds);
    }
  }

  const getStatusCounts = () => {
    const statuses: Record<string, number> = {};

    getTestRunData?.status.forEach((item) => {
      statuses[item.status.toLowerCase()] = item.statusCount;
    });

    return statuses;
  };

  useUpdateEffect(() => {
    if (!getTestRunTestPlanByTestRunId.isFetching) {
      if ((getTestRunTestPlanByTestRunId.error as any)?.status === 404) {
        if (!notFound) setNotFound(true);
      } else {
        if (notFound) setNotFound(false);
      }
    }
  }, [getTestRunTestPlanByTestRunId.isFetching]);

  const menu = (
    <Menu onClick={handleMenuClick}>
      <Menu.Item key="1" icon={<RetweetOutlined />}>
        Retest
      </Menu.Item>
      <Menu.Item key="2" icon={<UserAddOutlined />}>
        Assign
      </Menu.Item>
      <Menu.Item key="3" icon={<UserDeleteOutlined />}>
        Unassign
      </Menu.Item>
      <Menu.Item key="4" icon={<DeleteOutlined />}>
        Delete
      </Menu.Item>
    </Menu>
  );

  const filterData = [
    {
      key: "Assignee",
      values: getProjectMemberListData?.data.map((member) => member.name) || [],
    },
  ];

  return (
    <>
      <Row className="test-run-dashboard">
        <Col span={20}>
          <div className="header">
            {isGetTestRunDataLoading ? (
              <div className="mx-4 mt-3 mb-5">
                <Spin />
              </div>
            ) : (
              <>
                <div>
                  <div>
                    <ArrowLeftOutlined
                      onClick={() =>
                        history.push(
                          `/project/${route.params.projectSlug}/test-runs`
                        )
                      }
                    />
                    <Typography.Title level={3}>
                      {getTestRunData?.name ||
                        "Test Run Name will display here"}
                    </Typography.Title>
                  </div>
                  <Space>
                    <Button
                      type="primary"
                      icon={<PlayCircleFilled />}
                      onClick={() => setIsWizardModalVisible(true)}
                    >
                      Open wizard
                    </Button>
                    {/* <Button
                    icon={<ShareAltOutlined />}
                    onClick={() => setIsShareModalVisible(true)}
                  >
                    Share report
                  </Button> */}
                    <Button
                      icon={<ExportOutlined />}
                      onClick={() => setIsExportModalVisible(true)}
                    >
                      Export
                    </Button>
                  </Space>
                </div>
                <Typography.Paragraph>
                  {getTestRunData?.description ||
                    "Description not provided yet."}
                </Typography.Paragraph>
              </>
            )}
          </div>
          <Tabs
            defaultActiveKey={tabKey}
            onChange={(key: string) => setTabKey(key)}
          >
            <Tabs.TabPane tab="Test Cases" key="1">
              <div className="test-case-header">
                <div className="left-side-header">
                  <Input
                    placeholder="Search..."
                    onChange={(e) => setSearch(e.target.value)}
                  />
                  <TableFilter
                    currentFilters={currentFilters}
                    setCurrentFilters={setCurrentFilters}
                    filterData={filterData}
                    filterInputWidth={185}
                    filterValueWidth={135}
                  />
                </div>
                {Object.values(isDropdownVisible).includes(true) ? (
                  <Dropdown overlay={menu}>
                    <Button>
                      Update selected <CaretDownOutlined />
                    </Button>
                  </Dropdown>
                ) : (
                  <></>
                )}
              </div>
              {getTestRunTestPlanByTestRunId.isLoading ? (
                <div className="spinner-container">
                  <Spin />
                </div>
              ) : !notFound ? (
                getTestRunTestPlanByTestRunId.data?.data.map((item) => (
                  <React.Fragment key={item.testPlanId}>
                    <TableLayout
                      testCaseDelete={testCaseDelete as boolean}
                      setTestCaseDelete={setTestCaseDelete}
                      key={item.testPlanId}
                      statusCounts={{
                        pendingCount: item.pendingCount,
                        passedCount: item.passedCount,
                        failedCount: item.failedCount,
                        blockedCount: item.blockedCount,
                      }}
                      testPlanId={item.testPlanId}
                      testPlanName={item.testPlanName}
                      setIsDropdownVisible={setIsDropdownVisible}
                      setIsAssignModalVisible={setIsAssignModalVisible}
                      setTestCasesIds={setTestCasesIds}
                      setTestPlanAndCaseIds={setTestPlanAndCaseIds}
                      updateUserFromTestCase={updateUserFromTestCase}
                      testCaseIdForAssign={testCaseIdForAssign}
                      setIsTestCaseStatusDetailModalVisible={
                        setIsTestCaseStatusDetailModalVisible
                      }
                      testRunId={+route.params.testRunId}
                      debouncedSearch={debouncedSearch}
                      currentFilters={currentFilters}
                      getProjectMemberListData={getProjectMemberListData}
                    />
                  </React.Fragment>
                ))
              ) : (
                <Empty image={Empty.PRESENTED_IMAGE_SIMPLE} />
              )}
            </Tabs.TabPane>
            <Tabs.TabPane tab="Team Stats" key="2">
              {isGetTestRunTeamStatsLoading ? (
                <div className="spinner-container">
                  <Spin />
                </div>
              ) : (
                getTestRunTeamStatsData?.map((stat) => (
                  <TeamStats
                    key={stat.user.userId}
                    user={stat.user}
                    timeSpent={stat.timeSpent}
                    testRunData={stat.status}
                  />
                ))
              )}
            </Tabs.TabPane>
          </Tabs>
        </Col>

        <Col span={4} className="right-container">
          <Typography.Title level={5}>Test Run Details</Typography.Title>
          {isGetTestRunDataLoading ? (
            <div className="spinner-container">
              <Spin />
            </div>
          ) : (
            <>
              <div className="progress-circle">
                <Doughnut
                  data={doughnutData(getStatusCounts())}
                  options={doughnutOptions}
                />
              </div>
              <div className="detail">
                <Typography.Paragraph>Completion rate</Typography.Paragraph>
                <Typography.Paragraph strong>
                  Completed {getTestRunData?.completionRate || "0%"}
                </Typography.Paragraph>
              </div>
              <div className="detail">
                <Typography.Paragraph>Started by</Typography.Paragraph>
                <Typography.Paragraph strong>
                  {getTestRunData?.startedBy}
                </Typography.Paragraph>
              </div>
              {/* <div className="detail">
            <Typography.Paragraph>Start time</Typography.Paragraph>
            <Typography.Paragraph strong>
              2020-01-10 16:21:04
            </Typography.Paragraph>
          </div>
          <div className="detail">
            <Typography.Paragraph>Estimated</Typography.Paragraph>
            <Typography.Paragraph strong>00:41:51</Typography.Paragraph>
          </div> */}
              <div className="detail">
                <Typography.Paragraph>Time spent</Typography.Paragraph>
                <Typography.Paragraph strong>
                  {getTestRunData?.timeSpent
                    ? moment
                        .utc(
                          moment
                            .duration(getTestRunData?.timeSpent, "seconds")
                            .asMilliseconds()
                        )
                        .format("HH:mm:ss")
                    : "00:00:00"}
                </Typography.Paragraph>
              </div>
            </>
          )}
        </Col>
      </Row>
      {/* <ShareReport
        isShareModalVisible={isShareModalVisible}
        setIsShareModalVisible={setIsShareModalVisible}
      /> */}
      <AssignMember
        isAssignModalVisible={isAssignModalVisible}
        setIsAssignModalVisible={setIsAssignModalVisible}
        testCasesIds={testCasesIds}
        updateUserFromTestCase={updateUserFromTestCase}
        testCaseIdForAssign={testCaseIdForAssign}
        isAssignUserToTestCasesLoading={isAssignUserToTestCasesLoading}
        projectSlug={route.params.projectSlug}
        setTestCasesIds={setTestCasesIds}
      />
      <TestRunWizard
        isWizardModalVisible={isWizardModalVisible}
        setIsWizardModalVisible={setIsWizardModalVisible}
      />
      <TestCaseStatusDetail
        isTestCaseStatusDetailModalVisible={isTestCaseStatusDetailModalVisible}
        setIsTestCaseStatusDetailModalVisible={
          setIsTestCaseStatusDetailModalVisible
        }
      />
      <Suspense fallback={<></>}>
        <ExportTestRun
          isExportModalVisible={isExportModalVisible}
          setIsExportModalVisible={setIsExportModalVisible}
        />
      </Suspense>
    </>
  );
};

export default Dashboard;
