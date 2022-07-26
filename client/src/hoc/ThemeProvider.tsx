import React, { lazy, Suspense } from "react";
import { ThemeList } from "../enum/enum";
import { getTheme, setTheme as setThemeLocal } from "../util/localStorage.util";

const LightTheme = lazy(() => import("../assets/themes/LightTheme"));
const DarkTheme = lazy(() => import("../assets/themes/DarkTheme"));

export const ThemeContext = React.createContext({
  toggleTheme: () => {},
  theme: ThemeList.DARK,
});

interface Props {
  children: React.ReactNode;
}

export const ThemeProvider = (props: Props) => {
  const changeTheme = (newTheme: ThemeList) => {
    setThemeLocal(newTheme);
  };

  const toggleTheme = () => {
    if (getTheme() === ThemeList.Light) {
      changeTheme(ThemeList.DARK);
      window.location.reload();
    } else {
      changeTheme(ThemeList.Light);
      window.location.reload();
    }
  };

  return (
    <Suspense fallback={<></>}>
      {getTheme() === ThemeList.Light ? <LightTheme /> : <DarkTheme />}
      <ThemeContext.Provider
        value={{ theme: getTheme() as ThemeList, toggleTheme }}
      >
        {props.children}
      </ThemeContext.Provider>
    </Suspense>
  );
};
