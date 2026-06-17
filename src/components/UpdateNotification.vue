<template>
  <section v-if="visible" class="update-notification" role="status" aria-live="polite">
    <div>
      <strong>Yeni surum hazir</strong>
      <span>Guncel uygulamayi kullanmak icin sayfayi yenileyin.</span>
    </div>
    <button type="button" @click="reloadApp">Yenile</button>
  </section>
</template>

<script setup lang="ts">
import { onBeforeUnmount, onMounted, ref } from 'vue';
import { activateWaitingServiceWorker } from '@/services/pwaService';

const visible = ref(false);
let refreshing = false;

function showUpdate() {
  visible.value = true;
}

function reloadApp() {
  const activated = activateWaitingServiceWorker();
  if (!activated) window.location.reload();
}

function handleControllerChange() {
  if (refreshing) return;

  refreshing = true;
  window.location.reload();
}

onMounted(() => {
  window.addEventListener('pwa-update-available', showUpdate);
  navigator.serviceWorker?.addEventListener('controllerchange', handleControllerChange);
});

onBeforeUnmount(() => {
  window.removeEventListener('pwa-update-available', showUpdate);
  navigator.serviceWorker?.removeEventListener('controllerchange', handleControllerChange);
});
</script>

<style scoped>
.update-notification {
  position: fixed;
  right: 1rem;
  bottom: 1rem;
  z-index: 1150;
  width: min(100% - 2rem, 24rem);
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 0.8rem;
  border: 1px solid var(--app-border);
  border-radius: 10px;
  background: var(--app-bg);
  color: var(--app-text);
  padding: 0.85rem;
  box-shadow: 0 20px 48px rgb(15 23 42 / 0.18);
}

.update-notification div {
  min-width: 0;
  display: grid;
  gap: 0.18rem;
}

.update-notification strong {
  font-size: 0.85rem;
}

.update-notification span {
  color: var(--app-text-muted);
  font-size: 0.75rem;
  line-height: 1.35;
}

.update-notification button {
  min-height: 2.2rem;
  border: 1px solid var(--app-primary);
  border-radius: 8px;
  background: var(--app-primary);
  color: #fff;
  padding: 0 0.8rem;
  font: inherit;
  font-size: 0.78rem;
  font-weight: 700;
  cursor: pointer;
}

@media (max-width: 560px) {
  .update-notification {
    left: 1rem;
    right: 1rem;
    bottom: 4.85rem;
    width: auto;
  }
}
</style>
