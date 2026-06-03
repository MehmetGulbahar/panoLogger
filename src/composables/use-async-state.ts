import { computed, ref, type Ref } from 'vue';

export function useAsyncState<T>(initialValue: T) {
  const data = ref(initialValue) as Ref<T>;
  const isPending = ref(false);
  const error = ref<string | null>(null);

  const hasError = computed(() => error.value !== null);

  return {
    data,
    isPending,
    error,
    hasError,
  };
}