import React from "react";
interface IProps {
  title: string;
  count: number;
}

export const StatsData = ({ title, count }: IProps) => {
  return (
    <div
      style={{
        display: "flex",
        alignItems: "center",
        justifyContent: "space-between",
        marginBottom: "10px",
      }}
    >
      <p style={{ fontWeight: "bolder" }}>{title}</p>
      <p style={{ fontWeight: "bolder" }}>{count}</p>
    </div>
  );
};
