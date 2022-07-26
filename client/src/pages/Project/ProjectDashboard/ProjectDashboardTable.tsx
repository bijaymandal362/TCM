import { Table } from "antd";
import { useRouteMatch } from "react-router";
import { useGetTestCaseListDetailStatusCountQuery } from "../../../store/services/main/project-dashboard";

const ProjectDashboardTable = () => {
  const route = useRouteMatch<{ projectSlug: string }>();

  const { data: getTestCaseListDetailStatusCountData, isLoading } =
    useGetTestCaseListDetailStatusCountQuery(route.params.projectSlug, {
      refetchOnMountOrArgChange: true,
    });

  const mostFailedTestCases = getTestCaseListDetailStatusCountData?.map(
    (data: any, index: any) => {
      return {
        ...data,
        key: index + 1,
      };
    }
  );

  const columns = [
    {
      title: "Test Cases",
      dataIndex: "testCaseName",
      key: "testCaseName",
      render: (name: string) => <div style={{ maxWidth: 350 }}>{name}</div>,
    },
    {
      title: "Function",
      dataIndex: "function",
      key: "function",
      width: 175,
    },
    {
      title: "Scenario",
      dataIndex: "testCaseScenario",
      key: "testCaseScenario",
      width: 475,
    },
    {
      title: "Expected Result",
      dataIndex: "expectedResult",
      key: "expectedResult",
    },
    {
      title: "Fail Count",
      dataIndex: "testCaseFailedCount",
      key: "testCaseFailedCount",
      width: 75,
    },
  ];

  return (
    <>
      <div className="project-dashboard-table-wrapper">
        <div style={{ marginBottom: "5px" }}>
          <b> Most Failed Test Cases</b>
        </div>

        <Table
          dataSource={mostFailedTestCases}
          columns={columns}
          loading={isLoading}
          pagination={false}
        />
      </div>
    </>
  );
};

export default ProjectDashboardTable;
