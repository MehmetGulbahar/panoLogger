export type UserRole = string;

export type Permission =
  | 'companies.view'
  | 'facilities.view'
  | 'panels.view'
  | 'files.view'
  | 'companies.manage'
  | 'facilities.manage'
  | 'panels.manage'
  | 'files.manage'
  | 'files.delete'
  | 'qr.manage'
  | 'reports.view'
  | 'public-panels.view'
  | 'users.manage'
  | 'roles.manage'
  | 'admin.view'
  | string;

export interface AuthUser {
  id: string;
  email: string;
  displayName: string;
  companyId?: string | null;
  roles: UserRole[];
  permissions?: Permission[];
}

export interface AuthSessionResponse {
  user: AuthUser;
}

export interface PermissionsResponse {
  roles: UserRole[];
  permissions: Permission[];
}
