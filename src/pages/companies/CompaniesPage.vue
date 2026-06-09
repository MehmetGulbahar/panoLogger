<template>
  <section class="page-card app-card">
    <div class="page-head">
      <div>
        <h1>Şirketler</h1>
        <p>Mevcut şirketlerin listesi.</p>
      </div>
      <div v-if="canManageCompanies" class="page-actions">
        <UiButton variant="primary" @click="openCreateSystem"><i class="pi pi-plus" aria-hidden="true"></i> Yeni Sistem</UiButton>
      </div>
    </div>

    <UiTable :columns="columns" :items="hierarchyStore.companies" rowKey="id" @row-click="onRowClick">
      <template #cell-actions="{ item }">
        <div class="row-actions">
          <button class="row-action" type="button" title="Düzenle" aria-label="Düzenle" @click.stop="openEditCompany(item)">
            <i class="pi pi-pencil" aria-hidden="true"></i>
          </button>
          <button class="row-action row-action--danger" type="button" title="Sil" aria-label="Sil" @click.stop="openDeleteCompany(item)">
            <i class="pi pi-trash" aria-hidden="true"></i>
          </button>
        </div>
      </template>
    </UiTable>
  </section>
</template>

<script setup lang="ts">
import { computed, onMounted } from 'vue';
import { useAuthStore, useHierarchyStore } from '@/stores';
import { useModalStore } from '@/stores/modal-store';
import CreateSystemModal from '@/components/hierarchy/CreateSystemModal.vue';
import DeleteCompanyModal from '@/components/hierarchy/DeleteCompanyModal.vue';
import EditCompanyModal from '@/components/hierarchy/EditCompanyModal.vue';
import type { HierarchyCompany } from '@/types';

const authStore = useAuthStore();
const hierarchyStore = useHierarchyStore();
const canManageCompanies = computed(() => authStore.hasPermission('companies.manage'));
onMounted(() => hierarchyStore.load());

import { useRouter } from 'vue-router';

const baseColumns = [
  { key: 'name', label: 'Şirket' },
  { key: 'projectName', label: 'Proje' },
  { key: 'companyCode', label: 'Kod' },
  { key: 'contactEmail', label: 'İletişim' },
  { key: 'taxNumber', label: 'Vergi No' },
  { key: 'address', label: 'Adres' },
];
const columns = computed(() => [
  ...baseColumns,
  ...(canManageCompanies.value ? [{ key: 'actions', label: '' }] : []),
]);

const router = useRouter();
function onRowClick(item: any) {
  router.push({ path: `/companies/${item.id}` });
}

function openCreateSystem() {
  const modals = useModalStore();
  modals.open(CreateSystemModal, {}, {});
}

function openEditCompany(company: HierarchyCompany) {
  const modals = useModalStore();
  modals.open(EditCompanyModal, { company }, {});
}

function openDeleteCompany(company: HierarchyCompany) {
  const modals = useModalStore();
  modals.open(DeleteCompanyModal, { company }, {});
}
</script>

<style scoped>
.companies-list { margin-top:1rem; display:flex; flex-direction:column; gap:0.9rem }
.row-actions { display:flex; justify-content:flex-end; gap:0.2rem }
.row-action { width:1.8rem; height:1.8rem; display:grid; place-items:center; border:0; border-radius:6px; background:transparent; color:var(--app-text-muted); cursor:pointer }
.row-action:hover { background:var(--app-surface-alt); color:var(--app-primary) }
.row-action--danger:hover { background:#fef2f2; color:#b91c1c }
.row-action .pi { font-size:0.78rem }
</style>

<style scoped>
/* Make the page container flat for Şirketler */
.page-card.app-card {
  border: none;
  box-shadow: none;
  background: transparent;
}
</style>
