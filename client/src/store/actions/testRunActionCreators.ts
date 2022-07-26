import { AxiosResponse } from "axios";
import axiosInstance from "../../axios/axios";
import {
  Pagination,
  TestRunTestCaseHistory,
  TestRunTestCaseHistoryParams,
} from "../../interfaces";
import { setHeaders } from "../../util/localStorage.util";

export const downloadTestRun = async (testRunId: any): Promise<[any, any]> => {
  setHeaders();
  try {
    const response = await axiosInstance({
      method: "GET",
      responseType: "blob",
      url: `TestRun/ExportTestRunTestCaseByTestRunId/${testRunId}`,
    });
    return [response, null];
  } catch (err) {
    return [null, err];
  }
};

export const getTestRunPDFData = async (
  testRunId: string
): Promise<[AxiosResponse | null, any]> => {
  setHeaders();
  try {
    const response = await axiosInstance({
      method: "GET",
      url: `TestRun/GeneratePdfReportForTestRunTestCaseByTestRunId/${testRunId}`,
    });
    return [response, null];
  } catch (err) {
    return [null, err];
  }
};

export const getTestRunTestCaseHistory = async (
  params: TestRunTestCaseHistoryParams
): Promise<[AxiosResponse<Pagination<TestRunTestCaseHistory>> | null, any]> => {
  setHeaders();
  try {
    const response = await axiosInstance({
      method: "POST",
      url: `TestRun/GetTestRunTestCaseWizard/${params.testCaseId}`,
      params: {
        PageNumber: params.pageNumber,
        PageSize: params.pageSize,
      },
    });
    return [response, null];
  } catch (err) {
    return [null, err];
  }
};
