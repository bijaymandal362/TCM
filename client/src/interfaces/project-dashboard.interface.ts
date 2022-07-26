import { ReactNode } from "react";
import { TestTypes } from "../enum/enum";

export interface ITestCaseTestPlanTestRunCount {
  testCase: string;
  testCaseCount: number;
  testPlan: string;
  testPlanCount: number;
  testRun: string;
  testRunCount: number;
}

//table
export interface ITestCaseListDetailStatusCountData {
  expectedResult: string;
  function: string;
  parentProjectModuleId: number;
  testCaseFailedCount: number;
  testCaseId: number;
  testCaseName: string;
  testCaseScenario: string;
}

export interface ITestCaseRepositoryList {
  childModule: any[];
  moduleName: string;
  parentProjectModuleId: number | null;
  projectId: number;
  projectModuleId: number;
  projectModuleListItemId: number;
  projectModuleType: TestTypes;
  description: string;
  expectedResult: string;
  icon: ReactNode;
}

export interface IFunctionTestCaseListCount {
  moduleName: string;
  parentProjectModuleId: number;
  projectId: number;
  projectModuleId: number;
  testCaseCount: number;
}

export interface IDefaultFunctionTestCaseListCountByProjectSlug {
  childModule: any[];
  moduleName: string;
  parentProjectModuleId: number | null;
  projectId: number;
  projectModuleId: number;
  projectModuleListItemId: number;
  projectModuleType: TestTypes;
  description: string;
  expectedResult: string;
  icon: ReactNode;
  testCaseCount: number;
}

export interface ITestRunList {
  testRunId: number;
  testRunName: string;
}

export interface ITestRunStatusCountById {
  blocked: string;
  blockedCount: number;
  failed: string;
  failedCount: number;
  passed: string;
  passedCount: number;
  pending: string;
  pendingCount: number;
  testRunId: number;
  testRunName: string;
}
