import React, { useRef, useState } from "react";
import useInfiniteScroll from "react-infinite-scroll-hook";
import {
  Button,
  Col,
  Divider,
  Empty,
  Input,
  message,
  Modal,
  Row,
  Space,
  Spin,
  Typography,
} from "antd";
import moment from "moment";
import "moment-duration-format";
import TestRunStatus from "../components/TestRunStatus";
import {
  CheckOutlined,
  CloseOutlined,
  DownloadOutlined,
  EditOutlined,
  EyeOutlined,
  StepForwardOutlined,
  StopOutlined,
} from "@ant-design/icons";
import { useDebounce } from "use-debounce";
import {
  useDownloadTestRunFileMutation,
  useGetProjectMemberListQuery,
  useGetTestRunDataQuery,
  useGetTestRunTestCaseDetailQuery,
  useGetTestRunTestPlanByTestRunIdQuery,
} from "../../../store/services/main/test-run";
import { useRouteMatch } from "react-router";
import AddRunResult from "./AddRunResult";
import useUpdateEffect from "../../../util/custom-hooks/useUpdateEffect";
import { Pagination, TestRunTestCaseHistory } from "../../../interfaces";
import { getTestRunTestCaseHistory } from "../../../store/actions/testRunActionCreators";
import WizardCollapsePanel from "../components/WizardCollapsePanel";
import TableFilter, { FilterState } from "../../../components/TableFilter";

interface TestRunWizardProps {
  isWizardModalVisible: boolean;
  setIsWizardModalVisible: (value: boolean) => void;
}

const fourActionButtons = [
  {
    status: "Passed",
    icon: <CheckOutlined />,
  },
  {
    status: "Failed",
    icon: <CloseOutlined />,
  },
  {
    status: "Blocked",
    icon: <StopOutlined />,
  },
  {
    status: "Pending",
    icon: <StepForwardOutlined />,
  },
];

export const downloadUploadedFile = (data: any, fileName: string) => {
  const blob = new Blob([data]);
  const url = window.URL.createObjectURL(blob);
  const link = document.createElement("a");
  link.href = url;
  link.setAttribute("download", fileName);
  document.body.appendChild(link);
  link.click();

  // Release the object URL
  URL.revokeObjectURL(url);
};

