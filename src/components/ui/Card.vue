<template>
  <div :class="classes">
    <slot />
  </div>
</template>

<script setup lang="ts">
const props = defineProps({
  flat: { type: Boolean, default: false },
  padding: { type: String as () => 'sm' | 'md' | 'lg', default: 'md' },
  className: { type: String, default: '' },
});

import { computed } from 'vue';

const classes = computed(() => [
  'ui-card',
  props.flat ? 'ui-card--flat' : 'ui-card--elevated',
  `ui-card--padding-${props.padding}`,
  props.className,
]);
</script>

<style scoped>
.ui-card { border-radius: var(--radius-md); background: var(--app-surface); border: 1px solid var(--app-border); box-shadow: var(--shadow-1); }
.ui-card--flat { border: none; box-shadow: none; background: transparent }
.ui-card--elevated:hover { box-shadow: var(--shadow-2); transform: translateY(-3px); }
.ui-card--padding-sm { padding: var(--space-xs) }
.ui-card--padding-md { padding: var(--space-md) }
.ui-card--padding-lg { padding: var(--space-lg) }
</style>
