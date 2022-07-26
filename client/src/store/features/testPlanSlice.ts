import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { message } from "antd";
import { ITestPlanLocalTestCase, TestPlanState } from "../../interfaces";
import { getUniqueListBy } from "../../util/functions.utils";

const initialState: TestPlanState = {
  selectedTestCases: [],
  newTestCasesForTestPlan: [],
  testCasesToBeDeleteIds: [],
};

export const testPlanSlice = createSlice({
  name: "testPlan",
  initialState,
  reducers: {
    addSelectedTestCases: (
      state,
      action: PayloadAction<ITestPlanLocalTestCase[]>
    ) => {
      state.selectedTestCases = action.payload;
    },
    addNewTestCasesForTestPlan: (
      state,
      action: PayloadAction<ITestPlanLocalTestCase[]>
    ) => {
      const uniqueDataList = getUniqueListBy(
        [...state.newTestCasesForTestPlan, ...action.payload],
        "testCaseName"
      );

      // for displaying error message
      const duplicateCount =
        state.newTestCasesForTestPlan.length +
        action.payload.length -
        uniqueDataList.length;
      if (duplicateCount !== 0) {
        message.error(
          `${duplicateCount} duplicate test case(s) found and are not added.`
        );
      }

      state.newTestCasesForTestPlan = uniqueDataList;
    },
    moveSelectedTestCases: (state) => {
      const uniqueDataList = getUniqueListBy(
        [...state.newTestCasesForTestPlan, ...state.selectedTestCases],
        "testCaseName"
      );

      // for displaying error message
      const duplicateCount =
        state.newTestCasesForTestPlan.length +
        state.selectedTestCases.length -
        uniqueDataList.length;
      if (duplicateCount !== 0) {
        message.error(`The test case is already added.`);
      }

      state.newTestCasesForTestPlan = uniqueDataList;
    },
    deleteNewTestCasesFromTestPlan: (state, action: PayloadAction<number>) => {
      state.newTestCasesForTestPlan = state.newTestCasesForTestPlan.filter(
        (item) => item.id !== action.payload
      );
    },
    deleteFetchedTestCasesFromTestPlan: (
      state,
      action: PayloadAction<number>
    ) => {
      state.testCasesToBeDeleteIds.push(action.payload);
    },
    clearTestCasesProjectModuleId: (state) => {
      state.selectedTestCases = [];
      state.newTestCasesForTestPlan = [];
      state.testCasesToBeDeleteIds = [];
    },
  },
});

export const {
  addSelectedTestCases,
  addNewTestCasesForTestPlan,
  moveSelectedTestCases,
  deleteNewTestCasesFromTestPlan,
  deleteFetchedTestCasesFromTestPlan,
  clearTestCasesProjectModuleId,
} = testPlanSlice.actions;

export default testPlanSlice.reducer;
