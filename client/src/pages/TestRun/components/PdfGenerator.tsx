import {
  Page,
  Text,
  View,
  Document,
  StyleSheet,
  Font,
  Image,
} from "@react-pdf/renderer";
import React, { useEffect, useState } from "react";
import { getTestRunPDFData } from "../../../store/actions/testRunActionCreators";
import { getTodayDate } from "../../../util/functions.utils";
const ChartJsImage = require("chartjs-to-image");

// Register font
Font.register({
  family: "GenShinGothic",
  fonts: [
    {
      src: "/fonts/Gen-Shin-Gothic-Monospace-Regular.ttf",
    },
  ],
});

function oneDecimalPlacesIfCents(amount: number) {
  return amount % 1 !== 0 ? amount.toFixed(2) : amount;
}

const setPieData = (data: any) => {
  return {
    labels: ["Pending", "Passed", "Failed", "Blocked"],
    datasets: [
      {
        label: "Test Execution Status",
        data: [
          oneDecimalPlacesIfCents(data.pendingPercentage),
          oneDecimalPlacesIfCents(data.passedPercentage),
          oneDecimalPlacesIfCents(data.failedPercentage),
          oneDecimalPlacesIfCents(data.blockedPercentage),
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
    labels: data.map((item: any) => item.functionName),
    datasets: [
      {
        label: "Test Cases",
        data: data.map((item: any) => item.totalCountTestCaseByTestRunId),
        backgroundColor: "rgba(255, 99, 132, 0.2)",
        borderColor: "rgba(255, 99, 132, 1)",
        borderWidth: 1,
      },
    ],
  };
};

const barData = (data: any) => {
  return {
    labels: data.map((item: any) => item.testPlanNameForCount),
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

const PdfGenerator = ({
  testRunId,
  isDownloadingPDF,
  setIsDownloadingPDF,
  setIsExportModalVisible,
}: {
  testRunId: string;
  isDownloadingPDF: "idle" | "pending" | "done";
  setIsDownloadingPDF: React.Dispatch<
    React.SetStateAction<"idle" | "pending" | "done">
  >;
  setIsExportModalVisible: any;
}) => {
  const [pdfData, setPdfData] = useState<any>({});
  const [pieDataUrl, setPieDataUrl] = useState("");
  const [radarDataUrl, setRadarDataUrl] = useState("");
  const [barDataUrl, setBarDataUrl] = useState("");
  const [PDFDone, setPDFDone] = useState(false);

  const pieChart = new ChartJsImage();
  const radarChart = new ChartJsImage();
  const barChart = new ChartJsImage();

  useEffect(() => {
    if (isDownloadingPDF === "pending") {
      const getUrl = async () => {
        const res = await getTestRunPDFData(testRunId);
        setPdfData(res[0]?.data);

        // Pie
        pieChart.setConfig({
          type: "pie",
          data: setPieData(res[0]?.data?.pieChart),
          options: {
            legend: {
              position: "right" as const,
              labels: {
                padding: 20,
                boxWidth: 20,
              },
            },
          },
        });
        pieChart.setWidth(800).setHeight(325);

        // Radar
        radarChart.setConfig({
          type: "radar",
          data: radarData(res[0]?.data?.redarDiagram),
          options: {
            legend: {
              position: "right" as const,
              labels: {
                padding: 20,
                boxWidth: 20,
              },
            },
          },
        });
        radarChart.setWidth(800).setHeight(325);

        // Bar
        barChart.setConfig({
          type: "horizontalBar",
          data: barData(res[0]?.data?.barChart),
          options: {
            scales: {
              xAxes: [
                {
                  stacked: true,
                },
              ],
              yAxes: [
                {
                  stacked: true,
                },
              ],
            },
            legend: {
              position: "right" as const,
              labels: {
                padding: 20,
                boxWidth: 20,
              },
            },
          },
        });
        barChart.setWidth(800).setHeight(325);

        const urls = await Promise.all([
          pieChart.toDataUrl(),
          radarChart.toDataUrl(),
          barChart.toDataUrl(),
        ]);

        setPieDataUrl(urls[0]);
        setRadarDataUrl(urls[1]);
        setBarDataUrl(urls[2]);

        setPDFDone(true);
      };

      getUrl();
    }
  }, [isDownloadingPDF]);

  useEffect(() => {
    if (PDFDone) {
      setPDFDone(false);

      setTimeout(() => {
        setIsExportModalVisible(false);
        setIsDownloadingPDF("done");
      }, 500);
    }
  }, [PDFDone]);

  return (
    <Document>
      <Page size="A4" style={styles.page}>
        <View style={styles.header} fixed>
          <Image
            src={window.location.origin + "/logo.png"}
            style={styles.logo}
          />
          <Text>Date : {getTodayDate()}</Text>
        </View>

        <View style={styles.body}>
          <Text style={styles.title}>QA EXECUTION REPORT</Text>

          <View style={styles.subTitleContainer}>
            <View style={styles.subTitle1}>
              <Text style={styles.subSubTitle1}>Project</Text>
              <Text style={styles.subSubTitle2}>
                : {pdfData?.testRunExcelModelTitle?.projectName || "N/A"}
              </Text>
            </View>
            <View style={styles.subTitle2}>
              <Text style={styles.subSubTitle1}>Environment</Text>
              <Text style={styles.subSubTitle2}>
                : {pdfData?.testRunExcelModelTitle?.environment || "N/A"}
              </Text>
            </View>
          </View>

          <View style={styles.subTitleContainer}>
            <View style={styles.subTitle1}>
              <Text style={styles.subSubTitle1}>Test Run</Text>
              <Text style={styles.subSubTitle2}>
                :{" "}
                {pdfData?.testRunExcelModelTitle?.testCaseManagementSystem ||
                  "N/A"}
              </Text>
            </View>
            <View style={styles.subTitle2}>
              {/* <Text style={styles.subSubTitle1}>Option</Text>
              <Text style={styles.subSubTitle2}>: Dummy</Text> */}
            </View>
          </View>

          <View
            style={{ ...styles.chartContainer, ...styles.firstChartContainer }}
            wrap={false}
          >
            <Text style={styles.chartTitle}>Test Execution Status</Text>
            <View style={{ width: "95%" }}>
              <Image src={pieDataUrl} />
            </View>
          </View>

          <View style={styles.chartContainer} wrap={false}>
            <Text style={styles.chartTitle}>Function Map</Text>
            <View style={{ width: "95%" }}>
              <Image src={radarDataUrl} />
            </View>
          </View>

          <View style={styles.chartContainer} wrap={false}>
            <Text style={styles.chartTitle}>Test Plan Wise Report</Text>
            <View style={{ width: "95%" }}>
              <Image src={barDataUrl} />
            </View>
          </View>

          {pdfData?.functionTestCase?.map((item: any) => (
            <View style={styles.tableContainer} key={item.functionId} break>
              <View style={styles.tableTitleContainer} fixed>
                <Text>Function : {item.functionName}</Text>
              </View>
              <View>
                <View style={styles.tableGroupCell} fixed>
                  <Text style={{ ...styles.tableCell, ...styles.idTableCell }}>
                    ID
                  </Text>
                  <Text style={styles.tableCell}>Test Case</Text>
                  <Text style={styles.tableCell}>Scenario</Text>
                  <Text style={styles.tableCell}>Expected Result</Text>
                  <Text
                    style={{ ...styles.tableCell, ...styles.statusTableCell }}
                  >
                    Status
                  </Text>
                  <Text style={styles.tableCell}>Remarks</Text>
                </View>
                <View>
                  {item?.testCaseDetail.map((testCaseItem: any, index: any) => (
                    <View style={styles.tableGroupCell} wrap={false}>
                      <Text
                        style={{
                          ...styles.tableCell,
                          ...styles.idTableCell,
                          ...styles.tableBodyStyle,
                        }}
                      >
                        {index + 1}
                      </Text>
                      <Text
                        style={{
                          ...styles.tableCell,
                          ...styles.tableBodyStyle,
                          ...styles.alignLeft,
                        }}
                      >
                        {testCaseItem.testCaseName}
                      </Text>
                      <Text
                        style={{
                          ...styles.tableCell,
                          ...styles.tableBodyStyle,
                          ...styles.alignLeft,
                        }}
                      >
                        {testCaseItem.preConditon}
                      </Text>
                      <Text
                        style={{
                          ...styles.tableCell,
                          ...styles.tableBodyStyle,
                          ...styles.alignLeft,
                        }}
                      >
                        {testCaseItem.exceptedResult}
                      </Text>
                      <Text
                        style={{
                          ...styles.tableCell,
                          ...styles.statusTableCell,
                          ...styles.tableBodyStyle,
                        }}
                      >
                        {testCaseItem.status}
                      </Text>
                      <Text
                        style={{
                          ...styles.tableCell,
                          ...styles.tableBodyStyle,
                          ...styles.alignLeft,
                        }}
                      ></Text>
                    </View>
                  ))}
                </View>
              </View>
            </View>
          ))}
        </View>

        <Text
          style={styles.pageCount}
          render={({ pageNumber, totalPages }) =>
            `Page : ${pageNumber} / ${totalPages}`
          }
          fixed
        />
      </Page>
    </Document>
  );
};

const styles = StyleSheet.create({
  page: {
    paddingTop: 15,
    paddingBottom: 41,
    fontSize: 12,
    fontFamily: "GenShinGothic",
  },
  header: {
    flexDirection: "row",
    justifyContent: "space-between",
    borderBottom: "1px solid #ddd",
    marginBottom: 25,
    padding: "0 15 15 15",
    alignItems: "center",
  },
  logo: {
    height: 25,
    width: 25,
  },
  body: {
    padding: "0 15 0 15",
  },
  title: {
    backgroundColor: "#f1f1f1",
    fontWeight: "light",
    padding: "10 20",
    marginBottom: 15,
    fontSize: 16,
  },
  subTitleContainer: {
    flexDirection: "row",
    marginBottom: 10,
  },
  subTitle1: {
    flexDirection: "row",
    flex: 1,
    paddingRight: 15,
  },
  subTitle2: {
    flexDirection: "row",
    flex: 1,
  },
  subSubTitle1: {
    flex: 1,
  },
  subSubTitle2: {
    flex: 2,
  },
  firstChartContainer: {
    marginTop: 20,
  },
  chartContainer: {
    height: 290,
    border: "1px solid #ccc",
    marginBottom: 20,
  },
  chartTitle: {
    padding: "8 15",
    borderBottom: "1px solid #ccc",
    marginBottom: 20,
  },
  tableContainer: {
    marginBottom: 20,
  },
  tableTitleContainer: {
    padding: "15 20",
    border: "1px solid #ccc",
  },
  tableGroupCell: {
    flexDirection: "row",
    borderBottom: "1px solid #ccc",
    borderLeft: "1px solid #ccc",
    borderRight: "1px solid #ccc",
  },
  tableCell: {
    flex: 1,
    padding: 5,
  },
  idTableCell: {
    flex: 0.3,
    textAlign: "center",
  },
  statusTableCell: {
    flex: 0.5,
    textAlign: "center",
  },
  tableBodyStyle: {
    fontSize: 11,
    color: "#444",
  },
  alignLeft: {
    textAlign: "left",
  },
  pageCount: {
    position: "absolute",
    bottom: 15,
    right: 15,
  },
});

export default PdfGenerator;
