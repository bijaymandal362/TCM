import { createSlice } from "@reduxjs/toolkit";
import { ProjectState } from "../../interfaces";

const initialState: ProjectState = {
  drawerVisible: false,
  isSidebarCollapsed: false,
  projectPermissions: [],
};

export const projectSlice = createSlice({
  name: "project",
  initialState,
  reducers: {
    toggleDrawer: (state) => {
      state.drawerVisible = !state.drawerVisible;
    },
    toggleSidebarCollapse: (state) => {
      state.isSidebarCollapsed = !state.isSidebarCollapsed;
    },
    collapseSidebar: (state) => {
      state.isSidebarCollapsed = true;
    },
    getProjectPermissions: (state, action) => {
      state.projectPermissions = action.payload;
    },
  },
});

export const {
  toggleDrawer,
  toggleSidebarCollapse,
  collapseSidebar,
  getProjectPermissions,
} = projectSlice.actions;

export default projectSlice.reducer;
