import BreadcrumHeader from "../../components/breadcrumHeader";

interface Props {
  match: {
    params: {
      projectId: string;
    };
  };
}

export const ProjectDashboard = (props: Props) => {
  return (
    <>
      <BreadcrumHeader projectName="TestMink" routeName="Dashboard" />
    </>
  );
};

export default ProjectDashboard;
