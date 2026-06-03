import { defineStore } from 'pinia';
import { computed, ref } from 'vue';
export const useUiStore = defineStore('ui', () => {
    const isLoading = ref(false);
    const globalError = ref(null);
    const hasError = computed(() => Boolean(globalError.value));
    function setLoading(value) {
        isLoading.value = value;
    }
    function setError(message) {
        globalError.value = message;
    }
    function resetError() {
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
let uiStoreInstance = null;
export function setGlobalLoaderState(value) {
    uiStoreInstance ??= useUiStore();
    uiStoreInstance.setLoading(value);
}
