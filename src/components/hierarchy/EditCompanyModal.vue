<template>
  <Modal title="Sistemi Düzenle" @close="$emit('close')">
    <form id="edit-company-form" class="company-form" @submit.prevent="submit">
      <label class="field">
        <span>Sistem Adı</span>
        <input v-model="form.projectName" required />
      </label>
      <label class="field">
        <span>Şirket Adı</span>
        <input v-model="form.companyName" required />
      </label>
      <label class="field">
        <span>Şirket Kodu</span>
        <input v-model="form.companyCode" required />
      </label>
      <label class="field">
        <span>Vergi No</span>
        <input v-model="form.taxNumber" required />
      </label>
      <label class="field">
        <span>İletişim E-postası</span>
        <input v-model="form.contactEmail" type="email" required />
      </label>
      <label class="field field--wide">
        <span>Adres</span>
        <textarea v-model="form.address" rows="2" required />
      </label>
      <p v-if="errorMessage" class="form-error" role="alert">{{ errorMessage }}</p>
    </form>

    <template #footer>
      <UiButton variant="ghost" :disabled="isSubmitting" @click="$emit('close')">İptal</UiButton>
      <UiButton type="submit" form="edit-company-form" variant="primary" :disabled="isSubmitting" @click="submit">
        <i :class="isSubmitting ? 'pi pi-spin pi-spinner' : 'pi pi-check'" aria-hidden="true"></i>
        {{ isSubmitting ? 'Kaydediliyor' : 'Kaydet' }}
      </UiButton>
    </template>
  </Modal>
</template>

<script setup lang="ts">
import axios from 'axios';
import { reactive, ref } from 'vue';

import Modal from '@/components/ui/Modal.vue';
import { useHierarchyStore } from '@/stores';
import type { HierarchyCompany, UpdateCompanyRequest } from '@/types';

const props = defineProps<{ company: HierarchyCompany }>();
const emit = defineEmits(['close', 'updated']);
const hierarchyStore = useHierarchyStore();
const isSubmitting = ref(false);
const errorMessage = ref('');

const form = reactive<UpdateCompanyRequest>({
  projectName: props.company.projectName,
  companyName: props.company.name,
  companyCode: props.company.companyCode,
  taxNumber: props.company.taxNumber,
  address: props.company.address,
  contactEmail: props.company.contactEmail,
});

async function submit() {
  if (isSubmitting.value) {
    return;
  }

  isSubmitting.value = true;
  errorMessage.value = '';

  try {
    await hierarchyStore.updateCompany(props.company.id, form);
    emit('updated');
    emit('close');
  } catch (error) {
    errorMessage.value = axios.isAxiosError(error) && typeof error.response?.data?.detail === 'string'
      ? error.response.data.detail
      : 'Sistem güncellenemedi. Bilgileri kontrol edip tekrar deneyin.';
  } finally {
    isSubmitting.value = false;
  }
}
</script>

<style scoped>
.company-form { display:grid; grid-template-columns:repeat(2, minmax(0, 1fr)); gap:0.7rem; width:min(100%, 38rem) }
.field { display:grid; gap:0.3rem; min-width:0 }
.field--wide { grid-column:1 / -1 }
.field span { color:var(--app-text-muted); font-size:0.6875rem }
.field input,
.field textarea { width:100%; border:1px solid var(--app-border); border-radius:7px; background:var(--app-bg); color:var(--app-text); padding:0.55rem 0.65rem; font:inherit; font-size:0.8125rem; outline:none }
.field textarea { resize:vertical }
.field input:focus,
.field textarea:focus { border-color:var(--app-primary); box-shadow:0 0 0 3px rgba(37,99,235,0.1) }
.form-error { grid-column:1 / -1; margin:0; padding:0.55rem 0.65rem; border-radius:7px; background:#fef2f2; color:#b91c1c; font-size:0.75rem }
@media (max-width: 620px) {
  .company-form { grid-template-columns:1fr }
  .field--wide,
  .form-error { grid-column:auto }
}
</style>
