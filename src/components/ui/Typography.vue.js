/// <reference types="../../../node_modules/.vue-global-types/vue_3.5_0_0_0.d.ts" />
import { computed } from 'vue';
const props = defineProps({
    variant: { type: String, default: 'p' },
    className: { type: String, default: '' },
});
const tag = computed(() => props.variant);
const classes = computed(() => {
    const base = props.variant.startsWith('h') ? `h-${props.variant.slice(1)}` : props.variant;
    return [base, props.className].filter(Boolean).join(' ');
});
debugger; /* PartiallyEnd: #3632/scriptSetup.vue */
const __VLS_ctx = {};
let __VLS_components;
let __VLS_directives;
// CSS variable injection 
// CSS variable injection end 
const __VLS_0 = ((__VLS_ctx.tag));
// @ts-ignore
const __VLS_1 = __VLS_asFunctionalComponent(__VLS_0, new __VLS_0({
    ...{ class: (__VLS_ctx.classes) },
}));
const __VLS_2 = __VLS_1({
    ...{ class: (__VLS_ctx.classes) },
}, ...__VLS_functionalComponentArgsRest(__VLS_1));
var __VLS_4 = {};
__VLS_3.slots.default;
var __VLS_5 = {};
var __VLS_3;
// @ts-ignore
var __VLS_6 = __VLS_5;
var __VLS_dollars;
const __VLS_self = (await import('vue')).defineComponent({
    setup() {
        return {
            tag: tag,
            classes: classes,
        };
    },
    props: {
        variant: { type: String, default: 'p' },
        className: { type: String, default: '' },
    },
});
const __VLS_component = (await import('vue')).defineComponent({
    setup() {
        return {};
    },
    props: {
        variant: { type: String, default: 'p' },
        className: { type: String, default: '' },
    },
});
export default {};
; /* PartiallyEnd: #4569/main.vue */
