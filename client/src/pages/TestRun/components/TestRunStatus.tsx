import React from "react";
import { Typography, Tag, Button } from "antd";

interface TestRunStatusProps {
  type: "text" | "tag" | "button";
  status: "passed" | "failed" | "blocked" | "pending";
  children: JSX.Element | string;
  icon?: JSX.Element;
  size?: "small" | "middle" | "large";
  onClick?: () => any;
  className?: any;
}

const TestRunStatus: React.FC<TestRunStatusProps> = ({
  type,
  status,
  children,
  className,
  ...otherProps
}): JSX.Element => {
  const getStatus = (): JSX.Element => {
    if (type === "text") {
      return (
        <Typography.Paragraph
          className={`${status} ${className ? className : ""}`}
          {...otherProps}
        >
          {children}
        </Typography.Paragraph>
      );
    }

    if (type === "tag") {
      return (
        <Tag
          className={`${status} ${className ? className : ""}`}
          {...otherProps}
        >
          {children}
        </Tag>
      );
    }

    if (type === "button") {
      return (
        <Button
          type="text"
          className={`status-btn-hover ${status} ${className ? className : ""}`}
          {...otherProps}
        >
          {children}
        </Button>
      );
    }

    return <></>;
  };

  return getStatus();
};

export default TestRunStatus;
