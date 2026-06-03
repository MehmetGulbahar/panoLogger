<template>
  <Message v-if="globalError" severity="error" :closable="true" @close="resetError">
    {{ globalError }}
  </Message>
</template>

<script setup lang="ts">
import Message from 'primevue/message';
import { storeToRefs } from 'pinia';

import { useUiStore } from '@/stores/ui-store';

const uiStore = useUiStore();
const { globalError } = storeToRefs(uiStore);
const { resetError } = uiStore;
</script>