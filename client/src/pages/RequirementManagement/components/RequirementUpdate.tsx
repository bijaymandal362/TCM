import {
  Button,
  Select,
  Col,
  Divider,
  Row,
  message,
  Spin,
  Alert,
  Tag,
  Tabs,
  Typography,
  Empty,
} from "antd";
import { CloseCircleOutlined } from "@ant-design/icons";
import { TestTypes } from "../../../enum/enum";
import {
  addDevelopers,
  removeDevelopers,
  updateDevelopers,
} from "../../../store/actions/requirementManagementAction";
import { useState, useRef } from "react";
import CheckProjectPermission from "../../../hoc/checkProjectPermission";
import { Pagination, TestRunTestCaseHistory } from "../../../interfaces";
import useInfiniteScroll from "react-infinite-scroll-hook";
import { getTestRunTestCaseHistory } from "../../../store/actions/testRunActionCreators";
import useUpdateEffect from "../../../util/custom-hooks/useUpdateEffect";
import moment from "moment";
import TestRunStatus from "../../TestRun/components/TestRunStatus";

interface RequirementUpdateInterface {
  data: any;
  requirementDetailLoading: boolean;
  selectedNode: any;
  openEditForm: (selectedNode: any) => void;
  openDeleteForm: (selectedNode: any) => void;
  closeRequirementDetail: () => void;
  usersList: Array<{
    key: number;
    label: string;
    value: number;
  }>;
  handleNodeSelection: (selectedNode: any) => void;
  getFunctionInfo: () => void;
}

