<template>
  <section class="page-card app-card">
    <div class="page-head">
      <div>
        <h1>Tesisler</h1>
        <p>Mevcut tesislerin listesi.</p>
      </div>
      <div v-if="canManageFacilities" class="page-actions">
        <UiButton variant="primary" @click="openCreate">
          <i class="pi pi-plus" aria-hidden="true"></i>
          Yeni Tesis
        </UiButton>
      </div>
    </div>

    <UiTable :columns="columns" :items="hierarchyStore.facilities" rowKey="id" @row-click="onRowClick">
      <template #cell-actions="{ item }">
        <div class="row-actions">
          <button class="row-action" type="button" title="Düzenle" aria-label="Düzenle" @click.stop="openEdit(item)"><i class="pi pi-pencil" aria-hidden="true"></i></button>
          <button class="row-action row-action--danger" type="button" title="Sil" aria-label="Sil" @click.stop="openDelete(item)"><i class="pi pi-trash" aria-hidden="true"></i></button>
        </div>
      </template>
    </UiTable>
  </section>
</template>

<script setup lang="ts">
import { computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import CreateFacilityModal from '@/components/hierarchy/CreateFacilityModal.vue';
import DeleteFacilityModal from '@/components/hierarchy/DeleteFacilityModal.vue';
import EditFacilityModal from '@/components/hierarchy/EditFacilityModal.vue';
import { useAuthStore, useHierarchyStore } from '@/stores';
import { useModalStore } from '@/stores/modal-store';
import type { HierarchyFacility } from '@/types';

const router = useRouter();
const authStore = useAuthStore();
const hierarchyStore = useHierarchyStore();
const canManageFacilities = computed(() => authStore.hasPermission('facilities.manage'));
const baseColumns = [
  { key: 'name', label: 'Tesis' },
  { key: 'city', label: 'Şehir' },
  { key: 'district', label: 'İlçe' },
  { key: 'address', label: 'Adres' },
];
const columns = computed(() => [...baseColumns, ...(canManageFacilities.value ? [{ key: 'actions', label: '' }] : [])]);

onMounted(() => hierarchyStore.load());

function onRowClick(item: HierarchyFacility) {
  router.push({ path: `/facilities/${item.id}` });
}

function openCreate() {
  useModalStore().open(CreateFacilityModal, {}, {});
}

function openEdit(facility: HierarchyFacility) {
  useModalStore().open(EditFacilityModal, { facility }, {});
}

function openDelete(facility: HierarchyFacility) {
  useModalStore().open(DeleteFacilityModal, { facility }, {});
}
</script>

<style scoped>
.page-card.app-card { border:none; box-shadow:none; background:transparent }
.page-head { display:flex; align-items:flex-start; justify-content:space-between; gap:1rem }
.row-actions { display:flex; justify-content:flex-end; gap:0.2rem }
.row-action { width:1.8rem; height:1.8rem; display:grid; place-items:center; border:0; border-radius:6px; background:transparent; color:var(--app-text-muted); cursor:pointer }
.row-action:hover { background:var(--app-surface-alt); color:var(--app-primary) }
.row-action--danger:hover { background:#fef2f2; color:#b91c1c }
.row-action .pi { font-size:0.78rem }
</style>
