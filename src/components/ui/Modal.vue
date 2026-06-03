<template>
  <transition name="ui-modal-fade" appear>
    <div class="ui-modal__overlay" @click.self="handleBackdrop" role="presentation">
      <div
        class="ui-modal"
        role="dialog"
        :aria-labelledby="title ? titleId : undefined"
        aria-modal="true"
        ref="dialog"
        tabindex="-1"
      >
      <header class="ui-modal__header">
        <h3 v-if="title" :id="titleId">{{ title }}</h3>
        <button class="ui-modal__close" @click="close" aria-label="Kapat"><i class="pi pi-times" aria-hidden="true"></i></button>
      </header>

      <section class="ui-modal__body">
        <slot />
      </section>

      <footer class="ui-modal__footer">
        <slot name="footer" />
      </footer>
      </div>
    </div>
  </transition>
</template>

<script setup lang="ts">
import { onMounted, onBeforeUnmount, ref } from 'vue';

const props = defineProps({
  title: { type: String, default: '' },
  closeOnEsc: { type: Boolean, default: true },
  closeOnBackdrop: { type: Boolean, default: true },
});
const emit = defineEmits(['close']);

const titleId = `modal-title-${Math.random().toString(36).slice(2,8)}`;
const dialog = ref<HTMLElement | null>(null);

function close() { emit('close'); }

function handleKey(e: KeyboardEvent) {
  if (e.key === 'Escape' && props.closeOnEsc) close();
}

function handleBackdrop() {
  if (props.closeOnBackdrop) close();
}

onMounted(() => {
  document.addEventListener('keydown', handleKey);
  // focus dialog
  setTimeout(() => dialog.value?.focus(), 0);
});
onBeforeUnmount(() => document.removeEventListener('keydown', handleKey));
</script>

<style scoped>
.ui-modal__overlay { position: fixed; inset: 0; background: rgba(15,23,42,0.4); display:flex; align-items:center; justify-content:center; z-index: 1200 }
.ui-modal { background: var(--app-surface); border-radius: 10px; min-width: 320px; max-width: 920px; width: 90%; box-shadow: var(--shadow-3); outline: none; opacity: 1; transform: translateY(0) scale(1) }
.ui-modal__header { display:flex; align-items:center; justify-content:space-between; padding:0.8rem 1rem; border-bottom:1px solid var(--app-border) }
.ui-modal__header h3 { margin:0; font-size:0.95rem; line-height:1.25 }
.ui-modal__body { padding:1rem; font-size:0.8125rem; line-height:1.4 }
.ui-modal__footer { padding:0.8rem 1rem; border-top:1px solid var(--app-border); display:flex; justify-content:flex-end; gap:0.5rem }
.ui-modal__close { display:inline-flex; align-items:center; justify-content:center; width:2rem; height:2rem; background:transparent; border:none; border-radius:8px; color:var(--app-text-muted); cursor:pointer }
.ui-modal__close .pi { font-size:0.85rem }

/* transition */
.ui-modal-fade-enter-active, .ui-modal-fade-leave-active { transition: opacity 180ms ease, transform 180ms ease }
.ui-modal-fade-enter-from, .ui-modal-fade-leave-to { opacity: 0 }
.ui-modal-fade-enter-to, .ui-modal-fade-leave-from { opacity: 1 }
.ui-modal-fade-enter-from .ui-modal, .ui-modal-fade-leave-to .ui-modal { transform: translateY(10px) scale(0.98); opacity: 0 }
.ui-modal-fade-enter-to .ui-modal, .ui-modal-fade-leave-from .ui-modal { transform: translateY(0) scale(1); opacity: 1 }
</style>
