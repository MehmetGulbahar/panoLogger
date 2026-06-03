/// <reference types="../../../node_modules/.vue-global-types/vue_3.5_0_0_0.d.ts" />
const props = defineProps({
    flat: { type: Boolean, default: false },
    padding: { type: String, default: 'md' },
    className: { type: String, default: '' },
});
import { computed } from 'vue';
const classes = computed(() => [
    'ui-card',
    props.flat ? 'ui-card--flat' : 'ui-card--elevated',
    `ui-card--padding-${props.padding}`,
    props.className,
]);
debugger; /* PartiallyEnd: #3632/scriptSetup.vue */
const __VLS_ctx = {};
let __VLS_components;
let __VLS_directives;
// CSS variable injection 
// CSS variable injection end 
__VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({
    ...{ class: (__VLS_ctx.classes) },
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
    props: {
        flat: { type: Boolean, default: false },
        padding: { type: String, default: 'md' },
        className: { type: String, default: '' },
    },
});
const __VLS_component = (await import('vue')).defineComponent({
    setup() {
        return {};
    },
    props: {
        flat: { type: Boolean, default: false },
        padding: { type: String, default: 'md' },
        className: { type: String, default: '' },
    },
});
export default {};
; /* PartiallyEnd: #4569/main.vue */
