<template>
  <div class="page page--panel-detail">
    <header class="panel-header">
      <div>
        <h1 class="panel-title">{{ panel.name }}</h1>
        <div class="panel-sub">{{ panel.code }} - {{ facility.address ? facility.address.split(',')[0] : facility.city }} - {{ facility.name }}</div>
      </div>

      <div v-if="hasPanelActions" class="panel-actions">
        <UiButton v-if="canManageQr" variant="ghost" class="mr" :disabled="isCreatingQr" @click="onCreateQr">
          <i :class="isCreatingQr ? 'pi pi-spin pi-spinner' : 'pi pi-qrcode'" aria-hidden="true"></i>
          {{ isCreatingQr ? 'Olusturuluyor' : 'QR Kodu Oluştur' }}
        </UiButton>
        <UiButton v-if="canManageFiles" color="primary" :disabled="isUploading" @click="openFilePicker">
          <i :class="isUploading ? 'pi pi-spin pi-spinner' : 'pi pi-upload'" aria-hidden="true"></i>
          {{ isUploading ? 'Yükleniyor' : `${activeTab.label} Yükle` }}
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

    <div class="category-strip">
      <nav class="panel-tabs" role="tablist" aria-label="Dosya kategorileri">
        <button
          v-for="tab in fileTabs"
          :key="tab.key"
          :class="['tab', { active: activeFileTab === tab.key }]"
          type="button"
          role="tab"
          :aria-selected="activeFileTab === tab.key"
          @click="activeFileTab = tab.key"
        >
          {{ tab.label }}
        </button>
      </nav>

      <div v-if="canManageFiles" class="category-actions" aria-label="Kategori yönetimi">
        <input v-model="newCategoryName" type="text" placeholder="Yeni kategori" @keydown.enter.prevent="createCategory" />
        <button type="button" class="category-action" :disabled="isSavingCategory || !newCategoryName.trim()" @click="createCategory">
          <i :class="isSavingCategory ? 'pi pi-spin pi-spinner' : 'pi pi-plus'" aria-hidden="true"></i>
          Ekle
        </button>
        <button type="button" class="category-action" :disabled="isSavingCategory" @click="startRenameCategory">
          <i class="pi pi-pencil" aria-hidden="true"></i>
          Adını Değiştir
        </button>
        <button type="button" class="category-action category-action--danger" :disabled="isSavingCategory || activePanelFiles.length > 0" @click="deleteCategory">
          <i class="pi pi-trash" aria-hidden="true"></i>
          Sil
        </button>
      </div>

      <div v-if="editingCategory" class="category-edit">
        <input v-model="editCategoryName" type="text" @keydown.enter.prevent="saveCategoryName" />
        <button type="button" class="category-action" :disabled="isSavingCategory || !editCategoryName.trim()" @click="saveCategoryName">Kaydet</button>
        <button type="button" class="category-action" :disabled="isSavingCategory" @click="cancelRenameCategory">Vazgeç</button>
      </div>
    </div>

    <main>
      <section class="panel-card card">
        <div class="panel-card__header">
          <h3><i :class="activeTab.icon" aria-hidden="true"></i> {{ activeTab.title }}</h3>
        </div>

        <div class="panel-card__body">
          <div v-if="isLoadingFiles" class="empty-state">
            <i class="pi pi-spin pi-spinner" aria-hidden="true"></i>
            <p class="empty-title">Dosyalar yükleniyor</p>
          </div>

          <div v-else-if="activePanelFiles.length === 0" class="empty-state">
            <i class="pi pi-file" aria-hidden="true"></i>
            <p class="empty-title">Bu kategoride henuz dosya yok</p>
            <p class="empty-sub">{{ emptyStateSubtitle }}</p>
          </div>

          <div v-else class="file-list">
            <div v-for="item in activePanelFiles" :key="item.id" class="file-row">
              <i class="pi pi-file file-row__icon" aria-hidden="true"></i>
              <div class="file-row__copy">
                <strong>{{ item.fileName }}</strong>
                <span>{{ getCategoryLabel(item.category) }} - {{ formatFileSize(item.sizeBytes) }}</span>
              </div>
              <button
                type="button"
                class="file-row__preview"
                title="Goruntule"
                :disabled="previewLoadingFileId === item.id"
                @click="viewFile(item)"
              >
                <i :class="previewLoadingFileId === item.id ? 'pi pi-spin pi-spinner' : 'pi pi-eye'" aria-hidden="true"></i>
              </button>
              <button class="file-row__download" type="button" title="Indir" @click="downloadFile(item)">
                <i class="pi pi-download" aria-hidden="true"></i>
              </button>
              <button
                v-if="canDeleteFiles"
                class="file-row__delete"
                type="button"
                title="Sil"
                :disabled="deletingFileIds.has(item.id)"
                @click="confirmDeleteFile($event, item)"
              >
                <i :class="deletingFileIds.has(item.id) ? 'pi pi-spin pi-spinner' : 'pi pi-trash'" aria-hidden="true"></i>
              </button>
            </div>
          </div>
        </div>
      </section>
    </main>

    <Teleport to="body">
      <div v-if="previewFile" class="preview-backdrop" role="presentation" @click.self="closePreview">
        <section
          class="preview-dialog"
          role="dialog"
          aria-modal="true"
          :aria-label="`${previewFile.fileName} dosya onizleme`"
        >
          <header class="preview-dialog__header">
            <div class="preview-dialog__title">
              <strong>{{ previewFile.fileName }}</strong>
              <span>{{ getCategoryLabel(previewFile.category) }} - {{ formatFileSize(previewFile.sizeBytes) }}</span>
            </div>
            <div class="preview-dialog__actions">
              <button type="button" class="preview-action" title="Indir" @click="downloadFile(previewFile)">
                <i class="pi pi-download" aria-hidden="true"></i>
              </button>
              <button type="button" class="preview-action" title="Kapat" @click="closePreview">
                <i class="pi pi-times" aria-hidden="true"></i>
              </button>
            </div>
          </header>

          <iframe
            v-if="previewUrl"
            class="preview-dialog__frame"
            :src="previewUrl"
            :title="previewFile.fileName"
          ></iframe>
        </section>
      </div>
    </Teleport>
  </div>
