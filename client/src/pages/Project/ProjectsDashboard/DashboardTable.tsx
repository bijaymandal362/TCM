import React, { useEffect, useState } from "react";
import { DeleteOutlined, EditOutlined } from "@ant-design/icons";
import { Space, Table, Tooltip } from "antd";
import { Pagination } from "../../../interfaces";
import { Projects } from "../../../interfaces/admin.interface";
import { FilterState } from "./TableFilter";

interface Iprops {
  getProjectsData: any;
  setPage: (value: number) => void;
  page: number;
  setPageSize: (value: number) => void;
  pageSize: number;
  currentFilters: FilterState[];
}

const columns = [
  {
    title: "Name",
    dataIndex: "projectName",
    key: "name",
  },
  {
    title: "Market",
    dataIndex: "projectMarketName",
    key: "market",
  },
  {
    title: "Start date",
    dataIndex: "projectStartDate",
    key: "start-date",
  },
  {
    title: "Stat",
    dataIndex: "stat",
    key: "stat",
    render: (_: any, record: any) => (
      <Space size="middle">
        <p>{`TC ${record.testCaseCount}`}</p>
        <p>{`TP ${record.testPlanCount}`}</p>
        <p>{`TR ${record.testRunCount}`}</p>
      </Space>
    ),
  },
  {
    title: "Action",
    dataIndex: "action",
    key: "action",
    render: (text: any, record: any) => (
      <Space size="middle">
        <Tooltip title="Edit">
          <EditOutlined className="table-action" />
        </Tooltip>
        <Tooltip title="Delete">
          <DeleteOutlined className="table-action" />
        </Tooltip>
      </Space>
    ),
  },
];
const DashboardTable = (props: Iprops) => {
  return (
    <div className="project-dashboard-table-wrapper">
      <Table
        rowKey={(record) => record.projectId}
        columns={columns}
        loading={props.getProjectsData?.isLoading}
        dataSource={props.getProjectsData?.data?.data}
        pagination={{
          onChange: (page: number, pagesize: any) => {
            props.setPage(page);
            props.setPageSize(pagesize);
          },
          pageSize: props.pageSize,
          total: props.getProjectsData?.data?.totalRecords,
          showSizeChanger: true,
          pageSizeOptions: ["10", "20", "30"],
        }}
      />
    </div>
  );
};

export default DashboardTable;