const TestRunWizard: React.FC<TestRunWizardProps> = (props): JSX.Element => {
  const route = useRouteMatch<{ projectSlug: string; testRunId: string }>();
  const clickedDownloadDocumentId = useRef<number | null>(null);
  const page = useRef(1);

  // States
  const [currentFilters, setCurrentFilters] = useState<FilterState[]>([]);
  const [infiniteHistoryLoading, setInfiniteHistoryLoading] =
    useState<boolean>(false);
  const [infiniteHistoryPagination, setInfiniteHistoryPagination] = useState<
    Pagination<any> | undefined
  >(undefined);
  const [infiniteHistoryError, setInfiniteHistoryError] = useState(undefined);
  const [infiniteHistoryData, setInfiniteHistoryData] = useState<
    TestRunTestCaseHistory[] | undefined
  >(undefined);
  const [testPlansCount, setTestPlansCount] = useState(0);
  const [isAddRunResultModalVisible, setIsAddRunResultModalVisible] =
    useState(false);
  const [search, setSearch] = useState("");
  const [notFound, setNotFound] = useState(false);
  const [testPlanID, setTestPlanID] = useState<number | null>(null);
  const [selectedTestCaseId, setSelectedTestCaseId] = useState<number | null>(
    null
  );
  const [statusChange, setStatusChange] = useState<boolean>(false);
  const triggerStatusRef = useRef<{
    status: string;
    step: number | null;
    comment: string | null;
  }>({
    status: "",
    step: null,
    comment: null,
  });
  const wizardTestCaseIdRef = useRef<number | undefined>(undefined);

  const [debouncedSearch] = useDebounce(search, 500);

  const { data: getTestRunData } = useGetTestRunDataQuery(
    +route.params.testRunId
  );

  const getTestRunTestPlanByTestRunId = useGetTestRunTestPlanByTestRunIdQuery(
    {
      testRunId: +route.params.testRunId,
      searchValue: debouncedSearch,
      pageNumber: 1,
      pageSize: 1000,
    },
    {
      refetchOnMountOrArgChange: true,
    }
  );

  const {
    data: getTestRunTestCaseDetailData,
    isLoading: isGetTestRunTestCaseDetailLoading,
  } = useGetTestRunTestCaseDetailQuery(
    {
      testRunID: +route.params.testRunId,
      testCaseID: wizardTestCaseIdRef.current as number,
      testPlanID: testPlanID as number,
    },
    {
      // refetchOnFocus: true,
      skip: selectedTestCaseId === null,
      refetchOnMountOrArgChange: true,
    }
  );

  const { data: getProjectMemberListData } = useGetProjectMemberListQuery(
    route.params.projectSlug
  );

  const fetchTestRunHistory = async (isRefetch = false) => {
    if (page.current < 2) {
      setInfiniteHistoryLoading(true);
    }

    const data = await getTestRunTestCaseHistory({
      pageNumber: page.current,
      pageSize: 15,
      searchValue: "",
      testCaseId: wizardTestCaseIdRef.current!,
    });

    if (data[0]) {
      const { data: historyData, ...others } = data[0].data;
      setInfiniteHistoryPagination({ ...others, data: [] });

      if (!isRefetch) {
        // Append data to existing data
        setInfiniteHistoryData((prevState) => {
          if (prevState?.length) {
            return [...prevState, ...historyData];
          } else {
            return historyData;
          }
        });
      } else {
        setInfiniteHistoryData(historyData);
      }
    }
    if (data[1]) {
      setInfiniteHistoryError(data[1]?.data);
    }

    if (page.current < 2) {
      setInfiniteHistoryLoading(false);
    }
  };

  const hasNextPage =
    infiniteHistoryPagination?.pageNumber! <
    Math.ceil(
      infiniteHistoryPagination?.totalRecords! /
        infiniteHistoryPagination?.pageSize!
    );

  const [sentryRef] = useInfiniteScroll({
    loading: infiniteHistoryLoading,
    hasNextPage,
    onLoadMore: () => {
      page.current += 1;
      fetchTestRunHistory();
    },
    // When there is an error, we stop infinite loading.
    disabled: !!infiniteHistoryError,
    rootMargin: "0px 0px 30px 0px",
  });

  const [downloadTestRunFile, downloadTestRunFileData] =
    useDownloadTestRunFileMutation();

  const handleAddRunResultOpen = (
    status: string,
    step: number | null,
    comment: string | null
  ) => {
    setIsAddRunResultModalVisible(true);
    triggerStatusRef.current = {
      status: status.toLowerCase(),
      step: step || step === 0 ? step : null,
      comment: comment,
    };
  };

  const handleCancel = () => {
    props.setIsWizardModalVisible(false);
    setSelectedTestCaseId(null);
  };

  const getTotalTestCases = () => {
    let totalCount = 0;

    getTestRunTestPlanByTestRunId.data?.data.forEach((item) => {
      totalCount +=
        item.blockedCount +
        item.failedCount +
        item.passedCount +
        item.pendingCount;
    });

    return totalCount;
  };

  useUpdateEffect(() => {
    if (!getTestRunTestPlanByTestRunId.isFetching) {
      const count = getTestRunTestPlanByTestRunId.data?.data.length || 0;

      if (count) setNotFound(false);
      else setNotFound(true);

      setTestPlansCount(count);
    }
  }, [getTestRunTestPlanByTestRunId.isFetching]);

  useUpdateEffect(() => {
    if (selectedTestCaseId) {
      // reset
      page.current = 1;
      setInfiniteHistoryData([]);

      fetchTestRunHistory();
    }
  }, [selectedTestCaseId]);

  const filterData = [
    {
      key: "Assignee",
      values: getProjectMemberListData?.data.map((member) => member.name) || [],
    },
  ];

  return (
    <>
      <Modal
        visible={props.isWizardModalVisible}
        onCancel={handleCancel}
        className="wizard-modal"
        footer={null}
        centered
        width="100vw"
        bodyStyle={{ height: "95vh", marginTop: "20px", paddingRight: 0 }}
      >
        <Row>
          <Col span={5} className="left-container">
            <Typography.Title level={4}>
              {getTestRunData?.name}
            </Typography.Title>
            <div className="small-details">
              <Typography.Text>{testPlansCount} plans </Typography.Text>
              &middot;
              <Typography.Text> {getTotalTestCases()} cases</Typography.Text>
            </div>
            <Input
              placeholder="Search for cases"
              onChange={(e) => setSearch(e.target.value)}
            />
            <div className="filter-container">
              <TableFilter
                currentFilters={currentFilters}
                setCurrentFilters={setCurrentFilters}
                filterData={filterData}
                filterInputWidth={135}
                filterValueWidth={105}
              />
            </div>
            {!notFound ? (
              <div className="overflow-container">
                {getTestRunTestPlanByTestRunId.data?.data.map((item) => (
                  <WizardCollapsePanel
                    key={item.testPlanId}
                    data={item}
                    debouncedSearch={debouncedSearch}
                    testRunId={+route.params.testRunId}
                    selectedTestCaseId={selectedTestCaseId}
                    setSelectedTestCaseId={setSelectedTestCaseId}
                    wizardTestCaseIdRef={wizardTestCaseIdRef}
                    setTestPlanID={setTestPlanID}
                    currentFilters={currentFilters}
                    getProjectMemberListData={getProjectMemberListData}
                    setStatusChange={setStatusChange}
                    statusChange={statusChange}
                  />
                ))}
              </div>
            ) : (
              <Empty image={Empty.PRESENTED_IMAGE_SIMPLE} />
            )}
          </Col>

          <Col span={15} className="middle-container">
            {selectedTestCaseId !== null ? (
              !isGetTestRunTestCaseDetailLoading ? (
                <>
                  <div>
                    <div style={{ maxWidth: "75%" }}>
                      <Typography.Paragraph className="testplan-name">
                        {getTestRunData?.name}
                      </Typography.Paragraph>
                      <Typography.Title level={4} className="testcase-name">
                        {getTestRunTestCaseDetailData?.testCaseName}
                      </Typography.Title>
                    </div>
                    <Space>
                      <Button icon={<EyeOutlined />}>View</Button>
                      {/* <Button icon={<EditOutlined />}>Edit</Button> */}
                    </Space>
                  </div>

                  <div
                    style={{
                      display: "flex",
                      justifyContent: "space-between",
                      alignItems: "center",
                    }}
                  >
                    {/* {!getTestRunTestCaseDetailData?.status ? ( */}
                    <div>
                      {fourActionButtons.map((item) => (
                        <TestRunStatus
                          key={item.status}
                          type="button"
                          status={item.status.toLowerCase() as any}
                          icon={item.icon}
                          size="small"
                          onClick={() =>
                            handleAddRunResultOpen(
                              item.status,
                              null,
                              getTestRunTestCaseDetailData?.comment as
                                | string
                                | null
                            )
                          }
                        >
                          {item.status}
                        </TestRunStatus>
                      ))}
                    </div>
                    {/* ) : (
                    <TestRunStatus
                      key={getTestRunTestCaseDetailData?.status}
                      type="tag"
                      status={
                        getTestRunTestCaseDetailData?.status.toLowerCase() as any
                      }
                      size="small"
                      >
                      {getTestRunTestCaseDetailData?.status}
                      </TestRunStatus>
                    )} */}

                    {getTestRunTestCaseDetailData?.historyDocumentId ? (
                      <Button
                        type="text"
                        icon={<DownloadOutlined />}
                        loading={
                          downloadTestRunFileData.isLoading &&
                          getTestRunTestCaseDetailData.historyDocumentId ===
                            clickedDownloadDocumentId.current
                        }
                        onClick={() => {
                          clickedDownloadDocumentId.current =
                            getTestRunTestCaseDetailData.historyDocumentId;

                          downloadTestRunFile(
                            `${getTestRunTestCaseDetailData.historyDocumentId}`
                          )
                            .unwrap()
                            .then((data) => {
                              downloadUploadedFile(
                                data,
                                getTestRunTestCaseDetailData.fileName!
                              );
                            })
                            .catch((err) => {
                              if (err?.status === 500) {
                                message.error("Internal Server Error !", 3);
                              } else {
                                message.error(err.data, 3);
                              }
                            });
                        }}
                      >
                        File
                      </Button>
                    ) : null}
                  </div>

                  <Divider className="my-2" />

                  <div className="test-case-details">
                    <div>
                      <Typography.Paragraph>Environment</Typography.Paragraph>
                      <Typography.Paragraph>
                        {getTestRunTestCaseDetailData?.environment}
                      </Typography.Paragraph>
                    </div>
                    {/* <div>
                    <Typography.Paragraph>
                      TestPlan Description
                    </Typography.Paragraph>
                    <Typography.Paragraph>
                    This case checks new project creation process
                    </Typography.Paragraph>
                  </div> */}
                    <div>
                      <Typography.Paragraph>
                        TestCase Description/Scenario
                      </Typography.Paragraph>
                      <Typography.Paragraph>
                        {getTestRunTestCaseDetailData?.testCaseScenario}
                      </Typography.Paragraph>
                    </div>
                    <div>
                      <Typography.Paragraph>
                        Pre-conditions
                      </Typography.Paragraph>
                      <Typography.Paragraph>
                        {getTestRunTestCaseDetailData?.preConditions}
                      </Typography.Paragraph>
                    </div>
                    <div>
                      <Typography.Paragraph>
                        Steps to reproduce
                      </Typography.Paragraph>
                      <div className="all-steps">
                        {getTestRunTestCaseDetailData?.stepsToReproduce?.map(
                          (item, index) => (
                            <div key={item.testCaseStepResultId}>
                              <Typography.Text>{index + 1}</Typography.Text>
                              <div>
                                <Typography.Paragraph>
                                  {item.stepDescription}
                                </Typography.Paragraph>
                                <Typography.Paragraph>
                                  Expected result:
                                </Typography.Paragraph>
                                <Typography.Paragraph>
                                  {item.expectedResult}
                                </Typography.Paragraph>
                                <div className="action-area">
                                  <Space>
                                    {fourActionButtons.map((btnItem) => (
                                      <React.Fragment key={btnItem.status}>
                                        {btnItem.status.toLowerCase() ===
                                        item.status.toLowerCase() ? (
                                          <TestRunStatus
                                            type="button"
                                            className="button-active"
                                            status={
                                              btnItem.status.toLowerCase() as any
                                            }
                                            icon={btnItem.icon}
                                            size="small"
                                          >
                                            {btnItem.status}
                                          </TestRunStatus>
                                        ) : (
                                          <Button
                                            size="small"
                                            icon={btnItem.icon}
                                            className={`${btnItem.status.toLowerCase()} button`}
                                            onClick={() =>
                                              handleAddRunResultOpen(
                                                btnItem.status,
                                                item.testRunTestCaseStepHistoryId,
                                                item.comment
                                              )
                                            }
                                          >
                                            {btnItem.status}
                                          </Button>
                                        )}
                                      </React.Fragment>
                                    ))}
                                  </Space>

                                  {item.stepHistoryDocumentId ? (
                                    <Button
                                      type="text"
                                      icon={<DownloadOutlined />}
                                      loading={
                                        downloadTestRunFileData.isLoading &&
                                        item.stepHistoryDocumentId ===
                                          clickedDownloadDocumentId.current
                                      }
                                      onClick={() => {
                                        clickedDownloadDocumentId.current =
                                          item.stepHistoryDocumentId;
                                        downloadTestRunFile(
                                          `${item.stepHistoryDocumentId}`
                                        )
                                          .unwrap()
                                          .then((data) => {
                                            downloadUploadedFile(
                                              data,
                                              item.fileName!
                                            );
                                          })
                                          .catch((err) => {
                                            if (err?.status === 500) {
                                              message.error(
                                                "Internal Server Error !",
                                                3
                                              );
                                            } else {
                                              message.error(err.data, 3);
                                            }
                                          });
                                      }}
                                    >
                                      File
                                    </Button>
                                  ) : null}
                                </div>
                              </div>
                            </div>
                          )
                        )}
                      </div>
                    </div>
                  </div>
                </>
              ) : (
                <div className="spinner-container">
                  <Spin />
                </div>
              )
            ) : (
              <div className="empty-container">
                <Empty
                  image={Empty.PRESENTED_IMAGE_SIMPLE}
                  description="Please select a Test Case"
                />
              </div>
            )}
          </Col>

          <Col span={4} className="right-container">
            <Typography.Title level={5}>Case Run History</Typography.Title>
            {selectedTestCaseId !== null ? (
              !infiniteHistoryLoading ? (
                <>
                  <div>
                    {infiniteHistoryData?.map((item) => (
                      <div key={item.testRunTestCaseHistoryId}>
                        <div className="header">
                          <Typography.Paragraph>
                            {item.testRunName}
                          </Typography.Paragraph>
                          <TestRunStatus
                            type="text"
                            status={item.status.toLowerCase() as any}
                          >
                            {item.status}
                          </TestRunStatus>
                        </div>
                        <Typography.Paragraph>
                          {item.timeSpent
                            ? moment
                                .utc(
                                  moment
                                    .duration(item.timeSpent, "seconds")
                                    .asMilliseconds()
                                )
                                .format("HH:mm:ss")
                            : "00:00:00"}
                        </Typography.Paragraph>
                      </div>
                    ))}
                  </div>
                  {infiniteHistoryLoading || hasNextPage ? (
                    <div
                      ref={sentryRef}
                      className="infinite-scroll-spinner-container"
                    >
                      <Spin />
                    </div>
                  ) : null}
                </>
              ) : (
                <div className="spinner-container">
                  <Spin />
                </div>
              )
            ) : (
              <Empty image={Empty.PRESENTED_IMAGE_SIMPLE} />
            )}
          </Col>
        </Row>
      </Modal>
      {isAddRunResultModalVisible && (
        <AddRunResult
          isAddRunResultModalVisible={isAddRunResultModalVisible}
          setIsAddRunResultModalVisible={setIsAddRunResultModalVisible}
          triggerStatus={triggerStatusRef}
          getTestRunTestCaseDetailData={getTestRunTestCaseDetailData}
          fetchTestRunHistory={fetchTestRunHistory}
          setStatusChange={setStatusChange}
          statusChange={statusChange}
        />
      )}
    </>
  );
};

export default TestRunWizard;
