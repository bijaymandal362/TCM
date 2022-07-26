import React, { useRef, useState } from "react";
import moment from "moment";
import "moment-duration-format";
import {
  Button,
  Col,
  Collapse,
  Divider,
  message,
  Modal,
  Row,
  Spin,
  Typography,
} from "antd";
import TestRunStatus from "../components/TestRunStatus";
import {
  useDownloadTestRunFileMutation,
  useGetTestCaseResultsDataQuery,
} from "../../../store/services/main/test-run";
import { useRouterQuery } from "../../../util/functions.utils";
import { useHistory, useRouteMatch } from "react-router";
import { DownloadOutlined } from "@ant-design/icons";
import { downloadUploadedFile } from "./TestRunWizard";

interface TestCaseStatusDetailProps {
  isTestCaseStatusDetailModalVisible: boolean;
  setIsTestCaseStatusDetailModalVisible: (value: boolean) => void;
}

const TestCaseStatusDetail: React.FC<TestCaseStatusDetailProps> = (
  props
): JSX.Element => {
  const history = useHistory();
  const query = useRouterQuery();
  const route = useRouteMatch<{ testRunId: string }>();
  const clickedDownloadDocumentId = useRef<number | null>(null);
  const [ellipsis, setEllipsis] = useState(true);

  const {
    data: getTestCaseResultsData,
    isLoading: getTestCaseResultsDataLoading,
  } = useGetTestCaseResultsDataQuery(
    {
      testRunId: +route.params.testRunId,
      // @ts-ignore: Object is possibly 'null'.
      testCaseId: +query.get("testCaseId"),
      // @ts-ignore: Object is possibly 'null'.
      testPlanId: +query.get("testPlanId"),
    },
    {
      skip: !query.get("testCaseId") && !query.get("testPlanId"),
      refetchOnMountOrArgChange: true,
    }
  );

  const [downloadTestRunFile, downloadTestRunFileData] =
    useDownloadTestRunFileMutation();

  const handleCancel = () => {
    props.setIsTestCaseStatusDetailModalVisible(false);
    history.goBack();
  };

  return (
    <Modal
      visible={props.isTestCaseStatusDetailModalVisible}
      onCancel={handleCancel}
      className="testcase-status-detail-modal"
      footer={null}
      centered
      width="80vw"
    >
      {!getTestCaseResultsDataLoading ? (
        <>
          <Typography.Title level={3}>
            {getTestCaseResultsData?.testCaseTitle}
          </Typography.Title>
          {getTestCaseResultsData?.results.map((item, index) => (
            <div key={index} className="data">
              <Typography.Title level={5}>Result #{index + 1}</Typography.Title>
              <Divider className="mt-2 mb-3" />
              <div className="sub-data">
                <div>
                  <Typography.Paragraph className="data-header">
                    Status
                  </Typography.Paragraph>
                  <TestRunStatus
                    type="text"
                    status={item.status.toLowerCase() as any}
                  >
                    <>{item.status}</>
                  </TestRunStatus>
                </div>
                <div>
                  <Typography.Paragraph className="data-header">
                    Time spent
                  </Typography.Paragraph>
                  <Typography.Paragraph>
                    {item.timeSpent
                      ? moment
                          .utc(
                            moment
                              .duration(item.timeSpent, "seconds")
                              .asMilliseconds()
                          )
                          .format("HH:mm:ss")
                      : "00:00:00"}
                  </Typography.Paragraph>
                </div>
                <div>
                  <Typography.Paragraph className="data-header">
                    User
                  </Typography.Paragraph>
                  <Typography.Paragraph>
                    {item.user || "N/A"}
                  </Typography.Paragraph>
                </div>
                <div>
                  <Typography.Paragraph className="data-header">
                    Remark
                  </Typography.Paragraph>
                  <Typography.Paragraph>
                    {item.comment || "Not Provided"}
                  </Typography.Paragraph>
                </div>
                <div>
                  <Typography.Paragraph className="data-header">
                    Finish time
                  </Typography.Paragraph>
                  <Typography.Paragraph>
                    {item.finishTime || "N/A"}
                  </Typography.Paragraph>
                </div>
                {item.documentId ? (
                  <div className="download-btn">
                    <Button
                      type="text"
                      icon={<DownloadOutlined />}
                      loading={
                        downloadTestRunFileData.isLoading &&
                        item.documentId === clickedDownloadDocumentId.current
                      }
                      onClick={() => {
                        clickedDownloadDocumentId.current = item.documentId;
                        downloadTestRunFile(`${item.documentId}`)
                          .unwrap()
                          .then((data) => {
                            downloadUploadedFile(data, item.fileName!);
                          })
                          .catch((err) => message.error(err.data, 3));
                      }}
                    >
                      File
                    </Button>
                  </div>
                ) : null}
              </div>
              {/* {item.comment && (
                <div>
                  <Typography.Paragraph className="data-header">
                    Comment
                  </Typography.Paragraph>
                  <Typography.Paragraph>
                    {item.comment || "Not Provided"}
                  </Typography.Paragraph>
                </div>
              )} */}
              {item.stepsToReproduce?.length && (
                <div>
                  <Typography.Paragraph className="data-header">
                    Steps
                  </Typography.Paragraph>
                  <Collapse ghost>
                    {item.stepsToReproduce.map((stepItem) => (
                      <Collapse.Panel
                        key={stepItem.testCaseStepResultId}
                        header={
                          <Typography.Text>{stepItem.step}</Typography.Text>
                        }
                      >
                        <Row
                          className="content"
                          gutter={12}
                          style={{ verticalAlign: "top" }}
                        >
                          <Col md={10}>
                            <Typography.Paragraph className="text-gray">
                              Expected result
                            </Typography.Paragraph>
                            <Typography.Paragraph>
                              {stepItem.expectedResult}
                            </Typography.Paragraph>
                          </Col>
                          <Col md={10}>
                            <Typography.Paragraph className="text-gray">
                              Remark
                            </Typography.Paragraph>
                            <Typography.Paragraph>
                              {stepItem.comment}
                            </Typography.Paragraph>
                          </Col>
                          <Col md={4}>
                            <Typography.Paragraph className="text-gray">
                              File
                            </Typography.Paragraph>
                            {stepItem.documentId ? (
                              <Typography.Paragraph>
                                <Button
                                  type="text"
                                  icon={<DownloadOutlined />}
                                  loading={
                                    downloadTestRunFileData.isLoading &&
                                    stepItem.documentId ===
                                      clickedDownloadDocumentId.current
                                  }
                                  onClick={() => {
                                    clickedDownloadDocumentId.current =
                                      stepItem.documentId;
                                    downloadTestRunFile(
                                      `${stepItem.documentId}`
                                    )
                                      .unwrap()
                                      .then((data) => {
                                        downloadUploadedFile(
                                          data,
                                          stepItem.fileName!
                                        );
                                      })
                                      .catch((err) =>
                                        message.error(err.data, 3)
                                      );
                                  }}
                                >
                                  File
                                </Button>
                              </Typography.Paragraph>
                            ) : null}
                          </Col>
                        </Row>
                      </Collapse.Panel>
                    ))}
                  </Collapse>
                </div>
              )}
            </div>
          ))}
        </>
      ) : (
        <div className="spinner-container">
          <Spin />
        </div>
      )}
    </Modal>
  );
};

export default TestCaseStatusDetail;
