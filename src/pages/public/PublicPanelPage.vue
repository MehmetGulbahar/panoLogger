<template>
  <main class="public-panel-page">
    <section class="public-shell">
      <header class="public-header">
        <div class="public-header__mark">
          <i class="pi pi-bolt" aria-hidden="true"></i>
        </div>
        <div>
          <p class="public-eyebrow">PanoVeri</p>
          <h1>{{ panelTitle }}</h1>
          <p>{{ panelSubtitle }}</p>
        </div>
      </header>

      <section v-if="isLoading" class="public-state" aria-live="polite">
        <i class="pi pi-spin pi-spinner" aria-hidden="true"></i>
        <strong>Panel bilgileri yukleniyor</strong>
        <span>QR baglantisi kontrol ediliyor.</span>
      </section>

      <section v-else-if="errorMessage" class="public-state public-state--error" role="alert">
        <i class="pi pi-exclamation-triangle" aria-hidden="true"></i>
        <strong>Panel bulunamadi</strong>
        <span>{{ errorMessage }}</span>
      </section>

      <template v-else-if="publicPanel">
        <section class="panel-summary">
          <div>
            <span>Proje</span>
            <strong>{{ publicPanel.projectName }}</strong>
          </div>
          <div>
            <span>Konum</span>
            <strong>{{ publicPanel.city }}</strong>
          </div>
          <div>
            <span>Pano Kodu</span>
            <strong>{{ publicPanel.panelCode }}</strong>
          </div>
        </section>

        <section class="document-list" aria-label="Panel dosyalari">
          <article v-for="card in documentCards" :key="card.key" class="document-card">
            <div class="document-card__icon">
              <i :class="card.icon" aria-hidden="true"></i>
            </div>

            <div class="document-card__content">
              <div class="document-card__head">
                <div>
                  <h2>{{ card.title }}</h2>
                  <p>{{ card.description }}</p>
                </div>
                <span class="document-card__count">{{ card.count }} dosya</span>
              </div>

              <div v-if="card.files.length > 0" class="public-file-list">
                <div v-for="file in card.files" :key="file.id" class="public-file-row">
                  <div class="public-file-row__copy">
                    <strong>{{ file.fileName }}</strong>
                    <span>{{ formatFileSize(file.sizeBytes) }}</span>
                  </div>
                  <div class="public-file-row__actions">
                    <button type="button" class="file-action" title="Goruntule" @click="viewFile(file)">
                      <i class="pi pi-eye" aria-hidden="true"></i>
                    </button>
                    <button type="button" class="file-action file-action--primary" title="Indir" @click="downloadFile(file)">
                      <i class="pi pi-download" aria-hidden="true"></i>
                    </button>
                  </div>
                </div>
              </div>

              <p v-else class="document-card__empty">Bu kategoride dosya yok.</p>
            </div>
          </article>
        </section>

        <footer class="public-footer">
          <span>QR erisim baglantisi</span>
          <strong>{{ publicPanel.panelCode }}</strong>
        </footer>
      </template>
    </section>
  </main>
</template>

<script setup lang="ts">
import axios from 'axios';
import { computed, ref, watch } from 'vue';
import { useRoute } from 'vue-router';
import { apiClient } from '@/api/client';
import { apiEndpoints } from '@/api/endpoints';
import type { PublicPanelFileResponse, PublicPanelResponse } from '@/types/public-panel';

type PublicDocumentCard = {
  key: string;
  category: string;
  title: string;
  description: string;
  icon: string;
  count: number;
  files: PublicPanelFileResponse[];
};

const route = useRoute();
const publicPanel = ref<PublicPanelResponse | null>(null);
const isLoading = ref(false);
const errorMessage = ref('');

const panelCode = computed(() => String(route.params.panelCode ?? '').trim());
const panelTitle = computed(() => publicPanel.value?.panelName ?? 'Panel bilgisi');
const panelSubtitle = computed(() => {
  if (!publicPanel.value) {
    return panelCode.value || 'QR baglantisi';
  }

  return `${publicPanel.value.panelCode} · ${publicPanel.value.facilityName}`;
});

