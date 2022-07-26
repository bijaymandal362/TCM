export interface GetTestRunListParams {
  projectSlug: string;
  PageNumber: number;
  SearchValue: string;
  PageSize: number;
}

export interface TestRunList {
  testRunId: number;
  title: string;
  environment: string;
  time: string | null;
  status: Status[];
}

export interface GetEditTestRun {
  testRunId: number;
  title: string;
  environment: string;
  environmentId: number;
  assigneeId: number;
  description: string;
  testPlanId: number[];
}

export interface AddTestRunBody {
  title: string;
  description: string;
  testPlanId: number[];
  environmentId: number;
  defaultAssigneeProjectMemberId: number;
  projectSlug: string;
}

export interface UpdateTestRunBody {
  testRunId: number;
  title: string;
  description: string;
  environmentId: number;
}

export interface TestRunTestPlans {
  testPlanId: number;
  testPlanTitle: string;
  testCases: TestCase[];
}

export interface TestCase {
  testCaseTitle: string;
  assignee: string | null;
  timeSpent: string | null;
  results: string;
  projectModuleId: number;
  testPlanTestCaseId: number;
  testRunTestCaseHistoryId: number;
  testRunHistoryId: number;
}

export interface TestRunTeamStats {
  id: number;
  user: TeamStatsUserData;
  timeSpent: string;
  status: Status[];
}

export interface TeamStatsUserData {
  image: string | null;
  name: string;
  username: string | null;
  userId: number;
  role: number;
  roleName: number;
}

export interface TestRunData {
  testRunId: number;
  name: string;
  description: string;
  status: Status[];
  completionRate: string;
  startedBy: number;
  timeSpent: string | null;
}

export interface Status {
  statusId: number;
  status: string;
  statusCount: number;
}

export interface AssignAndUnAssignUserBody {
  assigneProjectMemberId: number | null;
  testRunTestCaseHistoryId: number[];
}

export interface GetTestRunTestCaseParams {
  testRunID: number;
  testCaseID: number;
  testPlanID: number;
}

export interface TestRunTestCase {
  testRunTestCaseHistoryId: number;
  testCaseName: string;
  testCaseScenario: string;
  preConditions: string;
  environment: string;
  environmentId: number;
  testRunStatusListItemId: number;
  status: string;
  stepsToReproduce: TestCaseStepsToReproduce[];
  testRunTestCaseHistoryDocumentId: number | null;
  fileName: string | null;
  extension: string | null;
  comment: string;
  historyDocumentId: number | null;
}

export interface TestRunTestCaseHistory {
  testRunTestCaseHistoryId: number;
  timeSpent?: any;
  projectModuleId: number;
  testRunStatusListItemId: number;
  testRunName: string;
  status: string;
  updateDate: string;
}

export interface TestRunTestCaseHistoryParams {
  pageNumber: number;
  searchValue: string;
  pageSize: number;
  testCaseId: number;
}

export interface TestPlan {
  testPlanId: number;
  testPlanName: string;
}

export interface ProjectMember {
  projectMemberId: number;
  name: string;
  userName: string;
}

export interface TestRunResults {
  id: number;
  testCaseTitle: string;
  results: TestCaseResult[];
}

export interface DeleteTestCaseParams {
  testRunId: number;
  projectModuleId: number;
  testPlanId: number;
}

export interface DeleteAndRetestMultipleTestCaseParams {
  testRunId: number;
  testPlan: TestPlanAndCaseIdsForDeleteAndRetest[];
}

export interface EnvironmentList {
  environmentId: number;
  environmentName: string;
}

export interface TestPlanAndCaseIdsForDeleteAndRetest {
  testPlanId: number;
  testCaseId: number[];
}

export interface Result {
  result: number;
  status: string;
}

export interface getTestRunTestPlanByTestRunIdParams {
  testRunId: number;
  pageNumber: number;
  searchValue: string;
  pageSize: number;
}

export interface getTestRunTestCaseDetailsParams {
  testRunId: number;
  testPlanId: number;
  pageNumber: number;
  searchValue: string;
  pageSize: number;
  filters: {
    assignee: number | string;
  };
}

export interface TestRunTestPlansByTestRunId {
  testRunId: number;
  testPlanId: number;
  testPlanName: string;
  blockedCount: number;
  failedCount: number;
  passedCount: number;
  pendingCount: number;
}

export interface TestRunTestCaseDetails {
  testRunId: number;
  testPlanId: number;
  projectModuleId: number;
  testCaseName: string;
  totalTimeSpent?: any;
  assigneeListItemId?: any;
  testRunTestCaseStatusListItemId: number;
  assignee?: any;
  testRunTestCaseStatusListItemSystemName: string;
  insertDate: string;
  testRunTestCaseHistoryId: number;
}

interface TestCaseResult {
  userId: number;
  user: string | null;
  timeSpent?: any;
  status: string;
  finishTime?: any;
  comment?: any;
  stepsToReproduce: TestCaseStepsToReproduce[];
  documentId: number | null;
  fileName: string | null;
}

interface TestCaseStepsToReproduce {
  testCaseStepResultId: number;
  testRunTestCaseStepHistoryId: number;
  stepDescription: string;
  step: string;
  expectedResult: string;
  status: string;
  testRunTestCaseHistoryDocumentId: number | null;
  fileName: string | null;
  documentId: number | null;
  comment: string | null;
  stepHistoryDocumentId: number | null;
}