</template>

<script setup lang="ts">
import axios from 'axios';
import { computed, onBeforeUnmount, ref, watch } from 'vue';
import { useConfirm } from 'primevue/useconfirm';
import { useRoute } from 'vue-router';
import { apiClient } from '@/api/client';
import { apiEndpoints } from '@/api/endpoints';
import QrCodeModal from '@/components/qr/QrCodeModal.vue';
import { defaultPanelFileCategories, getPanelFileCategoryDefinition, type PanelFileCategoryDefinition } from '@/constants/file-categories';
import { useAuthStore, useHierarchyStore } from '@/stores';
import { useFileStore } from '@/stores/file-store';
import { useModalStore } from '@/stores/modal-store';
import { appRoles } from '@/utils/authorization';
import type { PanelFileResponse } from '@/types/files';
import type { GeneratedQrCodeResponse, QrCodeResponse } from '@/types/qr';

const route = useRoute();
const confirm = useConfirm();
const panelId = computed(() => String(route.params.panelId ?? ''));
const authStore = useAuthStore();
const hierarchyStore = useHierarchyStore();
const modalStore = useModalStore();
const isCreatingQr = ref(false);
const qrError = ref('');
const fileInput = ref<HTMLInputElement | null>(null);
const panelFiles = ref<PanelFileResponse[]>([]);
const fileTabs = ref<PanelFileCategoryDefinition[]>([...defaultPanelFileCategories]);
const activeFileTab = ref('MaintenanceReport');
const isLoadingFiles = ref(false);
const isUploading = ref(false);
const fileError = ref('');
const isSavingCategory = ref(false);
const newCategoryName = ref('');
const editingCategory = ref(false);
const editCategoryName = ref('');
const canManageQr = computed(() => authStore.hasPermission('qr.manage'));
const canManageFiles = computed(() => authStore.hasPermission('files.manage'));
const canDeleteFiles = computed(() => (
  authStore.hasPermission('files.delete')
  || authStore.hasAnyRole([appRoles.superAdmin])
));
const hasPanelActions = computed(() => canManageQr.value || canManageFiles.value);
const activeTab = computed(() => (
  fileTabs.value.find((category) => category.key === activeFileTab.value)
  ?? getPanelFileCategoryDefinition(activeFileTab.value)
));
const activePanelFiles = computed(() => panelFiles.value.filter((file) => file.category === activeFileTab.value));
const emptyStateSubtitle = computed(() => (
  canManageFiles.value
    ? `${activeTab.value.label} sekmesindeyken yukleme butonunu kullanabilirsiniz`
    : 'Bu kategori icin dosya bulunmuyor'
));

