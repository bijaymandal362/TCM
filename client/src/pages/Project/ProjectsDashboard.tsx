import { Divider, PageHeader } from "antd";
import React, { useEffect, useRef, useState } from "react";
import DashboardTable from "./ProjectsDashboard/DashboardTable";
import DashboardTop from "./ProjectsDashboard/DashboardTop";
import { useLazyGetProjectsQuery } from "../../store/services/main/admin-dashboard";
import { Pagination } from "../../interfaces";
import { Projects } from "../../interfaces/admin.interface";
import { FilterState } from "./ProjectsDashboard/TableFilter";
import { useDebounce } from "use-debounce/lib";

const ProjectsDashboard = () => {
  const [page, setPage] = useState<number>(1);
  const [search, setSearch] = useState<string>("");
  const [pageSize, setPageSize] = useState<number>(10);
  const [currentFilters, setCurrentFilters] = useState<FilterState[]>([]);
  const [filterClose, setFilterClose] = useState(false);
  const [filter, setFilter] = useState("");
  const [debouncedSearch] = useDebounce(search, 500);

  const [getProjectsDataTrigger, getProjectsData] = useLazyGetProjectsQuery();
  const getProjects = () => {
    getProjectsDataTrigger({
      filterValue: filter,
      pageNumber: page,
      searchValue: debouncedSearch,
      pageSize: pageSize,
    });
  };
  useEffect(() => {
    if (filterClose) {
      setFilter("");
    } else {
      setFilter(currentFilters.map((filter) => filter.filterValue)[0]);
    }
  }, [filterClose, currentFilters]);

  useEffect(() => {
    getProjects();
  }, [page, pageSize, debouncedSearch, filter, search]);

  console.log(getProjectsData, "hhh");
  return (
    <div className="mx-auto">
      <PageHeader title="Projects" className="py-0 px-3" />
      <div>
        <Divider className="my-2" />
        <DashboardTop
          currentFilters={currentFilters}
          setCurrentFilters={setCurrentFilters}
          setSearch={setSearch}
          setFilterClose={setFilterClose}
        />
        <DashboardTable
          getProjectsData={getProjectsData}
          setPage={setPage}
          page={page}
          setPageSize={setPageSize}
          pageSize={pageSize}
          currentFilters={currentFilters}
        />
      </div>
    </div>
  );
};
export default ProjectsDashboard;
