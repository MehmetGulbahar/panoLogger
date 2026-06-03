<template>
  <div class="page page--panel-detail">
    <header class="panel-header">
      <div>
        <h1 class="panel-title">{{ panel.name }}</h1>
        <div class="panel-sub">{{ panel.code }} · {{ facility.address ? facility.address.split(',')[0] : facility.city }} · {{ facility.name }}</div>
      </div>

      <div v-if="hasPanelActions" class="panel-actions">
        <UiButton v-if="canManageQr" variant="ghost" class="mr" :disabled="isCreatingQr" @click="onCreateQr">
          <i :class="isCreatingQr ? 'pi pi-spin pi-spinner' : 'pi pi-qrcode'" aria-hidden="true"></i>
          {{ isCreatingQr ? 'Oluşturuluyor' : 'QR Kodu Oluştur' }}
        </UiButton>
        <UiButton v-if="canManageFiles" color="primary" :disabled="isUploading" @click="openFilePicker">
          <i :class="isUploading ? 'pi pi-spin pi-spinner' : 'pi pi-upload'" aria-hidden="true"></i>
          {{ isUploading ? 'Yükleniyor' : 'Dosya Yükle' }}
        </UiButton>
        <input
          ref="fileInput"
          class="sr-only"
          type="file"
          accept=".pdf,.jpg,.jpeg,.png,.docx"
          @change="onFileSelected"
        />
      </div>
    </header>

    <p v-if="qrError" class="panel-error" role="alert">{{ qrError }}</p>
    <p v-if="fileError" class="panel-error" role="alert">{{ fileError }}</p>

    <nav class="panel-tabs" role="tablist">
      <button class="tab active" role="tab">Bakım</button>
      <button class="tab" role="tab">Tek Hat</button>
      <button class="tab" role="tab">Proje</button>
    </nav>

    <main>
      <section class="panel-card card">
        <div class="panel-card__header">
          <h3><i class="pi pi-wrench"></i> Bakım Raporu</h3>
        </div>

        <div class="panel-card__body">
          <div v-if="isLoadingFiles" class="empty-state">
            <i class="pi pi-spin pi-spinner" aria-hidden="true"></i>
            <p class="empty-title">Dosyalar yükleniyor</p>
          </div>

          <div v-else-if="panelFiles.length === 0" class="empty-state">
            <i class="pi pi-file" aria-hidden="true"></i>
            <p class="empty-title">Bu kategoride henüz dosya yok</p>
            <p class="empty-sub">{{ emptyStateSubtitle }}</p>
          </div>

          <div v-else class="file-list">
            <div v-for="item in panelFiles" :key="item.id" class="file-row">
              <i class="pi pi-file file-row__icon" aria-hidden="true"></i>
              <div class="file-row__copy">
                <strong>{{ item.fileName }}</strong>
                <span>{{ formatFileSize(item.sizeBytes) }}</span>
              </div>
              <button class="file-row__download" type="button" title="İndir" @click="downloadFile(item)">
                <i class="pi pi-download" aria-hidden="true"></i>
              </button>
            </div>
          </div>
        </div>
      </section>
    </main>
  </div>
</template>

<script setup lang="ts">
import axios from 'axios';
import { computed, ref, watch } from 'vue';
import { useRoute } from 'vue-router';
import { apiClient } from '@/api/client';
import { apiEndpoints } from '@/api/endpoints';
import QrCodeModal from '@/components/qr/QrCodeModal.vue';
import { useAuthStore, useHierarchyStore } from '@/stores';
import { useFileStore } from '@/stores/file-store';
import { useModalStore } from '@/stores/modal-store';
import type { FileDownloadResponse, PanelFileResponse } from '@/types/files';
import type { GeneratedQrCodeResponse } from '@/types/qr';

