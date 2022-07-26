import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";

const baseUrl = process.env.REACT_APP_BASE_URL;
// const baseUrl =
//   "https://f0992a09-2cad-45f5-8e8e-088daf8baa43.mock.pstmn.io/api";

export const mainServerApi = createApi({
  baseQuery: fetchBaseQuery({ baseUrl }),
  endpoints: () => ({}),
  tagTypes: [
    "Preferences",
    "Projects",
    "ProjectDashboard",
    "TestPlan",
    "TestPlanTestCase",
    "TestRun",
    "TestRunTestCase",
    "TestRunEditData",
    "TestRunWizard",
    "TestRunList",
  ],
});
