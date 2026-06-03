import { defineStore } from 'pinia';
import { computed, reactive } from 'vue';

export const useFileStore = defineStore('files', () => {
  const fileCounts = reactive<Record<string, number>>({});

  function getPanelFileCount(panelId: string): number {
    return fileCounts[panelId] ?? 0;
  }

  function setPanelFileCount(panelId: string, count: number): void {
    fileCounts[panelId] = Math.max(0, count);
  }

  function addPanelFiles(panelId: string, amount = 1): void {
    const currentCount = getPanelFileCount(panelId);
    fileCounts[panelId] = currentCount + Math.max(0, amount);
  }

  const totalFileCount = computed(() => Object.values(fileCounts).reduce((sum, count) => sum + count, 0));

  return {
    fileCounts,
    totalFileCount,
    getPanelFileCount,
    setPanelFileCount,
    addPanelFiles,
  };
});
