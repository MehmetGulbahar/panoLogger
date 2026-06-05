import type { RouteRecordRaw } from 'vue-router';

import { routeNames } from '@/constants/routes';
import { appRoles } from '@/utils/authorization';

export const appRoutes: RouteRecordRaw[] = [
  {
    path: '/',
    redirect: { name: routeNames.dashboard },
  },
  {
    path: '/login',
    name: routeNames.login,
    component: () => import('@/pages/auth/LoginPage.vue'),
    meta: { title: 'Login', layout: 'public' },
  },
  {
    path: '/dashboard',
    name: routeNames.dashboard,
    component: () => import('@/pages/dashboard/DashboardPage.vue'),
    meta: { title: 'Dashboard', requiresAuth: true },
  },
  {
    path: '/companies',
    name: routeNames.companies,
    component: () => import('@/pages/companies/CompaniesPage.vue'),
    meta: { title: 'Companies', requiresAuth: true, requiredPermissions: ['companies.view'] },
  },
  {
    path: '/companies/:companyId',
    name: routeNames.companyDetail,
    component: () => import('@/pages/companies/CompanyDetailPage.vue'),
    meta: { title: 'Company Detail', requiresAuth: true, requiredPermissions: ['companies.view'] },
  },
  {
    path: '/facilities',
    name: routeNames.facilities,
    component: () => import('@/pages/facilities/FacilitiesPage.vue'),
    meta: { title: 'Facilities', requiresAuth: true, requiredPermissions: ['facilities.view'] },
  },
  {
    path: '/facilities/:facilityId',
    name: routeNames.facilityDetail,
    component: () => import('@/pages/facilities/FacilityDetailPage.vue'),
    meta: { title: 'Facility Detail', requiresAuth: true, requiredPermissions: ['facilities.view'] },
  },
  {
    path: '/panels',
    name: routeNames.panels,
    component: () => import('@/pages/panels/PanelsPage.vue'),
    meta: { title: 'Panels', requiresAuth: true, requiredPermissions: ['panels.view'] },
  },
  {
    path: '/panels/:panelId',
    name: routeNames.panelDetail,
    component: () => import('@/pages/panels/PanelDetailPage.vue'),
    meta: { title: 'Panel Detail', requiresAuth: true, requiredPermissions: ['panels.view'] },
  },
  {
    path: '/admin',
    name: routeNames.admin,
    component: () => import('@/pages/admin/AdminDashboardPage.vue'),
    meta: { title: 'Super Admin', requiresAuth: true, requiredRoles: [appRoles.superAdmin] },
  },
  {
    path: '/p/:panelCode',
    name: routeNames.publicPanel,
    component: () => import('@/pages/public/PublicPanelPage.vue'),
    meta: { title: 'Panel Access', layout: 'public' },
  },
  {
    path: '/design/colors',
    name: routeNames.designColors,
    component: () => import('@/pages/design/ColorsPage.vue'),
    meta: { title: 'Design - Colors', requiresAuth: true, requiredRoles: [appRoles.superAdmin] },
  },
];
