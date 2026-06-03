<template>
  <header class="app-header">
    <div class="app-header__brand">
      <div class="app-header__logo" aria-hidden="true">
        <i class="pi pi-bolt app-header__brand-icon"></i>
      </div>
      <div>
        <div class="app-header__title">PanoVeri</div>
        <div class="app-header__subtitle">Elektrik Panoları Bilgi Platformu</div>
      </div>
    </div>

    <div class="app-header__right">
      <div class="app-header__search">
        <span class="app-header__search-icon pi pi-search" aria-hidden="true"></span>
        <InputText v-model="searchValue" placeholder="Ara" class="app-header__search-input" />
      </div>

      <div class="app-header__actions">
        <div ref="userMenuRoot" class="app-header__user">
          <button
            class="app-header__user-trigger"
            type="button"
            :aria-expanded="isUserMenuOpen"
            aria-haspopup="menu"
            aria-label="Kullanıcı menüsü"
            @click="isUserMenuOpen = !isUserMenuOpen"
          >
            <Avatar :label="avatarLabel" shape="circle" class="app-header__avatar" />
            <span class="app-header__user-copy">
              <strong>{{ displayName }}</strong>
              <small>{{ primaryRoleLabel }}</small>
            </span>
            <i class="pi pi-chevron-down app-header__user-chevron" aria-hidden="true"></i>
          </button>

          <div v-if="isUserMenuOpen" class="app-header__user-menu" role="menu">
            <div class="app-header__user-summary">
              <strong>{{ displayName }}</strong>
              <span>{{ authStore.user?.email }}</span>
            </div>
            <button class="app-header__logout" type="button" role="menuitem" :disabled="isLoggingOut" @click="onLogout">
              <i :class="isLoggingOut ? 'pi pi-spin pi-spinner' : 'pi pi-sign-out'" aria-hidden="true"></i>
              {{ isLoggingOut ? 'Çıkış yapılıyor' : 'Çıkış Yap' }}
            </button>
          </div>
        </div>
      </div>
    </div>
  </header>
</template>

<script setup lang="ts">
import Avatar from 'primevue/avatar';
import Button from 'primevue/button';
import InputText from 'primevue/inputtext';
import { computed, onBeforeUnmount, onMounted, ref } from 'vue';
import { useRouter } from 'vue-router';
import { routeNames } from '@/constants/routes';
import { useAuthStore } from '@/stores';
import type { UserRole } from '@/types/auth';

const searchValue = ref('');
const authStore = useAuthStore();
const router = useRouter();
const userMenuRoot = ref<HTMLElement | null>(null);
const isUserMenuOpen = ref(false);
const isLoggingOut = ref(false);

const roleLabels: Record<UserRole, string> = {
  SuperAdmin: 'Süper Admin',
  CompanyAdmin: 'Şirket Yöneticisi',
  FacilityManager: 'Tesis Yöneticisi',
  Viewer: 'Görüntüleyici',
};

const displayName = computed(() => authStore.user?.displayName || authStore.user?.email || 'Kullanıcı');
const primaryRoleLabel = computed(() => {
  const role = authStore.roles[0];
  return role ? roleLabels[role] : 'Kullanıcı';
});
const avatarLabel = computed(() => {
  const words = displayName.value.trim().split(/\s+/).filter(Boolean);

  if (words.length >= 2) {
    return `${words[0][0]}${words[words.length - 1][0]}`.toUpperCase();
  }

  return displayName.value.slice(0, 2).toUpperCase();
});

async function onLogout() {
  isLoggingOut.value = true;

  try {
    await authStore.logout();
  } finally {
    isLoggingOut.value = false;
    isUserMenuOpen.value = false;
    await router.replace({ name: routeNames.login });
  }
}

function onDocumentClick(event: MouseEvent) {
  if (userMenuRoot.value && !userMenuRoot.value.contains(event.target as Node)) {
    isUserMenuOpen.value = false;
  }
}

onMounted(() => document.addEventListener('click', onDocumentClick));
onBeforeUnmount(() => document.removeEventListener('click', onDocumentClick));
</script>

<style scoped>
.app-header {
  min-height: 56px;
  border-bottom: 1px solid var(--app-border);
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 0.75rem;
  padding: 0.55rem 1rem;
  background: var(--app-bg);
}

