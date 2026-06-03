<template>
  <div v-if="isLoading" class="app-global-loading app-card">
    <ProgressBar mode="indeterminate" style="height: 0.35rem" />
    <span>Loading data...</span>
  </div>
</template>

<script setup lang="ts">
import ProgressBar from 'primevue/progressbar';

import { useGlobalLoading } from '@/composables/use-global-loading';

const { isLoading } = useGlobalLoading();
</script>

<style scoped>
.app-global-loading {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  padding: 0.75rem;
  margin-bottom: 1rem;
  color: var(--app-text-muted);
}
</style>