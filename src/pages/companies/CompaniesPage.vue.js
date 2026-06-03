/// <reference types="../../../node_modules/.vue-global-types/vue_3.5_0_0_0.d.ts" />
import { mockCompanies } from '@/mocks/mock-data';
import { ref } from 'vue';
import { useModalStore } from '@/stores/modal-store';
import SampleModal from '@/components/samples/SampleModal.vue';
const companies = ref(mockCompanies);
import { useRouter } from 'vue-router';
const columns = [
    { key: 'projectName', label: 'Proje' },
    { key: 'name', label: 'Şirket' },
    { key: 'contact', label: 'İletişim' },
    { key: 'taxNumber', label: 'Vergi No' },
    { key: 'address', label: 'Adres' },
];
const router = useRouter();
function onRowClick(item) {
    router.push({ path: `/companies/${item.id}` });
}
function openSample() {
    const modals = useModalStore();
    modals.open(SampleModal, {}, {});
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
__VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({
    ...{ class: "page-head" },
});
__VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({});
__VLS_asFunctionalElement(__VLS_intrinsicElements.h1, __VLS_intrinsicElements.h1)({});
__VLS_asFunctionalElement(__VLS_intrinsicElements.p, __VLS_intrinsicElements.p)({});
__VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({
    ...{ class: "page-actions" },
});
const __VLS_0 = {}.UiButton;
/** @type {[typeof __VLS_components.UiButton, typeof __VLS_components.UiButton, ]} */ ;
// @ts-ignore
const __VLS_1 = __VLS_asFunctionalComponent(__VLS_0, new __VLS_0({
    ...{ 'onClick': {} },
    variant: "primary",
}));
const __VLS_2 = __VLS_1({
    ...{ 'onClick': {} },
    variant: "primary",
}, ...__VLS_functionalComponentArgsRest(__VLS_1));
let __VLS_4;
let __VLS_5;
let __VLS_6;
const __VLS_7 = {
    onClick: (__VLS_ctx.openSample)
};
__VLS_3.slots.default;
var __VLS_3;
const __VLS_8 = {}.UiTable;
/** @type {[typeof __VLS_components.UiTable, ]} */ ;
// @ts-ignore
const __VLS_9 = __VLS_asFunctionalComponent(__VLS_8, new __VLS_8({
    ...{ 'onRowClick': {} },
    columns: (__VLS_ctx.columns),
    items: (__VLS_ctx.companies),
    rowKey: "id",
}));
const __VLS_10 = __VLS_9({
    ...{ 'onRowClick': {} },
    columns: (__VLS_ctx.columns),
    items: (__VLS_ctx.companies),
    rowKey: "id",
}, ...__VLS_functionalComponentArgsRest(__VLS_9));
let __VLS_12;
let __VLS_13;
let __VLS_14;
const __VLS_15 = {
    onRowClick: (__VLS_ctx.onRowClick)
};
var __VLS_11;
/** @type {__VLS_StyleScopedClasses['page-card']} */ ;
/** @type {__VLS_StyleScopedClasses['app-card']} */ ;
/** @type {__VLS_StyleScopedClasses['page-head']} */ ;
/** @type {__VLS_StyleScopedClasses['page-actions']} */ ;
var __VLS_dollars;
const __VLS_self = (await import('vue')).defineComponent({
    setup() {
        return {
            companies: companies,
            columns: columns,
            onRowClick: onRowClick,
            openSample: openSample,
        };
    },
});
export default (await import('vue')).defineComponent({
    setup() {
        return {};
    },
});
; /* PartiallyEnd: #4569/main.vue */
