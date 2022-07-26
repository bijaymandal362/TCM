import {
  Card,
  Col,
  Divider,
  message,
  PageHeader,
  Row,
  Spin,
  Typography,
} from "antd";
import UserCard from "./components/UserCard";
import Statistics from "./components/Statistics";
import { Line } from "react-chartjs-2";
import {
  useGetProjectsCountQuery,
  useGetTestCaseCountFromLastMonthQuery,
  useGetUsersCountQuery,
} from "../../store/services/main/admin-dashboard";

import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Legend,
} from "chart.js";

ChartJS.register(
  Tooltip,
  Legend,
  PointElement,
  LineElement,
  CategoryScale,
  LinearScale,
  Title
);

interface ITestCaseChart {
  date: string;
  testCaseCountPerDay: number;
  day: number;
}

const options = {
  responsive: true,
  plugins: {
    legend: {
      position: "top" as const,
    },
    title: {
      display: true,
      text: "Test Case Count From Last Month",
    },
  },
};

const AdminDashboard = () => {
  const {
    data: getProjectsCountData,
    isError: isGetProjectsCountDataError,
    isLoading: getProjectsCountDataLoading,
  } = useGetProjectsCountQuery();

  isGetProjectsCountDataError && message.error("something went wrong");

  const { data: getUsersCountData, isLoading: getUsersCountDataLoading } =
    useGetUsersCountQuery();
  const { data: testCaseCountFromLastMonth } =
    useGetTestCaseCountFromLastMonthQuery();

  const data = {
    labels: testCaseCountFromLastMonth?.map(
      (item: ITestCaseChart) => item.date
    ),
    datasets: [
      {
        label: "Test case number",
        data: testCaseCountFromLastMonth?.map(
          (item: ITestCaseChart) => item.testCaseCountPerDay
        ),
        borderColor: "rgb(53, 162, 235)",
        backgroundColor: "rgb(53, 162, 235)",
      },
    ],
  };

  return (
    <div className="mx-auto">
      <PageHeader title="Dashboard" className="py-0 px-3" />
      <div>
        <Divider className="my-2" />
        <div className="pt__15">
          <Row gutter={24}>
            <Col md={12} sm={24}>
              {!getProjectsCountDataLoading ? (
                <UserCard
                  title="Projects"
                  count={getProjectsCountData as number}
                  isUser={false}
                />
              ) : (
                <Spin />
              )}
            </Col>

            <Col md={12} sm={24}>
              {!getUsersCountDataLoading ? (
                <UserCard
                  title="Users"
                  count={getUsersCountData as number}
                  isUser={true}
                />
              ) : (
                <Spin />
              )}
            </Col>

            <Col md={12} sm={24}>
              <Statistics />
            </Col>
            <Col md={24}>
              <Card>
                <Typography.Title level={2}>Test Case Chart</Typography.Title>
                <div style={{ padding: "40px" }}>
                  <Line options={options} data={data} />
                </div>
              </Card>
            </Col>
          </Row>
        </div>
      </div>
    </div>
  );
};

export default AdminDashboard;
