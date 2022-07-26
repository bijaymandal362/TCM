import { Modal } from "antd";

type CustomModalProps = {
  title: any;
  modalContent: any;
  visible: boolean;
  confirmLoading: boolean;
  handleOk: any;
  handleCancel: any;
  okText: string;
  cancelText: string;
}

const CustomModal = ({
  title,
  modalContent,
  visible,
  confirmLoading,
  handleOk,
  handleCancel,
  okText,
  cancelText,
}: CustomModalProps) => {


  return (
    <Modal
      title={title}
      visible={visible}
      okText={okText}
      cancelText={cancelText}
      onOk={handleOk}
      confirmLoading={confirmLoading}
      onCancel={handleCancel}
    >
      {modalContent}
    </Modal>

  );
};

export default CustomModal;
