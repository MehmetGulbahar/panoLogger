<template>
  <Modal title="Yeni Sistem Ekle" @close="$emit('close')">
    <form id="create-system-form" class="system-form" @submit.prevent="submit">
      <section class="form-section">
        <div class="form-section__head">
          <i class="pi pi-folder" aria-hidden="true"></i>
          <div>
            <h4>Sistem ve Şirket</h4>
            <p>Hiyerarşide görünecek ana proje bilgileri.</p>
          </div>
        </div>
        <div class="form-grid">
          <label class="field">
            <span>Sistem Adı</span>
            <input v-model="form.projectName" required placeholder="Örn. AVM Elektrik Yönetimi" />
          </label>
          <label class="field">
            <span>Şirket Adı</span>
            <input v-model="form.companyName" required placeholder="Örn. Enerji Teknolojileri A.Ş." />
          </label>
          <label class="field">
            <span>Vergi No</span>
            <input v-model="form.taxNumber" required placeholder="10 haneli vergi numarası" />
          </label>
          <label class="field">
            <span>İletişim E-postası</span>
            <input v-model="form.contactEmail" type="email" required placeholder="info@sirket.com" />
          </label>
          <label class="field field--wide">
            <span>Şirket Adresi</span>
            <input v-model="form.companyAddress" required placeholder="Şirket adresi" />
          </label>
        </div>
      </section>

      <section class="form-section">
        <div class="form-section__head">
          <i class="pi pi-building" aria-hidden="true"></i>
          <div>
            <h4>Tesis</h4>
            <p>Sistemin ilk tesis veya mağaza kaydı.</p>
          </div>
        </div>
        <div class="form-grid">
          <label class="field">
            <span>Tesis Adı</span>
            <input v-model="form.facilityName" required placeholder="Örn. Merkez Üretim Tesisleri" />
          </label>
          <label class="field">
            <span>Şehir</span>
            <input v-model="form.city" required placeholder="Örn. İstanbul" />
          </label>
          <label class="field">
            <span>İlçe</span>
            <input v-model="form.district" required placeholder="Örn. Kadıköy" />
          </label>
          <label class="field field--wide">
            <span>Tesis Adresi</span>
            <input v-model="form.facilityAddress" required placeholder="Tesis adresi" />
          </label>
        </div>
      </section>

      <section class="form-section">
        <div class="form-section__head">
          <i class="pi pi-bolt" aria-hidden="true"></i>
          <div>
            <h4>Pano</h4>
            <p>Tesise bağlı ilk elektrik panosu.</p>
          </div>
        </div>
        <div class="form-grid">
          <label class="field">
            <span>Pano Kodu</span>
            <input v-model="form.panelCode" required placeholder="Örn. P-001" />
          </label>
          <label class="field">
            <span>Pano Adı</span>
            <input v-model="form.panelName" required placeholder="Örn. Ana Dağıtım Paneli" />
          </label>
          <label class="field field--wide">
            <span>Açıklama</span>
            <textarea v-model="form.panelDescription" required rows="2" placeholder="Pano açıklaması" />
          </label>
        </div>
      </section>

      <p v-if="errorMessage" class="form-error" role="alert">{{ errorMessage }}</p>
    </form>

    <template #footer>
      <UiButton variant="ghost" :disabled="isSubmitting" @click="$emit('close')">İptal</UiButton>
      <UiButton type="submit" form="create-system-form" variant="primary" :disabled="isSubmitting" @click="submit">
        <i :class="isSubmitting ? 'pi pi-spin pi-spinner' : 'pi pi-plus'" aria-hidden="true"></i>
        {{ isSubmitting ? 'Ekleniyor' : 'Sistemi Ekle' }}
      </UiButton>
    </template>
  </Modal>
</template>

<script setup lang="ts">
import axios from 'axios';
import { reactive, ref } from 'vue';

import Modal from '@/components/ui/Modal.vue';
import { useHierarchyStore } from '@/stores';
import type { CreateSystemRequest } from '@/types';

const emit = defineEmits(['close', 'created']);
const hierarchyStore = useHierarchyStore();
const isSubmitting = ref(false);
const errorMessage = ref('');

const form = reactive<CreateSystemRequest>({
  projectName: '',
  companyName: '',
  taxNumber: '',
  companyAddress: '',
  contactEmail: '',
  facilityName: '',
  city: '',
  district: '',
  facilityAddress: '',
  panelCode: '',
  panelName: '',
  panelDescription: '',
});

async function submit() {
  if (isSubmitting.value) {
    return;
  }

  isSubmitting.value = true;
  errorMessage.value = '';

  try {
    const company = await hierarchyStore.createSystem(form);
    emit('created', company);
    emit('close');
  } catch (error) {
    errorMessage.value = axios.isAxiosError(error) && typeof error.response?.data?.detail === 'string'
      ? error.response.data.detail
      : 'Sistem eklenemedi. Bilgileri kontrol edip tekrar deneyin.';
  } finally {
    isSubmitting.value = false;
  }
}
</script>

<style scoped>
.system-form { display:grid; gap:1rem; width:min(100%, 44rem) }
.form-section { display:grid; gap:0.7rem; padding-bottom:1rem; border-bottom:1px solid var(--app-border) }
.form-section:last-of-type { padding-bottom:0; border-bottom:0 }
.form-section__head { display:flex; align-items:flex-start; gap:0.55rem }
.form-section__head .pi { margin-top:0.1rem; color:var(--app-primary); font-size:0.9rem }
.form-section__head h4 { margin:0; font-size:0.8125rem; line-height:1.25 }
.form-section__head p { margin:0.1rem 0 0; color:var(--app-text-muted); font-size:0.6875rem; line-height:1.3 }
.form-grid { display:grid; grid-template-columns:repeat(2, minmax(0, 1fr)); gap:0.65rem }
.field { display:grid; gap:0.3rem; min-width:0 }
.field--wide { grid-column:1 / -1 }
.field span { color:var(--app-text-muted); font-size:0.6875rem; line-height:1.2 }
.field input,
.field textarea { width:100%; border:1px solid var(--app-border); border-radius:7px; background:var(--app-bg); color:var(--app-text); padding:0.55rem 0.65rem; font:inherit; font-size:0.8125rem; outline:none }
.field textarea { resize:vertical }
.field input:focus,
.field textarea:focus { border-color:var(--app-primary); box-shadow:0 0 0 3px rgba(37,99,235,0.1) }
.form-error { margin:0; padding:0.55rem 0.65rem; border-radius:7px; background:#fef2f2; color:#b91c1c; font-size:0.75rem }
@media (max-width: 620px) {
  .form-grid { grid-template-columns:1fr }
  .field--wide { grid-column:auto }
}
</style>
