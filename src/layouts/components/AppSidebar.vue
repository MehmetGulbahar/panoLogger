<template>
  <aside class="app-sidebar">
    <div class="app-sidebar__main">
      <nav class="app-sidebar__nav" aria-label="Primary">
        <div class="app-sidebar__section-title">GENEL BAKIŞ</div>
        <RouterLink to="/" class="app-sidebar__link small">
          <span class="app-sidebar__icon-wrap"><i class="pi pi-home"></i></span>
          <span class="app-sidebar__label ">Genel Bakış</span>
        </RouterLink>

        <template v-if="isSuperAdmin">
          <div class="app-sidebar__section-title">YONETIM</div>
          <RouterLink :to="{ name: routeNames.admin }" class="app-sidebar__link small">
            <span class="app-sidebar__icon-wrap"><i class="pi pi-shield"></i></span>
            <span class="app-sidebar__label font-bold">Süper Admin</span>
          </RouterLink>
        </template>

        <div class="app-sidebar__section-title">HIYERARSI</div>

        <div v-if="hierarchyStore.isLoading" class="app-sidebar__empty">Hiyerarsi yükleniyor...</div>
        <div v-else-if="hierarchyStore.companies.length === 0" class="app-sidebar__empty">Henuz sistem eklenmedi.</div>

        <div v-for="company in hierarchyStore.companies" :key="company.id" class="app-sidebar__folder">
          <div class="accordion-row">
            <button
              class="accordion-toggle"
              type="button"
              :aria-expanded="isCompanyExpanded(company.id)"
              @click="toggleCompany(company.id)"
            >
              <span class="app-sidebar__icon-wrap"><i class="pi pi-folder"></i></span>
              <span class="app-sidebar__label">{{ company.projectName }}</span>
              <i :class="['pi accordion-chevron', isCompanyExpanded(company.id) ? 'pi-chevron-down' : 'pi-chevron-right']" aria-hidden="true"></i>
            </button>
            <RouterLink
              :to="{ name: routeNames.companyDetail, params: { companyId: company.id } }"
              class="accordion-detail-link"
              title="Sistem detayina git"
              aria-label="Sistem detayina git"
            >
              <i class="pi pi-arrow-up-right" aria-hidden="true"></i>
            </RouterLink>
          </div>

          <div v-if="isCompanyExpanded(company.id)" class="app-sidebar__company-content">
            <div v-for="facility in company.facilities" :key="facility.id" class="app-sidebar__children">
              <div class="accordion-row facility-row">
                <button
                  class="accordion-toggle facility-toggle"
                  type="button"
                  :aria-expanded="isFacilityExpanded(facility.id)"
                  @click="toggleFacility(facility.id)"
                >
                  <span class="app-sidebar__icon-wrap branch-icon"><i class="pi pi-building"></i></span>
                  <span class="app-sidebar__label">{{ facility.name }}</span>
                  <i :class="['pi accordion-chevron', isFacilityExpanded(facility.id) ? 'pi-chevron-down' : 'pi-chevron-right']" aria-hidden="true"></i>
                </button>
                <RouterLink
                  :to="{ name: routeNames.facilityDetail, params: { facilityId: facility.id } }"
                  class="accordion-detail-link"
                  title="Tesis detayina git"
                  aria-label="Tesis detayina git"
                >
                  <i class="pi pi-arrow-up-right" aria-hidden="true"></i>
                </RouterLink>
              </div>

              <div v-if="isFacilityExpanded(facility.id)" class="app-sidebar__panel-list">
                <RouterLink v-for="panel in facility.panels" :key="panel.id" :to="{ name: routeNames.panelDetail, params: { panelId: panel.id } }" class="app-sidebar__sub-item">
                  <span class="sub-icon"><i class="pi pi-bolt" aria-hidden="true"></i></span>
                  <span class="sub-label">{{ panel.name }}</span>
                </RouterLink>
              </div>
            </div>
          </div>
        </div>
      </nav>
    </div>

    <div class="app-sidebar__bottom">
      <RouterLink to="/" class="app-sidebar__exit">
        <span class="exit-icon"><i class="pi pi-arrow-left" aria-hidden="true"></i></span>
        Anasayfaya Don
      </RouterLink>
    </div>
  </aside>
</template>

<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue';
import { useRoute } from 'vue-router';
import { routeNames } from '@/constants/routes';
import { useAuthStore, useHierarchyStore } from '@/stores';
import { appRoles } from '@/utils/authorization';

