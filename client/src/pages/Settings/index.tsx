import React, { useEffect, useState } from "react";
import { Button, Divider, message, Modal, Space, Table, Input } from "antd";
import {
  DeleteOutlined,
  EditOutlined,
  PlusCircleOutlined,
} from "@ant-design/icons";
import AddEnvironmentModal from "./AddEnvironmentModal";
import {
  deleteEnvironment,
  getEnvironment,
} from "../../store/actions/environmentActionCreators";
import CustomModal from "../../components/customModal";
import EditEnvironmentModal from "./EditEnvironmentModal";
import { useRouteMatch } from "react-router";
interface Props {}

export const Settings: React.FC<Props> = (props) => {
  const route = useRouteMatch<{ projectSlug: string }>();
  const [isAddEnvironmentModalVisible, setIsAddEnvironmentModalVisible] =
    useState<boolean>(false);
  const [isEditEnvironmentModalVisible, setIsEditEnvironmentModalVisible] =
    useState(false);
  const [environmentList, setEnvironmentList] = useState([]);
  const [environmentListDetails, setEnvironmentListDetails] = useState(null);

  const [page, setPage] = useState(1);
  const [search, setSearch] = useState("");

  const [environmentId, setEnvironmentId] = useState<string | null>("");
  const [visible, setVisible] = useState<boolean>(false);
  const [confirmLoading, setConfirmLoading] = useState<boolean>(false);
  const [passData, setPassData] = useState({});
  const [tableLoading, setTableLoading] = useState(false);

  const getEnvironments = async () => {
    setTableLoading(true);

    const [response, error] = await getEnvironment(
      {
        page,
        search,
      },
      route.params.projectSlug
    );

    setTableLoading(false);

    if (error) {
      setEnvironmentList([]);
    }
    if (response) {
      setEnvironmentList(
        response.data.data.map((item: any, index: any) => {
          return {
            ...item,
            key: index + 1,
          };
        })
      );
      setEnvironmentListDetails(response.data);
    }
  };

  //Modal
  const showModal = () => {
    setIsAddEnvironmentModalVisible(true);
  };

  const handleOk = () => {
    setIsAddEnvironmentModalVisible(false);
  };

  const handleCancel = () => {
    setIsAddEnvironmentModalVisible(false);
  };

  const handleEditOk = () => {
    setIsEditEnvironmentModalVisible(false);
  };

  const handleEditCancel = () => {
    setIsEditEnvironmentModalVisible(false);
  };

  const onEditEnvironment = (data: any) => {
    setPassData({ ...data, projectSlug: route.params.projectSlug });
    setIsEditEnvironmentModalVisible(true);
  };

  //table
  const columns = [
    {
      title: "SN",
      dataIndex: "key",
      key: "key",
    },
    {
      title: "Environment",
      dataIndex: "environmentName",
      key: "environmentName",
    },
    {
      title: "URL",
      dataIndex: "url",
      key: "url",
    },
    {
      title: "Actions",
      key: "actions",
      render: (value: any) => (
        <Space size="middle">
          {/* {console.log(value, "value")} */}
          <EditOutlined onClick={() => onEditEnvironment(value)} />
          <DeleteOutlined
            onClick={() => {
              setEnvironmentId(value.environmentId);
              setVisible(true);
            }}
          />
        </Space>
      ),
    },
  ];

  useEffect(() => {
    if (page || search) {
      getEnvironments();
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [page, search]);

  return (
    <>
      <CustomModal
        visible={visible}
        title="Delete Environment !"
        modalContent="Are you sure want delete Environment ?"
        okText="Yes"
        cancelText="No"
        confirmLoading={confirmLoading}
        handleOk={async () => {
          setConfirmLoading(true);
          const [response, error] = await deleteEnvironment(environmentId);

          setConfirmLoading(false);
          if (response) {
            setVisible(false);
            message.success(response.data);
            getEnvironments();
          }

          if (error) {
            message.error(error.response.data);
          }
        }}
        handleCancel={() => {
          setEnvironmentId(null);
          setVisible(false);
        }}
      />

      <div>
        <div>
          <div className="test-run-home" style={{ display: "flex" }}>
            <Button
              type="primary"
              icon={<PlusCircleOutlined />}
              onClick={showModal}
            >
              Create Environment
            </Button>
            <Input
              placeholder="Search for Environment"
              onChange={(e) => setSearch(e.target.value)}
            />
          </div>

          <Modal
            title="Add Environment"
            visible={isAddEnvironmentModalVisible}
            onOk={handleOk}
            onCancel={handleCancel}
            footer={null}
            destroyOnClose={true}
          >
            <AddEnvironmentModal
              onOk={handleOk}
              getEnvironments={getEnvironments}
              projectSlug={route.params.projectSlug}
            />
          </Modal>

          <Modal
            title="Edit Environment"
            visible={isEditEnvironmentModalVisible}
            onOk={handleEditOk}
            onCancel={handleEditCancel}
            destroyOnClose={true}
            footer={false}
          >
            <EditEnvironmentModal
              data={passData}
              onOk={handleEditOk}
              getEnvironments={getEnvironments}
            />
          </Modal>
        </div>
        <Divider />
        <div className="mt-3">
          <Table
            dataSource={environmentList}
            columns={columns as any}
            loading={tableLoading}
          />
        </div>
      </div>
    </>
  );
};

export default Settings;