.app-header__brand {
  display: flex;
  align-items: center;
  gap: 0.6rem;
  min-width: 0;
}

.app-header__logo {
  width: 30px;
  height: 30px;
  border-radius: 50%;
  display: grid;
  place-items: center;
  background: var(--app-surface);
  border: 1px solid var(--app-border);
}

.app-header__brand-icon {
  color: var(--app-primary);
  font-size: 0.95rem;
}

.app-header__title {
  font-weight: 600;
  line-height: 1.15;
  font-size: 0.875rem;
}

.app-header__subtitle {
  color: var(--app-text-muted);
  font-size: 0.6875rem;
  line-height: 1.25;
  margin-top: 0.05rem;
}

.app-header__right {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin-left: auto;
}

.app-header__search {
  flex: 0 0 auto;
  width: 240px;
  max-width: 240px;
  position: relative;
}

.app-header__search-icon {
  position: absolute;
  left: 0.75rem;
  top: 50%;
  transform: translateY(-50%);
  color: var(--app-text-muted);
  font-size: 0.75rem;
}

.app-header__search-input {
  width: 100%;
  height: 2rem;
  padding-left: 2rem;
  border-radius: 999px;
  font-size: 0.8125rem;
}

.app-header__actions {
  display: flex;
  align-items: center;
  gap: 0.25rem;
}

.app-header__action-button {
  width: 2rem;
  height: 2rem;
}

.app-header__action-button :deep(.p-button-icon) {
  font-size: 0.85rem;
}

.app-header__avatar {
  width: 2rem;
  height: 2rem;
  font-size: 0.75rem;
  font-weight: 700;
  background: var(--app-primary);
  color: var(--color-white);
}

.app-header__user {
  position: relative;
}

.app-header__user-trigger {
  display: flex;
  align-items: center;
  gap: 0.45rem;
  min-width: 0;
  padding: 0.15rem 0.25rem;
  border: 0;
  border-radius: 8px;
  background: transparent;
  color: var(--app-text);
  cursor: pointer;
}

.app-header__user-trigger:hover {
  background: var(--app-surface-alt);
}

.app-header__user-copy {
  display: grid;
  min-width: 0;
  text-align: left;
}

.app-header__user-copy strong {
  max-width: 10rem;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  font-size: 0.75rem;
  line-height: 1.2;
}

.app-header__user-copy small {
  color: var(--app-text-muted);
  font-size: 0.625rem;
  line-height: 1.2;
}

.app-header__user-chevron {
  color: var(--app-text-muted);
  font-size: 0.65rem;
}

.app-header__user-menu {
  position: absolute;
  top: calc(100% + 0.4rem);
  right: 0;
  z-index: 100;
  width: 15rem;
  padding: 0.35rem;
  border: 1px solid var(--app-border);
  border-radius: 8px;
  background: var(--app-bg);
  box-shadow: var(--app-shadow);
}

.app-header__user-summary {
  display: grid;
  gap: 0.15rem;
  padding: 0.55rem 0.6rem 0.65rem;
  border-bottom: 1px solid var(--app-border);
}

.app-header__user-summary strong,
.app-header__user-summary span {
  overflow-wrap: anywhere;
}

.app-header__user-summary strong {
  font-size: 0.8125rem;
}

.app-header__user-summary span {
  color: var(--app-text-muted);
  font-size: 0.6875rem;
}

.app-header__logout {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  width: 100%;
  margin-top: 0.3rem;
  padding: 0.55rem 0.6rem;
  border: 0;
  border-radius: 6px;
  background: transparent;
  color: #b91c1c;
  font-size: 0.75rem;
  cursor: pointer;
}

.app-header__logout:hover:not(:disabled) {
  background: #fef2f2;
}

.app-header__logout:disabled {
  opacity: 0.6;
  cursor: wait;
}

@media (max-width: 960px) {
  .app-header {
    flex-wrap: wrap;
    min-height: 52px;
    padding: 0.5rem 0.85rem;
  }

  .app-header__search {
      order: 3;
      max-width: none;
      width: 100%;
    }

  .app-header__right {
      width: 100%;
      justify-content: flex-end;
    }

  .app-header__user-copy {
    display: none;
  }
}
</style>