const route = useRoute();
const panelId = computed(() => String(route.params.panelId ?? ''));
const authStore = useAuthStore();
const hierarchyStore = useHierarchyStore();
const modalStore = useModalStore();
const isCreatingQr = ref(false);
const qrError = ref('');
const fileInput = ref<HTMLInputElement | null>(null);
const panelFiles = ref<PanelFileResponse[]>([]);
const isLoadingFiles = ref(false);
const isUploading = ref(false);
const fileError = ref('');
const canManageQr = computed(() => authStore.hasPermission('qr.manage'));
const canManageFiles = computed(() => authStore.hasPermission('files.manage'));
const hasPanelActions = computed(() => canManageQr.value || canManageFiles.value);
const emptyStateSubtitle = computed(() => (
  canManageFiles.value
    ? 'Yukarıdaki butonu kullanarak dosya yükleyebilirsiniz'
    : 'Bu kategori için dosya bulunmuyor'
));

const panel = computed(() => {
  if (panelId.value) return hierarchyStore.panels.find((p) => p.id === panelId.value) ?? emptyPanel;
  return emptyPanel;
});

const emptyPanel = { id: '', facilityId: '', code: '', name: '', description: '' };
const emptyFacility = { id: '', companyId: '', name: '', city: '', district: '', address: '', panels: [] };
const facility = computed(() => hierarchyStore.facilities.find((f) => f.id === panel.value.facilityId) ?? emptyFacility);
const fileStore = useFileStore();

watch(panelId, async (nextPanelId) => {
  panelFiles.value = [];
  fileError.value = '';
  await hierarchyStore.load();
  if (nextPanelId) {
    await loadFiles(nextPanelId);
  }
}, { immediate: true });

async function onCreateQr() {
  isCreatingQr.value = true;
  qrError.value = '';

  try {
    const { data } = await apiClient.post<GeneratedQrCodeResponse>(`${apiEndpoints.qr}/panel-codes`, {
      prefix: 'PNL',
    });

    panel.value.code = data.code;
    modalStore.open(QrCodeModal, {
      panelName: panel.value.name,
      qr: data,
    });
  } catch (error) {
    qrError.value = axios.isAxiosError(error) && typeof error.response?.data?.detail === 'string'
      ? error.response.data.detail
      : 'QR kodu oluşturulamadı. API bağlantısını ve kullanıcı yetkisini kontrol edin.';
  } finally {
    isCreatingQr.value = false;
  }
}

function openFilePicker() {
  fileInput.value?.click();
}

async function onFileSelected(event: Event) {
  const input = event.target as HTMLInputElement;
  const selectedFile = input.files?.[0];

  if (!selectedFile) {
    return;
  }

  isUploading.value = true;
  fileError.value = '';

  try {
    const formData = new FormData();
    formData.append('file', selectedFile);
    formData.append('category', 'MaintenanceReport');

    const { data } = await apiClient.post<PanelFileResponse>(
      `${apiEndpoints.files}/panels/${panel.value.id}`,
      formData,
      { headers: { 'Content-Type': 'multipart/form-data' } },
    );

    panelFiles.value.unshift(data);
    fileStore.setPanelFileCount(panel.value.id, panelFiles.value.length);
  } catch (error) {
    fileError.value = getApiError(error, 'Dosya yüklenemedi. Supabase Storage ayarlarını ve dosya türünü kontrol edin.');
  } finally {
    isUploading.value = false;
    input.value = '';
  }
}

async function loadFiles(requestedPanelId = panel.value.id) {
  if (!requestedPanelId) {
    return;
  }

  isLoadingFiles.value = true;
  fileError.value = '';

  try {
    const { data } = await apiClient.get<PanelFileResponse[]>(`${apiEndpoints.files}/panels/${requestedPanelId}`);
    fileStore.setPanelFileCount(requestedPanelId, data.length);
    if (panelId.value === requestedPanelId) {
      panelFiles.value = data;
    }
  } catch (error) {
    if (panelId.value === requestedPanelId) {
      fileError.value = getApiError(error, 'Dosyalar yüklenemedi.');
    }
  } finally {
    if (panelId.value === requestedPanelId) {
      isLoadingFiles.value = false;
    }
  }
}

