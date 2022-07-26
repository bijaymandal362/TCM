import React from "react";
import { getPermissions, getRoleId } from "../util/auth.util";

export const CheckPermission: any = ({
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
  if (slug) {
    if (getPermissions()?.includes(slug) || getRoleId() === roleId) {
      return <>{children}</>;
    }
  }

  return <></>;
};

export default CheckPermission;
