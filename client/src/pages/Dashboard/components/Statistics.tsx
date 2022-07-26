import React from "react";
import { Typography, Card } from "antd";
import { StatsData } from "./StatsData";
import { useGetTotalTestCaseTestPlanTestRunCountQuery } from "../../../store/services/main/admin-dashboard";

const Statistics = () => {
  const { data: getTotalTestCaseTestPlanTestRunCountData } =
    useGetTotalTestCaseTestPlanTestRunCountQuery();

  return (
    <Card>
      <Typography.Title level={3}>Statistics</Typography.Title>
      <div>
        <StatsData
          title="Test Case"
          count={getTotalTestCaseTestPlanTestRunCountData?.testCaseCount}
        />
        <StatsData
          title="Test Plan"
          count={getTotalTestCaseTestPlanTestRunCountData?.testPlanCount}
        />
        <StatsData
          title="Test Run"
          count={getTotalTestCaseTestPlanTestRunCountData?.testRunCount}
        />
      </div>
    </Card>
  );
};

export default Statistics;
