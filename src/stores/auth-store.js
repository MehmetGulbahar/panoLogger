import { defineStore } from 'pinia';
import { computed, ref } from 'vue';
import { apiClient } from '@/api/client';
import { apiEndpoints } from '@/api/endpoints';
import { useHierarchyStore } from './hierarchy-store';
const LEGACY_AUTH_KEYS = [
    'panologger.accessToken',
    'panologger.refreshToken',
    'panologger.user',
];
export const useAuthStore = defineStore('auth', () => {
    const user = ref(null);
    const permissions = ref([]);
    const isHydrated = ref(false);
    const isAuthenticated = computed(() => Boolean(user.value));
    const roles = computed(() => user.value?.roles ?? []);
    function setSession(session) {
        resetTenantScopedStores();
        user.value = session.user;
    }
    async function loginDev(email, rolesToRequest = ['SuperAdmin']) {
        const { data } = await apiClient.post(`${apiEndpoints.auth}/dev-token`, {
            email,
            roles: rolesToRequest,
        });
        setSession(data);
        await loadPermissions();
    }
    async function login(email, password) {
        const { data } = await apiClient.post(`${apiEndpoints.auth}/login`, {
            email,
            password,
        });
        setSession(data);
        await loadPermissions();
    }
    async function register(displayName, email, password, companyCode) {
        const { data } = await apiClient.post(`${apiEndpoints.auth}/register`, {
            displayName,
            email,
            password,
            companyCode,
        });
        setSession(data);
        await loadPermissions();
    }
    async function hydrate() {
        clearLegacyAuthStorage();
        try {
            const { data } = await apiClient.get(`${apiEndpoints.auth}/me`);
            user.value = data;
            await loadPermissions();
        }
        catch {
            user.value = null;
            permissions.value = [];
            resetTenantScopedStores();
        }
        finally {
            isHydrated.value = true;
        }
    }
    async function loadPermissions() {
        if (!user.value) {
            permissions.value = [];
            return;
        }
        const { data } = await apiClient.get(`${apiEndpoints.auth}/permissions`);
        permissions.value = data.permissions;
    }
    function hasAnyRole(requiredRoles) {
        if (!requiredRoles?.length) {
            return true;
        }
        return requiredRoles.some((role) => roles.value.includes(role));
    }
    function hasPermission(permission) {
        return permissions.value.includes(permission);
    }
    function hasAllPermissions(requiredPermissions) {
        if (!requiredPermissions?.length) {
            return true;
        }
        return requiredPermissions.every((permission) => permissions.value.includes(permission));
    }
    async function logout() {
        try {
            await apiClient.post(`${apiEndpoints.auth}/logout`);
        }
        finally {
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
        register,
        hydrate,
        loadPermissions,
        hasAnyRole,
        hasPermission,
        hasAllPermissions,
        logout,
    };
});
function clearLegacyAuthStorage() {
    for (const key of LEGACY_AUTH_KEYS) {
        localStorage.removeItem(key);
    }
}
function resetTenantScopedStores() {
    useHierarchyStore().reset();
}
