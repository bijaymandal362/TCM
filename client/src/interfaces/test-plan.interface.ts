import { TestTypes } from "../enum/enum";

export interface IAddUpdateTestPlanParams {
  testPlanId: number;
  parentTestPlanId: number | null;
  testPlanName: string;
  orderDate?: string;
  title: string;
  projectSlug: string;
  description?: string;
  testPlanTypeListItemId: number;
  testPlanType: string;
  projectModuleId?: number[];
  testPlanTestCaseId?: number[];
}

export interface IFolderAndTestPlan {
  testPlanId: number;
  parentTestPlanId: number | null;
  testPlanName: string;
  orderDate: string;
  title: string;
  projectId: number;
  projectSlug: string;
  description: string | null;
  testPlanTypeListItemId: number;
  testPlanType: string;
  testPlanChildModule: IFolderAndTestPlan[];
}

export interface FormattedTestPlanTreeInterface {
  title: string;
  key: number;
  children?: FormattedTestPlanTreeInterface[];
  type: string;
  description: string | null;
  parentTestPlanId: number | null;
  // value: string;
}

export interface ITestPlanTreeStructure {
  title: string;
  key: string;
  parentKey: number | null;
  children?: ITestPlanTreeStructure[];
  isLeaf?: boolean;
  testPlanName: string;
  orderDate: string;
  projectId: number;
  projectSlug: string;
  description: string | null;
  testPlanTypeListItemId: number;
  testPlanType: string;
}

export interface ITestPlanLocalTestCase {
  key: number;
  id: number;
  testCaseName: string;
  scenario: string;
  action: number | string;
}

export interface TestPlanState {
  selectedTestCases: ITestPlanLocalTestCase[];
  newTestCasesForTestPlan: ITestPlanLocalTestCase[];
  testCasesToBeDeleteIds: number[];
}

export interface ITestPlanTestCasesParams {
  pageNumber: number;
  pageSize: number;
  searchValue: string;
  testPlanId: number | string;
}

export interface ITestPlanTestCases {
  testPlanTestCaseId: number;
  testCaseId: number;
  testPlanName: string;
  description: string;
  title: string;
  projectModuleId: number;
  testCaseName: string;
  scenario: string;
  expectedResult: string;
  author: string;
}

export interface IDragDropOrderingViewChildren {
  testPlanId: number;
  parentTestPlanId: number;
  testPlanName: string;
  orderDate: string;
  title: string;
  projectId: number;
  projectSlug: string;
  description: string | null;
  testPlanTypeListItemId: number;
  testPlanType: string;
  testPlanChildModule: IDragDropOrderingViewChildren[];
}

export interface IDragDropTestPlanBody {
  testPlanId: number;
  parentTestPlanId: number | null;
  dragDropTestPlanId: number | null;
  projectSlug: string;
  searchValue: string;
  testPlanTypeListItemId: number;
  dragDropOrderingView: IDragDropOrderingViewChildren[];
}

export interface ITestRunTestPlanList {
  testPlanId: number;
  timeSpent?: any;
  testRunName: string;
  updateDate: string;
  blockedCount: number;
  passedCount: number;
  failedCount: number;
  pendingCount: number;
  testRunId: number;
}
