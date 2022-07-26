import React, { useState } from "react";
import { Modal, Select, Typography } from "antd";
import { getSelectedTestCasesIds } from "../../../util/functions.utils";
import { useGetProjectMemberListQuery } from "../../../store/services/main/test-run";

interface AssignMemberProps {
  isAssignModalVisible: boolean;
  setIsAssignModalVisible: (value: boolean) => void;
  testCasesIds: Record<string, boolean>;
  updateUserFromTestCase: (
    userId: number | undefined,
    testCasesIds: number[],
    operationAfterSuccess: () => void
  ) => void;
  testCaseIdForAssign: any;
  isAssignUserToTestCasesLoading: boolean;
  projectSlug: string;
  setTestCasesIds: React.Dispatch<any>;
}

const AssignMember: React.FC<AssignMemberProps> = (props): JSX.Element => {
  // States
  const [assignValue, setAssignValue] = useState("");

  const {
    data: getProjectMemberListData,
    isLoading: isGetProjectMemberListLoading,
  } = useGetProjectMemberListQuery(props.projectSlug, {
    refetchOnMountOrArgChange: true,
  });

  const emptyTestCaseIdForAssign = () => {
    if (props.testCaseIdForAssign.current.length) {
      props.testCaseIdForAssign.current = [];
    }

    props.setTestCasesIds({});
  };

  const handleOk = () => {
    props.updateUserFromTestCase(
      +assignValue,
      !props.testCaseIdForAssign.current.length
        ? getSelectedTestCasesIds(props.testCasesIds)
        : props.testCaseIdForAssign.current,
      () => {
        props.setIsAssignModalVisible(false);
        setAssignValue("");
      }
    );
    emptyTestCaseIdForAssign();
  };

  const handleCancel = () => {
    props.setIsAssignModalVisible(false);

    setAssignValue("");
    emptyTestCaseIdForAssign();
  };

  return (
    <>
      <Modal
        title="Assign Member"
        visible={props.isAssignModalVisible}
        onOk={handleOk}
        onCancel={handleCancel}
        okText="Assign"
        className="assign-modal"
        confirmLoading={props.isAssignUserToTestCasesLoading}
      >
        <Typography.Paragraph>Assign to member</Typography.Paragraph>
        <Select
          onChange={(value: string) => setAssignValue(value)}
          value={assignValue}
          loading={isGetProjectMemberListLoading}
        >
          {getProjectMemberListData?.data.map((user: any) => (
            <Select.Option
              key={user.projectMemberId}
              value={user.projectMemberId}
            >
              {user.name}
            </Select.Option>
          ))}
        </Select>
      </Modal>
    </>
  );
};

export default AssignMember;
