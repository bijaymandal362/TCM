import { FilterFilled } from "@ant-design/icons";
import { Button, Input } from "antd";
import React, { useState } from "react";
import TableFilter, { FilterData, FilterState } from "./TableFilter";

interface IProps {
  currentFilters: FilterState[];
  setCurrentFilters: (value: any) => void;
  setSearch: (value: string) => void;
  setFilterClose: (value: boolean) => void;
}

const DashboardTop = (props: IProps) => {
  const filterData = [
    {
      key: "Market",
      values: ["Nepal", "Japan", "USA", "Singapore", "UK"],
    },
  ];

  return (
    <div
      style={{
        display: "flex",
        justifyContent: "flex-start",
        alignItems: "center",
      }}
    >
      <Input
        placeholder="search for project"
        onChange={(e) => props.setSearch(e.target.value)}
        style={{ width: "300px" }}
      />
      <TableFilter
        currentFilters={props.currentFilters}
        setCurrentFilters={props.setCurrentFilters}
        filterData={filterData}
        filterInputWidth={185}
        filterValueWidth={135}
        setFilterClose={props.setFilterClose}
      />
    </div>
  );
};

export default DashboardTop;
