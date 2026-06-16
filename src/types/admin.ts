import type { Permission, UserRole } from './auth';

export interface AdminOverview {
  companies: number;
  facilities: number;
  panels: number;
  files: number;
  users: number;
  activeUsers: number;
  inactiveUsers: number;
  roles: number;
}

export interface AdminCompanyOption {
  id: string;
  projectName: string;
  name: string;
  companyCode: string;
}

export interface AdminRole {
  id: string;
  name: UserRole;
  description: string;
  permissions: Permission[];
  isProtected: boolean;
}

export interface AdminUser {
  id: string;
  username: string;
  displayName: string;
  companyId?: string | null;
  companyName?: string | null;
  projectName?: string | null;
  companyCode?: string | null;
  isActive: boolean;
  roles: UserRole[];
  createdAtUtc: string;
}

export interface AdminAuditLog {
  id: string;
  action: string;
  entityName: string;
  entityId?: string | null;
  userId?: string | null;
  username?: string | null;
  panelName?: string | null;
  occurredAtUtc: string;
  metadata: string;

}

export interface AdminMaintenanceReport {
  id: string;
  panelId: string;
  panelName: string;
  panelCode: string;
  facilityName: string;
  projectName: string;
  title: string;
  reportDateUtc: string;
  notes: string;
  createdByUsername?: string | null;
  createdAtUtc: string;
}

export interface CreateAdminUserRequest {
  username: string;
  displayName: string;
  password: string;
  companyId?: string | null;
  roles: UserRole[];
}

export interface UpdateAdminUserRequest {
  displayName: string;
  companyId?: string | null;
  isActive: boolean;
  roles: UserRole[];
}

export interface ResetAdminUserPasswordRequest {
  password: string;
}

export interface CreateAdminRoleRequest {
  name: string;
  description: string;
  permissions: Permission[];
}

export interface UpdateAdminRoleRequest {
  description: string;
  permissions: Permission[];
}
