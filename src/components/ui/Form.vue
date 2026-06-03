<template>
  <form class="ui-form" @submit.prevent="onSubmit">
    <slot />

    <div class="ui-form__actions">
      <slot name="actions"></slot>
    </div>
  </form>
</template>

<script setup lang="ts">
const emit = defineEmits(['submit']);

function onSubmit(e: Event) {
  emit('submit', e);
}
</script>

<style scoped>
.ui-form { display:flex; flex-direction:column; gap:var(--space-md) }
.ui-form__actions { display:flex; gap:var(--space-sm); justify-content:flex-end }
</style>
