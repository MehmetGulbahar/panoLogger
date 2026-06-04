export type UserRole = 'SuperAdmin' | 'CompanyAdmin' | 'FacilityManager' | 'Viewer';

export type Permission =
  | 'companies.view'
  | 'facilities.view'
  | 'panels.view'
  | 'files.view'
  | 'companies.manage'
  | 'facilities.manage'
  | 'panels.manage'
  | 'files.manage'
  | 'qr.manage'
  | 'reports.view'
  | 'public-panels.view';

export interface AuthUser {
  id: string;
  email: string;
  displayName: string;
  companyId?: string | null;
  roles: UserRole[];
}

export interface AuthSessionResponse {
  user: AuthUser;
}

export interface PermissionsResponse {
  roles: UserRole[];
  permissions: Permission[];
}