const documentCards = computed<PublicDocumentCard[]>(() => [
  {
    key: 'electrical-project',
    category: 'ElectricalProject',
    title: 'Elektrik Projesi',
    description: 'Tek hat ve proje dosyalari',
    icon: 'pi pi-sitemap',
    count: publicPanel.value?.documents.electricalProjectCount ?? 0,
    files: filesByCategory('ElectricalProject'),
  },
  {
    key: 'maintenance-report',
    category: 'MaintenanceReport',
    title: 'Bakım Raporu',
    description: 'Periyodik bakım kayıtları',
    icon: 'pi pi-wrench',
    count: publicPanel.value?.documents.maintenanceReportCount ?? 0,
    files: filesByCategory('MaintenanceReport'),
  },
  {
    key: 'panel-documents',
    category: 'PanelDocument',
    title: 'Pano Dokumanlari',
    description: 'Yuklenen teknik dosyalar',
    icon: 'pi pi-file',
    count: publicPanel.value?.documents.panelDocumentCount ?? 0,
    files: filesByCategory('PanelDocument'),
  },
]);

watch(panelCode, loadPanel, { immediate: true });

async function loadPanel(code: string) {
  publicPanel.value = null;
  errorMessage.value = '';

  if (!code) {
    errorMessage.value = 'QR baglantisinda panel kodu bulunamadi.';
    return;
  }

  isLoading.value = true;

  try {
    const { data } = await apiClient.get<PublicPanelResponse>(
      `${apiEndpoints.public}/panels/${encodeURIComponent(code)}`,
    );
    publicPanel.value = data;
  } catch (error) {
    errorMessage.value = axios.isAxiosError(error) && error.response?.status === 404
      ? 'Bu QR koduna bagli kayitli bir pano yok.'
      : 'Panel bilgileri yuklenemedi. Baglantiyi veya API durumunu kontrol edin.';
  } finally {
    isLoading.value = false;
  }
}

function filesByCategory(category: string) {
  return publicPanel.value?.documents.files.filter((file) => file.category === category) ?? [];
}

function viewFile(file: PublicPanelFileResponse) {
  window.open(resolveApiUrl(file.viewUrl), '_blank', 'noopener,noreferrer');
}

function downloadFile(file: PublicPanelFileResponse) {
  const link = document.createElement('a');

  link.href = resolveApiUrl(file.downloadUrl);
  link.download = file.fileName;
  link.rel = 'noopener noreferrer';
  document.body.appendChild(link);
  link.click();
  link.remove();
}

function resolveApiUrl(path: string) {
  const baseUrl = apiClient.defaults.baseURL ?? window.location.origin;
  return new URL(path, baseUrl).toString();
}

function formatFileSize(sizeBytes: number) {
  if (sizeBytes < 1024 * 1024) {
    return `${Math.max(1, Math.round(sizeBytes / 1024))} KB`;
  }

  return `${(sizeBytes / (1024 * 1024)).toFixed(1)} MB`;
}
</script>

<style scoped>
.public-panel-page {
  min-height: 100vh;
  background: #f5f7fb;
  color: var(--app-text);
  padding: 1rem;
}

.public-shell {
  width: min(100%, 30rem);
  margin: 0 auto;
}

.public-header {
  display: flex;
  gap: 0.75rem;
  align-items: center;
  padding: 0.5rem 0 1rem;
}

.public-header__mark {
  width: 2.25rem;
  height: 2.25rem;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  border-radius: 10px;
  background: var(--primary-50);
  color: var(--app-primary);
  flex: 0 0 auto;
}

.public-header__mark .pi {
  font-size: 1rem;
}

.public-eyebrow {
  margin: 0 0 0.1rem;
  font-size: 0.6875rem;
  line-height: 1.2;
  color: var(--app-primary);
  font-weight: 700;
  text-transform: uppercase;
}

.public-header h1 {
  margin: 0;
  font-size: 1.25rem;
  line-height: 1.15;
  font-weight: 800;
}

.public-header p:last-child {
  margin: 0.15rem 0 0;
  font-size: 0.8125rem;
  line-height: 1.35;
  color: var(--app-text-muted);
}

.panel-summary {
  display: grid;
  grid-template-columns: 1fr;
  gap: 0.65rem;
  margin-bottom: 0.85rem;
}

