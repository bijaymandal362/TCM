import { Col, Modal, Row, Form, message } from "antd";
import React, { useEffect, useState } from "react";
import RequirementUpdate from "./components/RequirementUpdate";
import { CreateModuleForm } from "./components/CreateModuleForm";
import TreeLayout from "./components/TreeLayout";
import { EventDataNode } from "rc-tree/lib/interface";
import { TestTypes } from "../../enum/enum";
import {
  deleteModules,
  getFunctionDetails,
  getTestCaseDetails,
} from "../../store/actions/requirementManagementAction";
import { getDevelopers } from "../../store/actions/projectMembersActionCreators";
import { useRouteMatch } from "react-router";
import { useDebounce } from "use-debounce/lib";
import { useGetTestCasesQuery } from "../../store/services/main/test-case";

interface Props {
  match: {
    params: {
      projectSlug: any;
    };
  };
  isTestPlanPage: boolean;
}

const RequirementManagement: React.FC<Props> = (props: Props) => {
  const route = useRouteMatch<{ projectSlug: string }>();
  const [form] = Form.useForm();

  // States
  const [search, setSearch] = useState("");
  const [editData, setEditData] = useState(false);
  const [usersList, setUsersList] = useState<any>([]);
  const [requirementDetailLoading, setRequirementDetailLoading] =
    useState<boolean>(false);
  const [selectedNode, setSelectedNode] = useState<null | EventDataNode | any>(
    null
  );
  const [currentAction, setCurrentAction] = useState<
    null | "create" | "edit" | "delete"
  >(null);

  const [debouncedSearch] = useDebounce(search, 500);

  const {
    data: testCasesList,
    isLoading: isTestCasesListLoading,
    isError: isTestCasesListError,
    refetch: refetchTestCasesList,
  } = useGetTestCasesQuery(
    {
      projectSlug: route.params.projectSlug,
      searchValue: debouncedSearch,
    },
    {
      // refetchOnFocus: true,
      refetchOnReconnect: true,
      refetchOnMountOrArgChange: true,
    }
  );

  const handleNodeSelection = (selectedNode: EventDataNode) => {
    setCurrentAction(null);
    setSelectedNode((prevValue: any) => ({ ...prevValue, ...selectedNode }));
  };
  const openEditForm = (selectedNode: EventDataNode) => {
    setSelectedNode(null);
    setCurrentAction(null);
    setSelectedNode(selectedNode);
    setCurrentAction("edit");
  };
  const openDeleteForm = (selectedNode: EventDataNode) => {
    setSelectedNode(null);
    setSelectedNode(selectedNode);
    setCurrentAction("delete");
  };

  const addModule = (selectedNode?: EventDataNode) => {
    handleReset();

    if (!selectedNode) {
      setSelectedNode({
        type: TestTypes.MODULE,
      });
    } else {
      setSelectedNode({
        ...selectedNode,
      });
    }
    setCurrentAction("create");
  };

  const addFunction = (selectedNode?: EventDataNode) => {
    handleReset();
    setCurrentAction("create");

    if (!selectedNode) {
      setSelectedNode({
        type: TestTypes.FUNCTION,
      });
    } else {
      setSelectedNode({
        ...selectedNode,
        type: TestTypes.FUNCTION,
      });
    }
  };

  const addTestCase = (selectedNode?: EventDataNode) => {
    handleReset();

    if (!selectedNode) {
      setSelectedNode({
        type: TestTypes.TESTCASES,
      });
    } else {
      setSelectedNode({
        ...selectedNode,
        type: TestTypes.TESTCASES,
      });
    }
    setCurrentAction("create");
  };

  const closeRequirementDetail = () => {
    handleReset();
    setSelectedNode(null);
  };

  const handleReset = () => {
    form.resetFields();
    setCurrentAction(null);
    setSelectedNode(null);
  };
  const createFormTitle = (
    <>
      &#160;
      {selectedNode?.type === TestTypes.MODULE && currentAction && (
        <>{currentAction === "create" ? "Create Module" : "Update Module"}</>
      )}
      {selectedNode?.type === TestTypes.FUNCTION && currentAction && (
        <>
          {currentAction === "create" ? "Create Function" : "Update Function"}
        </>
      )}
      {selectedNode?.type === TestTypes.TESTCASES && currentAction && (
        <>
          {currentAction === "create" ? "Create Test Case" : "Update Test Case"}
        </>
      )}
    </>
  );

  const getUsers = async () => {
    setUsersList([]);

    const [response, error] = await getDevelopers({
      projectId: props.match.params.projectSlug,
    });
    if (response) {
      setUsersList(
        response.data.data.map((item: any, index: any) => {
          return {
            key: index + 1,

            label: item?.personName,
            value: item?.projectMemberId,
          };
        })
      );
    }

    if (error) {
      // message.error("Failed to display roles");
    }
  };

  useEffect(() => {
    getUsers();
  }, []);

  // useEffect(() => {
  //   document.body.style.overflow = "hidden";
  //   return () => {
  //     document.body.style.overflow = "";
  //   };
  // }, []);

  const getTestCaseInfo = async () => {
    setRequirementDetailLoading(true);
    const [response, error] = await getTestCaseDetails(selectedNode?.key);
    setRequirementDetailLoading(false);
    if (response) {
      setSelectedNode({
        ...selectedNode,
        testCaseInfos: response.data,
      });
    }
    if (error) {
    }
  };
  const getFunctionInfo = async () => {
    setRequirementDetailLoading(true);
    const [response, error] = await getFunctionDetails(selectedNode?.key);
    setRequirementDetailLoading(false);
    if (response) {
      setSelectedNode({
        ...selectedNode,
        defaultUsersList:
          response.data?.projectModuleDeveloperFunctionModel?.map(
            (item: any, index: number) => {
              return {
                label: item?.member,
                value: item?.projectModuleDeveloperId,
              };
            }
          ),
      });
    }
    if (error) {
      setSelectedNode({
        ...selectedNode,
        defaultUsersList: [],
      });
    }
  };
  useEffect(() => {
    if (
      selectedNode?.type === TestTypes.TESTCASES &&
      (currentAction === null || currentAction === "edit")
    ) {
      //call api to get details of test case
      getTestCaseInfo();
    }
  }, [selectedNode?.key, currentAction]);

  useEffect(() => {
    if (selectedNode?.type === TestTypes.FUNCTION) {
      //call api to get details of function module
      getFunctionInfo();
    }
  }, [selectedNode?.key]);

  if (props.isTestPlanPage) {
    return (
      <TreeLayout
        handleNodeSelection={handleNodeSelection}
        openEditForm={openEditForm}
        openDeleteForm={openDeleteForm}
        addModule={addModule}
        addFunction={addFunction}
        addTestCase={addTestCase}
        getProjectModules={refetchTestCasesList}
        data={testCasesList}
        selectedNode={selectedNode}
        treeLoading={isTestCasesListLoading}
        slug={props.match.params.projectSlug}
        isTestPlanPage={props.isTestPlanPage}
        setSearch={setSearch}
        setEditData={setEditData}
      />
    );
  }

  return (
    <Row>
      <Modal
        visible={currentAction === "delete"}
        title="Delete !"
        okText="Yes"
        cancelText="No"
        onCancel={() => {
          handleReset();
        }}
        onOk={async () => {
          const [response, error] = await deleteModules(selectedNode.key);
          if (response) {
            message.success(response.data);
            refetchTestCasesList();
            setSelectedNode(null);
            form.resetFields();
          }
          if (error) {
            message.error(error?.response?.data || "Failed to delete.");
          }
          setCurrentAction(null);
        }}
        confirmLoading={false}
      >
        <span>Are you sure you want to delete?</span>
      </Modal>

      <Modal
        visible={currentAction === "edit" || currentAction === "create"}
        title={createFormTitle}
        onCancel={() => {
          handleReset();
        }}
        footer={null}
        confirmLoading={false}
        width={800}
        className="test-case-repository-modal"
      >
        {selectedNode && (
          <CreateModuleForm
            formInfo={form}
            handleReset={handleReset}
            getProjectModules={refetchTestCasesList}
            currentAction={currentAction}
            selectedNode={selectedNode}
            data={testCasesList}
            editData={editData}
          />
        )}
      </Modal>

      <Col
        span={selectedNode && selectedNode.type !== "module" ? 18 : 24}
        className="border-left border-success p-4"
      >
        <TreeLayout
          handleNodeSelection={handleNodeSelection}
          openEditForm={openEditForm}
          openDeleteForm={openDeleteForm}
          addModule={addModule}
          addFunction={addFunction}
          addTestCase={addTestCase}
          getProjectModules={refetchTestCasesList}
          data={testCasesList}
          selectedNode={selectedNode}
          treeLoading={isTestCasesListLoading}
          slug={props.match.params.projectSlug}
          isTestPlanPage={props.isTestPlanPage}
          setSearch={setSearch}
          setEditData={setEditData}
          notFound={isTestCasesListError}
        />
      </Col>
      {selectedNode && selectedNode.type !== "module" && (
        <Col
          span={6}
          style={{
            borderLeft: "1px solid #6c757d",
            padding: "1.5rem",
            paddingRight: "0.5rem",
          }}
        >
          <RequirementUpdate
            data={testCasesList}
            selectedNode={selectedNode}
            openEditForm={openEditForm}
            openDeleteForm={openDeleteForm}
            usersList={usersList}
            handleNodeSelection={handleNodeSelection}
            requirementDetailLoading={requirementDetailLoading}
            closeRequirementDetail={() => {
              closeRequirementDetail();
            }}
            getFunctionInfo={getFunctionInfo}
          />
        </Col>
      )}
    </Row>
  );
};

export default RequirementManagement;

RequirementManagement.defaultProps = {
  isTestPlanPage: false,
};
