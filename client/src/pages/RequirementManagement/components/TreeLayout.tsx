import React, { useState } from "react";
import {
  DeleteOutlined,
  DownloadOutlined,
  EditOutlined,
  FileAddFilled,
  PlusCircleOutlined,
  PropertySafetyFilled,
} from "@ant-design/icons";
import {
  Button,
  Divider,
  Input,
  Menu,
  Tree,
  Typography,
  Dropdown,
  Row,
  Col,
  message,
  Upload,
  Spin,
  Empty,
  Modal,
  Checkbox,
} from "antd";
import { EventDataNode } from "rc-tree/lib/interface";
import { TestTypes } from "../../../enum/enum";
import {
  downloadAllTestCases,
  downloadTestCases,
  dragAndDropTree,
  uploadTestCases,
} from "../../../store/actions/requirementManagementAction";
import { getToken } from "../../../util/auth.util";
import { formatTreeRequest, getTestCasesArr } from "../../../util/tree.util";
import CheckProjectPermission from "../../../hoc/checkProjectPermission";
import { useAppDispatch } from "../../../store/reduxHooks";
import {
  addNewTestCasesForTestPlan,
  addSelectedTestCases,
} from "../../../store/features/testPlanSlice";
import { ITestPlanLocalTestCase } from "../../../interfaces";

interface TreeLayoutProps {
  handleNodeSelection: (selectedNode: EventDataNode) => void;
  openEditForm: (selectedNode: EventDataNode) => void;
  openDeleteForm: (selectedNode: EventDataNode) => void;
  addModule: (selectedNode?: EventDataNode) => void;
  addFunction: (selectedNode?: EventDataNode) => void;
  addTestCase: (selectedNode?: EventDataNode) => void;
  getProjectModules: () => void;
  treeLoading: boolean;
  selectedNode: any;
  data: any;
  slug: string | null;
  isTestPlanPage: boolean;
  setSearch: (value: string) => void;
  setEditData: (value: React.SetStateAction<boolean>) => void;
  notFound?: boolean;
}

