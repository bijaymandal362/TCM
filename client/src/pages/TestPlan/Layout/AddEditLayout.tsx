import React, { useEffect, useState } from "react";
import { Input, Divider, Table, Button, message, Tag, Form } from "antd";
import { MinusCircleOutlined } from "@ant-design/icons";
import { useAppSelector } from "../../../store/reduxHooks";
import {
  IAddUpdateTestPlanParams,
  ITestPlanTestCases,
  Pagination,
} from "../../../interfaces";
import { useDebounce } from "use-debounce/lib";
import {
  testPlanApi,
  useGetTestPlanTestCasesQuery,
} from "../../../store/services/main/test-plan";
import {
  clearTestCasesProjectModuleId,
  deleteFetchedTestCasesFromTestPlan,
  deleteNewTestCasesFromTestPlan,
} from "../../../store/features/testPlanSlice";
import { useDispatch } from "react-redux";
import { Draft } from "immer";
import useUpdateEffect from "../../../util/custom-hooks/useUpdateEffect";

interface AddEditLayoutProps {
  toggleEditing: () => void;
  toggleAdding: () => void;
  isAdding: boolean;
  addFolderOrTestPlan: (params: IAddUpdateTestPlanParams) => any;
  isAddFolderOrTestPlanLoading: boolean;
  updateFolderOrTestPlan: (params: IAddUpdateTestPlanParams) => any;
  isUpdateFolderOrTestPlanLoading: boolean;
  parentFolderId: number | null;
  selectedTestPlanData: any;
  editTestPlanId: number | undefined;
  setIsTestPlanSelected: (value: boolean) => void;
  projectSlug: string;
}

