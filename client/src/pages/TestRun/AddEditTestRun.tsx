import React, { useState } from "react";
import moment from "moment";
import { useHistory, useRouteMatch } from "react-router";
import {
  Typography,
  Button,
  Form,
  Input,
  Select,
  Row,
  Col,
  Divider,
  Spin,
  message,
  TreeSelect,
} from "antd";
import { ArrowLeftOutlined, PlusCircleOutlined } from "@ant-design/icons";
import {
  useAddTestRunMutation,
  useGetAllEnvironmentListQuery,
  useGetEditTestRunDataQuery,
  useGetProjectMemberListQuery,
  useGetTestPlansByProjectSlugQuery,
  useUpdateTestRunMutation,
} from "../../store/services/main/test-run";
import { useDebounce } from "use-debounce/lib";
import useUpdateEffect from "../../util/custom-hooks/useUpdateEffect";
import { useGetFolderAndTestPlansQuery } from "../../store/services/main/test-plan";
import {
  formatTestPlanTreeResponse,
  formatTreeResponse,
} from "../../util/tree.util";
import { useForm } from "antd/lib/form/Form";

const AddEditTestRun: React.FC = (): JSX.Element => {
  const route = useRouteMatch<{ projectSlug: string; testRunId: string }>();
  const history = useHistory();

  // States
  const [isAdding, setIsAdding] = useState(
    route.params.testRunId === undefined
  );
  const [testPlanFilter, setTestPlanFilter] = useState<string>("");
  const [testPlanFilterDebounce] = useDebounce(testPlanFilter, 500);

  const { data: getEditTestRunData, isLoading: isGetEditTestRunDataLoading } =
    useGetEditTestRunDataQuery(+route.params.testRunId, {
      skip: isAdding,
    });
  const [form] = useForm();
  const [addTestRun, { isLoading: isAddTestRunLoading }] =
    useAddTestRunMutation();
  const [updateTestRun, { isLoading: isUpdateTestRunLoading }] =
    useUpdateTestRunMutation();

  // const {
  //   data: getTestPlansByProjectSlugData,
  //   isLoading: isGetTestPlansByProjectSlugLoading,
  //   isFetching: isGetTestPlansByProjectSlugFetching,
  //   isError: isGetTestPlansByProjectSlugError,
  //   refetch: refetchTestPlansByProjectSlug,
  // } = useGetTestPlansByProjectSlugQuery({
  //   projectSlug: route.params.projectSlug,
  //   searchValue: testPlanFilterDebounce,
  // });
  const {
    data: getAllEnvironmentListData,
    isLoading: isGetAllEnvironmentListLoading,
  } = useGetAllEnvironmentListQuery(route.params.projectSlug);
  const {
    data: getProjectMemberListData,
    isLoading: isGetProjectMemberListLoading,
  } = useGetProjectMemberListQuery(route.params.projectSlug);

  const onFinish = (values: any) => {
    // console.log("values", values);
    if (isAdding) {
      addTestRun({ ...values, projectSlug: route.params.projectSlug })
        .unwrap()
        .then(() => {
          message.success("Test Run added successfully.", 3);
          navigateBack();
        })
        .catch(() => message.error("Failed to add Test Run.", 3));
    } else {
      updateTestRun({
        testRunId: +route.params.testRunId,
        title: values.title,
        description: values.description,
        environmentId: values.environmentId,
      })
        .unwrap()
        .then(() => {
          message.success("Test Run updated successfully.", 3);
          navigateBack();
        })
        .catch(() => message.error("Failed to update Test Run.", 3));
    }
  };

  const onFinishFailed = (errorInfo: any) => {
    // console.log("Failed:", errorInfo);
  };

  const navigateBack = () =>
    history.push(`/project/${route.params.projectSlug}/test-runs`);

  const getFormInitialValues = () => {
    if (isAdding) {
      return {
        title: `Test run ${moment().format("L")}`,
        description: undefined,
        environmentId: undefined,
        testPlanId: undefined,
        defaultAssigneeProjectMemberId: undefined,
      };
    }

    return {
      title: getEditTestRunData?.title,
      description: getEditTestRunData?.description,
      environmentId: getEditTestRunData?.environmentId,
      testPlanId: getEditTestRunData?.testPlanId,
      defaultAssigneeProjectMemberId: getEditTestRunData?.assigneeId,
    };
  };

  // useUpdateEffect(() => {
  //   refetchTestPlansByProjectSlug();
  // }, [testPlanFilterDebounce]);

  const {
    data: getFolderAndTestPlansData,
    isLoading: isGetFolderAndTestPlansLoading,
    isError: isGetFolderAndTestPlansError,
    isFetching: isGetFolderAndTestPlansFetching,
    refetch: refetchGetFolderAndTestPlansDataQuery,
  } = useGetFolderAndTestPlansQuery(
    {
      projectSlug: route.params.projectSlug,
      searchValue: testPlanFilterDebounce,
    },
    {
      refetchOnMountOrArgChange: true,
    }
  );

  const planTreeData = formatTestPlanTreeResponse(
    getFolderAndTestPlansData?.data!
  );

  const treeSelectChangehandler = (value: any) => {};

  useUpdateEffect(() => {
    refetchGetFolderAndTestPlansDataQuery();
  }, [testPlanFilterDebounce]);

  return (
    <div className="add-edit-test-run">
      <div>
        <ArrowLeftOutlined onClick={navigateBack} />
        <Typography.Title level={4}>
          {isAdding ? "Start Test Run" : "Edit Test Run"}
        </Typography.Title>
      </div>
      {isGetEditTestRunDataLoading ? (
        <div className="spinner-container">
          <Spin />
        </div>
      ) : (
        <Form
          name="add-edit-test-run"
          initialValues={getFormInitialValues()}
          onFinish={onFinish}
          onFinishFailed={onFinishFailed}
          autoComplete="off"
          layout="vertical"
          form={form}
        >
          <Form.Item
            label="Run Title"
            name="title"
            rules={[
              { required: true, message: "Please enter a run title." },
              { whitespace: true, message: "Run title cannot be empty." },
              {
                max: 49,
                message: "Run Title should be less than 50 characters.",
              },
            ]}
            shouldUpdate
          >
            <Input />
          </Form.Item>
          <Form.Item label="Description" name="description" shouldUpdate>
            <Input.TextArea rows={4} showCount maxLength={500} />
          </Form.Item>
          <Row>
            <Col span={12}>
              <Form.Item
                label="Plan"
                name="testPlanId"
                wrapperCol={{ span: 23 }}
                rules={[{ required: true, message: "Please add a test plan." }]}
              >
                {/* <Select
                  mode="multiple"
                  placeholder="Select..."
                  filterOption={false}
                  loading={
                    isGetTestPlansByProjectSlugLoading ||
                    isGetTestPlansByProjectSlugFetching
                  }
                  disabled={!isAdding}
                  onSearch={(value) => {
                    setTestPlanFilter(value);
                  }}
                >
                  {!isGetTestPlansByProjectSlugError
                    ? getTestPlansByProjectSlugData?.data.map((plan) => (
                        <Select.Option
                          key={plan.testPlanId}
                          value={plan.testPlanId}
                        >
                          {plan.testPlanName}
                        </Select.Option>
                      ))
                    : null}
                </Select> */}
                <TreeSelect
                  showSearch
                  placeholder="Select..."
                  loading={
                    isGetFolderAndTestPlansLoading ||
                    isGetFolderAndTestPlansFetching
                  }
                  disabled={!isAdding}
                  dropdownStyle={{ maxHeight: 400, overflow: "auto" }}
                  treeDefaultExpandAll
                  treeData={isGetFolderAndTestPlansError ? [] : planTreeData}
                  multiple
                  onChange={treeSelectChangehandler}
                  treeNodeFilterProp="title"
                />
              </Form.Item>
            </Col>
            <Col span={12}>
              <Form.Item label="Environment" name="environmentId">
                <Select
                  placeholder="Select..."
                  loading={isGetAllEnvironmentListLoading}
                >
                  {getAllEnvironmentListData?.map((environment) => (
                    <Select.Option
                      key={environment.environmentId}
                      value={environment.environmentId}
                    >
                      {environment.environmentName}
                    </Select.Option>
                  ))}
                </Select>
              </Form.Item>
            </Col>
            <Col span={12}>
              <Form.Item
                label="Default Assignee"
                name="defaultAssigneeProjectMemberId"
                wrapperCol={{ span: 23 }}
              >
                <Select
                  placeholder="Select..."
                  loading={isGetProjectMemberListLoading}
                  disabled={!isAdding}
                >
                  {getProjectMemberListData?.data.map((user) => (
                    <Select.Option
                      key={user.projectMemberId}
                      value={user.projectMemberId}
                    >
                      {user.name}
                    </Select.Option>
                  ))}
                </Select>
              </Form.Item>
            </Col>
          </Row>

          {/* <div className="test-cases">
          <Typography.Title level={5}>Test Cases</Typography.Title>
          <Divider className="my-1" />
          <Typography.Paragraph className="mt-2">
            11 test cases selected
          </Typography.Paragraph>
          <Button icon={<PlusCircleOutlined />} type="text">
            Add cases
          </Button>
        </div> */}

          <Form.Item>
            <Button
              type="primary"
              htmlType="submit"
              loading={isAddTestRunLoading || isUpdateTestRunLoading}
            >
              {isAdding ? "Start Run" : "Save Run"}
            </Button>
            <Button type="ghost" className="mx-3" onClick={navigateBack}>
              Cancel
            </Button>
          </Form.Item>
        </Form>
      )}
    </div>
  );
};

export default AddEditTestRun;
