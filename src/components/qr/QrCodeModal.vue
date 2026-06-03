<template>
  <Modal title="QR Kodu Oluşturuldu" @close="$emit('close')">
    <div class="qr-result">
      <div class="qr-preview" v-html="qr.svg" />

      <div class="qr-details">
        <span>Pano</span>
        <strong>{{ panelName }}</strong>
        <span>QR Kodu</span>
        <strong>{{ qr.code }}</strong>
        <span>Bağlantı</span>
        <a :href="qr.publicUrl" target="_blank" rel="noopener noreferrer">{{ qr.publicUrl }}</a>
      </div>
    </div>

    <template #footer>
      <UiButton variant="ghost" @click="$emit('close')">Kapat</UiButton>
      <UiButton variant="secondary" @click="downloadQr">
        <i class="pi pi-download" aria-hidden="true"></i>
        SVG İndir
      </UiButton>
      <UiButton variant="primary" @click="openPublicPage">
        <i class="pi pi-external-link" aria-hidden="true"></i>
        Bağlantıyı Aç
      </UiButton>
    </template>
  </Modal>
</template>

<script setup lang="ts">
import Modal from '@/components/ui/Modal.vue';
import type { GeneratedQrCodeResponse } from '@/types/qr';

const props = defineProps<{
  panelName: string;
  qr: GeneratedQrCodeResponse;
}>();

defineEmits(['close']);

function downloadQr() {
  const blob = new Blob([props.qr.svg], { type: 'image/svg+xml;charset=utf-8' });
  const url = URL.createObjectURL(blob);
  const link = document.createElement('a');

  link.href = url;
  link.download = `${props.qr.code}-qr.svg`;
  link.click();
  URL.revokeObjectURL(url);
}

function openPublicPage() {
  window.open(props.qr.publicUrl, '_blank', 'noopener,noreferrer');
}
</script>

<style scoped>
.qr-result { display:grid; grid-template-columns:180px minmax(0, 1fr); gap:1rem; align-items:start }
.qr-preview { width:180px; aspect-ratio:1; border:1px solid var(--app-border); padding:0.5rem; background:var(--color-white) }
.qr-preview :deep(svg) { display:block; width:100%; height:100% }
.qr-details { display:grid; gap:0.25rem; min-width:0 }
.qr-details span { margin-top:0.45rem; color:var(--app-text-muted); font-size:0.6875rem }
.qr-details strong,
.qr-details a { overflow-wrap:anywhere; color:var(--app-text); font-size:0.8125rem }
.qr-details a { color:var(--app-primary) }

@media (max-width: 560px) {
  .qr-result { grid-template-columns:1fr }
  .qr-preview { margin:0 auto }
}
</style>
