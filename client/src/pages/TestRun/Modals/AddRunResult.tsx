import React, { useEffect, useState } from "react";
import {
  Input,
  message,
  Upload,
  Modal,
  TimePicker,
  Typography,
  Spin,
  Form,
} from "antd";
import moment from "moment";
import "moment-duration-format";
import { UploadOutlined } from "@ant-design/icons";
import { TestRunTestCase } from "../../../interfaces";
import {
  useUpdateTestCaseFromWizardMutation,
  useUpdateTestCaseStepFromWizardMutation,
} from "../../../store/services/main/test-run";

const statusWithId: Record<string, number> = {
  passed: 42,
  failed: 43,
  blocked: 44,
  pending: 45,
};

interface AddRunResultProps {
  isAddRunResultModalVisible: boolean;
  setIsAddRunResultModalVisible: (value: boolean) => void;
  triggerStatus: any;
  getTestRunTestCaseDetailData: TestRunTestCase | undefined;
  fetchTestRunHistory: (isRefetch?: boolean) => Promise<void>;
  setStatusChange: (value: boolean) => void;
  statusChange: boolean;
}

const AddRunResult: React.FC<AddRunResultProps> = (props): JSX.Element => {
  // console.log(props.triggerStatus);
  // States
  const [comment, setComment] = useState("");
  const [timeSpent, setTimeSpent] = useState(1800);
  const [files, setFiles] = useState(null);

  const [updateTestCaseFromWizard, { isLoading }] =
    useUpdateTestCaseFromWizardMutation();
  const [updateTestCaseStepFromWizard, { isLoading: isStepLoading }] =
    useUpdateTestCaseStepFromWizardMutation();

  const handleUpdateSuccess = () => {
    props.setStatusChange(true);
    props.fetchTestRunHistory(true);
    props.setIsAddRunResultModalVisible(false);
    props.triggerStatus.current = { status: "", step: null };
    setComment("");
    setFiles(null);
    props.setStatusChange(false);
  };
  const handleUpdateFailed = (msg = "") =>
    message.error(msg ? msg : "Unable to update.", 3);

  const handleOk = () => {
    if (!files && props.triggerStatus.current.status === "failed") {
      return message.error("Please upload an attachment.", 3);
    }

    const formData = new FormData();

    if (files !== null) {
      formData.append("files", files as any);
    }
    formData.append("comment", comment);
    formData.append("timeSpent", timeSpent.toString());
    formData.append(
      "testRunStatusListItemId",
      `${statusWithId[props.triggerStatus.current.status]}`
    );

    if (props.triggerStatus.current.step === null) {
      formData.append(
        "testRunTestCaseHistoryId",
        `${props.getTestRunTestCaseDetailData?.testRunTestCaseHistoryId}`
      );

      updateTestCaseFromWizard(formData)
        .unwrap()
        .then(handleUpdateSuccess)
        .catch((err) => {
          handleUpdateFailed(err?.data);
        });
    } else {
      formData.append(
        "testRunTestCaseStepHistoryId",
        `${props.triggerStatus.current.step}`
      );

      updateTestCaseStepFromWizard(formData)
        .unwrap()
        .then(handleUpdateSuccess)
        .catch((err) => {
          handleUpdateFailed(err?.data);
        });
    }
  };

  const handleCancel = () => {
    props.setIsAddRunResultModalVisible(false);
    props.triggerStatus.current = { status: "", step: null };
    setComment("");
    setFiles(null);
  };

  const onChange = (time: any, timeString: string) => {
    setTimeSpent(moment.duration(timeString).asSeconds());
  };

  const supportedFileTypes = [
    "pdf",
    "xls",
    "xlsx",
    "doc",
    "docx",
    "png",
    "jpg",
    "ppt",
    "pptx",
    "txt",
  ];
  const uploadProps = {
    multiple: false,
    onRemove() {
      setFiles(null);
    },
    beforeUpload: (file: any) => {
      const isLt25M = file.size / 1024 / 1024 < 25;
      const splitFileNames = file.name.split(".");
      const fileType = splitFileNames[splitFileNames.length - 1];

      if (!isLt25M) {
        message.error("File size must smaller than 25MB!", 3);
        return Upload.LIST_IGNORE;
      } else if (!supportedFileTypes.includes(fileType)) {
        message.error("File type is not supported!", 3);
        return Upload.LIST_IGNORE;
      } else {
        setFiles(file);
        return false;
      }
    },
    accept: "." + supportedFileTypes.join(",."),
  };

  // console.log(props.getTestRunTestCaseDetailData, "data here");

  const [form] = Form.useForm();
  useEffect(() => {
    form.setFieldsValue({
      comment: props.triggerStatus.current.comment
        ? props.triggerStatus.current.comment
        : null,
    });
  }, []);
  return (
    <>
      <Modal
        title="Add Run Result"
        visible={props.isAddRunResultModalVisible}
        onOk={handleOk}
        onCancel={handleCancel}
        className="add-run-result-modal"
        okText="Add Result"
      >
        <Form form={form} layout="vertical">
          <Typography.Paragraph>
            You can add a comment and attach artifacts to the result.
          </Typography.Paragraph>
          <Spin spinning={isLoading || isStepLoading}>
            <div>
              <Typography.Paragraph>Time spent</Typography.Paragraph>
              <TimePicker
                onChange={onChange}
                defaultValue={moment.utc(
                  moment.duration(1800, "seconds").asMilliseconds()
                )}
              />
            </div>
            <div style={{ marginTop: "20px" }}>
              <Form.Item label="Remark" name="comment">
                <Input.TextArea
                  placeholder="Add some additional information here"
                  rows={3}
                  value={comment}
                  onChange={(e) => setComment(e.target.value)}
                />
              </Form.Item>
            </div>
            <div>
              <Typography.Paragraph>
                {props.triggerStatus.current.status === "failed" && "*"}{" "}
                Attachments
              </Typography.Paragraph>
              <Upload.Dragger {...uploadProps}>
                <p className="ant-upload-drag-icon">
                  <UploadOutlined />
                </p>
                <p className="ant-upload-hint">
                  Click or drag file to this area to upload
                </p>
                <p className="ant-upload-hint">
                  Maximum upload file size: 25 MB
                </p>
              </Upload.Dragger>
            </div>
          </Spin>
        </Form>
      </Modal>
    </>
  );
};

export default AddRunResult;