const AddEditLayout: React.FC<AddEditLayoutProps> = (props) => {
  const dispatch = useDispatch();
  const [form] = Form.useForm();
  const newTestCasesForTestPlan = useAppSelector(
    (state) => state.testPlan.newTestCasesForTestPlan
  );
  const testCasesToBeDeleteIds = useAppSelector(
    (state) => state.testPlan.testCasesToBeDeleteIds
  );

  // States
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState<number>(10);
  const [search, setSearch] = useState("");
  const [notFound, setNotFound] = useState(false);

  const [debouncedSearch] = useDebounce(search, 500);

  const {
    data: getTestPlanTestCasesData,
    isLoading: isGetTestPlanTestCasesLoading,
    isFetching: isGetTestPlanTestCasesFetching,
    refetch: refetchGetTestPlanTestCases,
    error,
  } = useGetTestPlanTestCasesQuery(
    {
      pageNumber: page,
      pageSize,
      searchValue: debouncedSearch,
      testPlanId: props.selectedTestPlanData.key,
    },
    {
      skip: props.isAdding,
      refetchOnMountOrArgChange: true,
    }
  );

  const retrieveTestCases = () => {
    if (getTestPlanTestCasesData?.data?.length) {
      return getTestPlanTestCasesData?.data.map((testCase) => ({
        key: testCase.projectModuleId,
        testCaseName: testCase.testCaseName,
        scenario: testCase.scenario,
        expectedResult: testCase.expectedResult,
        action: testCase.testPlanTestCaseId,
      }));
    } else {
      return [];
    }
  };

  const handleCurrentState = () => {
    if (props.isAdding) {
      props.toggleAdding();
    } else {
      props.toggleEditing();
    }
    props.setIsTestPlanSelected(false);
    dispatch(clearTestCasesProjectModuleId());
  };

  const handleSave = () => {
    if (!form.getFieldValue("testPlanName")) {
      return message.error("Please fill up all the required fields.", 3);
    } else if (form.getFieldValue("testPlanName")?.length >= 50) {
      return message.error("Please fix the errors.", 3);
    }

    const newTestCasesToAddIds = newTestCasesForTestPlan.map((testCase) => {
      return testCase.id;
    });

    if (!newTestCasesForTestPlan.concat(retrieveTestCases() as []).length) {
      return message.error("Please include at least one test case.", 3);
    }

    const getApiBody = (testPlanId: number) => ({
      parentTestPlanId: props.parentFolderId,
      testPlanId,
      title: form.getFieldValue("testPlanName"),
      description: form.getFieldValue("testPlanDescription"),
      projectSlug: props.projectSlug,
      testPlanTypeListItemId: 38,
      testPlanType: "TestPlan",
      testPlanName: form.getFieldValue("testPlanName"),
      projectModuleId: newTestCasesToAddIds,
      testPlanTestCaseId: testCasesToBeDeleteIds,
    });

    const operationsAfterSuccess = (type: string): void => {
      props.setIsTestPlanSelected(false);
      dispatch(clearTestCasesProjectModuleId());
      message.success(`Test Plan ${type} successfully.`, 3);
    };

    if (props.isAdding) {
      props
        .addFolderOrTestPlan(getApiBody(0))
        .unwrap()
        .then(() => {
          props.toggleAdding();
          operationsAfterSuccess("added");
        })
        .catch((err: any) => message.error(err?.data, 3));
    } else {
      props
        .updateFolderOrTestPlan(getApiBody(props.editTestPlanId as number))
        .unwrap()
        .then(() => {
          refetchGetTestPlanTestCases();
          props.toggleEditing();
          operationsAfterSuccess("updated");
        })
        .catch((err: any) => message.error(err?.data, 3));
    }
  };

  useUpdateEffect(() => {
    if (!isGetTestPlanTestCasesFetching) {
      if ((error as any)?.status === 404) {
        if (!notFound) setNotFound(true);
      } else {
        if (notFound) setNotFound(false);
      }
    }
  }, [isGetTestPlanTestCasesFetching]);

  useEffect(() => {
    if (!props.isAdding) {
      form.setFieldsValue({
        testPlanName: props.selectedTestPlanData.title,
        testPlanDescription: props.selectedTestPlanData.description,
      });
    }

    return () => {
      dispatch(clearTestCasesProjectModuleId());
    };
  }, []);

  const columns = [
    {
      title: "Test Case Name",
      dataIndex: "testCaseName",
      key: "testCaseName",
      render: (name: string) => <div style={{ maxWidth: 350 }}>{name}</div>,
    },
    {
      title: "Scenario",
      dataIndex: "scenario",
      key: "scenario",
    },
    {
      title: "Expected Result",
      dataIndex: "expectedResult",
      key: "expectedResult",
    },
    {
      title: "Action",
      dataIndex: "action",
      key: "action",
      render: (id: any) => {
        if (typeof id !== "string") {
          return (
            <MinusCircleOutlined
              className="minus-icon"
              onClick={() => {
                dispatch(deleteFetchedTestCasesFromTestPlan(id));
                dispatch(
                  testPlanApi.util.updateQueryData(
                    "getTestPlanTestCases",
                    {
                      pageNumber: page,
                      pageSize,
                      searchValue: debouncedSearch,
                      testPlanId: props.selectedTestPlanData.key,
                    },
                    (draft: Draft<Pagination<ITestPlanTestCases>>) => {
                      const index = draft.data.findIndex(
                        (item) => item.testPlanTestCaseId === id
                      );
                      draft.data.splice(index, 1);
                    }
                  )
                );
              }}
            />
          );
        }

        return (
          <div
            style={{
              display: "flex",
              justifyContent: "space-between",
            }}
          >
            <MinusCircleOutlined
              className="minus-icon"
              onClick={() => dispatch(deleteNewTestCasesFromTestPlan(+id))}
            />
            <Tag color="red">New</Tag>
          </div>
        );
      },
    },
  ];

  return (
    <div className="add-edit-layout">
      <Form form={form} name="Add/Edit Test Plan">
        <div className="inputs">
          <Form.Item
            name="testPlanName"
            label="Test Plan Name"
            rules={[
              {
                required: true,
                whitespace: true,
                message: "Test Plan Name is required.",
              },
              {
                max: 49,
                message: "Test Plan Name should be less than 50 characters.",
              },
            ]}
          >
            <Input />
          </Form.Item>
          <Form.Item name="testPlanDescription" label="Test Plan Description">
            <Input.TextArea showCount maxLength={500} rows={4} />
          </Form.Item>
        </div>
      </Form>

      <Divider />

      <div className="input-search-and-buttons">
        <Input.Search
          placeholder="Search Test Case"
          className="mb-3"
          loading={isGetTestPlanTestCasesFetching}
          value={search}
          disabled={props.isAdding}
          onChange={(e) => {
            setSearch(e.target.value);
          }}
        />
        <div>
          <Button
            type="primary"
            onClick={handleSave}
            loading={
              props.isAddFolderOrTestPlanLoading ||
              props.isUpdateFolderOrTestPlanLoading
            }
          >
            Save
          </Button>
          <Button onClick={handleCurrentState}>Cancel</Button>
        </div>
      </div>
      <Table
        columns={columns}
        dataSource={newTestCasesForTestPlan.concat(retrieveTestCases() as [])}
        loading={isGetTestPlanTestCasesLoading}
        pagination={
          !getTestPlanTestCasesData?.totalPages
            ? false
            : {
                onChange: (newPage, newPageSize) => {
                  setPage(newPage);
                  // setPageSize(newPageSize as number);
                },
                pageSize: pageSize + newTestCasesForTestPlan.length,
                total:
                  getTestPlanTestCasesData?.totalRecords +
                  newTestCasesForTestPlan.length *
                    getTestPlanTestCasesData?.totalPages,
              }
        }
      />
    </div>
  );
};

export default AddEditLayout;
