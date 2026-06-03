import type { RouteRecordRaw } from 'vue-router';

import { routeNames } from '@/constants/routes';
import { appRoles, roleGroups } from '@/utils/authorization';

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
    meta: { title: 'Dashboard', requiresAuth: true, requiredRoles: roleGroups.readOnly },
  },
  {
    path: '/companies',
    name: routeNames.companies,
    component: () => import('@/pages/companies/CompaniesPage.vue'),
    meta: { title: 'Companies', requiresAuth: true, requiredRoles: roleGroups.readOnly },
  },
  {
    path: '/companies/:companyId',
    name: routeNames.companyDetail,
    component: () => import('@/pages/companies/CompanyDetailPage.vue'),
    meta: { title: 'Company Detail', requiresAuth: true, requiredRoles: roleGroups.readOnly },
  },
  {
    path: '/facilities',
    name: routeNames.facilities,
    component: () => import('@/pages/facilities/FacilitiesPage.vue'),
    meta: { title: 'Facilities', requiresAuth: true, requiredRoles: roleGroups.readOnly },
  },
  {
    path: '/facilities/:facilityId',
    name: routeNames.facilityDetail,
    component: () => import('@/pages/facilities/FacilityDetailPage.vue'),
    meta: { title: 'Facility Detail', requiresAuth: true, requiredRoles: roleGroups.readOnly },
  },
  {
    path: '/panels',
    name: routeNames.panels,
    component: () => import('@/pages/panels/PanelsPage.vue'),
    meta: { title: 'Panels', requiresAuth: true, requiredRoles: roleGroups.readOnly },
  },
  {
    path: '/panels/:panelId',
    name: routeNames.panelDetail,
    component: () => import('@/pages/panels/PanelDetailPage.vue'),
    meta: { title: 'Panel Detail', requiresAuth: true, requiredRoles: roleGroups.readOnly },
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
