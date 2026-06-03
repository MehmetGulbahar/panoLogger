/// <reference types="../../../node_modules/.vue-global-types/vue_3.5_0_0_0.d.ts" />
import EmptyState from './EmptyState.vue';
import { toRef } from 'vue';
const props = defineProps();
const emit = defineEmits();
const rowKey = toRef(props, 'rowKey');
import { ref, computed } from 'vue';
const sorters = ref([]);
function toggleSort(key, e) {
    const shift = !!(e && e.shiftKey);
    const idx = sorters.value.findIndex(s => s.key === key);
    if (!shift) {
        // single-column: toggle or set
        if (idx === -1)
            sorters.value = [{ key, dir: 'asc' }];
        else {
            const nextDir = sorters.value[idx].dir === 'asc' ? 'desc' : 'asc';
            sorters.value = [{ key, dir: nextDir }];
        }
        return;
    }
    // multi-column (shift): toggle or add
    if (idx === -1) {
        sorters.value.push({ key, dir: 'asc' });
    }
    else {
        const nextDir = sorters.value[idx].dir === 'asc' ? 'desc' : 'asc';
        sorters.value[idx].dir = nextDir;
    }
}
function handleKeydown(key, e) {
    const k = e.key;
    if (k === 'Enter' || k === ' ' || k === 'Spacebar') {
        e.preventDefault();
        toggleSort(key, e);
    }
}
function get(obj, path) {
    return path.split('.').reduce((acc, p) => (acc ? acc[p] : undefined), obj) ?? '';
}
const displayedItems = computed(() => {
    const list = props.items ? [...props.items] : [];
    if (!sorters.value || sorters.value.length === 0)
        return list;
    return list.sort((a, b) => {
        for (const s of sorters.value) {
            const va = (get(a, s.key) ?? '').toString();
            const vb = (get(b, s.key) ?? '').toString();
            if (va < vb)
                return s.dir === 'asc' ? -1 : 1;
            if (va > vb)
                return s.dir === 'asc' ? 1 : -1;
            // equal -> continue to next sorter
        }
        return 0;
    });
});
function getSortFor(col) {
    return sorters.value.find(s => s.key === col.key) ?? null;
}
function getSortIndex(col) {
    return Math.max(0, sorters.value.findIndex(s => s.key === col.key));
}
function getSortAria(col) {
    const s = getSortFor(col);
    if (!s)
        return `Sırala ${col.label}`;
    return `${col.label} sıralı, yön: ${s.dir}${sorters.value.length > 1 ? `, sıra ${getSortIndex(col) + 1}` : ''}`;
}
function getAriaSort(col) {
    const s = getSortFor(col);
    if (!s)
        return 'none';
    return s.dir === 'asc' ? 'ascending' : 'descending';
}
debugger; /* PartiallyEnd: #3632/scriptSetup.vue */
const __VLS_ctx = {};
let __VLS_components;
let __VLS_directives;
/** @type {__VLS_StyleScopedClasses['ui-table']} */ ;
/** @type {__VLS_StyleScopedClasses['ui-table']} */ ;
/** @type {__VLS_StyleScopedClasses['ui-table']} */ ;
/** @type {__VLS_StyleScopedClasses['ui-table']} */ ;
// CSS variable injection 
// CSS variable injection end 
__VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({
    ...{ class: "ui-table" },
});
__VLS_asFunctionalElement(__VLS_intrinsicElements.table, __VLS_intrinsicElements.table)({});
__VLS_asFunctionalElement(__VLS_intrinsicElements.thead, __VLS_intrinsicElements.thead)({});
__VLS_asFunctionalElement(__VLS_intrinsicElements.tr, __VLS_intrinsicElements.tr)({});
for (const [col] of __VLS_getVForSourceType((__VLS_ctx.columns))) {
    __VLS_asFunctionalElement(__VLS_intrinsicElements.th, __VLS_intrinsicElements.th)({
        ...{ onClick: ((e) => props.sortable && __VLS_ctx.toggleSort(col.key, e)) },
        ...{ onKeydown: ((e) => props.sortable && __VLS_ctx.handleKeydown(col.key, e)) },
        key: (col.key),
        role: (props.sortable ? 'button' : undefined),
        'aria-label': (__VLS_ctx.getSortAria(col)),
        'aria-sort': (__VLS_ctx.getAriaSort(col)),
        tabindex: "0",
    });
    __VLS_asFunctionalElement(__VLS_intrinsicElements.span, __VLS_intrinsicElements.span)({});
    (col.label);
    __VLS_asFunctionalElement(__VLS_intrinsicElements.span, __VLS_intrinsicElements.span)({
        ...{ class: "sort-indicator" },
        'aria-hidden': "true",
    });
    if (__VLS_ctx.getSortFor(col)) {
        __VLS_asFunctionalElement(__VLS_intrinsicElements.span, __VLS_intrinsicElements.span)({
            ...{ class: "sort-dir" },
        });
        (__VLS_ctx.getSortFor(col).dir === 'asc' ? '▲' : '▼');
    }
    if (__VLS_ctx.getSortFor(col)) {
        __VLS_asFunctionalElement(__VLS_intrinsicElements.span, __VLS_intrinsicElements.span)({
            ...{ class: "sort-index" },
        });
        (__VLS_ctx.getSortIndex(col) + 1);
    }
}
__VLS_asFunctionalElement(__VLS_intrinsicElements.tbody, __VLS_intrinsicElements.tbody)({});
if (!__VLS_ctx.displayedItems || __VLS_ctx.displayedItems.length === 0) {
    __VLS_asFunctionalElement(__VLS_intrinsicElements.tr, __VLS_intrinsicElements.tr)({});
    __VLS_asFunctionalElement(__VLS_intrinsicElements.td, __VLS_intrinsicElements.td)({
        colspan: (__VLS_ctx.columns.length),
    });
    /** @type {[typeof EmptyState, ]} */ ;
    // @ts-ignore
    const __VLS_0 = __VLS_asFunctionalComponent(EmptyState, new EmptyState({
        title: "Veri yok",
        description: "Gösterilecek kayıt bulunamadı.",
    }));
    const __VLS_1 = __VLS_0({
        title: "Veri yok",
        description: "Gösterilecek kayıt bulunamadı.",
    }, ...__VLS_functionalComponentArgsRest(__VLS_0));
}
for (const [item] of __VLS_getVForSourceType((__VLS_ctx.displayedItems))) {
    __VLS_asFunctionalElement(__VLS_intrinsicElements.tr, __VLS_intrinsicElements.tr)({
        ...{ onClick: (...[$event]) => {
                __VLS_ctx.$emit('row-click', item);
            } },
        key: (item[__VLS_ctx.rowKey]),
    });
    for (const [col] of __VLS_getVForSourceType((__VLS_ctx.columns))) {
        __VLS_asFunctionalElement(__VLS_intrinsicElements.td, __VLS_intrinsicElements.td)({
            key: (col.key),
        });
        var __VLS_3 = {
            item: (item),
        };
        var __VLS_4 = __VLS_tryAsConstant(`cell-${col.key}`);
        (String(__VLS_ctx.get(item, col.key)));
    }
}
/** @type {__VLS_StyleScopedClasses['ui-table']} */ ;
/** @type {__VLS_StyleScopedClasses['sort-indicator']} */ ;
/** @type {__VLS_StyleScopedClasses['sort-dir']} */ ;
/** @type {__VLS_StyleScopedClasses['sort-index']} */ ;
// @ts-ignore
var __VLS_5 = __VLS_4, __VLS_6 = __VLS_3;
var __VLS_dollars;
const __VLS_self = (await import('vue')).defineComponent({
    setup() {
        return {
            EmptyState: EmptyState,
            rowKey: rowKey,
            toggleSort: toggleSort,
            handleKeydown: handleKeydown,
            get: get,
            displayedItems: displayedItems,
            getSortFor: getSortFor,
            getSortIndex: getSortIndex,
            getSortAria: getSortAria,
            getAriaSort: getAriaSort,
        };
    },
    __typeEmits: {},
    __typeProps: {},
});
const __VLS_component = (await import('vue')).defineComponent({
    setup() {
        return {};
    },
    __typeEmits: {},
    __typeProps: {},
});
export default {};
; /* PartiallyEnd: #4569/main.vue */
