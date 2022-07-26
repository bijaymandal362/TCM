import { RightOutlined } from "@ant-design/icons";
import { Card, Typography } from "antd";
import React from "react";
import { Link } from "react-router-dom";

interface IProps {
  title: string;
  count: number;
  isUser: boolean;
}

const UserCard = ({ title, count, isUser }: IProps) => {
  return (
    <Card
      title={
        <>
          <Typography.Title level={3} style={{ marginBottom: "10px" }}>
            {title.toUpperCase()}
          </Typography.Title>
          <p style={{ fontSize: "18px" }}>{count}</p>
        </>
      }
    >
      <div>
        <Link to={!isUser ? "/" : "/admin/users"} className="user-card-bottom">
          {`View ${title}`}
          <RightOutlined />
        </Link>
      </div>
    </Card>
  );
};

export default UserCard;
