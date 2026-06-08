<template>
  <div :class="hiddenClass"><slot /></div>
</template>

<script setup lang="ts">
import { computed } from 'vue';

const props = defineProps({ at: { type: String, default: 'sm' } });

const hiddenClass = computed(() => {
  if (props.at === 'sm') return 'hidden-sm';
  if (props.at === 'md') return 'hidden-md';
  return 'hidden-lg';
});
</script>

<style scoped>
.hidden-sm { display:block }
@media (max-width:960px) { .hidden-sm { display:none } }

.hidden-md { display:block }
@media (max-width:1280px) { .hidden-md { display:none } }

.hidden-lg { display:block }
@media (max-width:1600px) { .hidden-lg { display:none } }
</style>
