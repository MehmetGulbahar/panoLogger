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
  email: string;
  displayName: string;
  companyId?: string | null;
  companyName?: string | null;
  projectName?: string | null;
  companyCode?: string | null;
  isActive: boolean;
  roles: UserRole[];
  createdAtUtc: string;
}

export interface UpdateAdminUserRequest {
  displayName: string;
  companyId?: string | null;
  isActive: boolean;
  roles: UserRole[];
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
