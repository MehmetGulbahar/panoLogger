import { defineStore } from 'pinia';
import { computed, ref } from 'vue';

export const useUiStore = defineStore('ui', () => {
  const isLoading = ref(false);
  const globalError = ref<string | null>(null);

  const hasError = computed(() => Boolean(globalError.value));

  function setLoading(value: boolean): void {
    isLoading.value = value;
  }

  function setError(message: string | null): void {
    globalError.value = message;
  }

  function resetError(): void {
    globalError.value = null;
  }

  return {
    isLoading,
    globalError,
    hasError,
    setLoading,
    setError,
    resetError,
  };
});

let uiStoreInstance: ReturnType<typeof useUiStore> | null = null;

export function setGlobalLoaderState(value: boolean): void {
  uiStoreInstance ??= useUiStore();
  uiStoreInstance.setLoading(value);
}