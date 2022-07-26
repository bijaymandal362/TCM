import { RouteProps } from "react-router-dom";

export interface IRouteItem extends RouteProps {
  name: string;
  path: string;
  exact?: boolean;
  LazyComponent?: any;
  icons?: React.ReactNode;
  permission?: any;
}

export interface NavLinks {
  href: string;
  icon: React.ReactNode;
  title: string;
  children?: Array<NavLinks>;
  permission?: any;
}
