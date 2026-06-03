/// <reference types="../../../node_modules/.vue-global-types/vue_3.5_0_0_0.d.ts" />
import { ref } from 'vue';
import { mockPanels } from '@/mocks/mock-data';
const panels = ref(mockPanels);
import { useRouter } from 'vue-router';
const columns = [
    { key: 'code', label: 'Kod' },
    { key: 'name', label: 'Ad' },
    { key: 'description', label: 'Açıklama' },
];
const router = useRouter();
function onRowClick(item) {
    router.push({ path: `/panels/${item.id}` });
}
debugger; /* PartiallyEnd: #3632/scriptSetup.vue */
const __VLS_ctx = {};
let __VLS_components;
let __VLS_directives;
// CSS variable injection 
// CSS variable injection end 
__VLS_asFunctionalElement(__VLS_intrinsicElements.section, __VLS_intrinsicElements.section)({
    ...{ class: "page-card app-card" },
});
__VLS_asFunctionalElement(__VLS_intrinsicElements.h1, __VLS_intrinsicElements.h1)({});
__VLS_asFunctionalElement(__VLS_intrinsicElements.p, __VLS_intrinsicElements.p)({});
const __VLS_0 = {}.UiTable;
/** @type {[typeof __VLS_components.UiTable, typeof __VLS_components.UiTable, ]} */ ;
// @ts-ignore
const __VLS_1 = __VLS_asFunctionalComponent(__VLS_0, new __VLS_0({
    ...{ 'onRowClick': {} },
    columns: (__VLS_ctx.columns),
    items: (__VLS_ctx.panels),
    rowKey: "id",
}));
const __VLS_2 = __VLS_1({
    ...{ 'onRowClick': {} },
    columns: (__VLS_ctx.columns),
    items: (__VLS_ctx.panels),
    rowKey: "id",
}, ...__VLS_functionalComponentArgsRest(__VLS_1));
let __VLS_4;
let __VLS_5;
let __VLS_6;
const __VLS_7 = {
    onRowClick: (__VLS_ctx.onRowClick)
};
__VLS_3.slots.default;
{
    const { 'cell-description': __VLS_thisSlot } = __VLS_3.slots;
    const [{ item }] = __VLS_getSlotParams(__VLS_thisSlot);
    (item.description);
}
var __VLS_3;
/** @type {__VLS_StyleScopedClasses['page-card']} */ ;
/** @type {__VLS_StyleScopedClasses['app-card']} */ ;
var __VLS_dollars;
const __VLS_self = (await import('vue')).defineComponent({
    setup() {
        return {
            panels: panels,
            columns: columns,
            onRowClick: onRowClick,
        };
    },
});
export default (await import('vue')).defineComponent({
    setup() {
        return {};
    },
});
; /* PartiallyEnd: #4569/main.vue */
