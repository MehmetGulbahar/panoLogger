<template>
  <button
    v-if="visible"
    type="button"
    class="install-button"
    :disabled="installing"
    @click="installApp"
  >
    <i class="pi pi-download" aria-hidden="true"></i>
    <span>{{ installing ? 'Hazırlanıyor' : 'Uygulamayı Yükle' }}</span>
  </button>
</template>

<script setup lang="ts">
import { onBeforeUnmount, onMounted, ref } from 'vue';
import { hasInstallPrompt, promptInstall } from '@/services/pwaService';

const visible = ref(false);
const installing = ref(false);

function syncVisibility() {
  visible.value = hasInstallPrompt();
}

async function installApp() {
  if (installing.value) return;

  installing.value = true;
  try {
    await promptInstall();
  } finally {
    installing.value = false;
    syncVisibility();
  }
}

onMounted(() => {
  syncVisibility();
  window.addEventListener('pwa-install-available', syncVisibility);
  window.addEventListener('pwa-install-unavailable', syncVisibility);
});

onBeforeUnmount(() => {
  window.removeEventListener('pwa-install-available', syncVisibility);
  window.removeEventListener('pwa-install-unavailable', syncVisibility);
});
</script>

<style scoped>
.install-button {
  position: fixed;
  right: 1rem;
  bottom: 1rem;
  z-index: 1100;
  min-height: 2.5rem;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 0.45rem;
  border: 1px solid var(--app-primary);
  border-radius: 8px;
  background: var(--app-primary);
  color: #fff;
  padding: 0 0.85rem;
  font: inherit;
  font-size: 0.8125rem;
  font-weight: 700;
  box-shadow: 0 12px 30px rgb(37 99 235 / 0.22);
  cursor: pointer;
}

.install-button:disabled {
  opacity: 0.72;
  cursor: wait;
}

@media (max-width: 960px) {
  .install-button {
    right: 1rem;
    bottom: 4.85rem;
  }
}
</style>
