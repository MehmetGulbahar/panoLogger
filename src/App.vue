<template>
  <Toast position="top-right" />
  <RouterView v-if="isPublicRoute" />
  <AppShell v-else>
    <RouterView />
  </AppShell>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useRoute } from 'vue-router';
import Toast from 'primevue/toast';
import AppShell from './layouts/AppShell.vue';

const route = useRoute();
const isPublicRoute = computed(() => route.meta.layout === 'public');
</script>

<style>
.p-toast {
  width: min(22rem, calc(100vw - 2rem));
}

.p-toast .p-toast-message {
  margin: 0 0 0.65rem;
  border: 1px solid var(--app-border);
  border-radius: 8px;
  background: var(--app-bg);
  box-shadow: var(--app-shadow);
  color: var(--app-text);
}

.p-toast .p-toast-message.p-toast-message-success {
  border-left: 3px solid var(--success-500);
}

.p-toast .p-toast-message-content {
  align-items: flex-start;
  gap: 0.65rem;
  padding: 0.75rem 0.85rem;
}

.p-toast .p-toast-message-icon {
  margin-top: 0.1rem;
  color: var(--success-600);
  font-size: 0.95rem;
}

.p-toast .p-toast-summary {
  color: var(--app-text);
  font-size: 0.8125rem;
  line-height: 1.25;
  font-weight: 700;
}

.p-toast .p-toast-detail {
  margin-top: 0.15rem;
  color: var(--app-text-muted);
  font-size: 0.75rem;
  line-height: 1.35;
}

.p-toast .p-toast-icon-close {
  width: 1.7rem;
  height: 1.7rem;
  color: var(--app-text-muted);
}
</style>
