import {
  DeleteOutlined,
  EditOutlined,
  PlusCircleOutlined,
} from "@ant-design/icons";
import {
  Tree,
  Menu,
  Typography,
  Dropdown,
  Button,
  Spin,
  Empty,
  Popconfirm,
  message,
  Input,
} from "antd";
import { useState } from "react";
import { useDebounce } from "use-debounce/lib";
import { IFolderAndTestPlan } from "../../interfaces";
import {
  useDeleteFolderOrTestPlanMutation,
  useDragAndDropTestPlanMutation,
} from "../../store/services/main/test-plan";
import {
  formatTestPlanTreeStructure,
  formatTestPlanTreeStructureForDrapDropApi,
} from "../../util/tree.util";
import { ModalType } from "./TestPlanModal";

interface TreeStructureProps {
  setIsTestPlanSelected: (value: boolean) => void;
  toggleModalAndProvideData: (
    type?: ModalType,
    editFolderName?: string
  ) => void;
  toggleAdding: () => void;
  toggleEditing: () => void;
  isGetFolderAndTestPlansLoading: boolean;
  setEditTestPlanId: (id: number) => void;
  getFolderAndTestPlansData: undefined | { data: IFolderAndTestPlan[] };
  setParentFolderId: (parentId: number | null) => void;
  setSelectedTestPlanData: any;
  isGetFolderAndTestPlansError: boolean;
  projectSlug: string;
  setSearch: (value: string) => void;
}

