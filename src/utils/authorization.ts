import type { UserRole } from '@/types';

export const appRoles = {
  superAdmin: 'SuperAdmin',
  companyAdmin: 'CompanyAdmin',
  facilityManager: 'FacilityManager',
  viewer: 'Viewer',
} as const satisfies Record<string, UserRole>;

export const roleGroups = {
  anyAuthenticated: [
    appRoles.superAdmin,
    appRoles.companyAdmin,
    appRoles.facilityManager,
    appRoles.viewer,
  ],
  companyManagement: [appRoles.superAdmin],
  facilityManagement: [appRoles.superAdmin, appRoles.companyAdmin],
  panelManagement: [appRoles.superAdmin, appRoles.companyAdmin, appRoles.facilityManager],
  readOnly: [
    appRoles.superAdmin,
    appRoles.companyAdmin,
    appRoles.facilityManager,
    appRoles.viewer,
  ],
} satisfies Record<string, UserRole[]>;
