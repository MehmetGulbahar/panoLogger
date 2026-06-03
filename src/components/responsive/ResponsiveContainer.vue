<template>
  <div class="responsive-container">
    <slot />
  </div>
</template>

<script setup lang="ts">
</script>

<style scoped>
.responsive-container { width:100%; margin-left:auto; margin-right:auto; padding-left:var(--space-md); padding-right:var(--space-md); max-width:1200px }
@media (max-width:960px) { .responsive-container { padding-left:var(--space-sm); padding-right:var(--space-sm); max-width: 100% } }
</style>
