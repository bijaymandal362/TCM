import {
  Button,
  Col,
  Form,
  Input,
  message,
  Row,
  Select,
  Typography,
} from "antd";
import { TestTypes } from "../../../enum/enum";
import {
  createModules,
  updateModules,
} from "../../../store/actions/requirementManagementAction";
import { useRouteMatch } from "react-router";
import { useEffect, useRef, useState } from "react";
import { FormattedModuleTreeInterface } from "../../../interfaces/projectmodule.interface";

import { FormListFieldData } from "antd/lib/form/FormList";
import { MinusCircleOutlined } from "@ant-design/icons";
import { getDropdownLists } from "../../../store/actions/projectActionCreators";

interface Props {
  formInfo: any;
  selectedNode: any;
  currentAction: String | null;
  handleReset: any;
  getProjectModules: () => void;
  data: any;
  editData: boolean;
}

export const CreateModuleForm = ({
  formInfo,
  handleReset,
  selectedNode,
  currentAction,
  data,
  getProjectModules,
  editData,
}: Props) => {
  const route = useRouteMatch<{ projectSlug: string }>();
  const deleteIdsRef = useRef<number[]>([]);

  // States
  const [testCaseList, setTestCaseList] = useState<any>([]);
  const [loading, setLoading] = useState(false);
  const [testCaseStepDetailIds, setTestCaseStepDetailIds] = useState<number[]>(
    []
  );

  const handleSubmit = async (values: any) => {
    setLoading(true);

    if (currentAction === "create") {
      if (selectedNode?.type === TestTypes.TESTCASES) {
        if (!values?.cases?.length) {
          message.error("Please add atleast one step to testcase.", 3);
          setLoading(false);
          return;
        }

        let hasEmptyFields = false;
        values?.cases?.forEach((item: any) => {
          if (!item?.stepDescription || !item?.expectedResultTestStep) {
            hasEmptyFields = true;
            return;
          }
        });
        if (hasEmptyFields) {
          message.error(
            "Step description & expected result fields cannot be empty.",
            3
          );
          setLoading(false);
          return;
        }

        const tdata: Record<string, any> = {
          projectModuleId: 0,
          parentProjectModuleId: selectedNode?.key,
          projectSlug: route.params?.projectSlug,
          moduleName: values?.name,
          projectModuleType: TestTypes.TESTCASES,
          description: values?.description,
          testCaseDetailId: 0,
          preCondition: values?.preCondition,
          expectedResult: values?.expectedResult,
          testCaseListItemId: values?.selectTypes,
          testCaseStepDetailId: 0,
          testCaseStepDetailModel: values?.cases?.map((item: any) => {
            return {
              ...item,
              testCaseStepDetailId: 0,
              testCaseStepDetailProjectModuleId: selectedNode?.key,
              stepNumber: item?.sn?.sn,
            };
          }),
        };

        const [response, error] = await createModules(tdata);
        setLoading(false);
        if (response?.data) {
          handleReset();
          getProjectModules();
          message.success("Test Case created successfully.", 3);
        }

        if (error) {
          message.error(error.response.data, 3);
        }
      } else if (selectedNode?.type === TestTypes.MODULE) {
        const data = {
          projectSlug: route.params?.projectSlug,
          moduleName: values?.name,
          projectModuleType: TestTypes.MODULE,
          description: values?.description,
          parentProjectModuleId: selectedNode?.key,
        };

        const [response, error] = await createModules(data);
        setLoading(false);
        if (response?.data) {
          handleReset();
          getProjectModules();
          message.success("Module created successfully.", 3);
        }

        if (error) {
          message.error(error.response.data, 3);
        }
      } else {
        const data = {
          projectSlug: route.params?.projectSlug,
          moduleName: values?.name,
          projectModuleType: TestTypes.FUNCTION,
          description: values?.description,
          parentProjectModuleId: selectedNode?.key,
        };

        const [response, error] = await createModules(data);
        setLoading(false);
        if (response?.data) {
          handleReset();
          getProjectModules();
          message.success("Function created successfully.", 3);
        }

        if (error) {
          message.error(error.response.data, 3);
        }
      }

      // ========================UPDATE====================================
    } else {
      if (selectedNode?.type === TestTypes.TESTCASES) {
        if (!values?.cases?.length) {
          message.error("Please add atleast one step to testcase.", 3);
          setLoading(false);
          return;
        }

        let hasEmptyFields = false;
        values?.cases?.forEach((item: any) => {
          if (!item?.stepDescription || !item?.expectedResultTestStep) {
            hasEmptyFields = true;
            return;
          }
        });
        if (hasEmptyFields) {
          message.error(
            "Step description & expected result fields cannot be empty.",
            3
          );
          setLoading(false);
          return;
        }

        const data: Record<string, any> = {
          projectModuleId: selectedNode?.key,
          parentProjectModuleId: selectedNode?.parentProjectModuleId,
          projectSlug: route.params?.projectSlug,
          moduleName: values?.name,
          projectModuleType: TestTypes.TESTCASES,
          description: values?.description,
          preCondition: values?.preCondition,
          expectedResult: values?.expectedResult,
          testCaseListItemId: values?.selectTypes,
          testCaseDetailId: selectedNode?.testCaseInfos?.testCaseDetailId,
          deleteTestCaseStepDetailId: deleteIdsRef.current,
          testCaseStepDetailModel: values?.cases.map((item: any) => {
            return {
              ...item,
              testCaseStepDetailId: item?.testCaseStepDetailId || 0,
              testCaseStepDetailProjectModuleId:
                selectedNode?.parentProjectModuleId, //parent id
              stepNumber: item?.sn[0]?.sn || item?.sn?.sn,
            };
          }),
        };

        const [response, error] = await updateModules(data);

        setLoading(false);
        if (response?.data) {
          handleReset();
          getProjectModules();
          message.success("Test Case updated successfully.", 3);
        }

        if (error) {
          message.error("Failed to update test case.", 3);
        }
      } else {
        const data = {
          projectModuleId: selectedNode?.key,
          projectSlug: route.params?.projectSlug,
          moduleName: values?.name,
          projectModuleType: selectedNode?.type,
          description: values?.description,
          parentProjectModuleId: selectedNode?.parentProjectModuleId
            ? selectedNode?.parentProjectModuleId
            : null,
        };

        const [response, error] = await updateModules(data);
        setLoading(false);
        if (response?.data) {
          handleReset();
          getProjectModules();
          message.success(`${selectedNode?.type} updated successfully.`, 3);
        }

        if (error) {
          message.error(error.response.data, 3);
        }
      }
    }
  };

  const getParentDropDownList = (
    list: FormattedModuleTreeInterface[]
  ): FormattedModuleTreeInterface[] => {
    return list.reduce((acc: FormattedModuleTreeInterface[], item: any) => {
      if (Array.isArray(item?.children) && item.children.length > 0) {
        const childrenList: any = getParentDropDownList(item.children);
        acc = acc.concat(item).concat(childrenList);
      } else {
        acc = acc.concat(item);
      }
      return acc;
    }, []);
  };
  const getParentDropDownFunctions = (list: FormattedModuleTreeInterface[]) => {
    return getParentDropDownList(list).filter(
      (item) => item.type === TestTypes.FUNCTION
    );
  };

  const getParentDropDownModules = (list: FormattedModuleTreeInterface[]) => {
    return getParentDropDownList(list).filter(
      (item) => item.type === TestTypes.MODULE
    );
  };

  const { Option } = Select;

  const getTestTypes = async () => {
    const [response, error] = await getDropdownLists("TestCaseType");
    if (response) {
      setTestCaseList(response.data);
    }
    if (error) {
      message.error("Failed to list Test Case Types.", 3);
    }
  };

  useEffect(() => {
    if (currentAction !== "create") {
      if (selectedNode.type === TestTypes.MODULE) {
        formInfo.setFieldsValue({
          name: selectedNode?.title,
          description: selectedNode?.description,
          parentProjectModuleId: selectedNode?.parentProjectModuleId,
        });
      } else if (selectedNode.type === TestTypes.FUNCTION) {
        formInfo.setFieldsValue({
          name: selectedNode?.title,
          description: selectedNode?.description,
          parentProjectModuleId: selectedNode?.parentProjectModuleId,
        });
      } else if (selectedNode.type === TestTypes.TESTCASES) {
        formInfo.setFieldsValue({
          name: selectedNode?.title,
          parentProjectModuleId: selectedNode?.parentProjectModuleId,
          description: selectedNode?.description,
          expectedResult: selectedNode?.testCaseInfos?.expectedResult,
          preCondition: selectedNode?.testCaseInfos?.preCondition,
          selectTypes: selectedNode?.testCaseInfos?.testCaseListItemId,
          cases: selectedNode?.testCaseInfos?.testCaseStepDetailModel
            .sort(function (a: any, b: any) {
              return a.testCaseStepDetailId - b.testCaseStepDetailId;
            })
            .map((item: any) => {
              return {
                ...item,
                sn: [
                  {
                    sn: item?.stepNumber,
                  },
                ],
                testCaseStepDetailProjectModuleId: selectedNode?.key,
              };
            }),
        });

        setTestCaseStepDetailIds(
          selectedNode?.testCaseInfos?.testCaseStepDetailModel?.map(
            (item: any) => item.testCaseStepDetailId
          )
        );
      }
    }

    if (currentAction && selectedNode.type === TestTypes.TESTCASES) {
      getTestTypes();
    }
  }, [selectedNode, currentAction, editData]);

  useEffect(() => {
    if (currentAction === "create") {
      formInfo.setFieldsValue({
        parentProjectModuleId: selectedNode?.key,
        cases: [],
      });
    }
  }, [selectedNode]);

  return (
    <Form
      className="mt-4 test-case-modal"
      form={formInfo}
      onFinish={handleSubmit}
      layout="vertical"
      name="form_in_modal"
    >
      <Form.Item
        name="name"
        label="Name"
        rules={[
          { required: true, message: "This field is required." },
          { whitespace: true, message: "This field cannot be empty." },
          {
            max: 99,
            message: "Name should be less than 100 characters.",
          },
        ]}
      >
        <Input className="mt-2" />
      </Form.Item>

      {/* {(selectedNode?.type === TestTypes.FUNCTION ||
        selectedNode?.type === TestTypes.MODULE) && (
        <Form.Item
          label="Parent Module"
          name="parentProjectModuleId"
          // rules={[{ required: true, message: 'Please input parent module!' }]}
        >
          <Select>
            {getParentDropDownModules(data).map((item: any, index: number) => (
              <Select.Option key={index} value={item.key}>
                {item.title}
              </Select.Option>
            ))}
          </Select>
        </Form.Item>
      )}

      {selectedNode?.type === TestTypes.TESTCASES && (
        <Form.Item
          label="Parent Function"
          name="parentProjectModuleId"
          // rules={[{ required: true, message: 'Please input parent module!' }]}
        >
          <Select>
            {getParentDropDownFunctions(data).map(
              (item: any, index: number) => (
                <Select.Option key={index} value={item.key}>
                  {item.title}
                </Select.Option>
              )
            )}
          </Select>
        </Form.Item>
      )} */}

      <Form.Item
        name="description"
        label={
          selectedNode?.type === TestTypes.TESTCASES
            ? "Scenario"
            : "Description"
        }
        rules={[
          {
            max: 499,
            message: `${
              selectedNode?.type === TestTypes.TESTCASES
                ? "Scenario"
                : "Description"
            } should be less than 500 characters.`,
          },
        ]}
      >
        <Input.TextArea className="mt-2" />
      </Form.Item>

      {selectedNode?.type === TestTypes.TESTCASES && (
        <>
          <Form.Item
            name="preCondition"
            label="Pre-condition"
            rules={[
              { required: true, message: "This field is required." },
              { whitespace: true, message: "This field cannot be empty." },
              {
                max: 499,
                message: "Pre-condition should be less than 500 characters.",
              },
            ]}
          >
            <Input.TextArea className="mt-2" />
          </Form.Item>

          <Form.Item
            name="expectedResult"
            label="Expected Result"
            rules={[
              { required: true, message: "This field is required." },
              { whitespace: true, message: "This field cannot be empty." },
            ]}
          >
            <Input.TextArea className="mt-2" />
          </Form.Item>

          <Form.Item
            name="selectTypes"
            label="Select Test Case Type"
            rules={[{ required: true, message: "This field is required." }]}
          >
            <Select value={testCaseList[0]?.listItemId}>
              {testCaseList.map((item: any, index: number) => {
                return (
                  <Option key={index} value={item?.listItemId}>
                    {item?.listItemName}
                  </Option>
                );
              })}
            </Select>
          </Form.Item>

          <Form.List name="cases">
            {(
              fields: FormListFieldData[],
              { add, remove }: any,
              { error }: any
            ) => (
              <>
                {fields.map(({ key, name, fieldKey, ...restField }, index) => {
                  return (
                    <Row key={key} className="mb-3">
                      <Col span={2} className="p-2 text-center pt-1">
                        <label>SN</label>
                        <Form.Item
                          {...restField}
                          name={[name, "sn"]}
                          fieldKey={[fieldKey, "sn"]}
                          initialValue={{
                            sn: index + 1,
                          }}
                          className="mb-0"
                        >
                          <Typography>{index + 1}</Typography>
                        </Form.Item>
                      </Col>

                      <Col span={10} className="px-2 text-center">
                        <label className="p-1">Step Description</label>
                        <Form.Item
                          {...restField}
                          name={[name, "stepDescription"]}
                          fieldKey={[fieldKey, "stepDescription"]}
                          className="mb-0"
                          rules={[
                            {
                              required: true,
                              message: "This field is required.",
                            },

                            {
                              whitespace: true,
                              message: "This field cannot be empty.",
                            },
                          ]}
                        >
                          <Input.TextArea />
                        </Form.Item>
                      </Col>
                      <Col span={10} className="px-2 text-center">
                        <label className="p-1">Expected Result</label>
                        <Form.Item
                          {...restField}
                          name={[name, "expectedResultTestStep"]}
                          fieldKey={[fieldKey, "expectedResultTestStep"]}
                          className="mb-0"
                          rules={[
                            {
                              required: true,
                              message: "This field is required.",
                            },

                            {
                              whitespace: true,
                              message: "This field cannot be empty.",
                            },
                          ]}
                        >
                          <Input.TextArea />
                        </Form.Item>
                      </Col>
                      <Col span={2} className="p-2 text-center">
                        <br />
                        <Form.Item className="mb-0 mt-2">
                          {index > 0 && (
                            <MinusCircleOutlined
                              onClick={() => {
                                let deleteFromAPI = false;
                                let deleteID = 0;

                                if (name < testCaseStepDetailIds.length) {
                                  deleteFromAPI = true;
                                  deleteID = testCaseStepDetailIds[name];
                                }

                                if (deleteFromAPI) {
                                  deleteIdsRef.current.push(deleteID);
                                  remove(name);
                                } else {
                                  remove(name);
                                }
                              }}
                            />
                          )}
                        </Form.Item>
                      </Col>
                    </Row>
                  );
                })}

                <Form.Item className="text-center">
                  <Button
                    type="dashed"
                    onClick={() => {
                      add();
                    }}
                  >
                    Add Steps
                  </Button>
                </Form.Item>
              </>
            )}
          </Form.List>
        </>
      )}

      <Form.Item className="mt-1">
        <Button
          loading={loading}
          type="primary"
          htmlType="submit"
          className="me-3"
        >
          {currentAction === "create" ? "Create" : "Save"}
        </Button>
        <Button
          htmlType="button"
          onClick={() => {
            handleReset();
          }}
        >
          Cancel
        </Button>
      </Form.Item>
    </Form>
  );
};
