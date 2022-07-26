import { useRef, useState } from "react";
import { Button, Collapse, Empty, Spin, Typography } from "antd";
import {
  ProjectMember,
  TestRunTestCaseDetails,
  TestRunTestPlansByTestRunId,
} from "../../../interfaces";
import { useLazyGetTestRunTestCaseDetailsQuery } from "../../../store/services/main/test-run";
import TestRunStatus from "./TestRunStatus";
import useUpdateEffect from "../../../util/custom-hooks/useUpdateEffect";
import { FilterState } from "../../../components/TableFilter";

interface WizardCollapsePanelProps {
  data: TestRunTestPlansByTestRunId;
  debouncedSearch: string;
  testRunId: number;
  selectedTestCaseId: number | null;
  setSelectedTestCaseId: (id: number) => void;
  wizardTestCaseIdRef: any;
  setTestPlanID: (id: number) => void;
  currentFilters: FilterState[];
  getProjectMemberListData:
    | {
        data: ProjectMember[];
      }
    | undefined;

  setStatusChange: (value: boolean) => void;
  statusChange: boolean;
}

const WizardCollapsePanel = (props: WizardCollapsePanelProps) => {
  const pageNumber = useRef(1);
  const pageSize = useRef(10);
  const appendDataRef = useRef(false);

  // States
  const [
    infiniteTestRunTestCaseDetailsData,
    setInfiniteTestRunTestCaseDetailsData,
  ] = useState<TestRunTestCaseDetails[]>([]);
  const [filterByAssignee, setFilterByAssignee] = useState<number>(0);
  const [hasTestPlanId, setHasTestPlanId] = useState(0);

  const [getTestRunTestCaseDetailsTrigger, getTestRunTestCaseDetailsData] =
    useLazyGetTestRunTestCaseDetailsQuery();

  const getTestCases = (assignee: number, fetchData = !!hasTestPlanId) => {
    if (fetchData) {
      getTestRunTestCaseDetailsTrigger({
        testRunId: props.testRunId,
        testPlanId: +props.data.testPlanId,
        searchValue: props.debouncedSearch,
        pageNumber: pageNumber.current,
        pageSize: pageSize.current || 10,
        filters: {
          assignee,
        },
      });
    }
  };
  // console.log(props.statusChange, "here is change");
  //for refresh issue
  useUpdateEffect(() => {
    getTestCases(filterByAssignee, true);
  }, [props.debouncedSearch]);

  useUpdateEffect(() => {
    if (props.statusChange) {
      pageSize.current = pageNumber.current * pageSize.current;
      pageNumber.current = 1;
      getTestCases(filterByAssignee);
      // props.setStatusChange(false);
    }
  }, [props.statusChange]);

  // auto opens the panel when data is found after searching
  useUpdateEffect(() => {
    if (
      (props.debouncedSearch || props.currentFilters.length) &&
      getTestRunTestCaseDetailsData.data?.data.length
    ) {
      setHasTestPlanId(props.data.testPlanId);
    }
  }, [getTestRunTestCaseDetailsData.data?.data]);

  useUpdateEffect(() => {
    if (appendDataRef.current) {
      setInfiniteTestRunTestCaseDetailsData((prevState) => [
        ...prevState,
        ...getTestRunTestCaseDetailsData.data?.data!,
      ]);
    } else {
      setInfiniteTestRunTestCaseDetailsData(
        getTestRunTestCaseDetailsData.data?.data!
      );
    }

    appendDataRef.current = false;
  }, [getTestRunTestCaseDetailsData.data]);

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
  }, [props.currentFilters.length]);

  return (
    <Collapse
      className="test-plans-and-cases"
      expandIconPosition="right"
      ghost
      activeKey={hasTestPlanId}
    >
      <Collapse.Panel
        key={props.data.testPlanId}
        header={
          <Typography.Title
            onClick={() => {
              const newState =
                hasTestPlanId === props.data.testPlanId
                  ? 0
                  : props.data.testPlanId;
              setHasTestPlanId(newState);
              getTestCases(filterByAssignee, !!newState);
            }}
            level={5}
          >
            {props.data.testPlanName}
          </Typography.Title>
        }
      >
        {!getTestRunTestCaseDetailsData.isLoading ? (
          !getTestRunTestCaseDetailsData.isError ? (
            <>
              <div className="cases">
                {infiniteTestRunTestCaseDetailsData?.map((childItem) => (
                  <div
                    key={childItem.testRunTestCaseHistoryId}
                    onClick={() => {
                      props.setSelectedTestCaseId(
                        childItem.testRunTestCaseHistoryId
                      );
                      props.setTestPlanID(props.data.testPlanId);
                      props.wizardTestCaseIdRef.current =
                        childItem.projectModuleId;
                    }}
                  >
                    <Typography.Paragraph
                      className={
                        props.selectedTestCaseId ===
                        childItem.testRunTestCaseHistoryId
                          ? "selected-title"
                          : ""
                      }
                      ellipsis={{ rows: 1 }}
                    >
                      {childItem.testCaseName}
                    </Typography.Paragraph>
                    <TestRunStatus
                      type="text"
                      status={
                        childItem.testRunTestCaseStatusListItemSystemName?.toLowerCase() as any
                      }
                    >
                      {childItem.testRunTestCaseStatusListItemSystemName}
                    </TestRunStatus>
                  </div>
                ))}
              </div>
              {getTestRunTestCaseDetailsData.data &&
              infiniteTestRunTestCaseDetailsData.length <
                getTestRunTestCaseDetailsData.data?.totalRecords ? (
                <div
                  style={{
                    marginTop: 20,
                    display: "flex",
                    justifyContent: "center",
                  }}
                >
                  <Button
                    onClick={() => {
                      pageNumber.current++;
                      appendDataRef.current = true;
                      getTestCases(filterByAssignee);
                    }}
                  >
                    Load More
                  </Button>
                </div>
              ) : null}
            </>
          ) : (
            <div>
              <Empty image={Empty.PRESENTED_IMAGE_SIMPLE} />
            </div>
          )
        ) : (
          <div className="spinner-container">
            <Spin />
          </div>
        )}
      </Collapse.Panel>
    </Collapse>
  );
};

export default WizardCollapsePanel;
