import { storeToRefs } from 'pinia';

import { useUiStore } from '@/stores/ui-store';

export function useGlobalLoading() {
  const uiStore = useUiStore();
  return storeToRefs(uiStore);
}