const emptyPanel = { id: '', facilityId: '', code: '', name: '', description: '' };
const emptyFacility = { id: '', companyId: '', name: '', city: '', district: '', address: '', panels: [] };
const panel = computed(() => {
  if (panelId.value) return hierarchyStore.panels.find((item) => item.id === panelId.value) ?? emptyPanel;
  return emptyPanel;
});
const facility = computed(() => hierarchyStore.facilities.find((item) => item.id === panel.value.facilityId) ?? emptyFacility);
const fileStore = useFileStore();
const deletingFileIds = ref<Set<string>>(new Set());
const previewFile = ref<PanelFileResponse | null>(null);
const previewUrl = ref('');
const previewLoadingFileId = ref<string | null>(null);

watch(panelId, async (nextPanelId) => {
  panelFiles.value = [];
  fileError.value = '';
  await hierarchyStore.load();
  await loadFileCategories();
  if (nextPanelId) {
    await loadFiles(nextPanelId);
  }
}, { immediate: true });

watch(previewFile, (file) => {
  if (file) {
    document.addEventListener('keydown', handlePreviewKeydown);
  } else {
    document.removeEventListener('keydown', handlePreviewKeydown);
  }
});

onBeforeUnmount(() => {
  document.removeEventListener('keydown', handlePreviewKeydown);
  revokePreviewUrl();
});

async function loadFileCategories() {
  if (!panelId.value) {
    fileTabs.value = [...defaultPanelFileCategories];
    return;
  }

  try {
    const { data } = await apiClient.get<FileCategoryResponse[]>(`${apiEndpoints.fileCategories}/panels/${panelId.value}`);
    fileTabs.value = uniqueCategories(data.map(mapCategoryResponse));

    if (!fileTabs.value.some((category) => category.key === activeFileTab.value)) {
      activeFileTab.value = fileTabs.value[0]?.key ?? 'MaintenanceReport';
    }
  } catch {
    fileTabs.value = uniqueCategories([...defaultPanelFileCategories]);
  }
}

