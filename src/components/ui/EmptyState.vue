<template>
  <div class="ui-empty">
    <div class="ui-empty__icon" v-if="$slots.icon"><slot name="icon" /></div>
    <div class="ui-empty__title">{{ title }}</div>
    <div class="ui-empty__desc" v-if="description">{{ description }}</div>
  </div>
</template>

<script setup lang="ts">
defineProps({ title: { type: String, default: 'Boş' }, description: { type: String, default: '' } });
</script>

<style scoped>
.ui-empty { display:flex; flex-direction:column; align-items:center; gap:var(--space-xs); padding:var(--space-md); color:var(--app-text-muted) }
.ui-empty__title { font-weight:var(--fw-medium); color:var(--app-text) }
.ui-empty__desc { font-size:var(--type-scale-sm) }
</style>
