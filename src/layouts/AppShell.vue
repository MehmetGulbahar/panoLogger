<template>
  <div class="app-shell">
    <div class="app-page-shell">
      <AppHeader />
      <div class="app-shell__body">
        <AppSidebar />
        <main class="app-shell__content">
          <section class="app-shell__toolbar">
            <AppBreadcrumbs />
          </section>
          <section class="app-shell__page">
            <AppGlobalLoading />
            <AppGlobalError />
            <slot />
          </section>
        </main>
      </div>
    </div>
    <ModalHost />
    <MobileBottomNav />
    <FloatingActionButton @click="onFabClick" />
    <MobileDrawer v-model="drawerOpen">
      <template #header>
        Hızlı Eylemler
      </template>
      <div>
        <UiButton variant="primary" @click="createQuick">Yeni Kayıt</UiButton>
      </div>
    </MobileDrawer>
  </div>
</template>

<script setup lang="ts">
import AppBreadcrumbs from '@/components/navigation/AppBreadcrumbs.vue';
import AppGlobalError from '@/components/system/AppGlobalError.vue';
import AppGlobalLoading from '@/components/system/AppGlobalLoading.vue';
import AppHeader from './components/AppHeader.vue';
import AppSidebar from './components/AppSidebar.vue';
import MobileBottomNav from '@/components/mobile/MobileBottomNav.vue';
import FloatingActionButton from '@/components/mobile/FloatingActionButton.vue';
import { ref } from 'vue';

const drawerOpen = ref(false);

function onFabClick() {
  drawerOpen.value = !drawerOpen.value;
}

function createQuick() {
  // placeholder quick create action
  // eslint-disable-next-line no-console
  console.log('quick create');
  drawerOpen.value = false;
}
</script>

<style scoped>
.app-shell {
  min-height: 100vh;
  display: block;
}

.app-page-shell {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

.app-shell__body {
  display: flex;
  min-height: 0;
  flex: 1;
}

.app-shell__content {
  display: flex;
  flex-direction: column;
  flex: 1;
  min-width: 0;
  background: var(--app-bg);
}

.app-shell__toolbar {
  padding: 0.75rem 1rem 0.35rem;
}

.app-shell__page {
  flex: 1;
  padding: 0.4rem 1rem 1rem;
}

@media (max-width: 960px) {
  .app-shell__body {
    flex-direction: column;
  }

  .app-page-shell {
    border-radius: 18px;
  }

  .app-shell__toolbar,
  .app-shell__page {
    padding-left: 1rem;
    padding-right: 1rem;
  }
}
</style>