async function onCreateQr() {
  isCreatingQr.value = true;
  qrError.value = '';

  try {
    const { data } = await apiClient.get<QrCodeResponse>(`${apiEndpoints.qr}/panels/${panelId.value}`);

    const qr: GeneratedQrCodeResponse = {
      code: data.panelCode,
      publicUrl: data.publicUrl,
      svg: data.svg,
    };

    panel.value.code = data.panelCode;
    modalStore.open(QrCodeModal, {
      panelName: data.panelName || panel.value.name,
      qr,
    });
  } catch (error) {
    qrError.value = getApiError(error, 'QR kodu oluşturulamadı. API bağlantısini ve kullanıcı yetkisini kontrol edin.');
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
    formData.append('category', activeFileTab.value);

    const { data } = await apiClient.post<PanelFileResponse>(
      `${apiEndpoints.files}/panels/${panel.value.id}`,
      formData,
      { headers: { 'Content-Type': 'multipart/form-data' } },
    );

    panelFiles.value.unshift(data);
    fileStore.setPanelFileCount(panel.value.id, panelFiles.value.length);
  } catch (error) {
    fileError.value = getApiError(error, 'Dosya yuklenemedi. Supabase Storage ayarlarini ve dosya turunu kontrol edin.');
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
      fileError.value = getApiError(error, 'Dosyalar yuklenemedi.');
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
    const { data } = await apiClient.get<Blob>(`${apiEndpoints.files}/${item.id}/view`, {
      responseType: 'blob',
    });
    const url = URL.createObjectURL(data);
    const link = document.createElement('a');
    link.href = url;
    link.download = item.fileName;
    link.rel = 'noopener noreferrer';
    document.body.appendChild(link);
    link.click();
    link.remove();
    URL.revokeObjectURL(url);
  } catch (error) {
    fileError.value = getApiError(error, 'Dosya indirilemedi.');
  }
}

async function viewFile(item: PanelFileResponse) {
  fileError.value = '';
  previewLoadingFileId.value = item.id;

  try {
    const { data } = await apiClient.get<Blob>(`${apiEndpoints.files}/${item.id}/view`, {
      responseType: 'blob',
    });
    revokePreviewUrl();
    previewFile.value = item;
    previewUrl.value = URL.createObjectURL(data);
  } catch (error) {
    fileError.value = getApiError(error, 'Dosya onizleme baglantisi olusturulamadi.');
  } finally {
    previewLoadingFileId.value = null;
  }
}

function closePreview() {
  previewFile.value = null;
  revokePreviewUrl();
}

function revokePreviewUrl() {
  if (previewUrl.value) {
    URL.revokeObjectURL(previewUrl.value);
  }

  previewUrl.value = '';
}

function handlePreviewKeydown(event: KeyboardEvent) {
  if (event.key === 'Escape') {
    closePreview();
  }
}

function confirmDeleteFile(event: MouseEvent, item: PanelFileResponse) {
  confirm.require({
    target: event.currentTarget as HTMLElement,
    message: `${item.fileName} dosyasini silmek istiyor musunuz?`,
    icon: 'pi pi-exclamation-triangle',
    acceptLabel: 'Dosyayı Sil',
    rejectLabel: 'Vazgeç',
    acceptClass: 'p-button-danger',
    accept: () => {
      void deleteFile(item);
    },
  });
}

async function deleteFile(item: PanelFileResponse) {
  fileError.value = '';
  deletingFileIds.value = new Set(deletingFileIds.value).add(item.id);

  try {
    await apiClient.delete(`${apiEndpoints.files}/${item.id}`);
    panelFiles.value = panelFiles.value.filter((file) => file.id !== item.id);
    fileStore.setPanelFileCount(panel.value.id, panelFiles.value.length);
  } catch (error) {
    fileError.value = getApiError(error, 'Dosya silinemedi. Yetkinizi ve API durumunu kontrol edin.');
  } finally {
    const nextDeletingFileIds = new Set(deletingFileIds.value);
    nextDeletingFileIds.delete(item.id);
    deletingFileIds.value = nextDeletingFileIds;
  }
}

function formatFileSize(sizeBytes: number) {
  if (sizeBytes < 1024 * 1024) {
    return `${Math.max(1, Math.round(sizeBytes / 1024))} KB`;
  }

  return `${(sizeBytes / (1024 * 1024)).toFixed(1)} MB`;
}

function getCategoryLabel(category: string) {
  return fileTabs.value.find((item) => item.key === category)?.label
    ?? getPanelFileCategoryDefinition(category).label;
}

