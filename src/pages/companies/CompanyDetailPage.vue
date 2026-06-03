<template>
  <div class="page page--company-detail">
    <header class="page__header">
      <h1 class="page__title">{{ company.projectName }}</h1>
      <p class="page__subtitle">{{ company.name }}</p>
    </header>

    <main>
      <section class="project-card card project-card--company">
        <div class="project-card__header">
          <h2 class="project-card__title"><i class="pi pi-folder" aria-hidden="true"></i> {{ company.projectName }}</h2>
        </div>

        <div class="project-card__stats">
          <div class="stat">
            <div class="stat__label">Mağaza Sayısı</div>
            <div class="stat__value">{{ shopCount }}</div>
          </div>
          <div class="stat">
            <div class="stat__label">Pano Sayısı</div>
            <div class="stat__value">{{ panelCount }}</div>
          </div>
          <div class="stat">
            <div class="stat__label">Dosya Sayısı</div>
            <div class="stat__value">0</div>
          </div>
          <div class="stat">
            <div class="stat__label">Durum</div>
            <div class="stat__value"><span class="pill">Aktif</span></div>
          </div>
        </div>

        <div class="project-card__body">
          <p>Bu demo projede {{ shopCount }} mağaza ve {{ panelCount }} pano bulunmaktadır. Pano detayına giderek dosya yükleyebilirsiniz.</p>
          <UiButton class="btn-go" @click="goToFirstFacility">
            <span class="btn-icon"><i class="pi pi-shopping-cart"></i></span>
            Mağazaya Git
          </UiButton>
        </div>
      </section>
    </main>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useHierarchyStore } from '@/stores';

const router = useRouter();
const route = useRoute();
const hierarchyStore = useHierarchyStore();
onMounted(() => hierarchyStore.load());

const company = computed(() => {
  const companyId = String(route.params.companyId ?? '');
  return hierarchyStore.companies.find((item) => item.id === companyId) ?? {
    id: '',
    name: '',
    projectName: '',
    taxNumber: '',
    address: '',
    contactEmail: '',
    facilities: [],
  };
});

const shopCount = computed(() => company.value.facilities.length);
const panelCount = computed(() => company.value.facilities.reduce((sum, facility) => sum + facility.panels.length, 0));

function goToFirstFacility() {
  const f = company.value.facilities[0];
  if (f) router.push({ name: 'facility-detail', params: { facilityId: f.id } });
  else router.push({ name: 'facilities' });
}
</script>

<style scoped>
.page__header { margin-bottom: 0.9rem }
.page__title { margin: 0; font-size: 1.25rem; line-height:1.15; font-weight:800 }
.page__subtitle { margin: 0.15rem 0 0; color: var(--app-text-muted); font-size:0.8125rem; line-height:1.35 }

.project-card.card { padding: 1rem 1.25rem; border-radius: 10px; border: 1px solid var(--app-border); background: var(--app-bg) }
.project-card__title { display:flex; align-items:center; gap:0.5rem; margin:0; font-size:1rem; line-height:1.25 }
.project-card__title .pi { font-size:0.95rem; color:var(--app-primary) }
.project-card__stats { display:flex; gap:0.85rem; margin-bottom:0.9rem }
.stat { background: var(--app-surface); padding:0.7rem 0.85rem; border-radius:8px; flex:1; border:0 }
.stat__label { font-size:0.6875rem; line-height:1.2; color:var(--app-text-muted) }
.stat__value { font-size:0.8125rem; line-height:1.2; font-weight:700; margin-top:0.1rem }
.pill { display:inline-block;background: #22c55e;color:#fff; padding:4px 8px; border-radius:999px; font-size:0.75rem }
.project-card__body { margin-top: 0.5rem; font-size:0.8125rem; line-height:1.4 }
.btn-go { margin-top:0.75rem }
</style>
