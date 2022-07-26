import React, { useState } from "react";
import { Button, Col, Row, Typography } from "antd";
import TreeStructure from "./TreeStructure";
import MainLayout from "./Layout/MainLayout";
import AddEditLayout from "./Layout/AddEditLayout";
import TestPlanModal, { ModalType } from "./TestPlanModal";
import RequirementManagement from "../RequirementManagement";
import {
  useGetFolderAndTestPlansQuery,
  useAddFolderOrTestPlanMutation,
  useUpdateFolderOrTestPlanMutation,
} from "../../store/services/main/test-plan";
import { useRouteMatch } from "react-router";
import { useAppDispatch, useAppSelector } from "../../store/reduxHooks";
import { moveSelectedTestCases } from "../../store/features/testPlanSlice";
import { useDebounce } from "use-debounce/lib";

interface TestPlanProps {
  match: {
    params: {
      projectSlug: any;
    };
  };
}

const TestPlan: React.FC<TestPlanProps> = (props) => {
  const route = useRouteMatch<{ projectSlug: string }>();
  const dispatch = useAppDispatch();
  const selectedTestCases = useAppSelector(
    (state) => state.testPlan.selectedTestCases
  );

  // States
  const [isAdding, setIsAdding] = useState<boolean>(false);
  const [isEditing, setIsEditing] = useState<boolean>(false);
  const [isTestPlanSelected, setIsTestPlanSelected] = useState<boolean>(false);
  const [selectedTestPlanData, setSelectedTestPlanData] = useState({});
  const [isModalVisible, setIsModalVisible] = useState(false);
  const [modalType, setModalType] = useState<ModalType>("ADD_FOLDER");
  const [editFolderNameData, setEditFolderNameData] = useState<string>("");
  const [parentFolderId, setParentFolderId] = useState<number | null>(null);
  const [editTestPlanId, setEditTestPlanId] = useState<number | undefined>(
    undefined
  );
  const [search, setSearch] = useState<string>("");
  const [debouncedSearch] = useDebounce(search, 500);

  const {
    data: getFolderAndTestPlansData,
    isLoading: isGetFolderAndTestPlansLoading,
    isError: isGetFolderAndTestPlansError,
  } = useGetFolderAndTestPlansQuery(
    { projectSlug: route.params.projectSlug, searchValue: debouncedSearch },
    {
      // refetchOnFocus: true,
      skip: isAdding || isEditing,
      refetchOnMountOrArgChange: true,
    }
  );
  // console.log(getFolderAndTestPlansData, "here");

  const [addFolderOrTestPlan, { isLoading: isAddFolderOrTestPlanLoading }] =
    useAddFolderOrTestPlanMutation();

  const [
    updateFolderOrTestPlan,
    { isLoading: isUpdateFolderOrTestPlanLoading },
  ] = useUpdateFolderOrTestPlanMutation();

  const toggleAdding = () => {
    setIsAdding((prevState) => !prevState);
  };

  const toggleEditing = () => {
    setIsEditing((prevState) => !prevState);
  };

  const toggleModalAndProvideData = (
    type: ModalType = "ADD_FOLDER",
    editFolderName: string = ""
  ) => {
    setModalType(type);
    setEditFolderNameData(editFolderName);
    setIsModalVisible((prevState) => !prevState);
  };

  return (
    <Row>
      <Col span={6}>
        <div className="tree-structure">
          {isAdding || isEditing ? (
            <div className="test-case-repo">
              <div>
                <Typography.Title level={4} className="mb-3">
                  {isAdding ? "Create Test Plan" : "Update Test Plan"}
                </Typography.Title>
                <Button
                  onClick={() => dispatch(moveSelectedTestCases())}
                  style={{
                    visibility: selectedTestCases.length ? "visible" : "hidden",
                  }}
                >
                  Add
                </Button>
              </div>
              <RequirementManagement match={props.match} isTestPlanPage />
            </div>
          ) : (
            <TreeStructure
              setIsTestPlanSelected={setIsTestPlanSelected}
              toggleModalAndProvideData={toggleModalAndProvideData}
              toggleAdding={() => {
                toggleAdding();
                if (isEditing) setIsEditing(false);
              }}
              toggleEditing={toggleEditing}
              isGetFolderAndTestPlansLoading={isGetFolderAndTestPlansLoading}
              getFolderAndTestPlansData={getFolderAndTestPlansData}
              setParentFolderId={setParentFolderId}
              setSelectedTestPlanData={setSelectedTestPlanData}
              setEditTestPlanId={setEditTestPlanId}
              isGetFolderAndTestPlansError={isGetFolderAndTestPlansError}
              projectSlug={route.params.projectSlug}
              setSearch={setSearch}
            />
          )}
        </div>
      </Col>
      <Col span={18}>
        {isAdding && (
          <AddEditLayout
            isAdding={isAdding}
            toggleEditing={toggleEditing}
            toggleAdding={toggleAdding}
            addFolderOrTestPlan={addFolderOrTestPlan}
            isAddFolderOrTestPlanLoading={isAddFolderOrTestPlanLoading}
            parentFolderId={parentFolderId}
            selectedTestPlanData={selectedTestPlanData}
            updateFolderOrTestPlan={updateFolderOrTestPlan}
            isUpdateFolderOrTestPlanLoading={isUpdateFolderOrTestPlanLoading}
            editTestPlanId={editTestPlanId}
            setIsTestPlanSelected={setIsTestPlanSelected}
            projectSlug={route.params.projectSlug}
          />
        )}
        {!isAdding && isTestPlanSelected && (
          <>
            {!isEditing ? (
              <MainLayout
                toggleEditing={toggleEditing}
                selectedTestPlanData={selectedTestPlanData}
              />
            ) : (
              <AddEditLayout
                isAdding={isAdding}
                toggleEditing={toggleEditing}
                toggleAdding={toggleAdding}
                addFolderOrTestPlan={addFolderOrTestPlan}
                isAddFolderOrTestPlanLoading={isAddFolderOrTestPlanLoading}
                parentFolderId={parentFolderId}
                selectedTestPlanData={selectedTestPlanData}
                updateFolderOrTestPlan={updateFolderOrTestPlan}
                isUpdateFolderOrTestPlanLoading={
                  isUpdateFolderOrTestPlanLoading
                }
                editTestPlanId={editTestPlanId}
                setIsTestPlanSelected={setIsTestPlanSelected}
                projectSlug={route.params.projectSlug}
              />
            )}
          </>
        )}
      </Col>
      <TestPlanModal
        type={modalType}
        isModalVisible={isModalVisible}
        toggleModalAndProvideData={toggleModalAndProvideData}
        editFolderNameData={editFolderNameData}
        setEditFolderNameData={setEditFolderNameData}
        parentFolderId={parentFolderId}
        addFolderOrTestPlan={addFolderOrTestPlan}
        isAddFolderOrTestPlanLoading={isAddFolderOrTestPlanLoading}
        updateFolderOrTestPlan={updateFolderOrTestPlan}
        isUpdateFolderOrTestPlanLoading={isUpdateFolderOrTestPlanLoading}
        setIsModalVisible={setIsModalVisible}
        editTestPlanId={editTestPlanId}
      />
    </Row>
  );
};

export default TestPlan;
