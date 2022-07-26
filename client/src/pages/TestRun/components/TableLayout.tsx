import React, { useEffect, useRef, useState, useCallback } from "react";
import {
  DeleteOutlined,
  // EditOutlined,
  UserAddOutlined,
  UserDeleteOutlined,
} from "@ant-design/icons";
import {
  Button,
  Checkbox,
  Collapse,
  message,
  Popconfirm,
  // Progress,
  Space,
  Spin,
  Tooltip,
  Typography,
  Empty,
  Pagination,
} from "antd";
import moment from "moment";
import "moment-duration-format";
import useUpdateEffect from "../../../util/custom-hooks/useUpdateEffect";
import TestRunStatus from "./TestRunStatus";
import {
  ProjectMember,
  TestCase,
  TestPlanAndCaseIdsForDeleteAndRetest,
} from "../../../interfaces";
// import { getResult } from "../../../util/functions.utils";
import { useHistory } from "react-router";
import {
  useDeleteTestCaseOfTestPlanFromTestRunMutation,
  useLazyGetTestRunTestCaseDetailsQuery,
  useRefreshTestPlanMutation,
} from "../../../store/services/main/test-run";
import CustomStatusBar from "../../../components/CustomStatusBar";
import { FilterState } from "../../../components/TableFilter";
import { setLabels } from "react-chartjs-2/dist/utils";

interface TableLayoutProps {
  testPlanId: number;
  testPlanName: string;
  statusCounts: {
    pendingCount: number;
    passedCount: number;
    failedCount: number;
    blockedCount: number;
  };
  setIsDropdownVisible: (value: any) => void;
  setIsAssignModalVisible: (value: boolean) => void;
  setTestCasesIds: (value: any) => void;
  setTestPlanAndCaseIds: React.Dispatch<
    React.SetStateAction<TestPlanAndCaseIdsForDeleteAndRetest[]>
  >;
  updateUserFromTestCase: (
    userId: number | undefined,
    testCasesIds: number[],
    operationAfterSuccess: () => void
  ) => void;
  testCaseIdForAssign: any;
  setIsTestCaseStatusDetailModalVisible: (val: boolean) => void;
  testRunId: number;
  debouncedSearch: string;
  currentFilters: FilterState[];
  getProjectMemberListData:
    | {
        data: ProjectMember[];
      }
    | undefined;

  testCaseDelete: boolean;
  setTestCaseDelete: (value: any) => void;
}