async function downloadFile(item: PanelFileResponse) {
  fileError.value = '';

  try {
    const { data } = await apiClient.get<FileDownloadResponse>(`${apiEndpoints.files}/${item.id}/download`);
    window.open(data.signedUrl, '_blank', 'noopener,noreferrer');
  } catch (error) {
    fileError.value = getApiError(error, 'Dosya indirme bağlantısı oluşturulamadı.');
  }
}

function formatFileSize(sizeBytes: number) {
  if (sizeBytes < 1024 * 1024) {
    return `${Math.max(1, Math.round(sizeBytes / 1024))} KB`;
  }

  return `${(sizeBytes / (1024 * 1024)).toFixed(1)} MB`;
}

function getApiError(error: unknown, fallback: string) {
  return axios.isAxiosError(error) && typeof error.response?.data?.detail === 'string'
    ? error.response.data.detail
    : fallback;
}
</script>

<style scoped>
.panel-header { display:flex; align-items:center; justify-content:space-between; gap:1rem; margin-bottom:0.9rem }
.panel-title { margin:0; font-size:1.25rem; line-height:1.15; font-weight:800 }
.panel-sub { color:var(--app-text-muted); margin-top:0.15rem; font-size:0.8125rem; line-height:1.35 }
.panel-actions { display:flex; gap:0.5rem }
.panel-error { margin:0 0 0.9rem; padding:0.6rem 0.75rem; border:1px solid #fecaca; border-radius:8px; background:#fef2f2; color:#b91c1c; font-size:0.8125rem }

.panel-tabs { display:flex; gap:0.4rem; margin-bottom:0.9rem }
.tab { background:var(--app-surface); border:1px solid var(--app-border); padding:0.45rem 0.75rem; border-radius:8px; color:var(--app-text-muted); font-size:0.8125rem; line-height:1.2 }
.tab.active { background:var(--app-surface-alt); color:var(--app-text); font-weight:600 }

.panel-card.card { padding:1rem 1.25rem; border-radius:10px; border:1px solid var(--app-border); background:var(--app-bg) }
.panel-card__header { margin-bottom:0.9rem }
.panel-card__header h3 { display:flex; align-items:center; gap:0.5rem; margin:0; font-size:0.95rem; line-height:1.25 }
.panel-card__header .pi { color:var(--app-primary); font-size:0.95rem }

.panel-card__body { min-height:180px; display:flex; align-items:center; justify-content:center }
.sr-only { position:absolute; width:1px; height:1px; padding:0; margin:-1px; overflow:hidden; clip:rect(0,0,0,0); white-space:nowrap; border:0 }
.empty-state { text-align:center; color:var(--app-text-muted) }
.empty-state .pi { font-size:1.4rem; display:block; margin-bottom:0.45rem }
.empty-title { font-weight:600; margin-bottom:0.2rem; font-size:0.875rem }
.empty-sub { font-size:0.8125rem; margin:0 }
.file-list { width:100%; display:grid; gap:0.5rem }
.file-row { display:flex; align-items:center; gap:0.65rem; padding:0.65rem 0.75rem; border:1px solid var(--app-border); border-radius:8px; background:var(--app-bg) }
.file-row__icon { color:var(--app-primary); font-size:0.95rem }
.file-row__copy { display:grid; gap:0.1rem; min-width:0; flex:1 }
.file-row__copy strong { overflow:hidden; text-overflow:ellipsis; white-space:nowrap; font-size:0.8125rem }
.file-row__copy span { color:var(--app-text-muted); font-size:0.6875rem }
.file-row__download { width:2rem; height:2rem; display:grid; place-items:center; border:0; border-radius:6px; background:transparent; color:var(--app-primary); cursor:pointer }
.file-row__download:hover { background:var(--app-surface-alt) }

@media (max-width: 760px) {
  .panel-header { flex-direction:column; align-items:flex-start }
  .panel-actions { width:100%; justify-content:flex-end }
}
</style>
