<template>
  <nav class="app-breadcrumbs" aria-label="Breadcrumb">
    <RouterLink to="/dashboard">Home</RouterLink>
    <template v-for="item in breadcrumbs" :key="item.to">
      <span class="app-breadcrumbs__separator">/</span>
      <RouterLink :to="item.to">{{ item.label }}</RouterLink>
    </template>
  </nav>
</template>

<script setup lang="ts">
import { useBreadcrumbs } from '@/composables/use-breadcrumbs';

const { breadcrumbs } = useBreadcrumbs();
</script>

<style scoped>
.app-breadcrumbs {
  display: flex;
  align-items: center;
  gap: 0.4rem;
  color: var(--app-text-muted);
  font-size: 0.75rem;
  line-height: 1.25;
}

.app-breadcrumbs__separator {
  color: #98a2b3;
}
</style>