const authStore = useAuthStore();
const hierarchyStore = useHierarchyStore();
const route = useRoute();
const isSuperAdmin = computed(() => authStore.hasAnyRole([appRoles.superAdmin]));
const expandedCompanyIds = ref<Set<string>>(new Set());
const expandedFacilityIds = ref<Set<string>>(new Set());

onMounted(async () => {
  await hierarchyStore.load();
  expandActiveHierarchy();
});

watch(
  () => [route.name, route.params.companyId, route.params.facilityId, route.params.panelId, hierarchyStore.companies.length],
  () => expandActiveHierarchy(),
);

function isCompanyExpanded(companyId: string): boolean {
  return expandedCompanyIds.value.has(companyId);
}

function isFacilityExpanded(facilityId: string): boolean {
  return expandedFacilityIds.value.has(facilityId);
}

function toggleCompany(companyId: string): void {
  const next = new Set(expandedCompanyIds.value);
  if (next.has(companyId)) {
    next.delete(companyId);
  } else {
    next.add(companyId);
  }
  expandedCompanyIds.value = next;
}

function toggleFacility(facilityId: string): void {
  const next = new Set(expandedFacilityIds.value);
  if (next.has(facilityId)) {
    next.delete(facilityId);
  } else {
    next.add(facilityId);
  }
  expandedFacilityIds.value = next;
}

function expandActiveHierarchy(): void {
  const active = findActiveHierarchy();
  if (!active.companyId && !active.facilityId) {
    return;
  }

  expandedCompanyIds.value = new Set([...expandedCompanyIds.value, active.companyId].filter(Boolean) as string[]);
  expandedFacilityIds.value = new Set([...expandedFacilityIds.value, active.facilityId].filter(Boolean) as string[]);
}

function findActiveHierarchy(): { companyId?: string; facilityId?: string } {
  const routeCompanyId = typeof route.params.companyId === 'string' ? route.params.companyId : undefined;
  const routeFacilityId = typeof route.params.facilityId === 'string' ? route.params.facilityId : undefined;
  const routePanelId = typeof route.params.panelId === 'string' ? route.params.panelId : undefined;

  if (routeCompanyId) {
    return { companyId: routeCompanyId };
  }

  if (routeFacilityId) {
    const facility = hierarchyStore.facilities.find((item) => item.id === routeFacilityId);
    return { companyId: facility?.companyId, facilityId: routeFacilityId };
  }

  if (routePanelId) {
    const panel = hierarchyStore.panels.find((item) => item.id === routePanelId);
    const facility = panel ? hierarchyStore.facilities.find((item) => item.id === panel.facilityId) : undefined;
    return { companyId: facility?.companyId, facilityId: facility?.id };
  }

  return {};
}
</script>

<style scoped>
.app-sidebar {
  width: 260px;
  min-width: 0;
  border-right: 1px solid var(--app-border);
  padding: 1rem 0.75rem;
  background: var(--app-surface);
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  height: calc(100vh - 56px);
}

.app-sidebar__main {
  min-height: 0;
}

