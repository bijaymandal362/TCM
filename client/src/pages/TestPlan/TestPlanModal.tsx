import React from "react";
import { Modal, Input, message, Form } from "antd";
import useUpdateEffect from "../../util/custom-hooks/useUpdateEffect";
import { IAddUpdateTestPlanParams } from "../../interfaces";
import { useRouteMatch } from "react-router";

export type ModalType = "ADD_FOLDER" | "EDIT_FOLDER";
interface TestPlanModalProps {
  type: ModalType;
  isModalVisible: boolean;
  toggleModalAndProvideData: (
    type?: ModalType,
    editFolderName?: string
  ) => void;
  editFolderNameData: string;
  parentFolderId: number | null;
  addFolderOrTestPlan: (params: IAddUpdateTestPlanParams) => any;
  isAddFolderOrTestPlanLoading: boolean;
  updateFolderOrTestPlan: (params: IAddUpdateTestPlanParams) => any;
  isUpdateFolderOrTestPlanLoading: boolean;
  setIsModalVisible: (value: boolean) => void;
  editTestPlanId: number | undefined;
  setEditFolderNameData: React.Dispatch<React.SetStateAction<string>>;
}

const TestPlanModal: React.FC<TestPlanModalProps> = (props) => {
  const route = useRouteMatch<{ projectSlug: string }>();
  const [form] = Form.useForm();

  const handleOk = () => {
    form
      .validateFields()
      .then((values) => {
        const getApiBody = (testPlanId: number) => ({
          parentTestPlanId: props.parentFolderId,
          testPlanId,
          title: values.folderName,
          projectSlug: route.params.projectSlug,
          testPlanTypeListItemId: 37,
          testPlanType: "Folder",
          testPlanName: values.folderName,
          testPlanTestCaseId: [],
        });

        if (props.type === "ADD_FOLDER") {
          props
            .addFolderOrTestPlan(getApiBody(0))
            .unwrap()
            .then((data: any) => {
              props.setIsModalVisible(false);
              form.resetFields();
              message.success(data);
            })
            .catch((err: any) => {
              message.error(err.data, 3);
            });
        } else if (props.type === "EDIT_FOLDER") {
          props
            .updateFolderOrTestPlan(getApiBody(props.editTestPlanId as number))
            .unwrap()
            .then((data: any) => {
              props.setIsModalVisible(false);
              form.resetFields();
              props.setEditFolderNameData("");
              message.success(data);
            })
            .catch((err: any) => message.error(err.data, 3));
        }
      })
      .catch(() => {});
  };

  useUpdateEffect(() => {
    form.setFieldsValue({
      folderName: props.editFolderNameData,
    });
  }, [props.editFolderNameData]);

  return (
    <Modal
      title={props.type?.replaceAll("_", " ")}
      visible={props.isModalVisible}
      onOk={handleOk}
      onCancel={() => {
        props.toggleModalAndProvideData();
        form.resetFields();
      }}
      confirmLoading={
        props.isAddFolderOrTestPlanLoading ||
        props.isUpdateFolderOrTestPlanLoading
      }
    >
      <Form form={form} layout="vertical" name="Test Plan Modal">
        <Form.Item
          name="folderName"
          label="Folder Name"
          rules={[
            {
              required: true,
              whitespace: true,
              message: "Folder Name is required.",
            },
            {
              max: 49,
              message: "Folder Name should be less than 50 characters.",
            },
          ]}
        >
          <Input />
        </Form.Item>
      </Form>
    </Modal>
  );
};

export default TestPlanModal;
