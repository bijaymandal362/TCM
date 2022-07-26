import { TestTypes } from "../enum/enum";
import { ProfileOutlined, BookOutlined } from "@ant-design/icons";
import {
  FormattedModuleTreeInterface,
  ProjectModuleTreeInterface,
} from "../interfaces/projectmodule.interface";
import {
  FormattedTestPlanTreeInterface,
  IDragDropOrderingViewChildren,
  IFolderAndTestPlan,
  ITestPlanLocalTestCase,
  ITestPlanTreeStructure,
} from "../interfaces";

const getIcons = (type: any) => {
  if (TestTypes.FUNCTION === type) return <BookOutlined />;
  if (TestTypes.TESTCASES === type) return <ProfileOutlined />;
};

export const formatTreeResponse = (
  data: ProjectModuleTreeInterface[]
): FormattedModuleTreeInterface[] => {
  const formattedResponse = data?.map((item: ProjectModuleTreeInterface) => {
    // console.log(item);
    if (
      item.childModule &&
      Array.isArray(item.childModule) &&
      item.childModule.length > 0
    ) {
      const children = formatTreeResponse(item.childModule);

      return {
        title: item.moduleName,
        key: item.projectModuleId,
        children: children,
        type: item.projectModuleType,
        description: item.description,
        expectedResult: item.expectedResult,
        icon: getIcons(item.projectModuleType),
        isLeaf: item?.projectModuleType === TestTypes.TESTCASES,
        parentProjectModuleId: item.parentProjectModuleId,
      };
    }
    return {
      title: item.moduleName,
      key: item.projectModuleId,
      type: item.projectModuleType,
      icon: getIcons(item.projectModuleType),
      description: item.description,
      expectedResult: item.expectedResult,
      parentProjectModuleId: item.parentProjectModuleId,
    };
  });
  return formattedResponse;
};

export const formatTestPlanTreeResponse = (
  data: IFolderAndTestPlan[]
): FormattedTestPlanTreeInterface[] => {
  const formattedTestPlanResponse = data?.map((item: IFolderAndTestPlan) => {
    if (
      item.testPlanChildModule &&
      Array.isArray(item.testPlanChildModule) &&
      item.testPlanChildModule.length > 0
    ) {
      const children = formatTestPlanTreeResponse(item.testPlanChildModule);

      return {
        disabled: item.testPlanType === "Folder" ? true : false,
        title: item.testPlanName,
        key: item.testPlanId,
        // value: item.testPlanName,
        children: children,
        type: item.testPlanType,
        description: item.description,
        icon: getIcons(item.testPlanType),
        isLeaf: item?.testPlanType === TestTypes.TESTCASES,
        parentTestPlanId: item.parentTestPlanId,
      };
    }
    return {
      disabled: item.testPlanType === "Folder" ? true : false,
      title: item.testPlanName,
      key: item.testPlanId,
      // value: item.testPlanName,
      type: item.testPlanType,
      icon: getIcons(item.testPlanType),
      description: item.description,
      parentTestPlanId: item.parentTestPlanId,
    };
  });
  return formattedTestPlanResponse;
};

export const attachChildToParent = (
  parent: any,
  child: FormattedModuleTreeInterface[]
): FormattedModuleTreeInterface[] => {
  return [
    {
      title: parent.title,
      key: parent.key,
      children: child,
      type: "Module" as TestTypes,
      description: "",
      expectedResult: "",
      // icon: undefined,
      isLeaf: false,
      parentProjectModuleId: null,
    },
  ];
};

export const formatTreeRequest = (data: any) => {
  const formatRequest = data.map((item: any) => {
    if (
      item.children &&
      Array.isArray(item.children) &&
      item.children.length > 0
    ) {
      const children = formatTreeRequest(item.children);

      return {
        moduleName: item.title,
        projectModuleId: item.key,
        childModule: children,
        projectModuleType: item.type,
        description: item.description,
        parentProjectModuleId: item.parentProjectModuleId,
      };
    }

    return {
      moduleName: item.title,
      projectModuleId: item.key,
      projectModuleType: item.type,
      description: item.description,
      parentProjectModuleId: item.parentProjectModuleId,
    };
  });

  return formatRequest;
};

export const formatTestPlanTreeStructure = (
  data: IFolderAndTestPlan[]
): ITestPlanTreeStructure[] => {
  return data.map((item: IFolderAndTestPlan) => {
    if (item.testPlanChildModule.length) {
      const parentChildren = formatTestPlanTreeStructure(
        item.testPlanChildModule
      );

      return {
        title: item.title,
        key: `${item.testPlanId}`,
        parentKey: item.parentTestPlanId,
        children: parentChildren,
        testPlanName: item.testPlanName,
        orderDate: item.orderDate,
        projectId: item.projectId,
        projectSlug: item.projectSlug,
        description: item.description,
        testPlanTypeListItemId: item.testPlanTypeListItemId,
        testPlanType: item.testPlanType,
      };
    }

    return {
      title: item.title,
      key: `${item.testPlanId}`,
      parentKey: item.parentTestPlanId,
      isLeaf: item.testPlanType !== "Folder",
      testPlanName: item.testPlanName,
      orderDate: item.orderDate,
      projectId: item.projectId,
      projectSlug: item.projectSlug,
      description: item.description,
      testPlanTypeListItemId: item.testPlanTypeListItemId,
      testPlanType: item.testPlanType,
    };
  });
};

export const formatTestPlanTreeStructureForDrapDropApi = (
  data: ITestPlanTreeStructure[]
): IDragDropOrderingViewChildren[] => {
  return data.map((item: ITestPlanTreeStructure) => {
    if (item.children?.length) {
      const parentChildren = formatTestPlanTreeStructureForDrapDropApi(
        item.children
      );

      return {
        title: item.title,
        testPlanId: +item.key,
        parentTestPlanId: item.parentKey as number,
        testPlanName: item.testPlanName,
        orderDate: item.orderDate,
        projectId: item.projectId,
        projectSlug: item.projectSlug,
        description: item.description,
        testPlanTypeListItemId: item.testPlanTypeListItemId,
        testPlanType: item.testPlanType,
        testPlanChildModule: parentChildren,
      };
    }

    return {
      title: item.title,
      testPlanId: +item.key,
      parentTestPlanId: item.parentKey as number,
      testPlanName: item.testPlanName,
      orderDate: item.orderDate,
      projectId: item.projectId,
      projectSlug: item.projectSlug,
      description: item.description,
      testPlanTypeListItemId: item.testPlanTypeListItemId,
      testPlanType: item.testPlanType,
      testPlanChildModule: [],
    };
  });
};

export const getTestCasesArr = (node: any): ITestPlanLocalTestCase[] => {
  const testCases = node.children?.map((item: any) => {
    if (item.children?.length) {
      return getTestCasesArr(item);
    }

    if (item.type === "TestCase") {
      // console.log("item", item);
      return {
        key: Math.floor(Math.random() * 1000000000),
        id: item.key,
        testCaseName: item.title,
        scenario: item.description,
        expectedResult: item.expectedResult,
        action: `${item.key}`,
      };
    }
  });

  return testCases.flat(4).filter((item: any) => item !== undefined);
};
