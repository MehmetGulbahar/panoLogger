import 'vue-router';
import type { UserRole } from '@/types';

declare module 'vue-router' {
  interface RouteMeta {
    title?: string;
    layout?: 'public';
    requiresAuth?: boolean;
    requiredRoles?: UserRole[];
  }
}
