import type { UserRole } from './auth';

export interface NavigationItem {
  label: string;
  icon?: string;
  to: { name: string } | { path: string };
  roles?: UserRole[];
}
