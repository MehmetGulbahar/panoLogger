/// <reference types="../../../node_modules/.vue-global-types/vue_3.5_0_0_0.d.ts" />
import { onMounted, onBeforeUnmount, ref } from 'vue';
const props = defineProps({
    title: { type: String, default: '' },
    closeOnEsc: { type: Boolean, default: true },
    closeOnBackdrop: { type: Boolean, default: true },
});
const emit = defineEmits(['close']);
const titleId = `modal-title-${Math.random().toString(36).slice(2, 8)}`;
const dialog = ref(null);
function close() { emit('close'); }
function handleKey(e) {
    if (e.key === 'Escape' && props.closeOnEsc)
        close();
}
function handleBackdrop() {
    if (props.closeOnBackdrop)
        close();
}
onMounted(() => {
    document.addEventListener('keydown', handleKey);
    // focus dialog
    setTimeout(() => dialog.value?.focus(), 0);
});
onBeforeUnmount(() => document.removeEventListener('keydown', handleKey));
debugger; /* PartiallyEnd: #3632/scriptSetup.vue */
const __VLS_ctx = {};
let __VLS_components;
let __VLS_directives;
/** @type {__VLS_StyleScopedClasses['ui-modal__header']} */ ;
/** @type {__VLS_StyleScopedClasses['ui-modal__close']} */ ;
/** @type {__VLS_StyleScopedClasses['ui-modal-fade-enter-from']} */ ;
/** @type {__VLS_StyleScopedClasses['ui-modal']} */ ;
/** @type {__VLS_StyleScopedClasses['ui-modal-fade-leave-to']} */ ;
/** @type {__VLS_StyleScopedClasses['ui-modal']} */ ;
/** @type {__VLS_StyleScopedClasses['ui-modal-fade-enter-to']} */ ;
/** @type {__VLS_StyleScopedClasses['ui-modal']} */ ;
/** @type {__VLS_StyleScopedClasses['ui-modal-fade-leave-from']} */ ;
/** @type {__VLS_StyleScopedClasses['ui-modal']} */ ;
// CSS variable injection 
// CSS variable injection end 
const __VLS_0 = {}.transition;
/** @type {[typeof __VLS_components.Transition, typeof __VLS_components.transition, typeof __VLS_components.Transition, typeof __VLS_components.transition, ]} */ ;
// @ts-ignore
const __VLS_1 = __VLS_asFunctionalComponent(__VLS_0, new __VLS_0({
    name: "ui-modal-fade",
    appear: true,
}));
const __VLS_2 = __VLS_1({
    name: "ui-modal-fade",
    appear: true,
}, ...__VLS_functionalComponentArgsRest(__VLS_1));
__VLS_3.slots.default;
__VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({
    ...{ onClick: (__VLS_ctx.handleBackdrop) },
    ...{ class: "ui-modal__overlay" },
    role: "presentation",
});
__VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({
    ...{ class: "ui-modal" },
    role: "dialog",
    'aria-labelledby': (__VLS_ctx.title ? __VLS_ctx.titleId : undefined),
    'aria-modal': "true",
    ref: "dialog",
    tabindex: "-1",
});
/** @type {typeof __VLS_ctx.dialog} */ ;
__VLS_asFunctionalElement(__VLS_intrinsicElements.header, __VLS_intrinsicElements.header)({
    ...{ class: "ui-modal__header" },
});
if (__VLS_ctx.title) {
    __VLS_asFunctionalElement(__VLS_intrinsicElements.h3, __VLS_intrinsicElements.h3)({
        id: (__VLS_ctx.titleId),
    });
    (__VLS_ctx.title);
}
__VLS_asFunctionalElement(__VLS_intrinsicElements.button, __VLS_intrinsicElements.button)({
    ...{ onClick: (__VLS_ctx.close) },
    ...{ class: "ui-modal__close" },
    'aria-label': "Kapat",
});
__VLS_asFunctionalElement(__VLS_intrinsicElements.i, __VLS_intrinsicElements.i)({
    ...{ class: "pi pi-times" },
    'aria-hidden': "true",
});
__VLS_asFunctionalElement(__VLS_intrinsicElements.section, __VLS_intrinsicElements.section)({
    ...{ class: "ui-modal__body" },
});
var __VLS_4 = {};
__VLS_asFunctionalElement(__VLS_intrinsicElements.footer, __VLS_intrinsicElements.footer)({
    ...{ class: "ui-modal__footer" },
});
var __VLS_6 = {};
var __VLS_3;
/** @type {__VLS_StyleScopedClasses['ui-modal__overlay']} */ ;
/** @type {__VLS_StyleScopedClasses['ui-modal']} */ ;
/** @type {__VLS_StyleScopedClasses['ui-modal__header']} */ ;
/** @type {__VLS_StyleScopedClasses['ui-modal__close']} */ ;
/** @type {__VLS_StyleScopedClasses['pi']} */ ;
/** @type {__VLS_StyleScopedClasses['pi-times']} */ ;
/** @type {__VLS_StyleScopedClasses['ui-modal__body']} */ ;
/** @type {__VLS_StyleScopedClasses['ui-modal__footer']} */ ;
// @ts-ignore
var __VLS_5 = __VLS_4, __VLS_7 = __VLS_6;
var __VLS_dollars;
const __VLS_self = (await import('vue')).defineComponent({
    setup() {
        return {
            titleId: titleId,
            dialog: dialog,
            close: close,
            handleBackdrop: handleBackdrop,
        };
    },
    emits: {},
    props: {
        title: { type: String, default: '' },
        closeOnEsc: { type: Boolean, default: true },
        closeOnBackdrop: { type: Boolean, default: true },
    },
});
const __VLS_component = (await import('vue')).defineComponent({
    setup() {
        return {};
    },
    emits: {},
    props: {
        title: { type: String, default: '' },
        closeOnEsc: { type: Boolean, default: true },
        closeOnBackdrop: { type: Boolean, default: true },
    },
});
export default {};
; /* PartiallyEnd: #4569/main.vue */
