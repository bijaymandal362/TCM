import React from "react";
import { Avatar, Typography } from "antd";
import TestRunStatus from "./TestRunStatus";
import { Status, TeamStatsUserData } from "../../../interfaces";
import moment from "moment";
import "moment-duration-format";

interface TeamStatsProps {
  user: TeamStatsUserData;
  timeSpent: string;
  testRunData: Status[];
}

const TeamStats: React.FC<TeamStatsProps> = (props): JSX.Element => {
  return (
    <div className="team-stats">
      <div className="user-details">
        <Avatar size="large">{props.user.name.charAt(0)}</Avatar>
        <div>
          <Typography.Title level={5}>
            {props.user.username ?? props.user.name}
          </Typography.Title>
          <Typography.Paragraph>{props.user.roleName}</Typography.Paragraph>
        </div>
      </div>
      <div className="time-spent">
        <Typography.Title level={5}>
          {props.timeSpent
            ? moment
                .utc(
                  moment.duration(props.timeSpent, "seconds").asMilliseconds()
                )
                .format("HH:mm:ss")
            : "00:00:00"}
        </Typography.Title>
        <Typography.Paragraph>Time spent</Typography.Paragraph>
      </div>
      <div className="stats">
        {props.testRunData.map((item) => (
          <div key={item.statusId}>
            <Typography.Title level={5}>{item.statusCount}</Typography.Title>
            <TestRunStatus
              type="text"
              status={item.status.toLowerCase() as any}
            >
              {item.status}
            </TestRunStatus>
          </div>
        ))}
      </div>
    </div>
  );
};

export default TeamStats;
