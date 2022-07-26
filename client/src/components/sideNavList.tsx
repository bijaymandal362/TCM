import {
  DashboardOutlined,
  TableOutlined,
  DotChartOutlined,
  SettingOutlined,
  UserAddOutlined,
  EnvironmentOutlined,
  UserOutlined,
  BookOutlined,
  PlayCircleOutlined,
  ProjectOutlined,
} from "@ant-design/icons";
import { NavLinks } from "../interfaces";

export const projectNavList = (projectSlug: string) => [
  {
    href: `/project/${projectSlug}`,
    icon: <DashboardOutlined />,
    title: "Project Dashboard",
  },
  {
    href: `/project/${projectSlug}/requirement-management`,
    icon: <BookOutlined />,
    title: "Test Case Repository",
  },
  // {
  //   href: `/project/${projectSlug}/test-case`,
  //   icon: <DotChartOutlined />,
  //   title: "Test Case",
  // },
  {
    href: `/project/${projectSlug}/test-plans`,
    icon: <TableOutlined />,
    title: "Test Plans",
  },
  {
    href: `/project/${projectSlug}/test-runs`,
    icon: <PlayCircleOutlined />,
    title: "Test Runs",
  },
  {
    href: `/project/${projectSlug}/settings`,
    icon: <SettingOutlined />,
    title: "Settings",
    permission: "projectrole.projectmember.read",
    children: [
      {
        href: `/project/${projectSlug}/settings/members`,
        icon: <UserAddOutlined />,
        title: "Project Members",
      },
      {
        href: `/project/${projectSlug}/settings/environment`,
        icon: <EnvironmentOutlined />,
        title: "Environment",
      },
    ],
  },
];

export function adminNavList(projectSlug: string): NavLinks[] {
  return [
    {
      href: "/admin",
      icon: <SettingOutlined />,
      title: "Overview",
      permission: "user.update",
      children: [
        {
          href: "/admin",
          icon: <DashboardOutlined />,
          title: "Dashboard",
          permission: "user.update",
        },
        {
          href: "/admin/users",
          icon: <UserOutlined />,
          title: "Users",
          permission: "user.update",
        },
        {
          href: "/admin/projects",
          icon: <ProjectOutlined />,
          title: "Projects",
          permission: "user.update",
        },
      ],
    },
  ];
}