const RequirementUpdate = ({
  data,
  selectedNode,
  openEditForm,
  openDeleteForm,
  closeRequirementDetail,
  requirementDetailLoading,
  handleNodeSelection,
  usersList,
  getFunctionInfo,
}: RequirementUpdateInterface) => {
  const page = useRef(1);
  const [editDevelopers, setEditDevelopers] = useState<boolean>(false);
  const [infiniteHistoryData, setInfiniteHistoryData] = useState<
    TestRunTestCaseHistory[] | undefined
  >(undefined);
  const [infiniteHistoryLoading, setInfiniteHistoryLoading] =
    useState<boolean>(false);
  const [infiniteHistoryPagination, setInfiniteHistoryPagination] = useState<
    Pagination<any> | undefined
  >(undefined);
  const [infiniteHistoryError, setInfiniteHistoryError] = useState(undefined);
  // console.log(selectedNode, "node");

  const { TabPane } = Tabs;

  function onChangeTabs() {
    // console.log(selectedNode.key);
  }

  const fetchTestCaseHistory = async () => {
    if (page.current < 2) {
      setInfiniteHistoryLoading(true);
    }

    const data = await getTestRunTestCaseHistory({
      pageNumber: page.current,
      pageSize: 15,
      searchValue: "",
      testCaseId: selectedNode.key,
    });
    // console.log(data,'run')

    if (data[0]) {
      // Append data to existing data
      const { data: historyData, ...others } = data[0].data;
      setInfiniteHistoryPagination({ ...others, data: [] });

      setInfiniteHistoryData((prevState) => {
        if (prevState?.length) {
          return [...prevState, ...historyData];
        } else {
          return historyData;
        }
      });
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
      fetchTestCaseHistory();
    },
    // When there is an error, we stop infinite loading.
    disabled: !!infiniteHistoryError,
    rootMargin: "0px 0px 30px 0px",
  });

  useUpdateEffect(() => {
    if (selectedNode?.key) {
      fetchTestCaseHistory();
    }
    page.current = 1;
    setInfiniteHistoryData([]);
  }, [selectedNode?.key]);

  return (
    <>
      <Col span={24}>
        <div className="close__btn__container">
          <div className="close__btn" onClick={() => closeRequirementDetail()}>
            <CloseCircleOutlined />
          </div>
        </div>
        <Typography.Paragraph className="main-title">
          {selectedNode?.title ? selectedNode?.title : "Title Not set"}
        </Typography.Paragraph>
        <p className="sub-title">
          {selectedNode?.description ? selectedNode?.description : "Not Set"}
        </p>

        <Divider
          style={{
            marginTop: 12,
          }}
        />
        <Row className="mt-4">
          {selectedNode?.type === TestTypes.FUNCTION && (
            <Col span={24} className="mt-2">
              <div>
                <div className="d-flex justify-content-between align-items-center">
                  <p className="title">Developers</p>
                  <CheckProjectPermission slug="projectrole.projectmoduledeveloper.update">
                    <Button
                      className="title"
                      onClick={() => {
                        setEditDevelopers(!editDevelopers);
                      }}
                    >
                      {editDevelopers ? "X" : "Edit"}
                    </Button>
                  </CheckProjectPermission>
                </div>
                {editDevelopers ? (
                  <Select
                    className="w-75 mt-2"
                    mode="multiple"
                    labelInValue
                    filterOption={false}
                    value={selectedNode?.defaultUsersList}
                    onSelect={async (value: any) => {
                      const [response, error] = await addDevelopers({
                        projectModuleDeveloperId: 0,
                        projectModuleId: selectedNode?.key,
                        projectMemberId: [value.value],
                        isDisabled: true,
                      });

                      setEditDevelopers(true);

                      if (response) {
                        getFunctionInfo();
                        message.success("Developers added successfully !");

                        handleNodeSelection({
                          ...selectedNode,
                          defaultUsersList: [],
                        });
                      }

                      if (error) {
                        message.error("Failed to add Developers");
                      }
                    }}
                    onDeselect={async (value: any) => {
                      const [response, error] = await removeDevelopers(
                        value.value
                      );
                      setEditDevelopers(true);
                      if (response) {
                        const defaultUsersList =
                          selectedNode?.defaultUsersList.filter(
                            (item: any) => item?.value !== value?.value
                          );
                        handleNodeSelection({
                          ...selectedNode,
                          defaultUsersList,
                        });
                        message.success("Developers removed successfully !");
                      }

                      if (error) {
                        message.error("Failed to remove Developers");
                      }
                    }}
                    onSearch={async (value: any) => {
                      // setFilter(value);
                    }}
                    onBlur={() => {
                      // fetchUserList("");
                    }}
                    options={usersList}
                  />
                ) : (
                  <>
                    {selectedNode?.defaultUsersList?.length > 0 ? (
                      selectedNode?.defaultUsersList.map((item: any) => {
                        return <Tag key={item.label}>{item.label}</Tag>;
                      })
                    ) : (
                      <p className="sub-title">No Developers Set</p>
                    )}
                  </>
                )}
              </div>
            </Col>
          )}

          {/* <Col span={24} className="mt-2">
            <div>
              <p className="title">Types</p>
              <p className="sub-title">
                {selectedNode?.type ? selectedNode?.type : "Not Set"}
              </p>
            </div>
          </Col> */}

          {selectedNode?.type === TestTypes.TESTCASES && (
            <>
              <Tabs defaultActiveKey="1" onChange={onChangeTabs}>
                <TabPane tab="General" key="1">
                  <div>
                    <Col span={24} className="mt-2">
                      <div>
                        <p className="title">Expected Result</p>
                        <p className="sub-title">
                          {selectedNode?.testCaseInfos?.expectedResult
                            ? selectedNode?.testCaseInfos?.expectedResult
                            : "Not Set"}
                        </p>
                      </div>
                    </Col>

                    <Col span={24} className="mt-3">
                      <div>
                        <p className="title">Precondition</p>
                        <p className="sub-title">
                          {selectedNode?.testCaseInfos?.preCondition
                            ? selectedNode?.testCaseInfos?.preCondition
                            : "Not Set"}
                        </p>
                      </div>
                    </Col>

                    <Col span={24} className="mt-3">
                      <div>
                        <p className="title">Created By</p>
                        <p className="sub-title">
                          {selectedNode?.testCaseInfos?.createdBy
                            ? selectedNode?.testCaseInfos?.createdBy
                            : "Not Set"}
                        </p>
                      </div>
                    </Col>

                    <Col span={24} className="mt-3">
                      <div>
                        <p className="title">Updated By</p>
                        <p className="sub-title">
                          {selectedNode?.testCaseInfos?.updatedBy
                            ? selectedNode?.testCaseInfos?.updatedBy
                            : "Not Set"}
                        </p>
                      </div>
                    </Col>

                    {selectedNode?.testCaseInfos?.testCaseStepDetailModel
                      .length > 0 && (
                      <>
                        <Divider
                          style={{ marginBottom: "0px", marginTop: "12px" }}
                        />
                        <Col span={24} className="mt-3">
                          <p className="steps-title">Steps</p>
                          <br />

                          {requirementDetailLoading ? (
                            <Spin tip="Loading..." />
                          ) : (
                            <>
                              {selectedNode?.testCaseInfos?.testCaseStepDetailModel.map(
                                (item: any, index: number) => {
                                  return (
                                    <ul
                                      key={index}
                                      className="d-flex"
                                      // style={{ backgroundColor: "pink" }}
                                    >
                                      <div className="step-position">
                                        {index + 1}
                                      </div>
                                      <div className="step-content">
                                        <div className="mt-3">
                                          <p className="sub-title">
                                            {item?.stepDescription
                                              ? item?.stepDescription
                                              : "Not Set"}
                                          </p>
                                        </div>
                                        <div className="mt-1">
                                          <p className="steps-title">
                                            Expected Result
                                          </p>
                                          <p className="sub-title">
                                            {item?.expectedResultTestStep
                                              ? item?.expectedResultTestStep
                                              : "Not Set"}
                                          </p>
                                        </div>
                                      </div>
                                    </ul>
                                  );
                                }
                              )}
                            </>
                          )}
                        </Col>
                      </>
                    )}
                  </div>
                </TabPane>
                <TabPane tab="Execution History" key="2">
                  <div className="requirement-page-overflow pt__15 pr__10">
                    <Typography.Title level={5}>
                      Test Case Execution History
                    </Typography.Title>
                    {selectedNode?.key !== null ? (
                      !infiniteHistoryLoading ? (
                        <>
                          <div>
                            {infiniteHistoryData?.map((item) => (
                              <div key={item.testRunTestCaseHistoryId}>
                                <div
                                  className="header"
                                  style={{
                                    display: "flex",
                                    justifyContent: "space-between",
                                    flexDirection: "row",
                                  }}
                                >
                                  <Typography.Paragraph
                                    style={{ marginRight: "50px" }}
                                  >
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
                  </div>
                </TabPane>
              </Tabs>
            </>
          )}
        </Row>
      </Col>
    </>
  );
};

export default RequirementUpdate;