const TreeStructure: React.FC<TreeStructureProps> = (props) => {
  const [deleteFolderOrTestPlan] = useDeleteFolderOrTestPlanMutation();
  const [dragAndDropTestPlan] = useDragAndDropTestPlanMutation();

  const handleConfirmDelete = (e: any, node: any) => {
    e.stopPropagation();
    deleteFolderOrTestPlan(node.key)
      .unwrap()
      .then((data) => {
        message.success(data, 3);
        props.setIsTestPlanSelected(false);
        props.setSelectedTestPlanData({});
      })
      .catch((err) => {
        message.error(err.data, 3);
      });
  };
  // console.log(props.getFolderAndTestPlansData, "test plan");
  const renderTitle = (node: any) => {
    // console.log("node", node);
    const menu = (
      <Menu
        onClick={(e: any) => {
          // console.log("e neeles", e);
          e.domEvent.stopPropagation();
          props.setParentFolderId(+node.key);

          if (e.key === "1") {
            props.toggleModalAndProvideData();
          }
          if (e.key === "2") {
            props.toggleAdding();
          }
        }}
      >
        <Menu.Item key="1">
          <Typography>Create New Folder</Typography>
        </Menu.Item>
        <Menu.Item key="2">
          <Typography>Create Test Plan</Typography>
        </Menu.Item>
      </Menu>
    );

    return (
      <>
        <Typography.Text
          ellipsis
          style={{
            width: node.title?.length > 30 ? 170 : "auto",
          }}
        >
          {node.title}
        </Typography.Text>
        <div className="action-triggers">
          {!node.isLeaf && (
            <Dropdown overlay={menu} trigger={["click"]}>
              <PlusCircleOutlined
                className="action-button"
                onClick={(e) => e.stopPropagation()}
              />
            </Dropdown>
          )}
          <EditOutlined
            className="action-button"
            onClick={(e) => {
              e.stopPropagation();
              props.setParentFolderId(node.parentKey);
              props.setEditTestPlanId(+node.key);

              if (!node.isLeaf) {
                props.toggleModalAndProvideData("EDIT_FOLDER", node.title);
              } else {
                props.toggleEditing();
                props.setIsTestPlanSelected(true);
                props.setSelectedTestPlanData(node);
              }
            }}
          />
          <Popconfirm
            title="Are you sure to delete?"
            onConfirm={(e) => handleConfirmDelete(e, node)}
            okText="Yes"
            cancelText="No"
          >
            <DeleteOutlined
              className="action-button"
              onClick={(e) => {
                e.stopPropagation();
              }}
            />
          </Popconfirm>
        </div>
      </>
    );
  };

  const getOrderedData = (info: any) => {
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

    const data = formatTestPlanTreeStructure(
      props.getFolderAndTestPlansData?.data as IFolderAndTestPlan[]
    );

    // Find dragObject
    let dragObj = {};
    loop(data, dragKey, (item: any, index: any, arr: any[]) => {
      arr.splice(index, 1);
      dragObj = item;
    });

    if (!info.dropToGap) {
      // Drop on the content
      loop(data, dropKey, (item: any) => {
        item.children = item.children || [];
        // where to insert
        item.children.unshift(dragObj);
      });
    } else if (
      (info.node.props.children || []).length > 0 && // Has children
      info.node.props.expanded && // Is expanded
      dropPosition === 1 // On the bottom gap
    ) {
      loop(data, dropKey, (item: any) => {
        item.children = item.children || [];
        // where to insert
        item.children.unshift(dragObj);
        // in previous version, we use item.children.push(dragObj) to insert the
        // item to the tail of the children
      });
    } else {
      let ar: any[] = [];
      let i = 0;
      loop(data, dropKey, (item: any, index: any, arr: any[]) => {
        ar = arr;
        i = index;
      });
      if (dropPosition === -1) {
        ar.splice(i, 0, dragObj);
      } else {
        ar.splice(i + 1, 0, dragObj);
      }
    }

    return data;
  };

  const onDrop = (info: any) => {
    // console.log("drOpInfo", info);

    // Condition 1: Nothing can be drop inside leaf
    if (info.node.isLeaf && !info.dropToGap) {
      return message.error("Test Plan can't have any child.", 2);
    }

    // Condition 2: Leaf can't be dropped at the root (i.e. Leaf should be only inside folder)
    if (info.dragNode.isLeaf && !info.node.parentKey && info.dropToGap) {
      return message.error("Test Plan can't be moved there.", 3);
    }

    dragAndDropTestPlan({
      testPlanId: +info.dragNode.key,
      parentTestPlanId: info.dragNode.parentKey,
      dragDropTestPlanId:
        !info.node.parentKey && info.dropToGap
          ? null
          : !info.dropToGap
          ? +info.node.key
          : info.node.parentKey,
      projectSlug: props.projectSlug,
      testPlanTypeListItemId: !info.dragNode.isLeaf ? 37 : 38,
      dragDropOrderingView: formatTestPlanTreeStructureForDrapDropApi(
        getOrderedData(info)
      ),
      searchValue: "",
    })
      .unwrap()
      .catch(() => message.error("Failed while moving it there.", 3));
  };

  const onSelect = (keys: React.Key[], info: any) => {
    // console.log("Trigger Select", keys, info);
    if (info.node.isLeaf) {
      props.setIsTestPlanSelected(true);
      props.setSelectedTestPlanData(info.selectedNodes[0]);
    } else {
      props.setIsTestPlanSelected(false);
      props.setSelectedTestPlanData({});
    }
  };

  const testPlanSearchHandler: React.ChangeEventHandler<HTMLInputElement> = (
    event
  ) => {
    props.setSearch(event.target.value);
  };

  return (
    <>
      <Button
        type="primary"
        icon={<PlusCircleOutlined />}
        onClick={() => {
          props.setParentFolderId(null);
          props.toggleModalAndProvideData();
        }}
      >
        Create Folder
      </Button>
      <Input
        type="search"
        placeholder="search for test plans"
        style={{ width: "96%" }}
        onChange={testPlanSearchHandler}
      />
      {props.isGetFolderAndTestPlansLoading ? (
        <div>
          <Spin className="spin" />
        </div>
      ) : props.getFolderAndTestPlansData?.data?.length &&
        !props.isGetFolderAndTestPlansError ? (
        <Tree.DirectoryTree
          titleRender={(node) => renderTitle(node)}
          draggable
          blockNode
          onDrop={onDrop}
          onSelect={onSelect}
          treeData={formatTestPlanTreeStructure(
            props.getFolderAndTestPlansData?.data
          )}
        />
      ) : (
        <Empty image={Empty.PRESENTED_IMAGE_SIMPLE} />
      )}
    </>
  );
};

export default TreeStructure;
