/// <reference types="../../../node_modules/.vue-global-types/vue_3.5_0_0_0.d.ts" />
const props = defineProps({ at: { type: String, default: 'sm' } });
const hiddenClass = computed(() => {
    if (props.at === 'sm')
        return 'hidden-sm';
    if (props.at === 'md')
        return 'hidden-md';
    return 'hidden-lg';
});
debugger; /* PartiallyEnd: #3632/scriptSetup.vue */
const __VLS_ctx = {};
let __VLS_components;
let __VLS_directives;
/** @type {__VLS_StyleScopedClasses['hidden-sm']} */ ;
/** @type {__VLS_StyleScopedClasses['hidden-md']} */ ;
/** @type {__VLS_StyleScopedClasses['hidden-lg']} */ ;
// CSS variable injection 
// CSS variable injection end 
__VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({
    ...{ class: (__VLS_ctx.hiddenClass) },
});
var __VLS_0 = {};
// @ts-ignore
var __VLS_1 = __VLS_0;
var __VLS_dollars;
const __VLS_self = (await import('vue')).defineComponent({
    setup() {
        return {
            hiddenClass: hiddenClass,
        };
    },
    props: { at: { type: String, default: 'sm' } },
});
const __VLS_component = (await import('vue')).defineComponent({
    setup() {
        return {};
    },
    props: { at: { type: String, default: 'sm' } },
});
export default {};
; /* PartiallyEnd: #4569/main.vue */
