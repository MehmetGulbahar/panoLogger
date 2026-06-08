<template>
  <div class="page page--facility-detail">
    <header class="page__header">
        <h1 class="page__title">{{ facility.name }}</h1>
        <p class="page__subtitle">{{ facility.city }}{{ facility.address ? ' • ' + facility.address.split(',')[0] : '' }}</p>
    </header>

    <main>
      <section class="project-card card project-card--facility">
        <div class="project-card__header">
          <h2 class="project-card__title"><i class="pi pi-building" aria-hidden="true"></i> {{ facility.name }} - Merkez Şube</h2>
        </div>

        <div class="project-card__grid">
          <div class="grid-item">
            <div class="grid-label">Şehir</div>
            <div class="grid-value">{{ facility.city }}</div>
          </div>
          <div class="grid-item">
            <div class="grid-label">İlçe</div>
            <div class="grid-value">{{ facility.district || '-' }}</div>
          </div>
          <div class="grid-item">
            <div class="grid-label">Mağaza Kodu</div>
            <div class="grid-value">KDK-001</div>
          </div>
          <div class="grid-item">
            <div class="grid-label">Pano Sayısı</div>
            <div class="grid-value">{{ panelCount }}</div>
          </div>
        </div>

        <div class="project-card__panels">
          <h3>Panolar</h3>
          <div class="panels-list">
            <RouterLink v-for="p in facilityPanels" :key="p.id" :to="{ name: routeNames.panelDetail, params: { panelId: p.id } }" class="panel-row">
              <div class="panel-row__left">
                <span class="panel-icon"><i class="pi pi-th-large"></i></span>
                <div>
                  <div class="panel-name">{{ p.name }}</div>
                  <div class="panel-meta">{{ p.description }}</div>
                </div>
              </div>
              <div class="panel-row__right">
                <span class="badge">{{ getPanelFileCount(p.id) }} dosya</span>
                <span class="panel-view-icon"><i class="pi pi-eye" aria-hidden="true"></i></span>
              </div>
            </RouterLink>
          </div>
        </div>
      </section>
    </main>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted } from 'vue';
import { useRoute } from 'vue-router';
import { routeNames } from '@/constants/routes';
import { useHierarchyStore } from '@/stores';
import { useFileStore } from '@/stores/file-store';

const route = useRoute();
const hierarchyStore = useHierarchyStore();
onMounted(() => hierarchyStore.load());

const facilityId = computed(() => String(route.params.facilityId ?? ''));

const facility = computed(() => {
  if (facilityId.value) {
    return hierarchyStore.facilities.find((f) => f.id === facilityId.value) ?? emptyFacility;
  }
  return emptyFacility;
});

const emptyFacility = { id: '', companyId: '', name: '', city: '', district: '', address: '', panels: [] };
const panelCount = computed(() => facility.value.panels.length);
const facilityPanels = computed(() => facility.value.panels);

const fileStore = useFileStore();

function getPanelFileCount(panelId: string): number {
  return fileStore.getPanelFileCount(panelId);
}
</script>

<style scoped>
.page__header { margin-bottom: 1.25rem }
.page__title { margin: 0; font-size: 1.35rem; line-height: 1.15; font-weight: 800 }
.page__subtitle { margin: 0.15rem 0 0; color: var(--app-text-muted); font-size: 0.8125rem; line-height: 1.35 }

.project-card.card { padding: 1rem 1.25rem; border-radius: 10px; border: 1px solid var(--app-border); background: var(--app-bg); width: 100% }
.project-card__header { margin-bottom: 1rem }
.project-card__title { display:flex; align-items:center; gap:0.5rem; margin:0; font-size:1rem; line-height:1.25; font-weight:700 }
.project-card__title .pi { color: var(--app-primary); font-size:1rem }

.project-card__stats { display:flex; gap:1rem; margin-bottom:1rem }
.stat { background: var(--app-surface); padding:1rem 1.1rem; border-radius:12px; flex:1; border:1px solid var(--app-border); min-width:0 }
.stat__label { font-size:0.78rem; color:var(--app-text-muted) }
.stat__value { font-size:1.1rem; font-weight:800; margin-top:0.25rem }

.pill { display:inline-block; background:var(--app-primary); color:#fff; padding:6px 10px; border-radius:999px; font-size:0.78rem }

.project-card__body { margin-top: 0.75rem }
.btn-go { margin-top:0.75rem }
.btn-icon { display:inline-flex; align-items:center; justify-content:center; margin-right:0.5rem }
.btn-go .pi { margin-right:0.35rem }

@media (max-width: 760px) {
  .project-card__stats { flex-direction: column }
}

.project-card--facility .project-card__grid { display:grid; grid-template-columns: 1fr 1fr; gap:0.85rem; margin-bottom:0.9rem }
.project-card--facility .grid-item { background: var(--app-surface); padding:0.7rem 0.85rem; border-radius:8px; border:0; min-height:3.25rem }
.project-card--facility .grid-label { font-size:0.6875rem; line-height:1.2; color:var(--app-text-muted) }
.project-card--facility .grid-value { font-size:0.8125rem; line-height:1.2; font-weight:700; margin-top:0.1rem }

.project-card__panels { margin-top:0.75rem }
.project-card__panels h3 { margin:0 0 0.7rem; font-size:0.8125rem; line-height:1.2; font-weight:700 }
.panels-list { margin-top:0 }
.panel-row { display:flex; align-items:center; justify-content:space-between; min-height:3.8rem; padding:0.65rem 0.85rem; border:1px solid var(--app-border); border-radius:8px; margin-bottom:0.5rem; background:var(--app-bg); text-decoration:none; color:var(--app-text) }
.panel-row { cursor: pointer }
.panel-row:hover { background: var(--app-surface-alt) }
.panel-row__left { display:flex; gap:0.65rem; align-items:center }
.panel-icon { display:inline-flex; width:1.55rem; height:1.55rem; align-items:center; justify-content:center; border-radius:6px; background:transparent; color:var(--app-primary) }
.panel-icon .pi { font-size:1rem }
.panel-name { font-size:0.8125rem; line-height:1.25; font-weight:700 }
.panel-meta { margin-top:0.15rem; font-size:0.75rem; line-height:1.25; color:var(--app-text-muted) }
.panel-row__right { display:flex; gap:0.5rem; align-items:center }
.badge { background:var(--app-bg); padding:0.2rem 0.55rem; border-radius:999px; font-size:0.75rem; line-height:1.2; font-weight:700; border:1px solid var(--app-border); color:var(--app-text) }
.panel-view-icon { display:inline-flex; width:1.25rem; height:1.25rem; align-items:center; justify-content:center; color:var(--app-text-muted) }
.panel-view-icon .pi { font-size:0.85rem }
</style>
 
