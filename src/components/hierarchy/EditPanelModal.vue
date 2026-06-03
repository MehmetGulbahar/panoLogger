<template>
  <Modal title="Panoyu Düzenle" @close="$emit('close')">
    <form id="edit-panel-form" class="entity-form" @submit.prevent="submit">
      <label class="field"><span>Pano Kodu</span><input v-model="form.code" required /></label>
      <label class="field"><span>Pano Adı</span><input v-model="form.name" required /></label>
      <label class="field field--wide"><span>Açıklama</span><textarea v-model="form.description" rows="2" required /></label>
      <p v-if="errorMessage" class="form-error" role="alert">{{ errorMessage }}</p>
    </form>
    <template #footer>
      <UiButton variant="ghost" :disabled="isSubmitting" @click="$emit('close')">İptal</UiButton>
      <UiButton type="submit" form="edit-panel-form" variant="primary" :disabled="isSubmitting" @click="submit">
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
import type { HierarchyPanel, UpdatePanelRequest } from '@/types';

const props = defineProps<{ panel: HierarchyPanel }>();
const emit = defineEmits(['close', 'updated']);
const store = useHierarchyStore();
const isSubmitting = ref(false);
const errorMessage = ref('');
const form = reactive<UpdatePanelRequest>({ code: props.panel.code, name: props.panel.name, description: props.panel.description });

async function submit() {
  if (isSubmitting.value) return;
  isSubmitting.value = true;
  errorMessage.value = '';
  try {
    await store.updatePanel(props.panel.id, form);
    emit('updated');
    emit('close');
  } catch (error) {
    errorMessage.value = axios.isAxiosError(error) && typeof error.response?.data?.detail === 'string'
      ? error.response.data.detail : 'Pano güncellenemedi.';
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
