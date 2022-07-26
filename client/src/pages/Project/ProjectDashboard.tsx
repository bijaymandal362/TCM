import { StarFilled, StarOutlined } from "@ant-design/icons";
import { Divider, Breadcrumb, Spin, Button, message } from "antd";
import { useEffect, useState } from "react";
import { useRouteMatch } from "react-router";
import {
  useGetProjectNameFromProjectSlugQuery,
  useStarProjectMutation,
  useUnStarProjectMutation,
} from "../../store/services/main/project-dashboard";
import ProjectDashboardCharts from "./ProjectDashboard/ProjectDashboardCharts";
import ProjectDashboardTable from "./ProjectDashboard/ProjectDashboardTable";
import ProjectDashboardTop from "./ProjectDashboard/ProjectDashboardTop";

interface Props {}

export const ProjectDashboard = (props: Props) => {
  const route = useRouteMatch<{ projectSlug: string }>();

  // States
  const [isStarred, setIsStarred] = useState(false);

  const getProjectNameFromProjectSlug = useGetProjectNameFromProjectSlugQuery(
    route.params.projectSlug,
    {
      // refetchOnFocus: true,
      refetchOnMountOrArgChange: true,
    }
  );

  const [starProject] = useStarProjectMutation();
  const [unStarProject] = useUnStarProjectMutation();

  useEffect(() => {
    setIsStarred(getProjectNameFromProjectSlug.data?.isStarredProject);
  }, [getProjectNameFromProjectSlug.data]);

  return (
    <>
      <div className="dashboard-header">
        <Breadcrumb>
          <Breadcrumb.Item>
            {getProjectNameFromProjectSlug.isLoading ? (
              <Spin />
            ) : (
              getProjectNameFromProjectSlug.data?.projectName
            )}
          </Breadcrumb.Item>
          <Breadcrumb.Item>Project Dashboard</Breadcrumb.Item>
        </Breadcrumb>

        {isStarred ? (
          <Button
            icon={<StarFilled />}
            onClick={() =>
              unStarProject(route.params.projectSlug)
                .unwrap()
                .then(() => setIsStarred(false))
                .catch(() => message.error("Failed to unstar project."))
            }
          >
            Unstar
          </Button>
        ) : (
          <Button
            icon={<StarOutlined />}
            onClick={() =>
              starProject(route.params.projectSlug)
                .unwrap()
                .then(() => setIsStarred(true))
                .catch(() => message.error("Failed to star project."))
            }
          >
            Star
          </Button>
        )}
      </div>

      <Divider />

      <ProjectDashboardTop />
      <ProjectDashboardCharts />
      <ProjectDashboardTable />
    </>
  );
};

export default ProjectDashboard;
