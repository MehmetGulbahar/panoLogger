<template>
  <Modal title="Tesisi Düzenle" @close="$emit('close')">
    <form id="edit-facility-form" class="entity-form" @submit.prevent="submit">
      <label class="field"><span>Tesis Adı</span><input v-model="form.name" required /></label>
      <label class="field"><span>Şehir</span><input v-model="form.city" required /></label>
      <label class="field"><span>İlçe</span><input v-model="form.district" required /></label>
      <label class="field field--wide"><span>Adres</span><textarea v-model="form.address" rows="2" required /></label>
      <p v-if="errorMessage" class="form-error" role="alert">{{ errorMessage }}</p>
    </form>
    <template #footer>
      <UiButton variant="ghost" :disabled="isSubmitting" @click="$emit('close')">İptal</UiButton>
      <UiButton type="submit" form="edit-facility-form" variant="primary" :disabled="isSubmitting" @click="submit">
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
import type { HierarchyFacility, UpdateFacilityRequest } from '@/types';

const props = defineProps<{ facility: HierarchyFacility }>();
const emit = defineEmits(['close', 'updated']);
const store = useHierarchyStore();
const isSubmitting = ref(false);
const errorMessage = ref('');
const form = reactive<UpdateFacilityRequest>({
  name: props.facility.name,
  city: props.facility.city,
  district: props.facility.district,
  address: props.facility.address,
});

async function submit() {
  if (isSubmitting.value) return;
  isSubmitting.value = true;
  errorMessage.value = '';
  try {
    await store.updateFacility(props.facility.id, form);
    emit('updated');
    emit('close');
  } catch (error) {
    errorMessage.value = axios.isAxiosError(error) && typeof error.response?.data?.detail === 'string'
      ? error.response.data.detail : 'Tesis güncellenemedi.';
  } finally {
    isSubmitting.value = false;
  }
}
</script>

<style scoped>
.entity-form { display:grid; grid-template-columns:repeat(2, minmax(0, 1fr)); gap:0.7rem; width:min(100%, 34rem) }
.field { display:grid; gap:0.3rem }.field--wide { grid-column:1 / -1 }
.field span { color:var(--app-text-muted); font-size:0.6875rem }
.field input,.field textarea { width:100%; border:1px solid var(--app-border); border-radius:7px; background:var(--app-bg); color:var(--app-text); padding:0.55rem 0.65rem; font:inherit; font-size:0.8125rem; outline:none }
.field textarea { resize:vertical }.field input:focus,.field textarea:focus { border-color:var(--app-primary); box-shadow:0 0 0 3px rgba(37,99,235,0.1) }
.form-error { grid-column:1 / -1; margin:0; padding:0.55rem 0.65rem; border-radius:7px; background:#fef2f2; color:#b91c1c; font-size:0.75rem }
@media (max-width:620px) { .entity-form { grid-template-columns:1fr }.field--wide,.form-error { grid-column:auto } }
</style>
