import { computed } from 'vue';
import { useRoute } from 'vue-router';

export function useBreadcrumbs() {
  const route = useRoute();

  const breadcrumbs = computed(() =>
    route.matched
      .filter((item) => item.meta?.title)
      .map((item) => ({
        label: String(item.meta.title),
        to: item.path,
      })),
  );

  return { breadcrumbs };
}