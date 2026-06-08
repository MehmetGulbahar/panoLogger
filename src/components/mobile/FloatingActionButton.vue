<template>
  <button class="fab" @click="$emit('click')" :aria-label="ariaLabel">
    <slot><i class="pi pi-plus" aria-hidden="true"></i></slot>
  </button>
</template>

<script setup lang="ts">
defineProps({ ariaLabel: { type: String, default: 'Eylem' } });
defineEmits(['click']);
</script>

<style scoped>
.fab { position: fixed; right: 1rem; bottom: 68px; width: 44px; height:44px; border-radius:999px; background:var(--color-primary); color:var(--color-white); display:flex; align-items:center; justify-content:center; box-shadow:var(--shadow-2); border:none; cursor:pointer; z-index:1400 }
.fab .pi { font-size:0.95rem }
@media (min-width: 961px) { .fab { display:none } }
</style>
