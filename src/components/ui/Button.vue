<template>
  <button
    :class="classes"
    :type="type"
    :disabled="disabled"
    @click="$emit('click', $event)"
  >
    <slot />
  </button>
</template>

<script setup lang="ts">
const props = defineProps({
  variant: { type: String as () => 'primary' | 'secondary' | 'ghost', default: 'primary' },
  size: { type: String as () => 'sm' | 'md' | 'lg', default: 'md' },
  disabled: { type: Boolean, default: false },
  type: { type: String as () => 'button' | 'submit' | 'reset', default: 'button' },
});

const emit = defineEmits(['click']);

import { computed } from 'vue';

const classes = computed(() => [
  'ui-button',
  `ui-button--${props.variant}`,
  `ui-button--${props.size}`,
  { 'is-disabled': props.disabled },
]);
</script>

<style scoped>
.ui-button {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: var(--space-xs);
  border-radius: var(--radius-sm);
  padding: 0.5rem 0.75rem;
  font-weight: var(--fw-medium);
  font-size: 0.8125rem;
  line-height: 1;
  cursor: pointer;
  transition: background-color 0.12s ease, box-shadow 0.12s ease, transform 0.08s ease;
  border: 1px solid transparent;
}

.ui-button:focus {
  outline: none;
  box-shadow: 0 0 0 4px rgba(37,99,235,0.12);
}

.ui-button.is-disabled {
  opacity: 0.6;
  cursor: not-allowed;
  pointer-events: none;
}

/* Variants */
.ui-button--primary {
  background: var(--color-primary);
  color: var(--color-white);
  border-color: transparent;
}
.ui-button--primary:hover:not(.is-disabled) {
  background: var(--primary-700);
}

.ui-button--secondary {
  background: var(--app-surface);
  color: var(--app-text);
  border-color: var(--app-border);
}
.ui-button--secondary:hover:not(.is-disabled) {
  background: var(--app-surface-alt);
}

.ui-button--ghost {
  background: transparent;
  color: var(--color-primary);
  border: none;
}
.ui-button--ghost:hover:not(.is-disabled) {
  background: rgba(37,99,235,0.06);
}

/* Sizes */
.ui-button--sm { padding: 0.35rem 0.55rem; font-size: var(--type-scale-xs) }
.ui-button--md { padding: 0.5rem 0.75rem; font-size: 0.8125rem }
.ui-button--lg { padding: 0.7rem 0.95rem; font-size: var(--type-scale-sm) }

/* Icon support */
.ui-button :is(svg, i) { display:inline-flex; vertical-align:middle; font-size:0.9em }
</style>