.app-sidebar__main::-webkit-scrollbar { width: 6px }
.app-sidebar__main::-webkit-scrollbar-thumb { background: transparent; border-radius: 999px }
.app-sidebar__main:hover::-webkit-scrollbar-thumb { background: rgba(100,116,139,0.28) }
.app-sidebar__nav { display: flex; flex-direction: column; gap: 0.1rem }
.app-sidebar__section-title { font-size:0.625rem; line-height:1.2; color:var(--app-text-muted); margin:1rem 0 0.55rem 0.45rem; letter-spacing:0.04em; font-weight:600 }
.app-sidebar__link { display: flex; align-items: center; gap: 0.65rem; padding: 0.5rem 0.6rem; border-radius: 8px; color: var(--app-text); transition: background-color .15s ease, color .15s ease }
.app-sidebar__link.small { padding:0.45rem 0.6rem; border-radius:8px }
.app-sidebar__link,
.app-sidebar__folder-title,
.app-sidebar__sub-item { text-decoration: none }
.app-sidebar__link:hover,
.app-sidebar__link:focus,
.app-sidebar__folder-title:hover,
.app-sidebar__folder-title:focus {
  background: var(--app-surface-alt);
  color: var(--app-text);
  font-weight: 600;
  text-decoration: none;
}
.app-sidebar__link.router-link-active { background: var(--app-surface-alt); color: var(--app-text); font-weight: 600 }
.app-sidebar__icon-wrap { width: 1rem; display:inline-flex; align-items:center; justify-content:center; color:var(--app-text-muted); flex:0 0 1rem }
.app-sidebar__icon-wrap .pi { font-size:0.85rem }
.app-sidebar__label { font-size:0.8125rem; line-height:1.25 }
.app-sidebar__folder { padding: 0.15rem 0 }
.accordion-row { display:flex; align-items:center; gap:0.25rem }
.accordion-toggle { min-width:0; flex:1; display:flex; align-items:center; gap:0.55rem; padding:0.4rem 0.55rem; border:0; border-radius:8px; background:transparent; color:var(--app-text-muted); text-align:left; cursor:pointer }
.accordion-toggle:hover,
.accordion-toggle:focus-visible { background:var(--app-surface-alt); color:var(--app-text); font-weight:600; outline:0 }
.accordion-toggle .app-sidebar__label { overflow:hidden; text-overflow:ellipsis; white-space:nowrap; flex:1 }
.accordion-chevron { color:var(--app-text-muted); font-size:0.65rem; flex:0 0 auto }
.accordion-detail-link { width:1.65rem; height:1.65rem; display:grid; place-items:center; border-radius:7px; color:var(--app-text-muted); text-decoration:none; flex:0 0 auto }
.accordion-detail-link:hover,
.accordion-detail-link:focus-visible { background:var(--app-surface-alt); color:var(--app-primary); outline:0 }
.accordion-detail-link .pi { font-size:0.72rem }
.facility-row { margin-left:-0.15rem }
.facility-toggle { min-height:2rem; padding:0.4rem 0.5rem; color:var(--app-text) }
.app-sidebar__company-content { display:flex; flex-direction:column; gap:0.15rem; margin-top:0.2rem }
.app-sidebar__children { display:flex; flex-direction:column; gap:0.15rem; margin-left: 0.9rem; padding-left:0.8rem; border-left:1px solid var(--app-border) }
.app-sidebar__panel-list { display:flex; flex-direction:column; gap:0.15rem; margin-top:0.1rem }
.app-sidebar__link.active-branch { display:flex; align-items:center; gap:0.55rem; min-height:2.2rem; padding:0.45rem 0.6rem; border-radius:9px; background: transparent; color:var(--app-text); font-weight:400 }
.app-sidebar__link.active-branch.router-link-active { background:#dbeafe; color:var(--app-primary); font-weight:700 }
.app-sidebar__link.active-branch.router-link-active .branch-icon { color:var(--app-primary) }
.app-sidebar__link.active-branch .branch-icon { color: var(--app-primary); background: transparent; padding:0; border-radius:0 }
.app-sidebar__link.active-branch:hover,
.app-sidebar__link.active-branch:focus { font-weight: 700 }
.app-sidebar__sub-item { display:flex; align-items:center; gap:0.45rem; padding:0.35rem 0.55rem; margin-left:0.9rem; color:var(--app-text-muted); font-size:0.75rem; line-height:1.25 }
.app-sidebar__sub-item:hover,
.app-sidebar__sub-item:focus {
  background: var(--app-surface-alt);
  color: var(--app-text);
  font-weight: 700;
  text-decoration: none;
  border-radius: 8px;
}

.pi-shield{
color:black;
font-weight:bold;
}
.app-sidebar__sub-item .sub-icon { font-size:0.75rem; color:var(--app-text-muted) }
.app-sidebar__sub-item .sub-icon .pi { font-size:0.7rem }
.app-sidebar__empty { padding:0.45rem 0.6rem; color:var(--app-text-muted); font-size:0.75rem; line-height:1.35 }
.app-sidebar__bottom { flex: 0 0 auto; padding: 0.85rem 0.5rem }
.app-sidebar__exit { width:100%; padding:10px 12px; border-radius:12px; background:var(--app-surface-alt); border:1px solid var(--app-border); display:flex; gap:8px; align-items:center; justify-content:center; color:var(--app-text); text-decoration:none; font-size:0.8125rem }
.app-sidebar__exit .exit-icon { display:inline-flex; background:transparent; padding:4px; border-radius:6px }

@media (max-width: 960px) {
  .app-sidebar { width: 100%; border-right: 0; border-bottom: 1px solid var(--app-border); padding: 0.75rem; height: auto }
  .app-sidebar__main { padding-right: 0 }
}
</style>
