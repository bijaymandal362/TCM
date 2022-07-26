export interface GetProjectsParams {
  PageNumber: number;
  SearchValue: string;
  PageSize: number;
}

export interface CreateNewProjectBody {
  projectName: string;
  startDate: string;
  projectMarketListItemId: number;
  projectDescription: string;
}

export interface Project {
  date: string;
  projectDescription?: string;
  projectId: number;
  projectMarket: string;
  projectMarketListItemId: number;
  projectName: string;
  projectRole: string;
  projectRoleId: number;
  projectSlug: string;
  testCaseCount: number;
  testRunCount: number;
  testPlanCount: number;
}

export interface ProjectState {
  drawerVisible: boolean;
  isSidebarCollapsed: boolean;
  projectPermissions: string[];
}