async function createCategory() {
  const name = newCategoryName.value.trim();

  if (!name || !panelId.value) {
    return;
  }

  isSavingCategory.value = true;
  fileError.value = '';

  try {
    const { data } = await apiClient.post<FileCategoryResponse>(`${apiEndpoints.fileCategories}/panels/${panelId.value}`, {
      name,
      description: `${name} dosyalari`,
      icon: 'pi pi-folder',
    });
    newCategoryName.value = '';
    await loadFileCategories();
    activeFileTab.value = data.key;
  } catch (error) {
    fileError.value = getApiError(error, 'Kategori olusturulamadi. Ayni isimde kategori olabilir.');
  } finally {
    isSavingCategory.value = false;
  }
}

function startRenameCategory() {
  editingCategory.value = true;
  editCategoryName.value = activeTab.value.label;
}

function cancelRenameCategory() {
  editingCategory.value = false;
  editCategoryName.value = '';
}

async function saveCategoryName() {
  const category = activeTab.value;
  const name = editCategoryName.value.trim();

  if (!category.id || !name) {
    return;
  }

  isSavingCategory.value = true;
  fileError.value = '';

  try {
    await apiClient.put(`${apiEndpoints.fileCategories}/${category.id}`, {
      name,
      description: category.description,
      icon: category.icon,
      sortOrder: category.sortOrder,
    });
    cancelRenameCategory();
    await loadFileCategories();
  } catch (error) {
    fileError.value = getApiError(error, 'Kategori adi guncellenemedi.');
  } finally {
    isSavingCategory.value = false;
  }
}

async function deleteCategory() {
  const category = activeTab.value;

  if (!category.id || activePanelFiles.value.length > 0) {
    return;
  }

  isSavingCategory.value = true;
  fileError.value = '';

  try {
    await apiClient.delete(`${apiEndpoints.fileCategories}/${category.id}`);
    await loadFileCategories();
  } catch (error) {
    fileError.value = getApiError(error, 'Kategori silinemedi. Icerisinde dosya varsa once dosyalari silin.');
  } finally {
    isSavingCategory.value = false;
  }
}

function mapCategoryResponse(category: FileCategoryResponse): PanelFileCategoryDefinition {
  return {
    id: category.id,
    key: category.key,
    label: category.name,
    title: category.name,
    description: category.description || `${category.name} dosyalari`,
    icon: category.icon || 'pi pi-file',
    sortOrder: category.sortOrder,
    isSystem: category.isSystem,
  };
}

function uniqueCategories(categories: PanelFileCategoryDefinition[]) {
  const categoriesByKey = new Map<string, PanelFileCategoryDefinition>();

  for (const category of categories) {
    if (!categoriesByKey.has(category.key)) {
      categoriesByKey.set(category.key, category);
    }
  }

  return [...categoriesByKey.values()].sort((first, second) => (
    (first.sortOrder ?? 0) - (second.sortOrder ?? 0)
    || first.label.localeCompare(second.label)
  ));
}

function getApiError(error: unknown, fallback: string) {
  return axios.isAxiosError(error) && typeof error.response?.data?.detail === 'string'
    ? error.response.data.detail
    : fallback;
}

type FileCategoryResponse = {
  id: string;
  panelId: string;
  key: string;
  name: string;
  description: string;
  icon: string;
  sortOrder: number;
  isSystem: boolean;
};
</script>

