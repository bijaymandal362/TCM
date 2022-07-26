import React from "react";
import { useRouteMatch } from "react-router";
import { useGetTestCaseTestPlanTestRunCountQuery } from "../../../store/services/main/project-dashboard";
import { Spin } from "antd";
import { MinusOutlined } from "@ant-design/icons";

const ProjectDashboardTop = () => {
  const route = useRouteMatch<{ projectSlug: string }>();

  const { data: getTestCaseTestPlanTestRunCountData, isLoading } =
    useGetTestCaseTestPlanTestRunCountQuery(route.params.projectSlug, {
      refetchOnMountOrArgChange: true,
    });

  return (
    <>
      {isLoading ? (
        <div className="top-spinner">
          <Spin className="spin" />
        </div>
      ) : (
        <div className="projectDashboardContainer">
          <div className="testCases projectDashboardChildren">
            <div className="pdl">
              <div
                style={{
                  borderLeft: " 6px solid #6e54ea",
                  height: "70px",
                  borderRadius: "50px",
                }}
              ></div>
              <div style={{ marginLeft: "10px" }}>
                <b>Total Test Cases</b>
              </div>
            </div>
            <div className="pdr">
              <div> {getTestCaseTestPlanTestRunCountData?.testCaseCount}</div>
            </div>
          </div>
          <div className="testPlan projectDashboardChildren">
            {" "}
            <div className="pdl">
              <div
                style={{
                  borderLeft: " 6px solid #51dcad",
                  height: "70px",
                  borderRadius: "50px",
                }}
              ></div>
              <div style={{ marginLeft: "10px" }}>
                {" "}
                <b>Total Test Plan</b>{" "}
              </div>
            </div>
            <div className="pdr">
              <div> {getTestCaseTestPlanTestRunCountData?.testPlanCount}</div>
            </div>
          </div>
          <div className="testRun projectDashboardChildren">
            {" "}
            <div className="pdl">
              <div
                style={{
                  borderLeft: " 6px solid #ff4b62",
                  height: "70px",
                  borderRadius: "50px",
                }}
              ></div>
              <div style={{ marginLeft: "10px" }}>
                {" "}
                <b> Total Test Run</b>{" "}
              </div>
            </div>
            <div className="pdr">
              <div> {getTestCaseTestPlanTestRunCountData?.testRunCount}</div>
            </div>
          </div>
        </div>
      )}
    </>
  );
};

export default ProjectDashboardTop;
