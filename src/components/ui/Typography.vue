<template>
  <component :is="tag" :class="classes"><slot /></component>
</template>

<script setup lang="ts">
import { computed } from 'vue';

const props = defineProps({
  variant: { type: String as () => 'h1'|'h2'|'h3'|'h4'|'h5'|'h6'|'p'|'small', default: 'p' },
  className: { type: String, default: '' },
});

const tag = computed(() => props.variant);
const classes = computed(() => {
  const base = props.variant.startsWith('h') ? `h-${props.variant.slice(1)}` : props.variant;
  return [base, props.className].filter(Boolean).join(' ');
});
</script>

<style scoped>
/* minimal; layout driven by tokens */
</style>
