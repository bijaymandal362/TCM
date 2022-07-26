import { FilterFilled, PlusOutlined } from "@ant-design/icons";
import { Button, Form, Select, Tag } from "antd";
import React, { useState } from "react";

export type FilterState = { filterKey: string; filterValue: string };
export type FilterData = { key: string; values: string[] }[];

interface TableFilterProps {
  currentFilters: FilterState[];
  setCurrentFilters: React.Dispatch<React.SetStateAction<FilterState[]>>;
  filterData: FilterData;
  filterInputWidth: number;
  filterValueWidth: number;
  setFilterClose: (value: boolean) => void;
}

const TableFilter = (props: TableFilterProps) => {
  const [isFilterOpen, setIsFilterOpen] = useState(false);
  const [isFilterSelected, setIsFilterSelected] = useState(false);
  const [selectedFilterKey, setSelectedFilterKey] = useState<string | null>(
    null
  );
  const handleFilterButtonClick = () => {
    setIsFilterOpen((prevState) => !prevState);
    setIsFilterSelected(false);
    props.setFilterClose(false);
  };

  const handleSelectFilter = (value: string) => {
    if (value === "none") {
      setIsFilterSelected(false);
      setSelectedFilterKey(null);
    } else {
      setIsFilterSelected(true);
      setSelectedFilterKey(value);
    }
  };

  const handleTagClose = (e: any, filter: FilterState) => {
    // console.log(e, "here");
    e.preventDefault();
    setIsFilterSelected(false);

    props.setCurrentFilters((prevState) => {
      return prevState.filter(
        (prevFilter) =>
          !(
            prevFilter.filterKey === filter.filterKey &&
            prevFilter.filterValue === filter.filterValue
          )
      );
    });
    props.setFilterClose(true);
  };

  const handleFinish = (values: FilterState) => {
    if (values.filterKey !== "none" && values.filterValue !== "none") {
      setIsFilterOpen((prevState) => !prevState);

      let isDuplicate = false;
      props.currentFilters.forEach((filter) => {
        if (
          filter.filterKey === values.filterKey &&
          filter.filterValue === values.filterValue
        ) {
          isDuplicate = true;
        }
      });

      if (!isDuplicate) {
        props.setCurrentFilters((prevState) => [...prevState, values]);
      }
    }
  };
  return (
    <div className="table-filter-container">
      <Button
        icon={<FilterFilled />}
        onClick={handleFilterButtonClick}
        disabled={isFilterSelected ? true : false}
      />
      {isFilterOpen ? (
        <Form
          layout="inline"
          onFinish={handleFinish}
          initialValues={{
            filterKey: "none",
            filterValue: "none",
          }}
        >
          <Form.Item name="filterKey">
            <Select
              style={{ width: props.filterInputWidth }}
              onChange={handleSelectFilter}
              showArrow={false}
            >
              <Select.Option value="none">Select Filter</Select.Option>
              {props.filterData.map((filter) => (
                <Select.Option key={filter.key} value={filter.key}>
                  {filter.key}
                </Select.Option>
              ))}
            </Select>
          </Form.Item>
          {isFilterSelected ? (
            <Form.Item name="filterValue">
              <Select
                style={{ width: props.filterValueWidth }}
                showArrow={false}
              >
                <Select.Option value="none">Select Market</Select.Option>
                {props.filterData.map(
                  (filter) =>
                    filter.key === selectedFilterKey &&
                    filter.values.map((value) => (
                      <Select.Option key={value} value={value}>
                        {value}
                      </Select.Option>
                    ))
                )}
              </Select>
            </Form.Item>
          ) : null}
          <Button icon={<PlusOutlined />} htmlType="submit" />
        </Form>
      ) : null}

      <div className="filter-tags">
        {props.currentFilters.length
          ? props.currentFilters.map((filter) => (
              <Tag
                key={`${filter.filterKey}-${filter.filterValue}`}
                closable
                onClose={(e) => handleTagClose(e, filter)}
              >
                {filter.filterKey} = {filter.filterValue}
              </Tag>
            ))
          : null}
      </div>
    </div>
  );
};

export default TableFilter;
