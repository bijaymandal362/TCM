// @ts-ignore
import NProgress from "nprogress";
import { useEffect } from "react";

// import "./nprogress.less";

export const LazyLoadProgressBar = () => {
  useEffect(() => {
    NProgress.start();

    return () => {
      NProgress.done();
      // NProgress.remove();
    };
  }, []);

  return null;
  // return <Spin className={style['suspense-fallback-loader']} />;
};
