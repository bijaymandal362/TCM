export interface IAdminData {
  firstname: string;
  lastname: string;
  phoneNumber: string;
  email: string;
}

export interface Projects {
  projectId: number;
  projectName: string;
  projectMarketName: string;
  projectStartDate: string;
  testCaseCount: number;
  testPlancount: number;
  testRunCount: number;
}

export interface ProjectsGetParams {
  searchValue: string;
  pageNumber: number;
  pageSize: number;
  filterValue: string;
}
