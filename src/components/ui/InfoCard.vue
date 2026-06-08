<template>
  <Card class="info-card" :padding="'md'">
    <div class="info-card__accent" :style="{ background: accentColor }" aria-hidden="true"></div>

    <div class="info-card__body">
      <header class="info-card__header">
        <div class="info-card__title">{{ title }}</div>
        <div v-if="subtitle" class="info-card__subtitle">{{ subtitle }}</div>
      </header>

      <div class="info-card__meta">
        <slot name="meta">
          <template v-for="m in meta" :key="m">
            <div class="info-card__meta-item">{{ m }}</div>
          </template>
        </slot>
      </div>

      <div v-if="description" class="info-card__desc">{{ description }}</div>
    </div>

    <div v-if="hasActions" class="info-card__actions">
      <slot name="actions"></slot>
    </div>
  </Card>
</template>

<script setup lang="ts">
import { computed, useSlots } from 'vue';
import Card from './Card.vue';

const slots = useSlots();

const props = defineProps<{
  title: string;
  subtitle?: string;
  description?: string;
  meta?: string[];
  accentColor?: string;
}>();

const accentColor = computed(() => props.accentColor ?? 'var(--app-primary)');
const meta = computed(() => props.meta ?? []);
const hasActions = computed(() => !!slots.actions);
</script>

<style scoped>
.info-card { display:flex; gap:var(--space-md); align-items:flex-start }
.info-card__accent { width:6px; border-radius:calc(var(--radius-sm)/2); flex:0 0 6px; margin-top:calc(var(--space-xs) + 2px) }
.info-card__body { flex:1; min-width:0 }
.info-card__header { display:flex; flex-direction:column }
.info-card__title { font-weight:var(--fw-bold); font-size:var(--type-scale-md); color:var(--app-text) }
.info-card__subtitle { color:var(--app-text-muted); font-size:var(--type-scale-sm); margin-top:4px }
.info-card__meta { display:flex; gap:var(--space-sm); margin-top:var(--space-xs); flex-wrap:wrap }
.info-card__meta-item { color:var(--app-text-muted); font-size:var(--type-scale-sm) }
.info-card__desc { margin-top:var(--space-xs); color:var(--app-text-muted); font-size:var(--type-scale-sm) }
.info-card__actions { margin-left:var(--space-md); display:flex; align-items:center }

@media (max-width:960px) {
  .info-card { gap:var(--space-sm) }
  .info-card__title { font-size:var(--type-scale-sm) }
}
</style>
