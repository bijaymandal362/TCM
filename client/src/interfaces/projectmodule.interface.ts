import { ReactNode } from "react";
import { TestTypes } from "../enum/enum";

export interface ProjectModuleTreeInterface {
  childModule: any[];
  moduleName: string;
  parentProjectModuleId: number | null;
  projectId: number;
  projectModuleId: number;
  projectModuleListItemId: number;
  projectModuleType: TestTypes;
  description: string;
  expectedResult: string;
  icon: ReactNode;
}

export interface FormattedModuleTreeInterface {
  title: string;
  key: number;
  children?: FormattedModuleTreeInterface[];
  type: TestTypes;
  description: string;
  expectedResult: string;
  parentProjectModuleId: number | null;
  // icon:ReactNode
  isLeaf?: boolean;
}
