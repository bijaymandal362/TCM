import { MenuOutlined } from "@ant-design/icons";
import { Button, Row, Col, Breadcrumb, Divider, Spin } from "antd";
import { useAppDispatch } from "../store/reduxHooks";
import { useRouteMatch } from "react-router";
import { Link } from "react-router-dom";
import { toggleDrawer } from "../store/features/projectSlice";
import { useGetProjectNameFromProjectSlugQuery } from "../store/services/main/project-dashboard";

interface Props {
  projectName: string;
  routeName: string;
}

export const BreadcrumHeader = (props: Props) => {
  const dispatch = useAppDispatch();
  const route = useRouteMatch<{ projectSlug: string }>();

  const getProjectNameFromProjectSlug = useGetProjectNameFromProjectSlugQuery(
    route.params.projectSlug
  );

  return (
    <>
      <Row>
        <Col xs={8} lg={0}>
          <Button
            className="mobile__menu__toggle"
            onClick={() => {
              dispatch(toggleDrawer());
            }}
          >
            <MenuOutlined />
          </Button>
        </Col>
        <Col xs={12} lg={12}>
          <Breadcrumb>
            <Breadcrumb.Item>
              {getProjectNameFromProjectSlug.isLoading ? (
                <Spin />
              ) : (
                getProjectNameFromProjectSlug.data?.projectName
              )}
            </Breadcrumb.Item>
            <Breadcrumb.Item>
              <Link to="/">{props?.routeName}</Link>
            </Breadcrumb.Item>
          </Breadcrumb>
        </Col>
        <Divider />
      </Row>
    </>
  );
};

export default BreadcrumHeader;
