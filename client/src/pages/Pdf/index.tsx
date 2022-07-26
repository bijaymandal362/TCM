import { Spin } from "antd";
import { useEffect, useState } from "react";
import { Pie, Radar, Bar } from "react-chartjs-2";
import { useRouteMatch } from "react-router";
import { getTestRunPDFData } from "../../store/actions/testRunActionCreators";
import useUpdateEffect from "../../util/custom-hooks/useUpdateEffect";
import { getTodayDate } from "../../util/functions.utils";
import Logo from "../../assets/images/pdfLogo.svg";

import "./style.css";
import {
  Chart as ChartJS,
  ArcElement,
  Tooltip,
  Legend,
  Filler,
  RadialLinearScale,
  PointElement,
  LineElement,
  CategoryScale,
  LinearScale,
  BarElement,
  Title,
} from "chart.js";
// const ChartJsImage = require("chartjs-to-image");

ChartJS.register(
  ArcElement,
  Tooltip,
  Legend,
  Filler,
  RadialLinearScale,
  PointElement,
  LineElement,
  CategoryScale,
  LinearScale,
  BarElement,
  Title
);

function oneDecimalPlacesIfCents(amount: number) {
  return amount % 1 !== 0 ? amount?.toFixed(2) : amount;
}

const pieOptions = {
  plugins: {
    legend: {
      position: "right" as const,
      labels: {
        padding: 40,
        boxWidth: 20,
      },
    },
  },
};

const radarOptions = {
  plugins: {
    legend: {
      position: "right" as const,
      labels: {
        padding: 40,
        boxWidth: 20,
      },
    },
  },
};

const barOptions = {
  indexAxis: "y" as const,
  responsive: true,
  plugins: {
    legend: {
      position: "bottom" as const,
      labels: {
        padding: 40,
        boxWidth: 20,
      },
    },
  },
};

const setPieData = (data: any) => {
  return {
    labels: ["Pending", "Passed", "Failed", "Blocked"],
    datasets: [
      {
        label: "Test Execution Status",
        data: [
          oneDecimalPlacesIfCents(data?.pendingPercentage),
          oneDecimalPlacesIfCents(data?.passedPercentage),
          oneDecimalPlacesIfCents(data?.failedPercentage),
          oneDecimalPlacesIfCents(data?.blockedPercentage),
        ],
        backgroundColor: [
          "rgb(53, 162, 235)",
          "rgb(75, 192, 192)",
          "rgb(255, 99, 132)",
          "rgb(255, 206, 86)",
        ],
        borderColor: [
          "rgb(53, 162, 235)",
          "rgb(75, 192, 192)",
          "rgb(255, 99, 132)",
          "rgb(255, 206, 86)",
        ],
        borderWidth: 1,
      },
    ],
  };
};

const radarData = (data: any) => {
  return {
    labels: data?.map((item: any) => item.functionName),
    datasets: [
      {
        label: "Test Cases",
        data: data?.map((item: any) => item.totalCountTestCaseByTestRunId),
        backgroundColor: "rgba(255, 99, 132, 0.2)",
        borderColor: "rgba(255, 99, 132, 1)",
        borderWidth: 1,
      },
    ],
  };
};

const barData = (data: any) => {
  return {
    labels: data?.map((item: any) => item.testPlanNameForCount),
    datasets: [
      {
        label: "Pending",
        data: data.map((item: any) => item.totalPendingCount),
        backgroundColor: "rgb(53, 162, 235)",
      },
      {
        label: "Passed",
        data: data.map((item: any) => item.totalPassedCount),
        backgroundColor: "rgb(75, 192, 192)",
      },
      {
        label: "Failed",
        data: data.map((item: any) => item.totalFailedCount),
        backgroundColor: "rgb(255, 99, 132)",
      },
      {
        label: "Blocked",
        data: data.map((item: any) => item.totalBlockCount),
        backgroundColor: "rgb(255, 206, 86)",
      },
    ],
  };
};

