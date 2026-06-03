<template>
  <aside class="app-sidebar">
    <div>
      <nav class="app-sidebar__nav" aria-label="Primary">
        <div class="app-sidebar__section-title">GENEL BAKIŞ</div>
        <RouterLink to="/" class="app-sidebar__link small">
          <span class="app-sidebar__icon-wrap"><i class="pi pi-home"></i></span>
          <span class="app-sidebar__label">Genel Bakış</span>
        </RouterLink>

        <div class="app-sidebar__section-title">HİYERARŞİ</div>

        <div v-if="hierarchyStore.isLoading" class="app-sidebar__empty">Hiyerarşi yükleniyor...</div>
        <div v-else-if="hierarchyStore.companies.length === 0" class="app-sidebar__empty">Henüz sistem eklenmedi.</div>

        <div v-for="company in hierarchyStore.companies" :key="company.id" class="app-sidebar__folder">
          <RouterLink :to="{ name: routeNames.companyDetail, params: { companyId: company.id } }" class="app-sidebar__folder-title">
            <span class="app-sidebar__icon-wrap"><i class="pi pi-folder"></i></span>
            <span class="app-sidebar__label">{{ company.projectName }}</span>
          </RouterLink>

          <div v-for="facility in company.facilities" :key="facility.id" class="app-sidebar__children">
            <RouterLink :to="{ name: routeNames.facilityDetail, params: { facilityId: facility.id } }" class="app-sidebar__link active-branch">
              <span class="app-sidebar__icon-wrap branch-icon"><i class="pi pi-building"></i></span>
              <span class="app-sidebar__label">{{ facility.name }}</span>
            </RouterLink>

            <RouterLink v-for="panel in facility.panels" :key="panel.id" :to="{ name: routeNames.panelDetail, params: { panelId: panel.id } }" class="app-sidebar__sub-item">
              <span class="sub-icon"><i class="pi pi-bolt" aria-hidden="true"></i></span>
              <span class="sub-label">{{ panel.name }}</span>
            </RouterLink>
          </div>
        </div>
      </nav>
    </div>

    <div class="app-sidebar__bottom">
      <RouterLink to="/" class="app-sidebar__exit ">
        <span class="exit-icon">↩</span>
        Anasayfaya Dön
      </RouterLink>
    </div>
  </aside>
</template>

<script setup lang="ts">
import { onMounted } from 'vue';
import { routeNames } from '@/constants/routes';
import { useHierarchyStore } from '@/stores';

const hierarchyStore = useHierarchyStore();

onMounted(() => hierarchyStore.load());
</script>

<style scoped>
.app-sidebar {
  width: 260px;
  border-right: 1px solid var(--app-border);
  padding: 1rem 0.75rem;
  background: var(--app-surface);
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  height: calc(100vh - 56px);
}

.app-sidebar__brand { display:flex; align-items:center; gap:0.5rem; padding:0.25rem 0.5rem 0.8rem }
.app-sidebar__brand-title { font-weight:800; color:var(--app-text); font-size:1.05rem }
.app-sidebar__brand-title .pro { color:var(--app-primary); font-weight:700 }

.app-sidebar__nav {
  display: flex;
  flex-direction: column;
  gap: 0.1rem;
}

.app-sidebar__section-title { font-size:0.625rem; line-height:1.2; color:var(--app-text-muted); margin:1rem 0 0.55rem 0.45rem; letter-spacing:0.04em; font-weight:600 }

.app-sidebar__link { display: flex; align-items: center; gap: 0.65rem; padding: 0.5rem 0.6rem; border-radius: 8px; color: var(--app-text); transition: background-color .15s ease, color .15s ease }
.app-sidebar__link.small { padding:0.45rem 0.6rem; border-radius:8px }

/* remove underline for links on hover/focus */
.app-sidebar__link,
.app-sidebar__folder-title,
.app-sidebar__sub-item {
  text-decoration: none;
}
.app-sidebar__link:hover,
.app-sidebar__link:focus,
.app-sidebar__folder-title:hover,
.app-sidebar__folder-title:focus,
.app-sidebar__sub-item:hover,
.app-sidebar__sub-item:focus {
  text-decoration: none;
}

