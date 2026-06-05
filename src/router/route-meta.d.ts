import 'vue-router';
import type { Permission, UserRole } from '@/types';

declare module 'vue-router' {
  interface RouteMeta {
    title?: string;
    layout?: 'public';
    requiresAuth?: boolean;
    requiredRoles?: UserRole[];
    requiredPermissions?: Permission[];
  }
}
