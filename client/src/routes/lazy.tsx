import lazyLib from "@loadable/component";

import { LazyLoadProgressBar } from "../components/lazyLoadProgressBar";

// @ts-ignore
export const lazy = (component: any) =>
  lazyLib(component, { fallback: <LazyLoadProgressBar /> });
