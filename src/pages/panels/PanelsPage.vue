<template>
  <section class="page-card app-card">
    <div class="page-head">
      <div>
        <h1>Paneller</h1>
        <p>Kayıtlı panellerin listesi.</p>
      </div>
      <div v-if="canManagePanels" class="page-actions">
        <UiButton variant="primary" @click="openCreate">
          <i class="pi pi-plus" aria-hidden="true"></i>
          Yeni Pano
        </UiButton>
      </div>
    </div>

    <UiTable :columns="columns" :items="hierarchyStore.panels" rowKey="id" @row-click="onRowClick">
      <template #cell-description="{ item }">{{ item.description }}</template>
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
import CreatePanelModal from '@/components/hierarchy/CreatePanelModal.vue';
import DeletePanelModal from '@/components/hierarchy/DeletePanelModal.vue';
import EditPanelModal from '@/components/hierarchy/EditPanelModal.vue';
import { useAuthStore, useHierarchyStore } from '@/stores';
import { useModalStore } from '@/stores/modal-store';
import type { HierarchyPanel } from '@/types';

const router = useRouter();
const authStore = useAuthStore();
const hierarchyStore = useHierarchyStore();
const canManagePanels = computed(() => authStore.hasPermission('panels.manage'));
const baseColumns = [
  { key: 'code', label: 'Kod' },
  { key: 'name', label: 'Ad' },
  { key: 'description', label: 'Açıklama' },
];
const columns = computed(() => [...baseColumns, ...(canManagePanels.value ? [{ key: 'actions', label: '' }] : [])]);

onMounted(() => hierarchyStore.load());

function onRowClick(item: HierarchyPanel) {
  router.push({ path: `/panels/${item.id}` });
}

function openCreate() {
  useModalStore().open(CreatePanelModal, {}, {});
}

function openEdit(panel: HierarchyPanel) {
  useModalStore().open(EditPanelModal, { panel }, {});
}

function openDelete(panel: HierarchyPanel) {
  useModalStore().open(DeletePanelModal, { panel }, {});
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
