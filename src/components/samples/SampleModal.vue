<template>
  <Modal title="Örnek Modal">
    <p>Bu, modal sisteminin örnek içeriğidir. Buraya form veya bilgileri yerleştirebilirsiniz.</p>

    <template #footer>
      <UiButton variant="ghost" @click="$emit('close')">İptal</UiButton>
      <UiButton variant="primary" @click="confirm">Tamam</UiButton>
    </template>
  </Modal>
</template>

<script setup lang="ts">
import Modal from '@/components/ui/Modal.vue';

const emit = defineEmits(['close','confirm']);

function confirm() { emit('confirm'); emit('close'); }
</script>

<style scoped>
p { margin: 0 0 var(--space-sm) 0 }
</style>
