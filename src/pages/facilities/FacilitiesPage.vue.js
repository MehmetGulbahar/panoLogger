/// <reference types="../../../node_modules/.vue-global-types/vue_3.5_0_0_0.d.ts" />
import { ref } from 'vue';
import { mockFacilities } from '@/mocks/mock-data';
const facilities = ref(mockFacilities);
import { useRouter } from 'vue-router';
const columns = [
    { key: 'name', label: 'Tesis' },
    { key: 'city', label: 'Şehir' },
    { key: 'address', label: 'Adres' },
];
const router = useRouter();
function onRowClick(item) {
    router.push({ path: `/facilities/${item.id}` });
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
/** @type {[typeof __VLS_components.UiTable, ]} */ ;
// @ts-ignore
const __VLS_1 = __VLS_asFunctionalComponent(__VLS_0, new __VLS_0({
    ...{ 'onRowClick': {} },
    columns: (__VLS_ctx.columns),
    items: (__VLS_ctx.facilities),
    rowKey: "id",
}));
const __VLS_2 = __VLS_1({
    ...{ 'onRowClick': {} },
    columns: (__VLS_ctx.columns),
    items: (__VLS_ctx.facilities),
    rowKey: "id",
}, ...__VLS_functionalComponentArgsRest(__VLS_1));
let __VLS_4;
let __VLS_5;
let __VLS_6;
const __VLS_7 = {
    onRowClick: (__VLS_ctx.onRowClick)
};
var __VLS_3;
/** @type {__VLS_StyleScopedClasses['page-card']} */ ;
/** @type {__VLS_StyleScopedClasses['app-card']} */ ;
var __VLS_dollars;
const __VLS_self = (await import('vue')).defineComponent({
    setup() {
        return {
            facilities: facilities,
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
