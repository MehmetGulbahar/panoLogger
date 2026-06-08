import { defineStore } from 'pinia';
import { computed, ref } from 'vue';

import { apiClient } from '@/api/client';
import { apiEndpoints } from '@/api/endpoints';
import { useHierarchyStore } from './hierarchy-store';
import type { AuthSessionResponse, AuthUser, Permission, PermissionsResponse, UserRole } from '@/types';

const LEGACY_AUTH_KEYS = [
  'panologger.accessToken',
  'panologger.refreshToken',
  'panologger.user',
];

export const useAuthStore = defineStore('auth', () => {
  const user = ref<AuthUser | null>(null);
  const permissions = ref<Permission[]>([]);
  const isHydrated = ref(false);

  const isAuthenticated = computed(() => Boolean(user.value));
  const roles = computed<UserRole[]>(() => user.value?.roles ?? []);

  function setSession(session: AuthSessionResponse): void {
    resetTenantScopedStores();
    user.value = session.user;
  }

  async function loginDev(username: string, rolesToRequest: UserRole[] = ['SuperAdmin']): Promise<void> {
    const { data } = await apiClient.post<AuthSessionResponse>(`${apiEndpoints.auth}/dev-token`, {
      username,
      roles: rolesToRequest,
    });

    setSession(data);
    await loadPermissions();
  }

  async function login(username: string, password: string): Promise<void> {
    const { data } = await apiClient.post<AuthSessionResponse>(`${apiEndpoints.auth}/login`, {
      username,
      password,
    });

    setSession(data);
    await loadPermissions();
  }

  async function hydrate(): Promise<void> {
    clearLegacyAuthStorage();

    try {
      const { data } = await apiClient.get<AuthUser>(`${apiEndpoints.auth}/me`);
      user.value = data;
      await loadPermissions();
    } catch {
      user.value = null;
      permissions.value = [];
      resetTenantScopedStores();
    } finally {
      isHydrated.value = true;
    }
  }

  async function loadPermissions(): Promise<void> {
    if (!user.value) {
      permissions.value = [];
      return;
    }

    const { data } = await apiClient.get<PermissionsResponse>(`${apiEndpoints.auth}/permissions`);
    permissions.value = data.permissions;
  }

  function hasAnyRole(requiredRoles?: UserRole[]): boolean {
    if (!requiredRoles?.length) {
      return true;
    }

    return requiredRoles.some((role) => roles.value.includes(role));
  }

  function hasPermission(permission: Permission): boolean {
    return permissions.value.includes(permission);
  }

  function hasAllPermissions(requiredPermissions?: Permission[]): boolean {
    if (!requiredPermissions?.length) {
      return true;
    }

    return requiredPermissions.every((permission) => permissions.value.includes(permission));
  }

  async function logout(): Promise<void> {
    try {
      await apiClient.post(`${apiEndpoints.auth}/logout`);
    } finally {
      user.value = null;
      permissions.value = [];
      resetTenantScopedStores();
    }
  }

  clearLegacyAuthStorage();

  return {
    user,
    permissions,
    roles,
    isAuthenticated,
    isHydrated,
    login,
    loginDev,
    hydrate,
    loadPermissions,
    hasAnyRole,
    hasPermission,
    hasAllPermissions,
    logout,
  };
});

function clearLegacyAuthStorage(): void {
  for (const key of LEGACY_AUTH_KEYS) {
    localStorage.removeItem(key);
  }
}

function resetTenantScopedStores(): void {
  useHierarchyStore().reset();
}
