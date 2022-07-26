import React, { useState } from "react";
import { Button, Input, Modal, Switch, Typography } from "antd";
import { CopyOutlined } from "@ant-design/icons";

interface ShareReportProps {
  isShareModalVisible: boolean;
  setIsShareModalVisible: (value: boolean) => void;
}

const ShareReport: React.FC<ShareReportProps> = (props): JSX.Element => {
  // States
  const [isChecked, setIsChecked] = useState(false);

  const toggleChecked = () => {
    setIsChecked((value) => !value);
  };

  const handleOk = () => {
    props.setIsShareModalVisible(false);
  };

  const handleCancel = () => {
    props.setIsShareModalVisible(false);
  };

  return (
    <>
      <Modal
        title="Share Report"
        visible={props.isShareModalVisible}
        onOk={handleOk}
        onCancel={handleCancel}
        className="share-modal"
      >
        <div>
          <div>
            <Switch checked={isChecked} onChange={toggleChecked} />
            <Typography.Text onClick={toggleChecked}>
              Public link is turned off
            </Typography.Text>
          </div>
          <Button
            icon={<CopyOutlined />}
            type="text"
            style={{ visibility: isChecked ? "visible" : "hidden" }}
          >
            Copy Link
          </Button>
        </div>
        <Input
          defaultValue="https://app.qase.io/public/report/c3ca73200b3ca"
          disabled
        />
      </Modal>
    </>
  );
};

export default ShareReport;
