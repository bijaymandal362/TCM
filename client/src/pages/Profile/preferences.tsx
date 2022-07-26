import {
  Typography,
  Divider,
  Row,
  Col,
  Radio,
  Select,
  Spin,
  message,
} from "antd";
import { useContext } from "react";
import timeZones from "../../assets/data/timezones";
import { ThemeList } from "../../enum/enum";
import { ThemeContext } from "../../hoc/ThemeProvider";
import {
  useGetUserPreferencesQuery,
  useUpdateUserPreferencesMutation,
} from "../../store/services/main/preferences";

const appearances = [
  {
    id: 36,
    name: "Light Mode",
    color: "#FFF",
    checked: ThemeList.Light,
  },
  {
    id: 35,
    name: "Dark Mode",
    color: "#1B2531",
    checked: ThemeList.DARK,
  },
];

const Preferences = () => {
  const { theme, toggleTheme } = useContext(ThemeContext);
  console.log(theme, "themes");
  const { data: userPreferences, isLoading: isUserPreferencesLoading } =
    useGetUserPreferencesQuery();
  const [updateUserPreferences] = useUpdateUserPreferencesMutation();

  const handleToggleTheme = (checked: "light" | "dark") => {
    if (theme !== checked) {
      handlePreferencesChange(userPreferences?.timeZone as string, true);
    }
  };

  const handlePreferencesChange = (
    value: string,
    willToggleTheme: boolean
  ): void => {
    const [{ id }] = appearances.filter((item) =>
      willToggleTheme ? item.checked !== theme : item.checked === theme
    );

    updateUserPreferences({
      personPersonalizationId:
        userPreferences?.personPersonalizationId as number,
      personId: userPreferences?.personId as number,
      themeListItemId: id,
      timeZone: value,
    })
      .unwrap()
      .then(() => {
        if (willToggleTheme) toggleTheme();
      })
      .catch(() => {
        message.error("Unable to update user preferences.");
      });
  };

  return (
    <div className="w-50 mx-auto">
      <Typography.Text strong>Preferences</Typography.Text>
      <Divider className="mt-2 mb-3" />

      <Spin spinning={isUserPreferencesLoading}>
        <Row className="justify-content-around">
          <Col span={10}>
            <Typography.Title level={3}>Appearance theme</Typography.Title>
            <Typography.Paragraph>
              You can customize the appearance theme of the whole application as
              you wish.
            </Typography.Paragraph>
          </Col>
          <Col span={12}>
            <Row>
              {appearances.map(({ id, name, color, checked }) => (
                <Col key={id} span={12} className="theme-card">
                  <div
                    style={{
                      backgroundColor: color,
                      height: "100px",
                      width: "100%",
                      cursor: "pointer",
                    }}
                    onClick={() => handleToggleTheme(checked)}
                  />
                  <Radio
                    checked={name === userPreferences?.themeType}
                    onClick={() => handleToggleTheme(checked)}
                  >
                    <Typography.Text strong>{name}</Typography.Text>
                  </Radio>
                </Col>
              ))}
            </Row>
          </Col>
        </Row>

        <Divider />

        <Row className="justify-content-around">
          <Col span={10}>
            <Typography.Title level={3}>Time settings</Typography.Title>
            <Typography.Paragraph>
              You can set your current timezone here.
            </Typography.Paragraph>
          </Col>
          <Col span={12}>
            <Typography.Paragraph strong>Time Zone</Typography.Paragraph>
            <Select
              showSearch
              style={{ width: "100%" }}
              placeholder="Select a Time Zone"
              optionFilterProp="children"
              onChange={(val) => handlePreferencesChange(val, false)}
              value={userPreferences?.timeZone}
            >
              {timeZones.map((timeZone) => (
                <Select.Option key={timeZone} value={timeZone}>
                  {timeZone}
                </Select.Option>
              ))}
            </Select>
          </Col>
        </Row>
      </Spin>
    </div>
  );
};

export default Preferences;
