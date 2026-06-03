/// <reference types="../../../node_modules/.vue-global-types/vue_3.5_0_0_0.d.ts" />
const props = defineProps({
    variant: { type: String, default: 'primary' },
    size: { type: String, default: 'md' },
    disabled: { type: Boolean, default: false },
    type: { type: String, default: 'button' },
});
const emit = defineEmits(['click']);
import { computed } from 'vue';
const classes = computed(() => [
    'ui-button',
    `ui-button--${props.variant}`,
    `ui-button--${props.size}`,
    { 'is-disabled': props.disabled },
]);
debugger; /* PartiallyEnd: #3632/scriptSetup.vue */
const __VLS_ctx = {};
let __VLS_components;
let __VLS_directives;
/** @type {__VLS_StyleScopedClasses['ui-button']} */ ;
/** @type {__VLS_StyleScopedClasses['ui-button']} */ ;
/** @type {__VLS_StyleScopedClasses['ui-button--primary']} */ ;
/** @type {__VLS_StyleScopedClasses['is-disabled']} */ ;
/** @type {__VLS_StyleScopedClasses['ui-button--secondary']} */ ;
/** @type {__VLS_StyleScopedClasses['is-disabled']} */ ;
/** @type {__VLS_StyleScopedClasses['ui-button--ghost']} */ ;
/** @type {__VLS_StyleScopedClasses['is-disabled']} */ ;
/** @type {__VLS_StyleScopedClasses['ui-button']} */ ;
// CSS variable injection 
// CSS variable injection end 
__VLS_asFunctionalElement(__VLS_intrinsicElements.button, __VLS_intrinsicElements.button)({
    ...{ onClick: (...[$event]) => {
            __VLS_ctx.$emit('click', $event);
        } },
    ...{ class: (__VLS_ctx.classes) },
    type: (__VLS_ctx.type),
    disabled: (__VLS_ctx.disabled),
});
var __VLS_0 = {};
// @ts-ignore
var __VLS_1 = __VLS_0;
var __VLS_dollars;
const __VLS_self = (await import('vue')).defineComponent({
    setup() {
        return {
            classes: classes,
        };
    },
    emits: {},
    props: {
        variant: { type: String, default: 'primary' },
        size: { type: String, default: 'md' },
        disabled: { type: Boolean, default: false },
        type: { type: String, default: 'button' },
    },
});
const __VLS_component = (await import('vue')).defineComponent({
    setup() {
        return {};
    },
    emits: {},
    props: {
        variant: { type: String, default: 'primary' },
        size: { type: String, default: 'md' },
        disabled: { type: Boolean, default: false },
        type: { type: String, default: 'button' },
    },
});
export default {};
; /* PartiallyEnd: #4569/main.vue */
