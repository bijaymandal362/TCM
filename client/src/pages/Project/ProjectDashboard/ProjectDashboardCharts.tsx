import { AutoComplete, Empty, Select, Spin, TreeSelect } from "antd";
import { Chart as ChartJS, ArcElement, Tooltip, Legend } from "chart.js";
import { Doughnut } from "react-chartjs-2";
import { RadialLinearScale, PointElement, LineElement, Filler } from "chart.js";
import { Radar } from "react-chartjs-2";
import {
  useGetDefaultFunctionTestCaseListCountByProjectSlugQuery,
  useGetFunctionTestCaseListCountQuery,
  useGetProjectNameFromProjectSlugQuery,
  useGetTestCaseRepositoryListQuery,
  useGetTestRunStatusCountByIdQuery,
} from "../../../store/services/main/project-dashboard";
import { useRouteMatch } from "react-router";
import { useGetTestRunListQuery } from "../../../store/services/main/project-dashboard";
import { useEffect, useState } from "react";
import {
  attachChildToParent,
  formatTreeResponse,
} from "../../../util/tree.util";
import Item from "antd/lib/list/Item";

const parentProjectId = 100;

const ProjectDashboardCharts = () => {
  const route = useRouteMatch<{ projectSlug: string }>();
  const [testRunId, setTestRunId] = useState();
  const [projectModuleId, setProjectModuleId] = useState(
    route.params.projectSlug
  );
  const [radarDefaultFlag, setRadarDefaultFlag] = useState(true);

  const { data: getTestCaseRepositoryList } = useGetTestCaseRepositoryListQuery(
    route.params.projectSlug,
    {
      refetchOnMountOrArgChange: true,
    }
  );

  const getProjectNameFromProjectSlug = useGetProjectNameFromProjectSlugQuery(
    route.params.projectSlug
  );

  const { data: getTestRunList, isLoading: isGetTestRunListLoading } =
    useGetTestRunListQuery(route.params.projectSlug, {
      refetchOnMountOrArgChange: true,
      // refetchOnFocus: true,
      refetchOnReconnect: true,
    });

  const { Option } = Select;

  const {
    data: getTestRunStatusCountById,
    isLoading: isGetTestRunStatusCountByIdLoading,
    error: getTestRunStatusCountByIdError,
  } = useGetTestRunStatusCountByIdQuery(
    testRunId ? testRunId : getTestRunList?.[0].testRunId,
    {
      refetchOnMountOrArgChange: true,
      // refetchOnFocus: true,
      refetchOnReconnect: true,
    }
  );

  const {
    data: getFunctionTestCaseListCount,
    isLoading: isGetFuntionTestCaseListCountLoading,
    error: getFunctionTestCaseListCountError,
  } = useGetFunctionTestCaseListCountQuery(
    {
      projectSlug: route.params.projectSlug,
      projectModuleId,
    },
    {
      skip: radarDefaultFlag,
    }
  );

  const { data: getDefaultRadarData, isLoading: isGetDefaultRadarDataLoading } =
    useGetDefaultFunctionTestCaseListCountByProjectSlugQuery(projectModuleId, {
      refetchOnMountOrArgChange: true,
      skip: !radarDefaultFlag,
    });

  function handleChangeTestRun(value: any) {
    // isGetTestRunStatusCountByIdLoading(truE)
    console.log(`selected test RUn ${value}`);
    setTestRunId(value);
  }

  function handleChangeTestCase(value: any) {
    // console.log(`selected projectModule id ${value}`);

    if (value === parentProjectId) {
      setRadarDefaultFlag(true);
      setProjectModuleId(route.params.projectSlug);
    } else {
      setRadarDefaultFlag(false);
      setProjectModuleId(value);
    }
  }

  //doughnut chart
  ChartJS.register(ArcElement, Tooltip, Legend);

  const data = {
    labels: ["Passed", "Pending", "Failed", "Blocked"],
    datasets: [
      {
        label: "# of Votes",
        // data: [3, 43, 34, 33],
        data: getTestRunStatusCountById && [
          getTestRunStatusCountById.passedCount,
          getTestRunStatusCountById.pendingCount,
          getTestRunStatusCountById.failedCount,
          getTestRunStatusCountById.blockedCount,
        ],
        // : [1, 4, 4, 8],
        backgroundColor: [
          "rgba(0,128,0, 0.7)",
          "rgba(255,165,0, 0.7)",
          "rgba(255,0,0, 0.7)",
          "rgba(0,0,0, 0.7)",
        ],
        borderColor: [
          "rgba(0,128,0, 0.7)",
          "rgba(255,165,0, 0.7)",
          "rgba(255,0,0, 0.7)",
          "rgba(0,0,0, 0.7)",
        ],
        borderWidth: 1,
      },
    ],
  };

  const doughnutDataIfUntested = {
    labels: ["Untested"],
    datasets: [
      {
        label: "# of Votes",
        // data: [3, 43, 34, 33],
        data: [1],
        backgroundColor: [
          "rgba(128,128,128, 0.2)",
          "rgba(54, 162, 235, 0.2)",
          "rgba(255, 206, 86, 0.2)",
          "rgba(75, 192, 192, 0.2)",
        ],
        borderColor: [
          "rgba(128,128,128, 1)",
          "rgba(54, 162, 235, 1)",
          "rgba(255, 206, 86, 1)",
          "rgba(75, 192, 192, 1)",
        ],
        borderWidth: 1,
      },
    ],
  };

  const doughnutOptionsIfUntested = {
    layout: {
      padding: 5,
    },

    plugins: {
      legend: {
        display: false,
      },
    },
    maintainAspectRatio: false,
  };

  const doughnutOptions = {
    layout: {
      padding: 5,
    },
    plugins: {
      legend: {
        position: "right" as const,
        align: "end" as const,
        display: true,
        labels: {
          padding: 15,
          // textAlign: "center" as const,
          // color: "rgb(255,255,255)",
          color: "rgb(120,120,120)",

          usePointStyle: true,
          pointStyle: "circle" as const,
        },
      },
    },
    maintainAspectRatio: false,
  };

  //Radar chart

  ChartJS.register(
    RadialLinearScale,
    PointElement,
    LineElement,
    Filler,
    Tooltip,
    Legend
  );

  const radarData = {
    labels: getFunctionTestCaseListCount?.map((item: any) => item.moduleName),
    datasets: [
      {
        label: "Test Case",
        data: getFunctionTestCaseListCount?.map(
          (item: any) => item.testCaseCount
        ),
        backgroundColor: "rgba(0,128,0, 0.7)",
        borderColor: "rgba(0,128,0, 0.7)",
        borderWidth: 1,
      },
    ],
  };

  const radarDefaultData = {
    labels: getDefaultRadarData?.map((item: any) => item.moduleName),
    datasets: [
      {
        label: "Test Case",
        data: getDefaultRadarData?.map((item: any) => item.testCaseCount),
        // backgroundColor: "rgba(255, 99, 132, 0.2)",
        // borderColor: "rgba(255, 99, 132, 1)",
        backgroundColor: "rgba(0,128,0, 0.7)",
        borderColor: "rgba(0,128,0, 0.7)",
        borderWidth: 1,
      },
    ],
  };

  const radarOptions = {
    plugins: {
      legend: {
        position: "top" as const,
        align: "center" as const,
        labels: {
          color: "rgb(120,120,120)",
          // color: "@heading-color",
        },
      },
    },
    maintainAspectRatio: false,
  };

  const radarTreeData = attachChildToParent(
    {
      title: getProjectNameFromProjectSlug.data?.projectName,
      key: parentProjectId,
    },
    formatTreeResponse(getTestCaseRepositoryList!)
  );

  const radarTreeDefaultData = formatTreeResponse(getDefaultRadarData!);
  return (
    <>
      <div className="PDashboardContainer2">
        <div className="PDashboardSubContainer1">
          <div className="testCaseSubContainer1">
            <div className="pdl">
              {" "}
              <b>Total Test Cases</b>{" "}
            </div>
            {getTestCaseRepositoryList && (
              <div className="pdr">
                <TreeSelect
                  style={{ width: 220 }}
                  defaultValue={getProjectNameFromProjectSlug.data?.projectName}
                  dropdownStyle={{ maxHeight: 400, overflow: "auto" }}
                  treeData={radarTreeData}
                  placeholder="Please select"
                  treeDefaultExpandAll
                  onChange={handleChangeTestCase}
                />
              </div>
            )}
          </div>
          <div className="testCaseSubContainer2">
            <div className="tcsc2diagram">
              {isGetDefaultRadarDataLoading ||
              isGetFuntionTestCaseListCountLoading ? (
                <div className="top-spinner">
                  <Spin className="spin" />
                </div>
              ) : (
                <>
                  {radarDefaultFlag || !getFunctionTestCaseListCountError ? (
                    <Radar
                      data={radarDefaultFlag ? radarDefaultData : radarData}
                      options={radarOptions}
                      height={500}
                      width={500}
                    />
                  ) : (
                    <Empty image={Empty.PRESENTED_IMAGE_SIMPLE} />
                  )}
                </>
              )}
            </div>
          </div>
        </div>
        <div className="PDashboardSubContainer2">
          <div className="testRunSubContainer1">
            <div className="pdl">
              {" "}
              <b> Test Run</b>
            </div>
            <div className="pdr">
              {" "}
              {getTestRunList && (
                <Select
                  placeholder="Select Data"
                  style={{ width: 220 }}
                  onChange={handleChangeTestRun}
                  defaultValue={getTestRunList?.[0].testRunId}
                >
                  {getTestRunList?.map((testRun: any, index: any) => {
                    return (
                      <Option value={testRun.testRunId} key={index + 1}>
                        {testRun.testRunName}
                      </Option>
                    );
                  })}
                </Select>
              )}
            </div>
          </div>

          {/* {console.log(isGetTestRunListLoading, "WHOLE loading")} */}
          <div className="testRunSubContainer2">
            <div className="trsc2diagram ">
              {isGetTestRunListLoading || isGetTestRunStatusCountByIdLoading ? (
                <div className="top-spinner">
                  <Spin className="spin" />
                </div>
              ) : (
                <>
                  {!getTestRunStatusCountByIdError ? (
                    <Doughnut
                      data={data}
                      options={doughnutOptions}
                      height={300}
                      width={400}
                    />
                  ) : (
                    <Empty image={Empty.PRESENTED_IMAGE_SIMPLE} />
                  )}
                </>
              )}
            </div>
          </div>
        </div>
      </div>
    </>
  );
};

export default ProjectDashboardCharts;
