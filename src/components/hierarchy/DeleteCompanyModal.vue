<template>
  <Modal title="Sistemi Sil" @close="$emit('close')">
    <div class="delete-copy">
      <div class="delete-icon"><i class="pi pi-trash" aria-hidden="true"></i></div>
      <div>
        <p><strong>{{ company.projectName }}</strong> sistemini silmek istediğinizden emin misiniz?</p>
        <span>Bu işlem bağlı tesisleri, panoları, dosya kayıtlarını ve depolanan dosyaları da siler.</span>
      </div>
    </div>
    <p v-if="errorMessage" class="delete-error" role="alert">{{ errorMessage }}</p>

    <template #footer>
      <UiButton variant="ghost" :disabled="isDeleting" @click="$emit('close')">İptal</UiButton>
      <button class="delete-button" type="button" :disabled="isDeleting" @click="remove">
        <i :class="isDeleting ? 'pi pi-spin pi-spinner' : 'pi pi-trash'" aria-hidden="true"></i>
        {{ isDeleting ? 'Siliniyor' : 'Sistemi Sil' }}
      </button>
    </template>
  </Modal>
</template>

<script setup lang="ts">
import axios from 'axios';
import { ref } from 'vue';

import Modal from '@/components/ui/Modal.vue';
import { useHierarchyStore } from '@/stores';
import type { HierarchyCompany } from '@/types';

const props = defineProps<{ company: HierarchyCompany }>();
const emit = defineEmits(['close', 'deleted']);
const hierarchyStore = useHierarchyStore();
const isDeleting = ref(false);
const errorMessage = ref('');

async function remove() {
  isDeleting.value = true;
  errorMessage.value = '';

  try {
    await hierarchyStore.deleteCompany(props.company.id);
    emit('deleted');
    emit('close');
  } catch (error) {
    errorMessage.value = axios.isAxiosError(error) && typeof error.response?.data?.detail === 'string'
      ? error.response.data.detail
      : 'Sistem silinemedi. Tekrar deneyin.';
  } finally {
    isDeleting.value = false;
  }
}
</script>

<style scoped>
.delete-copy { display:flex; gap:0.75rem; align-items:flex-start; width:min(100%, 30rem) }
.delete-icon { width:2rem; height:2rem; display:grid; place-items:center; flex:0 0 auto; border-radius:7px; background:#fef2f2; color:#b91c1c }
.delete-icon .pi { font-size:0.85rem }
.delete-copy p { margin:0; font-size:0.8125rem; line-height:1.4 }
.delete-copy span { display:block; margin-top:0.25rem; color:var(--app-text-muted); font-size:0.75rem; line-height:1.4 }
.delete-error { margin:0.75rem 0 0; padding:0.55rem 0.65rem; border-radius:7px; background:#fef2f2; color:#b91c1c; font-size:0.75rem }
.delete-button { display:inline-flex; align-items:center; justify-content:center; gap:0.4rem; border:0; border-radius:7px; padding:0.5rem 0.75rem; background:#dc2626; color:#fff; font-size:0.8125rem; cursor:pointer }
.delete-button:hover:not(:disabled) { background:#b91c1c }
.delete-button:disabled { opacity:0.6; cursor:wait }
</style>