const TableLayout: React.FC<TableLayoutProps> = (props): JSX.Element => {
  const history = useHistory();
  const pageNumber = useRef(1);
  const pageSize = useRef(10);
  // console.log(pageNumber.current, "page number");

  // States
  const [groupChecked, setGroupChecked] = useState<Record<number, boolean>>({});
  const [filterByAssignee, setFilterByAssignee] = useState<number>(0);
  const [hasTestPlanId, setHasTestPlanId] = useState(0);

  const [deleteTestCaseOfTestPlan] =
    useDeleteTestCaseOfTestPlanFromTestRunMutation();
  const [refreshTestPlan, { isLoading: isRefreshTestPlanLoading }] =
    useRefreshTestPlanMutation();
  const [getTestRunTestCaseDetailsTrigger, getTestRunTestCaseDetailsData] =
    useLazyGetTestRunTestCaseDetailsQuery();

  const handleRefreshTestPlan = (
    e: React.MouseEvent<HTMLElement, MouseEvent>
  ) => {
    e.stopPropagation();

    refreshTestPlan({
      testRunId: props.testRunId,
      testPlanId: props.testPlanId,
    })
      .unwrap()
      .then((data) => message.success(data, 3))
      .catch(() => message.error("Unable to refresh test plan.", 3));
  };

  const toggleHeaderChecked = (e: any) => {
    const updatedData: Record<number, boolean> = {};

    getTestRunTestCaseDetailsData.data?.data.forEach((item) => {
      updatedData[item.testRunTestCaseHistoryId] = e.target.checked;
    });
    setGroupChecked(updatedData);

    // for assign & unassign
    props.setTestCasesIds((prevValue: any) => ({
      ...prevValue,
      ...updatedData,
    }));

    // for delete
    if (e.target.checked) {
      props.setTestPlanAndCaseIds((prevValue) => {
        let testPlanFound = false;
        let testPlanFoundIndex = 0;

        prevValue.forEach((item, index) => {
          if (item.testPlanId === props.testPlanId) {
            testPlanFound = true;
            testPlanFoundIndex = index;
          }
        });

        if (testPlanFound) {
          const nextValue = [...prevValue];
          nextValue[testPlanFoundIndex] = {
            testPlanId: props.testPlanId,
            testCaseId: getTestRunTestCaseDetailsData.data?.data.map(
              (item) => item.projectModuleId
            )!,
          };
          return nextValue;
        } else {
          return [
            ...prevValue,
            {
              testPlanId: props.testPlanId,
              testCaseId: getTestRunTestCaseDetailsData.data?.data.map(
                (item) => item.projectModuleId
              )!,
            },
          ];
        }
      });
    } else {
      props.setTestPlanAndCaseIds((prevValue) =>
        prevValue.filter((item) => item.testPlanId !== props.testPlanId)
      );
    }
  };

  const toggleChecked = (e: any, projectModuleId: number) => {
    const clonedData = { ...groupChecked };
    clonedData[e.target.value] = e.target.checked;
    setGroupChecked(clonedData);

    // for assign & unassign
    props.setTestCasesIds((prevValue: any) => ({
      ...prevValue,
      ...clonedData,
    }));

    // for delete
    props.setTestPlanAndCaseIds((prevValue) => {
      if (
        prevValue.length === 1 &&
        prevValue[0].testCaseId.length === 1 &&
        !e.target.checked &&
        prevValue[0].testCaseId[0] === projectModuleId
      ) {
        return [];
      }

      let testPlanFound = false;
      let testPlanFoundIndex = 0;

      prevValue.forEach((item, index) => {
        if (item.testPlanId === props.testPlanId) {
          testPlanFound = true;
          testPlanFoundIndex = index;
        }
      });

      if (testPlanFound) {
        const nextValue = [...prevValue];

        nextValue[testPlanFoundIndex].testCaseId.forEach((id) => {
          if (e.target.checked) {
            if (
              !nextValue[testPlanFoundIndex].testCaseId.includes(
                projectModuleId
              )
            ) {
              nextValue[testPlanFoundIndex].testCaseId.push(projectModuleId);
            }
          } else {
            // delete whole array
            if (
              nextValue[testPlanFoundIndex].testCaseId.length === 1 &&
              id === projectModuleId
            ) {
              nextValue.splice(testPlanFoundIndex, 1);

              // delete only that item
            } else {
              nextValue[testPlanFoundIndex].testCaseId.forEach((id, index) => {
                if (id === projectModuleId) {
                  nextValue[testPlanFoundIndex].testCaseId.splice(index, 1);
                }
              });
            }
          }
        });
        return nextValue;
      } else {
        return [
          ...prevValue,
          {
            testPlanId: props.testPlanId,
            testCaseId: [projectModuleId],
          },
        ];
      }
    });
  };

  // const handleEditTestCase = () => {
  //   console.log("edit");
  // };

  const handleDeleteTestCase = (
    projectModuleId: number,
    testRunId: number,
    testPlanId: number
  ) => {
    deleteTestCaseOfTestPlan({
      testRunId,
      projectModuleId,
      testPlanId,
    })
      .unwrap()
      .then((msg) => message.success(msg, 3))
      .catch(() => message.error("Unable to delete.", 3));
  };

  const getTestCases = useCallback(
    (assignee: number, fetchData = !!hasTestPlanId) => {
      if (fetchData) {
        getTestRunTestCaseDetailsTrigger({
          testRunId: props.testRunId,
          testPlanId: props.testPlanId,
          searchValue: props.debouncedSearch,
          pageNumber: pageNumber.current,
          pageSize: pageSize.current || 10,
          filters: {
            assignee,
          },
        });
        // setnoOfPages(getTestRunTestCaseDetailsData?.data?.totalPages as number);
        // console.log(noOfPages);
      }
    },
    [
      getTestRunTestCaseDetailsTrigger,
      hasTestPlanId,
      props.debouncedSearch,
      props.testPlanId,
      props.testRunId,
    ]
  );

  useUpdateEffect(() => {
    getTestCases(filterByAssignee, true);
    // setnoOfPages(getTestRunTestCaseDetailsData?.data?.totalPages as number);
  }, [props.debouncedSearch]);

  // auto opens the panel when data is found after searching
  useUpdateEffect(() => {
    if (
      (props.debouncedSearch || props.currentFilters.length) &&
      getTestRunTestCaseDetailsData.data?.data.length
    ) {
      setHasTestPlanId(props.testPlanId);
    }
  }, [getTestRunTestCaseDetailsData.data?.data]);

  useUpdateEffect(() => {
    props.setIsDropdownVisible((prevValue: Record<string, boolean>) => {
      const clonedState = {
        ...prevValue,
      };

      clonedState[props.testPlanName] =
        Object.values(groupChecked).includes(true);
      return clonedState;
    });
  }, [groupChecked]);

  // for setting initial data & after retest
  useEffect(() => {
    const data: Record<number, boolean> = {};
    getTestRunTestCaseDetailsData.data?.data.forEach((item) => {
      data[item.testRunTestCaseHistoryId] = false;
    });

    setGroupChecked(data);
  }, [getTestRunTestCaseDetailsData.data?.data]);

  // for filters
  useUpdateEffect(() => {
    let projectMemberId = 0;

    props.getProjectMemberListData?.data.forEach((member) => {
      if (
        props.currentFilters
          .map((filter) => filter.filterValue)
          .includes(member.name)
      ) {
        projectMemberId = member.projectMemberId;
      }
    });

    setFilterByAssignee(projectMemberId);
    getTestCases(projectMemberId, true);
    // setnoOfPages(getTestRunTestCaseDetailsData?.data?.totalPages as number);
  }, [props.currentFilters.length]);

  useUpdateEffect(() => {
    if (props.testCaseDelete) {
      pageNumber.current = 1;
      getTestCases(filterByAssignee);
      props.setTestCaseDelete(false);
    }
  }, [props.testCaseDelete]);

  return (
    <Collapse ghost activeKey={hasTestPlanId}>
      <Collapse.Panel
        collapsible="header"
        header={
          <div
            onClick={() => {
              const newState =
                hasTestPlanId === props.testPlanId ? 0 : props.testPlanId;
              setHasTestPlanId(newState);
              getTestCases(filterByAssignee, !!newState);
              // setnoOfPages(
              //   getTestRunTestCaseDetailsData?.data?.totalPages as number
              // );
            }}
          >
            <Typography.Title level={5}>{props.testPlanName}</Typography.Title>
            <div style={{ width: "200px" }} className="mx-3">
              <CustomStatusBar
                counts={[
                  props.statusCounts.pendingCount,
                  props.statusCounts.passedCount,
                  props.statusCounts.failedCount,
                  props.statusCounts.blockedCount,
                ]}
                total={
                  props.statusCounts.pendingCount +
                  props.statusCounts.passedCount +
                  props.statusCounts.failedCount +
                  props.statusCounts.blockedCount
                }
                displayCount={false}
              />
            </div>
            <Button
              onClick={handleRefreshTestPlan}
              loading={isRefreshTestPlanLoading}
            >
              Refresh
            </Button>
          </div>
        }
        key={props.testPlanId}
      >
        <table className="table-layout">
          <thead>
            <tr>
              <th className="header-checkbox">
                <Checkbox
                  checked={
                    Object.keys(groupChecked).length
                      ? !Object.values(groupChecked).includes(false)
                      : false
                  }
                  onChange={toggleHeaderChecked}
                />
              </th>
              <th>Title</th>
              <th>Assignee</th>
              <th>Time spent</th>
              <th>Results</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {!getTestRunTestCaseDetailsData.isLoading ? (
              !getTestRunTestCaseDetailsData.isError ? (
                getTestRunTestCaseDetailsData.data?.data.map((item) => (
                  <tr key={item.projectModuleId}>
                    <td>
                      <Checkbox
                        checked={groupChecked[item.testRunTestCaseHistoryId]}
                        value={item.testRunTestCaseHistoryId}
                        onChange={(e) => toggleChecked(e, item.projectModuleId)}
                      />
                    </td>
                    <td>
                      <Typography.Paragraph
                        ellipsis={{ rows: 2 }}
                        className="m-0"
                      >
                        {item.testCaseName}
                      </Typography.Paragraph>
                    </td>
                    <td>
                      <Typography.Paragraph
                        ellipsis={{ rows: 2 }}
                        className="m-0"
                      >
                        {item.assignee}
                      </Typography.Paragraph>
                    </td>
                    <td>
                      {item.totalTimeSpent
                        ? moment
                            .utc(
                              moment
                                .duration(item.totalTimeSpent, "seconds")
                                .asMilliseconds()
                            )
                            .format("HH:mm:ss")
                        : "00:00:00"}
                    </td>
                    <td>
                      {item.testRunTestCaseStatusListItemSystemName && (
                        <TestRunStatus
                          className="table-status"
                          onClick={() => {
                            props.setIsTestCaseStatusDetailModalVisible(true);
                            history.push(
                              `?testPlanId=${props.testPlanId}&testCaseId=${item.projectModuleId}`
                            );
                          }}
                          type="tag"
                          status={
                            item.testRunTestCaseStatusListItemSystemName.toLowerCase() as any
                          }
                        >
                          <>{item.testRunTestCaseStatusListItemSystemName}</>
                        </TestRunStatus>
                      )}
                    </td>
                    <td>
                      <Space size="middle">
                        <Tooltip title="Assign">
                          <UserAddOutlined
                            className="table-action"
                            onClick={() => {
                              props.setIsAssignModalVisible(true);
                              props.testCaseIdForAssign.current = [
                                +item.testRunTestCaseHistoryId,
                              ];
                            }}
                            style={
                              item.assignee
                                ? { pointerEvents: "none", color: "gray" }
                                : {}
                            }
                          />
                        </Tooltip>
                        <Popconfirm
                          title="Are you sure to unassign user?"
                          okText="Yes"
                          cancelText="No"
                          onConfirm={() =>
                            props.updateUserFromTestCase(
                              undefined,
                              [+item.testRunTestCaseHistoryId],
                              () => {}
                            )
                          }
                        >
                          <UserDeleteOutlined
                            className="table-action"
                            style={
                              item.assignee
                                ? {}
                                : { pointerEvents: "none", color: "gray" }
                            }
                          />
                        </Popconfirm>
                        {/* <Tooltip title="Edit">
                            <EditOutlined
                              className="table-action"
                              onClick={handleEditTestCase}
                            />
                          </Tooltip> */}
                        <Popconfirm
                          title="Are you sure to delete?"
                          okText="Yes"
                          cancelText="No"
                          onConfirm={() =>
                            handleDeleteTestCase(
                              item.projectModuleId,
                              props.testRunId,
                              props.testPlanId
                            )
                          }
                        >
                          <DeleteOutlined className="table-action" />
                        </Popconfirm>
                      </Space>
                    </td>
                  </tr>
                ))
              ) : (
                <tr>
                  <td></td>
                  <td></td>
                  <td></td>
                  <td style={{ height: 250 }}>
                    <Empty image={Empty.PRESENTED_IMAGE_SIMPLE} />
                  </td>
                  <td></td>
                  <td></td>
                </tr>
              )
            ) : (
              <tr>
                <td></td>
                <td></td>
                <td></td>
                <td className="spinner-container">
                  <Spin />
                </td>
                <td></td>
                <td></td>
              </tr>
            )}
          </tbody>
        </table>
        {!getTestRunTestCaseDetailsData.isError &&
        getTestRunTestCaseDetailsData.data?.data.length ? (
          <div
            style={{
              display: "flex",
              justifyContent: "flex-end",
              marginTop: "20px",
            }}
          >
            <Pagination
              current={pageNumber.current}
              defaultCurrent={getTestRunTestCaseDetailsData.data?.pageNumber}
              total={getTestRunTestCaseDetailsData.data?.totalRecords}
              size="small"
              onChange={(pageNum, newPageSize) => {
                pageNumber.current = pageNum;
                if (newPageSize) {
                  pageSize.current = newPageSize;
                }

                getTestCases(filterByAssignee);
                // setnoOfPages(
                //   getTestRunTestCaseDetailsData?.data?.totalPages as number
                // );
              }}
            />
          </div>
        ) : null}
      </Collapse.Panel>
    </Collapse>
  );
};

export default TableLayout;
