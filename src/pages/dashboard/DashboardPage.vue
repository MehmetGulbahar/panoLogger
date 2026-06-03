<template>
  <div class="dashboard-page">
    <header class="dashboard-header">
      <h1>Hoş Geldiniz!</h1>
      <p class="muted">PanoDocs ortamını keşfedin. Soldaki menüden proje, mağaza ve pano detaylarını inceleyebilirsiniz.</p>
    </header>

    <section class="stats-row">
      <RouterLink :to="{ name: routeNames.companies }" class="stat-card">
        <div class="stat-icon"><i class="pi pi-folder"></i></div>
        <div class="stat-body">
          <div class="stat-value">{{ projectCount }}</div>
          <div class="stat-label">Projeler</div>
        </div>
      </RouterLink>

      <RouterLink :to="{ name: routeNames.facilities }" class="stat-card">
        <div class="stat-icon"><i class="pi pi-building"></i></div>
        <div class="stat-body">
          <div class="stat-value">{{ facilityCount }}</div>
          <div class="stat-label">Mağazalar</div>
        </div>
      </RouterLink>

      <RouterLink :to="{ name: routeNames.panels }" class="stat-card">
        <div class="stat-icon"><i class="pi pi-th-large"></i></div>
        <div class="stat-body">
          <div class="stat-value">{{ panelCount }}</div>
          <div class="stat-label">Panolar</div>
        </div>
      </RouterLink>
    </section>

    <section class="uploads-card card">
      <h3>Yüklenen Dosyalar ({{ fileSummary.panelsWithFiles }}/{{ fileSummary.totalPanels }})</h3>
      <p class="muted">{{ uploadSummaryText }}</p>
    </section>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, reactive } from 'vue';
import { apiClient } from '@/api/client';
import { apiEndpoints } from '@/api/endpoints';
import { routeNames } from '@/constants/routes';
import { useHierarchyStore } from '@/stores';

interface FileSummary {
  totalFiles: number;
  panelsWithFiles: number;
  totalPanels: number;
}

const hierarchyStore = useHierarchyStore();
const fileSummary = reactive<FileSummary>({ totalFiles: 0, panelsWithFiles: 0, totalPanels: 0 });
const projectCount = computed(() => hierarchyStore.companies.length);
const facilityCount = computed(() => hierarchyStore.facilities.length);
const panelCount = computed(() => hierarchyStore.panels.length);
const uploadSummaryText = computed(() => {
  if (fileSummary.totalFiles === 0) {
    return 'Henüz dosya yüklenmedi. Pano detayına gidip dosya yükleyebilirsiniz.';
  }

  return `${fileSummary.totalFiles} dosya, ${fileSummary.panelsWithFiles} panoya yüklenmiştir.`;
});

onMounted(async () => {
  await hierarchyStore.load();
  const { data } = await apiClient.get<FileSummary>(`${apiEndpoints.files}/summary`);
  Object.assign(fileSummary, data);
});
</script>

<style scoped>
.dashboard-header { margin-bottom: 0.9rem }
.dashboard-header h1 { margin:0; font-size:1.25rem; line-height:1.15; font-weight:800 }
.muted { color: var(--app-text-muted); margin-top:0.25rem; font-size:0.8125rem; line-height:1.35 }

.stats-row { display:flex; gap:0.85rem; margin-bottom:0.9rem }
.stat-card { background: var(--app-bg); border:1px solid var(--app-border); border-radius:10px; padding:0.8rem 0.95rem; display:flex; gap:0.75rem; align-items:center; flex:1; text-decoration:none; color:var(--app-text) }
.stat-icon { width:2.1rem; height:2.1rem; display:inline-flex; align-items:center; justify-content:center; border-radius:8px; background:var(--app-surface); color:var(--app-primary) }
.stat-icon .pi { font-size:0.95rem }
.stat-value { font-size:1.05rem; line-height:1.15; font-weight:700 }
.stat-label { color:var(--app-text-muted); font-size:0.75rem; line-height:1.2; margin-top:0.1rem }

.uploads-card { background: var(--app-bg); border:1px solid var(--app-border); border-radius:10px; padding:0.9rem 1rem }
.uploads-card h3 { margin:0 0 0.4rem 0; font-size:0.95rem; line-height:1.25 }

@media (max-width: 760px) {
  .stats-row { flex-direction:column }
}
</style>
