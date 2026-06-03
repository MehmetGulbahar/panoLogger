<template>
  <div :class="['mobile-drawer', { open: modelValue }]" @click.self="close">
    <div class="mobile-drawer__sheet" role="dialog" aria-modal="true">
      <header class="mobile-drawer__header">
        <slot name="header">Başlık</slot>
        <button class="close" @click="close" aria-label="Kapat"><i class="pi pi-times" aria-hidden="true"></i></button>
      </header>
      <div class="mobile-drawer__body">
        <slot />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { watch, onBeforeUnmount } from 'vue';

const props = defineProps({ modelValue: { type: Boolean, default: false } });
const emit = defineEmits(['update:modelValue','close']);

let previousOverflow: string | null = null;

watch(() => props.modelValue, (val) => {
  if (val) {
    previousOverflow = document.body.style.overflow;
    document.body.style.overflow = 'hidden';
  } else {
    if (previousOverflow !== null) document.body.style.overflow = previousOverflow;
    previousOverflow = null;
  }
});

onBeforeUnmount(() => {
  if (previousOverflow !== null) document.body.style.overflow = previousOverflow;
});

function close() { emit('update:modelValue', false); emit('close'); }
</script>

<style scoped>
.mobile-drawer { display:none }
@media (max-width:960px) {
  .mobile-drawer { display:block; position:fixed; inset:0; z-index:1250 }
  .mobile-drawer { background: rgba(0,0,0,0.4); opacity:0; transition:opacity .18s }
  .mobile-drawer.open { opacity:1 }
  .mobile-drawer__sheet { position: absolute; left:0; right:0; bottom:0; background:var(--app-surface); border-top-left-radius:10px; border-top-right-radius:10px; max-height:80vh; overflow:auto; transform: translateY(20%); transition: transform .22s cubic-bezier(.2,.9,.2,1), opacity .18s }
  .mobile-drawer.open .mobile-drawer__sheet { transform: translateY(0); opacity:1 }
  .mobile-drawer__header { padding: 0.8rem 1rem; display:flex; justify-content:space-between; align-items:center; border-bottom:1px solid var(--app-border); font-size:0.875rem; font-weight:700 }
  .mobile-drawer__body { padding: 1rem }
  .close { display:inline-flex; align-items:center; justify-content:center; width:2rem; height:2rem; border:0; border-radius:8px; background:transparent; color:var(--app-text-muted); cursor:pointer }
  .close .pi { font-size:0.85rem }
}
</style>
