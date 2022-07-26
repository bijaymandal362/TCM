import React from "react";
import { useAppSelector } from "../store/reduxHooks";
import { getPermissions, getRoleId } from "../util/auth.util";

export const CheckProjectPermission: any = ({
  slug,
  children,
  role,
  roleId,
}: {
  slug: string;
  children: React.ReactNode;
  role?: string;
  roleId?: number;
}) => {
  const { projectPermissions } = useAppSelector((state) => state?.project);

  if (slug) {
    if (projectPermissions?.includes(slug)) {
      return <>{children}</>;
    }
  }

  return <></>;
};

export default CheckProjectPermission;
