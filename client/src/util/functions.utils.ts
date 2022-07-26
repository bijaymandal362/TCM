import { useMemo } from "react";
import { useLocation } from "react-router";
import { Status } from "../interfaces";

export const getUniqueListBy = (arr: any[], key: string) => {
  return [...new Map(arr.map((item) => [item[key], item])).values()];
};

export const getSelectedTestCasesIds = (
  testCasesIds: Record<string, boolean>
): number[] => {
  const newTestCasesIds: any = {};

  Object.keys(testCasesIds).forEach((key) => {
    if (testCasesIds[key]) {
      newTestCasesIds[key] = testCasesIds[key];
    }
  });

  return Object.keys(newTestCasesIds).map((id) => +id);
};

export const getResult = (
  results: Status[]
): { status: string; total: number } => {
  const clonedResults = [...results];

  const statuses = clonedResults.filter((item: Status) => item.statusCount);
  const total = clonedResults.reduce((acc, item) => acc + item.statusCount, 0);

  return {
    status: statuses[statuses.length - 1].status,
    total,
  };
};

// A custom hook that builds on useLocation to parse the query string for you.
export function useRouterQuery() {
  const { search } = useLocation();

  return useMemo(() => new URLSearchParams(search), [search]);
}

export const getTodayDate = () => {
  const months = [
    "Jan",
    "Feb",
    "Mar",
    "Apr",
    "May",
    "Jun",
    "Jul",
    "Aug",
    "Sep",
    "Oct",
    "Nov",
    "Dec",
  ];

  const date = new Date();

  const year = date.getFullYear();
  const month = date.getMonth();
  const day = date.getDate();

  return `${day} ${months[month]}, ${year}`;
};
