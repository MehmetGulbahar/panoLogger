<template>
  <div>
    <teleport v-for="m in modals" :key="m.id" to="body">
      <component
        :is="m.component"
        v-bind="m.props"
        @close="close(m.id)"
      >
        <template v-for="(s, name) in m.slots" :key="name" v-slot:[name]>
          <component :is="s" />
        </template>
      </component>
    </teleport>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useModalStore } from '@/stores/modal-store';

const store = useModalStore();
const modals = computed(() => store.modals);

function close(id: string) { store.close(id); }
</script>

<style scoped>
/* host does not need styles */
</style>