const TreeLayout = ({
  handleNodeSelection,
  openDeleteForm,
  openEditForm,
  addModule,
  addFunction,
  addTestCase,
  getProjectModules,
  data: gData,
  selectedNode,
  slug,
  treeLoading,
  isTestPlanPage,
  setSearch,
  setEditData,
  notFound,
}: TreeLayoutProps) => {
  const dispatch = useAppDispatch();

  // States
  const [isFileUploading, setIsFileUploading] = useState(false);
  const [showErrorModal, setShowErrorModal] = useState(false);
  const [errors, setErrors] = useState<string[]>([]);

  const fileProps: any = {
    name: "File",
    method: "POST",
    action: `${process.env.REACT_APP_BASE_URL}/ProjectModule/ImportProjectModuleTestCase`,
    headers: {
      Authorization: `Bearer ${getToken()}`,
    },
    data: {
      projectModuleId: selectedNode?.key,
      projectSlug: slug,
      projectModuleType: TestTypes.FUNCTION,
    },
    onChange(info: any) {
      // console.log(info);
      if (info.file.status === "uploading") {
        setIsFileUploading(true);
      }
      if (info.file.status === "done") {
        getProjectModules();
        message.success(info.file.response);
        setIsFileUploading(false);
      } else if (info.file.status === "error") {
        setErrors(info.file.response);
        setShowErrorModal(true);

        setIsFileUploading(false);
      }
    },
    onRemove: (file: any) => {
      // console.log(file);
      // deleteImportTestCase(1)
      return true;
    },
    showUploadList: false,
    accept: ".xlsx",
  };

  const renderTitle = (item: any) => {
    // console.log(item,'item here')
    const menu = (
      <Menu
        onClick={(e: any) => {
          e.domEvent.stopPropagation();
          if (e.key == "1") {
            addModule(item);
          }
          if (e.key == "2") {
            addFunction(item);
          }

          if (e.key == "3") {
            addTestCase(item);
          }
        }}
      >
        {item.type !== TestTypes.FUNCTION && item.type !== TestTypes.TESTCASES && (
          <Menu.Item key="1">
            <Typography>Create Module</Typography>
          </Menu.Item>
        )}
        {item.type === TestTypes.MODULE && (
          <Menu.Item key="2">
            <Typography>Create Function</Typography>
          </Menu.Item>
        )}
        {item.type === TestTypes.FUNCTION && (
          <Menu.Item key="3">
            <Typography>Create Test Case</Typography>
          </Menu.Item>
        )}
      </Menu>
    );

    return (
      <div className="tree__title">
        <Typography.Text
          style={{
            width: item.title.length > 50 ? 500 : "auto",
            fontSize: 16,
          }}
          ellipsis
          className="ms-2 me-4"
        >
          {item.title}
        </Typography.Text>
        <div className="align-items-center d-flex tree__icons">
          {item.type !== TestTypes.TESTCASES && (
            <CheckProjectPermission slug="projectrole.projectmodule.create">
              <Dropdown overlay={menu} trigger={["click"]}>
                <Button
                  onClick={(e: React.SyntheticEvent) => {
                    e?.stopPropagation();
                  }}
                >
                  <PlusCircleOutlined />
                </Button>
              </Dropdown>
            </CheckProjectPermission>
          )}

          {item.type === TestTypes.FUNCTION && (
            <>
              <Upload {...fileProps}>
                <Button
                  className="mx-2"
                  icon={<FileAddFilled />}
                  onClick={() => {
                    handleNodeSelection(item);
                  }}
                ></Button>
              </Upload>

              <Button
                className="mx-2"
                icon={<DownloadOutlined />}
                onClick={async (e) => {
                  e?.stopPropagation();
                  const [response, error] = await downloadTestCases(item?.key);
                  if (response) {
                    const url = window.URL.createObjectURL(
                      new Blob([response?.data])
                    );
                    const link = document.createElement("a");
                    link.href = url;
                    link.setAttribute("download", "TestCaseDetails.xlsx");
                    document.body.appendChild(link);
                    console.log(link);
                    link.click();
                  }
                  if (error) {
                    message.error("Failed to download Test Cases.", 3);
                  }
                }}
              ></Button>
            </>
          )}

          <CheckProjectPermission slug="projectrole.projectmodule.update">
            <Button
              className="mx-2"
              icon={<EditOutlined />}
              onClick={(e) => {
                e?.stopPropagation();
                setEditData((prevValue) => !prevValue);
                openEditForm(item);
              }}
            ></Button>
          </CheckProjectPermission>
          <CheckProjectPermission slug="projectrole.projectmodule.delete">
            <Button
              className="mx-2"
              icon={<DeleteOutlined />}
              onClick={(e) => {
                e?.stopPropagation();
                openDeleteForm(item);
              }}
            ></Button>
          </CheckProjectPermission>
        </div>
      </div>
    );
  };

  const renderTitleForTestPlanPage = (node: any) => {
    // console.log("node", node);

    return (
      <>
        <Typography.Text
          style={{
            width: isTestPlanPage && node.title?.length > 20 ? 150 : "auto",
          }}
          ellipsis
        >
          {node.title}
        </Typography.Text>
        {node.type !== "TestCase" && node.children && (
          <div className="action-triggers">
            <PlusCircleOutlined
              className="action-button"
              onClick={(e) => {
                e.stopPropagation();
                dispatch(addNewTestCasesForTestPlan(getTestCasesArr(node)));
              }}
            />
          </div>
        )}
      </>
    );
  };

  const onDragEnter = (info: any) => {
    // expandedKeys 需要受控时设置
    // this.setState({
    //   expandedKeys: info.expandedKeys,
    // });
  };

  const onDrop = async (info: any) => {
    // console.log(info);
    let newData: any = {
      projectModuleId: info?.dragNode?.key,
      projectSlug: slug,
      moduleName: info?.dragNode?.title,
      projectModuleType: info?.dragNode?.type,
      description: info?.dragNode?.description,
      dragDropParentProjectModuleId: info?.node?.key,
    };

    //test case

    if (
      info?.dragNode?.type === TestTypes.TESTCASES &&
      info?.node?.type === TestTypes.TESTCASES
    ) {
      if (info.dropToGap) {
        newData.dragDropParentProjectModuleId =
          info?.node?.parentProjectModuleId;
      } else {
        // newData.dragDropParentProjectModuleId=info?.node?.key
        return message.error("test case can not have child");
      }
      // return message.error(dragDropError);
    }

    if (
      info?.dragNode?.type === TestTypes.TESTCASES &&
      info?.node?.type === TestTypes.MODULE
    ) {
      return message.error("can not move test casse outside function");
    }

    //function
    if (
      info?.dragNode?.type === TestTypes.FUNCTION &&
      info?.node?.type === TestTypes.FUNCTION
    ) {
      if (info.dropToGap) {
        newData.dragDropParentProjectModuleId =
          info?.node?.parentProjectModuleId;
      } else {
        // newData.dragDropParentProjectModuleId=info?.node?.key
        return message.error("you can not add function inside function");
      }
    }

    if (
      info?.dragNode?.type === TestTypes.FUNCTION &&
      info?.node?.type === TestTypes.TESTCASES
    ) {
      return message.error("test case can not have child");
    }

    if (
      info?.dragNode?.type === TestTypes.FUNCTION &&
      info?.node?.type === TestTypes.MODULE
    ) {
      if (info?.dropToGap) {
        newData["dragDropParentProjectModuleId"] =
          info?.node?.parentProjectModuleId;
      } else {
        newData["dragDropParentProjectModuleId"] = info?.node?.key;
      }
    }

    //  =============MODULE CONDITION =============

    if (
      info?.dragNode?.type === TestTypes.MODULE &&
      info?.node?.type === TestTypes.TESTCASES
    ) {
      return message.error("test case can not have child");
    }

    if (
      info?.dragNode?.type === TestTypes.MODULE &&
      info?.node?.type === TestTypes.MODULE
    ) {
      if (info?.dropToGap) {
        newData.dragDropParentProjectModuleId =
          info?.node?.parentProjectMoudleId;
      } else {
        newData.dragDropParentProjectModuleId = info?.node?.key;
      }
    }

    if (
      info?.dragNode?.type === TestTypes.MODULE &&
      info?.node?.type === TestTypes.FUNCTION
    ) {
      if (info?.dropToGap) {
        newData["dragDropParentProjectModuleId"] =
          info?.node?.parentProjectModuleId;
      } else {
        return message.error("module can not be child of function");
      }
    }

    if (info?.dragNode?.type === TestTypes.FUNCTION) {
      if (
        info?.dragNode?.pos?.split("-").length >= 3 &&
        info?.node?.parentProjectModuleId === null &&
        info?.dropToGap
      ) {
        return message.error("function should be inside module");
      }
    }

    if (info?.dragNode?.type === TestTypes.TESTCASES) {
      if (
        info?.dragNode?.pos?.split("-").length >= 4 &&
        info?.dragNode?.dragOverGapBottom
      ) {
        return message.error("error");
      }
    }

    // if(info?.node?.pos?.split('-').map(Number).every((e:Number) => e === 0)){
    //     newData["dragDropParentProjectModuleId"]= null;
    // }

    // if(info?.node?.pos?.split('-').map(Number).every((e:Number) => e === 0) && (info?.dragNode?.type===TestTypes.FUNCTION || info?.dragNode?.type===TestTypes.TESTCASES) ){
    //     if(info?.dropToGap){
    //         if(info?.dragNode?.type===TestTypes.FUNCTION && info?.node?.type===TestTypes.FUNCTION){
    //             newData["dragDropParentProjectModuleId"]= info?.node?.parentProjectModuleId;

    //         }else{
    //             return
    //         }
    //     }else{
    //         return;
    //     }
    // }

    const dropKey = info.node.key;
    const dragKey = info.dragNode.key;
    const dropPos = info.node.pos.split("-");
    const dropPosition =
      info.dropPosition - Number(dropPos[dropPos.length - 1]);

    const loop = (data: any, key: any, callback: any) => {
      for (let i = 0; i < data.length; i++) {
        if (data[i].key === key) {
          return callback(data[i], i, data);
        }
        if (data[i].children) {
          loop(data[i].children, key, callback);
        }
      }
    };
    const data = JSON.parse(JSON.stringify(gData));

    // Find dragObject
    let dragObj: any;
    loop(data, dragKey, (item: any, index: any, arr: any) => {
      arr.splice(index, 1);
      dragObj = item;
    });

    if (!info.dropToGap) {
      // Drop on the content
      loop(data, dropKey, (item: any) => {
        item.children = item.children || [];
        // where to insert 示例添加到头部，可以是随意位置
        item.children.unshift(dragObj);
      });
    } else if (
      (info.node.props.children || []).length > 0 && // Has children
      info.node.props.expanded && // Is expanded
      dropPosition === 1 // On the bottom gap
    ) {
      loop(data, dropKey, (item: any) => {
        item.children = item.children || [];
        // where to insert 示例添加到头部，可以是随意位置
        item.children.unshift(dragObj);
        // in previous version, we use item.children.push(dragObj) to insert the
        // item to the tail of the children
      });
    } else {
      let ar: any;
      let i: any;
      loop(data, dropKey, (item: any, index: any, arr: any) => {
        ar = arr;
        i = index;
      });
      if (dropPosition === -1) {
        ar.splice(i, 0, dragObj);
      } else {
        ar.splice(i + 1, 0, dragObj);
      }
    }

    newData.dragDropProjectModuleModel = formatTreeRequest(data);

    const [response, error] = await dragAndDropTree(newData);

    getProjectModules();

    if (error) {
      message.error(error.response.data);
      // setDragDropError(error.response.data);
    }
  };

  const handleSearch: React.ChangeEventHandler<HTMLInputElement> = (e) => {
    setSearch(e.target?.value);
  };

  const { DirectoryTree } = Tree;

  const [selectedKeys, setSelectedKeys] = useState<React.Key[]>([]);
  const [dragDropError, setDragDropError] = useState<string>("");

  const onSelect = (keys: React.Key[], info: any) => {
    // console.log("Trigger Select", keys, info);

    const filteredKeys: ITestPlanLocalTestCase[] = info.selectedNodes
      .map((item: any) => {
        if (item.type === "TestCase") {
          // console.log(item);
          return {
            key: Math.floor(Math.random() * 1000000000),
            id: item.key,
            testCaseName: item.title,
            scenario: item.description,
            expectedResult: item.expectedResult,
            action: `${item.key}`,
          };
        }
      })
      .filter((key: number | undefined) => key !== undefined);
    setSelectedKeys(keys);
    dispatch(addSelectedTestCases(filteredKeys));
  };

  if (isTestPlanPage) {
    return (
      <CheckProjectPermission slug="projectrole.projectmodule.read">
        {treeLoading ? (
          <Spin tip="Loading..." />
        ) : (
          <DirectoryTree
            titleRender={(node) => renderTitleForTestPlanPage(node)}
            multiple
            selectedKeys={selectedKeys}
            blockNode
            className="custom__tree"
            onDragEnter={onDragEnter}
            onDrop={onDrop}
            onSelect={onSelect}
            treeData={gData}
            draggable
          />
        )}
      </CheckProjectPermission>
    );
  }

  const cancelHandler = () => {
    setShowErrorModal(false);
  };

  return (
    <div>
      <Row className="tree__header">
        <CheckProjectPermission slug="projectrole.projectmodule.create">
          <Col>
            <Button
              type="primary"
              icon={<PlusCircleOutlined />}
              className="me-2"
              onClick={() => {
                addModule();
              }}
            >
              Create Module
            </Button>
            {/* <Button type="primary" icon={<PlusCircleOutlined />} >Create Function</Button> */}
          </Col>
        </CheckProjectPermission>
        <Col>
          <Input
            type="search"
            placeholder="search for cases"
            className="mx-2"
            onChange={handleSearch}
          />
        </Col>
        <Col
          style={{
            marginLeft: 38,
          }}
        >
          <Button
            type="primary"
            onClick={async () => {
              const [response, error] = await downloadAllTestCases(slug);
              if (response) {
                const url = window.URL.createObjectURL(
                  new Blob([response?.data])
                );
                const link = document.createElement("a");
                link.href = url;
                link.setAttribute("download", "TestCase.xlsx");
                document.body.appendChild(link);
                console.log(link);
                link.click();
              }

              if (error) {
                if (error.response.status === 500) {
                  message.error("No Test Cases available for download", 3);
                } else {
                  message.error("Failed to download Test Cases", 3);
                }
              }
            }}
          >
            Download Test Cases
          </Button>
        </Col>
      </Row>
      <Divider />
      <div>
        <CheckProjectPermission slug="projectrole.projectmodule.read">
          {treeLoading ? (
            <Spin
              style={{
                display: "flex",
                justifyContent: "center",
                marginTop: "120px",
              }}
            />
          ) : notFound ? (
            <Empty
              description="No Test Cases Found"
              image={Empty.PRESENTED_IMAGE_SIMPLE}
              style={{
                marginTop: "100px",
              }}
            />
          ) : (
            <Spin spinning={isFileUploading}>
              <DirectoryTree
                height={550}
                titleRender={(node) => renderTitle(node)}
                blockNode
                className="custom__tree"
                onDragEnter={onDragEnter}
                onDrop={onDrop}
                onSelect={(_key, info) => handleNodeSelection(info.node)}
                treeData={gData}
                draggable
              />
            </Spin>
          )}
        </CheckProjectPermission>
      </div>
      <Modal
        title="errors !"
        visible={showErrorModal}
        onCancel={cancelHandler}
        footer={[
          <Button key="ok" type="primary" onClick={cancelHandler}>
            ok
          </Button>,
        ]}
      >
        <p>{errors}</p>
      </Modal>
    </div>
  );
};

export default TreeLayout;