<style scoped>
.panel-header { display:flex; align-items:center; justify-content:space-between; gap:1rem; margin-bottom:0.9rem }
.panel-title { margin:0; font-size:1.25rem; line-height:1.15; font-weight:800 }
.panel-sub { color:var(--app-text-muted); margin-top:0.15rem; font-size:0.8125rem; line-height:1.35 }
.panel-actions { display:flex; gap:0.5rem }
.panel-error { margin:0 0 0.9rem; padding:0.6rem 0.75rem; border:1px solid #fecaca; border-radius:8px; background:#fef2f2; color:#b91c1c; font-size:0.8125rem }

.category-strip { display:grid; gap:0.65rem; margin-bottom:0.9rem }
.panel-tabs { display:flex; gap:0.4rem; flex-wrap:wrap }
.tab { background:var(--app-surface); border:1px solid var(--app-border); padding:0.45rem 0.75rem; border-radius:8px; color:var(--app-text-muted); font-size:0.8125rem; line-height:1.2; cursor:pointer }
.tab.active { background:var(--app-surface-alt); color:var(--app-text); font-weight:600; border-color:rgba(37,99,235,0.35) }
.category-actions,
.category-edit { display:flex; align-items:center; gap:0.5rem; padding:0.65rem; border:1px solid var(--app-border); border-radius:10px; background:var(--app-surface) }
.category-actions input,
.category-edit input { min-width:12rem; height:2.25rem; border:1px solid var(--app-border); border-radius:8px; background:var(--app-bg); padding:0 0.75rem; color:var(--app-text); font-size:0.8125rem }
.category-action { display:inline-flex; align-items:center; justify-content:center; gap:0.4rem; min-height:2.25rem; padding:0.45rem 0.7rem; border:1px solid var(--app-border); border-radius:8px; background:var(--app-bg); color:var(--app-text); font-size:0.8125rem; line-height:1.2; white-space:nowrap; cursor:pointer }
.category-action:hover:not(:disabled) { border-color:var(--app-primary); color:var(--app-primary); background:var(--primary-50) }
.category-action--danger { color:#b91c1c }
.category-action--danger:hover:not(:disabled) { border-color:#fecaca; color:#b91c1c; background:#fef2f2 }
.category-action:disabled { opacity:0.55; cursor:not-allowed }

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
.file-row__preview,
.file-row__download,
.file-row__delete { width:2rem; height:2rem; display:grid; place-items:center; border:0; border-radius:6px; background:transparent; cursor:pointer }
.file-row__preview { color:var(--app-text-muted) }
.file-row__download { color:var(--app-primary) }
.file-row__delete { color:#b91c1c }
.file-row__preview:hover:not(:disabled),
.file-row__download:hover,
.file-row__delete:hover:not(:disabled) { background:var(--app-surface-alt) }
.file-row__preview:disabled,
.file-row__delete:disabled { opacity:0.6; cursor:wait }

.preview-backdrop { position:fixed; inset:0; z-index:1000; display:flex; align-items:center; justify-content:center; padding:1rem; background:rgb(15 23 42 / 0.52) }
.preview-dialog { width:min(100%, 64rem); height:min(86vh, 48rem); display:flex; flex-direction:column; overflow:hidden; border:1px solid var(--app-border); border-radius:10px; background:var(--app-bg); box-shadow:0 24px 70px rgb(15 23 42 / 0.24) }
.preview-dialog__header { display:flex; align-items:center; justify-content:space-between; gap:0.75rem; padding:0.75rem; border-bottom:1px solid var(--app-border) }
.preview-dialog__title { display:grid; gap:0.15rem; min-width:0 }
.preview-dialog__title strong { overflow:hidden; text-overflow:ellipsis; white-space:nowrap; font-size:0.875rem; line-height:1.25 }
.preview-dialog__title span { color:var(--app-text-muted); font-size:0.75rem; line-height:1.2 }
.preview-dialog__actions { display:flex; gap:0.35rem; flex:0 0 auto }
.preview-action { width:2rem; height:2rem; display:grid; place-items:center; border:1px solid var(--app-border); border-radius:8px; background:var(--app-bg); color:var(--app-text-muted); cursor:pointer }
.preview-action:hover { border-color:var(--app-primary); color:var(--app-primary); background:var(--primary-50) }
.preview-dialog__frame { flex:1; width:100%; border:0; background:#f8fafc }

@media (max-width: 760px) {
  .panel-header { flex-direction:column; align-items:flex-start }
  .panel-actions { width:100%; justify-content:flex-end }
  .category-actions,
  .category-edit { align-items:stretch; flex-direction:column }
  .category-actions input,
  .category-edit input,
  .category-action { width:100% }
}
</style>
