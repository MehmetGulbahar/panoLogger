import { defineStore } from 'pinia';
import { computed, ref } from 'vue';

import { apiClient } from '@/api/client';
import { apiEndpoints } from '@/api/endpoints';
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
    user.value = session.user;
  }

  async function loginDev(email: string, rolesToRequest: UserRole[] = ['SuperAdmin']): Promise<void> {
    const { data } = await apiClient.post<AuthSessionResponse>(`${apiEndpoints.auth}/dev-token`, {
      email,
      roles: rolesToRequest,
    });

    setSession(data);
    await loadPermissions();
  }

  async function login(email: string, password: string): Promise<void> {
    const { data } = await apiClient.post<AuthSessionResponse>(`${apiEndpoints.auth}/login`, {
      email,
      password,
    });

    setSession(data);
    await loadPermissions();
  }

  async function register(displayName: string, email: string, password: string, companyCode: string): Promise<void> {
    const { data } = await apiClient.post<AuthSessionResponse>(`${apiEndpoints.auth}/register`, {
      displayName,
      email,
      password,
      companyCode,
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

  async function logout(): Promise<void> {
    try {
      await apiClient.post(`${apiEndpoints.auth}/logout`);
    } finally {
      user.value = null;
      permissions.value = [];
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
    register,
    hydrate,
    loadPermissions,
    hasAnyRole,
    hasPermission,
    logout,
  };
});

function clearLegacyAuthStorage(): void {
  for (const key of LEGACY_AUTH_KEYS) {
    localStorage.removeItem(key);
  }
}
