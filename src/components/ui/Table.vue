<template>
  <div class="ui-table">
    <table>
      <thead>
        <tr>
          <th
            v-for="col in columns"
            :key="col.key"
            :role="props.sortable ? 'button' : undefined"
            @click="(e) => props.sortable && toggleSort(col.key, e)"
            @keydown="(e) => props.sortable && handleKeydown(col.key, e)"
            :aria-label="getSortAria(col)"
            :aria-sort="getAriaSort(col)"
            tabindex="0"
          >
            <span>{{ col.label }}</span>
            <span class="sort-indicator" aria-hidden="true">
              <span v-if="getSortFor(col)" class="sort-dir">{{ getSortFor(col)!.dir === 'asc' ? '▲' : '▼' }}</span>
              <span v-if="getSortFor(col)" class="sort-index">{{ getSortIndex(col) + 1 }}</span>
            </span>
          </th>
        </tr>
      </thead>
      <tbody>
        <tr v-if="!displayedItems || displayedItems.length === 0">
          <td :colspan="columns.length">
            <EmptyState title="Veri yok" description="Gösterilecek kayıt bulunamadı." />
          </td>
        </tr>
        <tr v-for="item in displayedItems" :key="item[rowKey]" @click="$emit('row-click', item)">
          <td v-for="col in columns" :key="col.key">
            <slot :name="`cell-${col.key}`" :item="item">{{ String(get(item, col.key)) }}</slot>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';
import EmptyState from './EmptyState.vue';

type Column = { key: string; label: string };

const props = defineProps<{
  columns: Column[];
  items?: Record<string, any>[];
  rowKey?: string;
  sortable?: boolean;
}>();

defineEmits<{
  (e: 'row-click', item: Record<string, any>): void;
}>();

const rowKey = computed(() => props.rowKey ?? 'id');

const sorters = ref<Array<{ key: string; dir: 'asc' | 'desc' }>>([]);

function toggleSort(key: string, e?: MouseEvent | KeyboardEvent) {
  const shift = !!(e && e.shiftKey);
  const idx = sorters.value.findIndex(s => s.key === key);

  if (!shift) {
    // single-column: toggle or set
    if (idx === -1) sorters.value = [{ key, dir: 'asc' }];
    else {
      const nextDir = sorters.value[idx].dir === 'asc' ? 'desc' : 'asc';
      sorters.value = [{ key, dir: nextDir }];
    }
    return;
  }

  // multi-column (shift): toggle or add
  if (idx === -1) {
    sorters.value.push({ key, dir: 'asc' });
  } else {
    const nextDir = sorters.value[idx].dir === 'asc' ? 'desc' : 'asc';
    sorters.value[idx].dir = nextDir;
  }
}

function handleKeydown(key: string, e: KeyboardEvent) {
  const k = e.key;
  if (k === 'Enter' || k === ' ' || k === 'Spacebar') {
    e.preventDefault();
    toggleSort(key, e);
  }
}

function get(obj: Record<string, any>, path: string) {
  return path.split('.').reduce((acc, p) => (acc ? acc[p] : undefined), obj) ?? '';
}

const displayedItems = computed(() => {
  const list = props.items ? [...props.items] : [];
  if (!sorters.value || sorters.value.length === 0) return list;

  return list.sort((a, b) => {
    for (const s of sorters.value) {
      const va = (get(a, s.key) ?? '').toString();
      const vb = (get(b, s.key) ?? '').toString();
      if (va < vb) return s.dir === 'asc' ? -1 : 1;
      if (va > vb) return s.dir === 'asc' ? 1 : -1;
      // equal -> continue to next sorter
    }
    return 0;
  });
});

function getSortFor(col: Column) {
  return sorters.value.find(s => s.key === col.key) ?? null;
}

function getSortIndex(col: Column) {
  return Math.max(0, sorters.value.findIndex(s => s.key === col.key));
}

function getSortAria(col: Column) {
  const s = getSortFor(col);
  if (!s) return `Sırala ${col.label}`;
  return `${col.label} sıralı, yön: ${s.dir}${sorters.value.length > 1 ? `, sıra ${getSortIndex(col) + 1}` : ''}`;
}

function getAriaSort(col: Column) {
  const s = getSortFor(col);
  if (!s) return 'none';
  return s.dir === 'asc' ? 'ascending' : 'descending';
}
</script>

<style scoped>
.ui-table { width: 100%; overflow: auto }
.ui-table table { width: 100%; border-collapse: collapse }
.ui-table th { text-align: left; padding: calc(var(--space-sm)); color: var(--app-text-muted); font-size: var(--type-scale-sm); border-bottom: 1px solid var(--app-border) }
.ui-table td { padding: calc(var(--space-sm)); border-bottom: 1px dashed rgba(0,0,0,0.03); vertical-align: middle }
.ui-table tr:hover td { background: rgba(0,0,0,0.01); cursor: pointer }
.sort-indicator { margin-left: 0.5rem; display:inline-flex; align-items:center; gap:0.25rem }
.sort-dir { font-size:0.75rem; opacity:0.9 }
.sort-index { background:var(--app-surface); border:1px solid var(--app-border); padding:0 4px; border-radius:6px; font-size:0.65rem; color:var(--app-text-muted) }
</style>
