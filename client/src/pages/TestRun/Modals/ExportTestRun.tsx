import React, { useEffect, useState } from "react";
import { message, Modal, Select, Typography } from "antd";
import { useRouteMatch } from "react-router";
import { downloadTestRun } from "../../../store/actions/testRunActionCreators";
import { usePDF } from "@react-pdf/renderer";
import PdfGenerator from "../components/PdfGenerator";

type exportType = "excel" | "pdf";

interface ExportTestRunProps {
  isExportModalVisible: boolean;
  setIsExportModalVisible: (value: boolean) => void;
}

const ExportTestRun: React.FC<ExportTestRunProps> = (props): JSX.Element => {
  const route = useRouteMatch<{ testRunId: string }>();

  // States
  const [exportValue, setExportValue] = useState<exportType>("excel");
  const [isDownloading, setIsDownloading] = useState(false);
  const [isDownloadingPDF, setIsDownloadingPDF] = useState<
    "idle" | "pending" | "done"
  >("idle");

  const [instance, updateInstance] = usePDF({
    document: (
      <PdfGenerator
        testRunId={route.params.testRunId}
        isDownloadingPDF={isDownloadingPDF}
        setIsDownloadingPDF={setIsDownloadingPDF}
        setIsExportModalVisible={props.setIsExportModalVisible}
      />
    ),
  });

  const handleOk = async () => {
    if (exportValue === "excel") {
      setIsDownloading(true);
      const [response, error] = await downloadTestRun(route.params.testRunId);
      if (response) {
        const url = window.URL.createObjectURL(new Blob([response?.data]));
        const link = document.createElement("a");
        link.href = url;
        link.setAttribute("download", "TestRunDetails.xlsx");
        document.body.appendChild(link);
        link.click();
      }
      if (error) {
        message.error("Failed to download excel file.", 3);
      }
      setIsDownloading(false);
      props.setIsExportModalVisible(false);
    } else if (exportValue === "pdf") {
      window.open(`/pdf/${route.params.testRunId}`, "_blank");

      // if (isDownloadingPDF === "done") downloadPDF();
      // else setIsDownloadingPDF("pending");
    }
  };

  const downloadPDF = () => {
    const link = document.createElement("a");
    link.href = instance.url || "";
    link.setAttribute("download", "QAExecutionReport.pdf");
    document.body.appendChild(link);
    link.click();

    setExportValue("excel");
  };

  const handleCancel = () => {
    props.setIsExportModalVisible(false);
    setExportValue("excel");
  };

  useEffect(() => {
    if (isDownloadingPDF === "pending") {
      updateInstance();
    }
  }, [isDownloadingPDF]);

  useEffect(() => {
    if (isDownloadingPDF === "done") {
      if (instance.error) {
        message.error("Failed to download pdf file.", 3);
      }

      downloadPDF();
    }
  }, [isDownloadingPDF]);

  return (
    <Modal
      title="Export Test Run"
      visible={props.isExportModalVisible}
      onOk={handleOk}
      onCancel={handleCancel}
      okText="Export"
      className="export-modal"
      confirmLoading={isDownloading || isDownloadingPDF === "pending"}
    >
      <Typography.Paragraph>Export format</Typography.Paragraph>
      <Select
        value={exportValue}
        onChange={(value: exportType) => setExportValue(value)}
      >
        <Select.Option value="excel">Excel</Select.Option>
        <Select.Option value="pdf">PDF</Select.Option>
      </Select>
    </Modal>
  );

  // For PDF development
  // return (
  //   <PDFViewer style={{ height: "100vh", width: "100%" }}>
  //     <PdfGenerator
  //       testRunId={route.params.testRunId}
  //       isDownloadingPDF="pending"
  //       setIsDownloadingPDF={setIsDownloadingPDF}
  //       setIsExportModalVisible={props.setIsExportModalVisible}
  //     />
  //   </PDFViewer>
  // );
};

export default ExportTestRun;