const Pdf = () => {
  const route = useRouteMatch<{ testRunId: string }>();

  // States
  const [isLoading, setIsLoading] = useState(true);
  const [pdfData, setPdfData] = useState<any>({});
  // const [pieDataUrl, setPieDataUrl] = useState("");
  // const [radarDataUrl, setRadarDataUrl] = useState("");
  // const [barDataUrl, setBarDataUrl] = useState("");

  // const pieChart = new ChartJsImage();
  // const radarChart = new ChartJsImage();
  // const barChart = new ChartJsImage();

  useEffect(() => {
    const getUrl = async () => {
      const res = await getTestRunPDFData(route.params.testRunId);
      setPdfData(res[0]?.data);

      // Pie
      // pieChart.setConfig({
      //   type: "pie",
      //   data: setPieData(res[0]?.data?.pieChart),
      //   options: {
      //     legend: {
      //       position: "right" as const,
      //       labels: {
      //         padding: 20,
      //         boxWidth: 20,
      //       },
      //     },
      //   },
      // });
      // pieChart.setWidth(800).setHeight(325);

      // Radar
      // radarChart.setConfig({
      //   type: "radar",
      //   data: radarData(res[0]?.data?.redarDiagram),
      //   options: {
      //     legend: {
      //       position: "right" as const,
      //       labels: {
      //         padding: 20,
      //         boxWidth: 20,
      //       },
      //     },
      //   },
      // });
      // radarChart.setWidth(800).setHeight(325);

      // Bar
      // barChart.setConfig({
      //   type: "horizontalBar",
      //   data: barData(res[0]?.data?.barChart),
      //   options: {
      //     scales: {
      //       xAxes: [
      //         {
      //           stacked: true,
      //         },
      //       ],
      //       yAxes: [
      //         {
      //           stacked: true,
      //         },
      //       ],
      //     },
      //     legend: {
      //       position: "right" as const,
      //       labels: {
      //         padding: 20,
      //         boxWidth: 20,
      //       },
      //     },
      //   },
      // });
      // barChart.setWidth(800).setHeight(325);

      // const urls = await Promise.all([
      //   pieChart.toDataUrl(),
      //   radarChart.toDataUrl(),
      //   barChart.toDataUrl(),
      // ]);

      // setPieDataUrl(urls[0]);
      // setRadarDataUrl(urls[1]);
      // setBarDataUrl(urls[2]);

      setIsLoading(false);
    };

    getUrl();
  }, []);

  useUpdateEffect(() => {
    if (!isLoading) {
      setTimeout(() => {
        window.print();
      }, 1000);
    }
  }, [isLoading]);

  return (
    <>
      {isLoading ? (
        <div className="loading-ui">
          <Spin />
        </div>
      ) : (
        <div className="wrapper">
          <div className="page-break">
            <table className="page">
              <thead>
                <tr>
                  <td>
                    <div className="header">
                      <table>
                        <tbody>
                          <tr>
                            <td>
                              <div>
                                <img src={Logo} alt="Logo" className="logo" />
                              </div>
                            </td>
                            <td>
                              <p>
                                Date: <span>{getTodayDate()}</span>
                              </p>
                            </td>
                          </tr>
                        </tbody>
                      </table>
                    </div>
                  </td>
                </tr>
              </thead>

              <tbody>
                <tr>
                  <td>
                    <section>
                      <div className="section-head">
                        <h5 className="text-black">QA EXECUTION REPORT</h5>
                        <table>
                          <tbody>
                            <tr>
                              <th>
                                Project <span>:</span>
                              </th>
                              <td>
                                {pdfData?.testRunExcelModelTitle?.projectName ||
                                  "N/A"}
                              </td>
                              <th>
                                Environment <span>:</span>
                              </th>
                              <td>
                                {pdfData?.testRunExcelModelTitle?.environment ||
                                  "N/A"}
                              </td>
                            </tr>
                            <tr>
                              <th>
                                Test Run <span>:</span>
                              </th>
                              <td>
                                {pdfData?.testRunExcelModelTitle
                                  ?.testCaseManagementSystem || "N/A"}
                              </td>
                              {/* <th>
                            Dummy option <span>:</span>
                          </th>
                          <td>General Report</td> */}
                            </tr>
                          </tbody>
                        </table>
                      </div>
                    </section>
                  </td>
                </tr>

                {/* <tr>
                  <td></td>
                </tr> */}
                <tr>
                  <td>
                    <section style={{ pageBreakAfter: "always" }}>
                      <div className="data-section">
                        <div className="data-wrap">
                          <h5 className="data-head text-black">
                            Test Execution Status
                          </h5>
                          <div className="chart-wrap">
                            {/* <img
                              src={pieDataUrl}
                              alt="Pie chart"
                              style={{ maxWidth: "100%" }}
                            /> */}

                            <Pie
                              data={setPieData(pdfData?.pieChart)}
                              options={pieOptions}
                            />
                          </div>
                        </div>
                      </div>
                    </section>
                    <section
                      style={{ pageBreakAfter: "always", marginTop: "100px" }}
                    >
                      <div className="data-section">
                        <div className="data-wrap">
                          <h5 className="data-head text-black">Function Map</h5>
                          <div className="chart-wrap">
                            {/* <img
                              src={radarDataUrl}
                              alt="Radar chart"
                              style={{ maxWidth: "100%" }}
                            /> */}

                            <Radar
                              data={radarData(pdfData?.redarDiagram)}
                              options={radarOptions}
                            />
                          </div>
                        </div>
                      </div>
                    </section>
                    <section
                      style={{ pageBreakAfter: "always", marginTop: "100px" }}
                    >
                      <div className="data-section">
                        <div className="data-wrap">
                          <h5 className="data-head text-black">
                            Test Plan Wise Report
                          </h5>
                          <div className="bar-chart">
                            {/* <img
                              src={barDataUrl}
                              alt="Bar chart"
                              style={{ maxWidth: "100%" }}
                            /> */}

                            <Bar
                              options={barOptions}
                              data={barData(pdfData?.barChart)}
                            />
                          </div>
                        </div>
                      </div>
                    </section>
                    {pdfData?.functionTestCase?.map((item: any) => (
                      <>
                        <section key={item.testPlanId}>
                          <div className="function-table">
                            <table className="table">
                              <thead>
                                <tr>
                                  <td colSpan={6}>
                                    <div className="head">
                                      <h5 className="text-black">
                                        Test Plan :
                                        <span>{item.testPlanName}</span>
                                      </h5>
                                      {/* <p>Here is the data table Sub detail</p> */}
                                    </div>
                                  </td>
                                </tr>
                                <tr>
                                  <th>ID</th>
                                  <th>Test Case</th>
                                  <th> Scenario</th>
                                  <th>Expected Result</th>
                                  <th>Status</th>
                                  <th>Remarks</th>
                                </tr>
                              </thead>
                              <tbody>
                                {item?.testCaseDetail.map(
                                  (testCaseItem: any, index: any) => (
                                    <tr key={testCaseItem.projectModuleId}>
                                      <th>{index + 1}</th>
                                      <td style={{ wordBreak: "break-all" }}>
                                        {" "}
                                        {testCaseItem.testCaseName}
                                      </td>
                                      <td>{testCaseItem.preConditon}</td>
                                      <td>{testCaseItem.exceptedResult}</td>
                                      <td>{testCaseItem.status}</td>
                                      <td>{testCaseItem.remarks}</td>
                                    </tr>
                                  )
                                )}
                              </tbody>
                            </table>
                          </div>
                        </section>
                        <div className="page-break"></div>
                      </>
                    ))}
                  </td>
                </tr>
                {/* <tr>
                  <td></td>
                </tr> */}

                {/* <tr key={item.functionId}>
                  <td>
                    <div className="page-break"></div>
                  </td>
                </tr> */}
              </tbody>
            </table>
          </div>
        </div>
      )}
    </>
  );
};

export default Pdf;
