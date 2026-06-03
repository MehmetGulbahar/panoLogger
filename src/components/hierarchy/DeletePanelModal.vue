<template>
  <Modal title="Panoyu Sil" @close="$emit('close')">
    <div class="delete-copy"><i class="pi pi-trash" aria-hidden="true"></i><div><p><strong>{{ panel.name }}</strong> panosunu silmek istediğinizden emin misiniz?</p><span>Bağlı dosya kayıtları ve depolanan dosyalar da silinir.</span></div></div>
    <p v-if="errorMessage" class="delete-error" role="alert">{{ errorMessage }}</p>
    <template #footer>
      <UiButton variant="ghost" :disabled="isDeleting" @click="$emit('close')">İptal</UiButton>
      <button class="delete-button" type="button" :disabled="isDeleting" @click="remove"><i :class="isDeleting ? 'pi pi-spin pi-spinner' : 'pi pi-trash'" aria-hidden="true"></i>{{ isDeleting ? 'Siliniyor' : 'Panoyu Sil' }}</button>
    </template>
  </Modal>
</template>
<script setup lang="ts">
import axios from 'axios'; import { ref } from 'vue'; import Modal from '@/components/ui/Modal.vue'; import { useHierarchyStore } from '@/stores'; import type { HierarchyPanel } from '@/types';
const props = defineProps<{ panel: HierarchyPanel }>(); const emit = defineEmits(['close','deleted']); const store = useHierarchyStore(); const isDeleting = ref(false); const errorMessage = ref('');
async function remove() { isDeleting.value = true; errorMessage.value = ''; try { await store.deletePanel(props.panel.id); emit('deleted'); emit('close'); } catch (error) { errorMessage.value = axios.isAxiosError(error) && typeof error.response?.data?.detail === 'string' ? error.response.data.detail : 'Pano silinemedi.'; } finally { isDeleting.value = false; } }
</script>
<style scoped>
.delete-copy{display:flex;gap:.75rem;align-items:flex-start;width:min(100%,30rem)}.delete-copy>.pi{width:2rem;height:2rem;display:grid;place-items:center;flex:0 0 auto;border-radius:7px;background:#fef2f2;color:#b91c1c;font-size:.85rem}.delete-copy p{margin:0;font-size:.8125rem;line-height:1.4}.delete-copy span{display:block;margin-top:.25rem;color:var(--app-text-muted);font-size:.75rem;line-height:1.4}.delete-error{margin:.75rem 0 0;padding:.55rem .65rem;border-radius:7px;background:#fef2f2;color:#b91c1c;font-size:.75rem}.delete-button{display:inline-flex;align-items:center;gap:.4rem;border:0;border-radius:7px;padding:.5rem .75rem;background:#dc2626;color:#fff;font-size:.8125rem;cursor:pointer}.delete-button:hover:not(:disabled){background:#b91c1c}.delete-button:disabled{opacity:.6;cursor:wait}
</style>
