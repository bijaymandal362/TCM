import { Tooltip } from "antd";

interface ICustomStatusBar {
  counts: number[];
  total: number;
  displayCount?: boolean;
}

const status = ["Pending", "Passed", "Failed", "Blocked"];

const CustomStatusBar = ({
  counts,
  total,
  displayCount = true,
}: ICustomStatusBar) => {
  return (
    <div className="custom-status-bar">
      {total ? (
        <>
          {status.map((item, index) => {
            const lowerCaseStatus = item.toLowerCase();
            const width = (counts[index] / total) * 100;
            return (
              <Tooltip key={item} title={`${counts[index]} ${item}`}>
                {counts[index] !== 0 ? (
                  <span
                    style={{
                      width: `${width < 10 ? 10 : width}%`,
                    }}
                    className={`${lowerCaseStatus} ${
                      displayCount ? "" : "hide-count"
                    }`}
                  >
                    {counts[index]}
                  </span>
                ) : (
                  <></>
                )}
              </Tooltip>
            );
          })}
        </>
      ) : (
        <span
          style={{
            width: "100%",
          }}
        >
          No Data
        </span>
      )}
    </div>
  );
};

export default CustomStatusBar;
