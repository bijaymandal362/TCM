import { configureStore } from "@reduxjs/toolkit";
import { setupListeners } from "@reduxjs/toolkit/query";
// features - sync
import projectReducer from "./features/projectSlice";
import testPlanReducer from "./features/testPlanSlice";
// services - async
import { mainServerApi } from "./services/mainServer";

export const store = configureStore({
  reducer: {
    project: projectReducer,
    testPlan: testPlanReducer,
    [mainServerApi.reducerPath]: mainServerApi.reducer,
  },
  devTools: process.env.NODE_ENV === "development",
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware({
      serializableCheck: false,
    }).concat(mainServerApi.middleware),
});

setupListeners(store.dispatch);

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
