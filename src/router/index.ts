import { createRouter, createWebHistory } from 'vue-router';

import { routeNames } from '@/constants/routes';
import { useAuthStore } from '@/stores';

import { appRoutes } from './routes';

export const appRouter = createRouter({
  history: createWebHistory(),
  routes: appRoutes,
  scrollBehavior: () => ({ top: 0 }),
});

appRouter.beforeEach(async (to) => {
  if (!to.meta.title) {
    to.meta.title = 'PanelDocs';
  }

  if (to.meta.layout === 'public') {
    return true;
  }

  const authStore = useAuthStore();

  if (!authStore.isHydrated) {
    await authStore.hydrate();
  }

  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    return { name: routeNames.login, query: { redirect: to.fullPath } };
  }

  if (!authStore.hasAnyRole(to.meta.requiredRoles)) {
    return { name: routeNames.dashboard };
  }

  return true;
});

export { routeNames };
