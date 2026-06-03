<template>
  <Modal title="Yeni Tesis Ekle" @close="$emit('close')">
    <form id="create-facility-form" class="entity-form" @submit.prevent="submit">
      <label class="field field--wide">
        <span>Şirket / Sistem</span>
        <select v-model="form.companyId" required>
          <option value="" disabled>Şirket seçin</option>
          <option v-for="company in store.companies" :key="company.id" :value="company.id">{{ company.projectName }} · {{ company.name }}</option>
        </select>
      </label>
      <label class="field"><span>Tesis Adı</span><input v-model="form.name" required placeholder="Örn. Yeni Şube" /></label>
      <label class="field"><span>Şehir</span><input v-model="form.city" required placeholder="Örn. İstanbul" /></label>
      <label class="field"><span>İlçe</span><input v-model="form.district" required placeholder="Örn. Kadıköy" /></label>
      <label class="field field--wide"><span>Adres</span><textarea v-model="form.address" rows="2" required placeholder="Tesis adresi" /></label>
      <p v-if="errorMessage" class="form-error" role="alert">{{ errorMessage }}</p>
    </form>
    <template #footer>
      <UiButton variant="ghost" :disabled="isSubmitting" @click="$emit('close')">İptal</UiButton>
      <UiButton type="submit" form="create-facility-form" variant="primary" :disabled="isSubmitting" @click="submit">
        <i :class="isSubmitting ? 'pi pi-spin pi-spinner' : 'pi pi-plus'" aria-hidden="true"></i>
        {{ isSubmitting ? 'Ekleniyor' : 'Tesisi Ekle' }}
      </UiButton>
    </template>
  </Modal>
</template>

<script setup lang="ts">
import axios from 'axios';
import { onMounted, reactive, ref } from 'vue';
import Modal from '@/components/ui/Modal.vue';
import { useHierarchyStore } from '@/stores';
import type { CreateFacilityRequest } from '@/types';

const emit = defineEmits(['close', 'created']);
const store = useHierarchyStore();
const isSubmitting = ref(false);
const errorMessage = ref('');
const form = reactive<CreateFacilityRequest>({ companyId: '', name: '', city: '', district: '', address: '' });

onMounted(() => store.load());

async function submit() {
  if (isSubmitting.value) return;
  isSubmitting.value = true;
  errorMessage.value = '';
  try {
    await store.createFacility(form);
    emit('created');
    emit('close');
  } catch (error) {
    errorMessage.value = axios.isAxiosError(error) && typeof error.response?.data?.detail === 'string'
      ? error.response.data.detail : 'Tesis eklenemedi.';
  } finally {
    isSubmitting.value = false;
  }
}
</script>

<style scoped>
.entity-form { display:grid; grid-template-columns:repeat(2,minmax(0,1fr)); gap:.7rem; width:min(100%,34rem) }
.field { display:grid; gap:.3rem; min-width:0 }.field--wide { grid-column:1/-1 }
.field span { color:var(--app-text-muted); font-size:.6875rem }
.field input,.field textarea,.field select { width:100%; border:1px solid var(--app-border); border-radius:7px; background:var(--app-bg); color:var(--app-text); padding:.55rem .65rem; font:inherit; font-size:.8125rem; outline:none }
.field textarea { resize:vertical }.field input:focus,.field textarea:focus,.field select:focus { border-color:var(--app-primary); box-shadow:0 0 0 3px rgba(37,99,235,.1) }
.form-error { grid-column:1/-1; margin:0; padding:.55rem .65rem; border-radius:7px; background:#fef2f2; color:#b91c1c; font-size:.75rem }
@media(max-width:620px){.entity-form{grid-template-columns:1fr}.field--wide,.form-error{grid-column:auto}}
</style>
