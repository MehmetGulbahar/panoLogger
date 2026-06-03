/// <reference types="../../../node_modules/.vue-global-types/vue_3.5_0_0_0.d.ts" />
const props = defineProps({ title: { type: String, default: 'Boş' }, description: { type: String, default: '' } });
debugger; /* PartiallyEnd: #3632/scriptSetup.vue */
const __VLS_ctx = {};
let __VLS_components;
let __VLS_directives;
// CSS variable injection 
// CSS variable injection end 
__VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({
    ...{ class: "ui-empty" },
});
if (__VLS_ctx.$slots.icon) {
    __VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({
        ...{ class: "ui-empty__icon" },
    });
    var __VLS_0 = {};
}
__VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({
    ...{ class: "ui-empty__title" },
});
(__VLS_ctx.title);
if (__VLS_ctx.description) {
    __VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({
        ...{ class: "ui-empty__desc" },
    });
    (__VLS_ctx.description);
}
/** @type {__VLS_StyleScopedClasses['ui-empty']} */ ;
/** @type {__VLS_StyleScopedClasses['ui-empty__icon']} */ ;
/** @type {__VLS_StyleScopedClasses['ui-empty__title']} */ ;
/** @type {__VLS_StyleScopedClasses['ui-empty__desc']} */ ;
// @ts-ignore
var __VLS_1 = __VLS_0;
var __VLS_dollars;
const __VLS_self = (await import('vue')).defineComponent({
    setup() {
        return {};
    },
    props: { title: { type: String, default: 'Boş' }, description: { type: String, default: '' } },
});
const __VLS_component = (await import('vue')).defineComponent({
    setup() {
        return {};
    },
    props: { title: { type: String, default: 'Boş' }, description: { type: String, default: '' } },
});
export default {};
; /* PartiallyEnd: #4569/main.vue */