.app-sidebar__link.router-link-active { background: var(--app-surface-alt); color: var(--app-text); font-weight: 600 }
.app-sidebar__link:hover,
.app-sidebar__link:focus,
.app-sidebar__folder-title:hover,
.app-sidebar__folder-title:focus {
  background: var(--app-surface-alt);
  color: var(--app-text);
  font-weight: 600;
  text-decoration: none;
}

.app-sidebar__link:hover .branch-icon,
.app-sidebar__link:focus .branch-icon {
  color: var(--app-primary);
}

.app-sidebar__sub-item:hover,
.app-sidebar__sub-item:focus {
  background: var(--app-surface-alt);
  color: var(--app-text);
  font-weight: 700;
  text-decoration: none;
  border-radius: 8px;
}

.app-sidebar__icon-wrap { width: 1rem; display:inline-flex; align-items:center; justify-content:center; color:var(--app-text-muted); flex:0 0 1rem }
.app-sidebar__icon-wrap .pi { font-size:0.85rem }
.app-sidebar__label { font-size:0.8125rem; line-height:1.25 }

.app-sidebar__folder { padding: 0.15rem 0 }
.app-sidebar__folder-title { display:flex; align-items:center; gap:0.55rem; padding:0.4rem 0.6rem; color:var(--app-text-muted); font-size:0.8125rem }

.app-sidebar__children { display:flex; flex-direction:column; gap:0.15rem; margin-left: 0.9rem; margin-top:0.2rem; padding-left:0.8rem; border-left:1px solid var(--app-border) }

.app-sidebar__link.active-branch { display:flex; align-items:center; gap:0.55rem; min-height:2.2rem; padding:0.45rem 0.6rem; border-radius:9px; background: transparent; color:var(--app-text); font-weight:400 }
.app-sidebar__link.active-branch.router-link-active { background:#dbeafe; color:var(--app-primary); font-weight:700 }
.app-sidebar__link.active-branch.router-link-active .branch-icon { color:var(--app-primary) }
.app-sidebar__link.active-branch .branch-icon { color: var(--app-primary); background: transparent; padding:0; border-radius:0 }

.app-sidebar__link.active-branch:hover,
.app-sidebar__link.active-branch:focus {
  font-weight: 700;
}

.app-sidebar__sub-item { display:flex; align-items:center; gap:0.45rem; padding:0.35rem 0.55rem; margin-left:0.9rem; color:var(--app-text-muted); font-size:0.75rem; line-height:1.25 }
.app-sidebar__sub-item .sub-icon { font-size:0.75rem; color:var(--app-text-muted) }
.app-sidebar__sub-item .sub-icon .pi { font-size:0.7rem }
.app-sidebar__empty { padding:0.45rem 0.6rem; color:var(--app-text-muted); font-size:0.75rem; line-height:1.35 }

.app-sidebar__network-item { display: flex; align-items: center; gap: 0.65rem; padding: 0.65rem 0.75rem; color: var(--app-text); border-radius: 12px }
.app-sidebar__network-item:hover { background: var(--app-surface-alt) }

.app-sidebar__network-dot { width: 0.8rem; height: 0.8rem; border-radius: 50%; display: inline-block; background: var(--app-border) }
.app-sidebar__network-dot--gray { background: #c7c7c7 }
.app-sidebar__network-dot--amber { background: #fed7aa }
.app-sidebar__network-dot--green { background: #bbf7d0 }

@media (max-width: 960px) {
  .app-sidebar { width: 100%; border-right: 0; border-bottom: 1px solid var(--app-border); padding: 0.75rem; height: auto }
}

.app-sidebar__bottom { padding: 0.85rem 0.5rem }
.app-sidebar__exit { width:100%; padding:10px 12px; border-radius:12px; background:var(--app-surface-alt); border:1px solid var(--app-border); display:flex; gap:8px; align-items:center; justify-content:center; color:var(--app-text) }
.app-sidebar__exit .exit-icon { display:inline-flex; background:transparent; padding:4px; border-radius:6px }
</style>