.panel-summary div,
.document-card,
.public-state {
  background: var(--app-bg);
  border: 1px solid var(--app-border);
  border-radius: 10px;
  box-shadow: var(--app-shadow);
}

.panel-summary div {
  padding: 0.75rem 0.85rem;
}

.panel-summary span {
  display: block;
  font-size: 0.6875rem;
  line-height: 1.2;
  color: var(--app-text-muted);
}

.panel-summary strong {
  display: block;
  margin-top: 0.12rem;
  font-size: 0.8125rem;
  line-height: 1.2;
}

.public-state {
  display: grid;
  gap: 0.35rem;
  justify-items: start;
  padding: 1rem;
  color: var(--app-text-muted);
}

.public-state .pi {
  color: var(--app-primary);
  font-size: 1.15rem;
}

.public-state strong {
  color: var(--app-text);
  font-size: 0.95rem;
}

.public-state span {
  font-size: 0.8125rem;
  line-height: 1.35;
}

.public-state--error {
  border-color: #fecaca;
  background: #fef2f2;
}

.public-state--error .pi,
.public-state--error strong {
  color: #b91c1c;
}

.document-list {
  display: grid;
  gap: 0.75rem;
}

.document-card {
  display: flex;
  gap: 0.75rem;
  padding: 0.85rem;
}

.document-card__icon {
  width: 2rem;
  height: 2rem;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  border-radius: 8px;
  background: var(--primary-50);
  color: var(--app-primary);
  flex: 0 0 auto;
}

.document-card__icon .pi {
  font-size: 0.95rem;
}

.document-card__content {
  flex: 1;
  min-width: 0;
}

.document-card__head {
  display: flex;
  justify-content: space-between;
  gap: 0.75rem;
}

.document-card h2 {
  margin: 0;
  font-size: 0.9rem;
  line-height: 1.25;
  font-weight: 800;
}

.document-card p {
  margin: 0.2rem 0 0;
  color: var(--app-text-muted);
  font-size: 0.75rem;
  line-height: 1.3;
}

.document-card__empty {
  margin: 0.75rem 0 0;
  color: var(--app-text-muted);
  font-size: 0.75rem;
}

.document-card__count {
  align-self: flex-start;
  border: 1px solid var(--app-border);
  border-radius: 999px;
  padding: 0.18rem 0.5rem;
  color: var(--app-text);
  font-size: 0.6875rem;
  line-height: 1.2;
  font-weight: 700;
  white-space: nowrap;
}

.public-file-list {
  display: grid;
  gap: 0.45rem;
  margin-top: 0.75rem;
}

.public-file-row {
  display: flex;
  align-items: center;
  gap: 0.65rem;
  padding: 0.55rem 0.6rem;
  border: 1px solid var(--app-border);
  border-radius: 8px;
  background: #fbfdff;
}

.public-file-row__copy {
  display: grid;
  gap: 0.12rem;
  min-width: 0;
  flex: 1;
}

.public-file-row__copy strong {
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  font-size: 0.75rem;
  line-height: 1.25;
}

.public-file-row__copy span {
  color: var(--app-text-muted);
  font-size: 0.6875rem;
  line-height: 1.2;
}

.public-file-row__actions {
  display: flex;
  gap: 0.25rem;
  flex: 0 0 auto;
}

.file-action {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 2rem;
  height: 2rem;
  border: 1px solid var(--app-border);
  border-radius: 8px;
  background: var(--app-bg);
  color: var(--app-text-muted);
  cursor: pointer;
}

.file-action:hover {
  border-color: var(--app-primary);
  color: var(--app-primary);
  background: var(--primary-50);
}

.file-action--primary {
  background: var(--app-primary);
  border-color: var(--app-primary);
  color: var(--color-white);
}

.file-action--primary:hover {
  color: var(--color-white);
  background: #1d4ed8;
}

.file-action .pi {
  font-size: 0.85rem;
}

.public-footer {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 0.75rem;
  padding: 1rem 0.25rem 0;
  color: var(--app-text-muted);
  font-size: 0.75rem;
}

.public-footer strong {
  color: var(--app-text);
}

@media (min-width: 720px) {
  .public-panel-page {
    padding: 2rem;
  }

  .panel-summary {
    grid-template-columns: repeat(3, 1fr);
  }
}
</style>
