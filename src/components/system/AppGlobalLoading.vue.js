/// <reference types="../../../node_modules/.vue-global-types/vue_3.5_0_0_0.d.ts" />
import ProgressBar from 'primevue/progressbar';
import { useGlobalLoading } from '@/composables/use-global-loading';
const { isLoading } = useGlobalLoading();
debugger; /* PartiallyEnd: #3632/scriptSetup.vue */
const __VLS_ctx = {};
let __VLS_components;
let __VLS_directives;
// CSS variable injection 
// CSS variable injection end 
if (__VLS_ctx.isLoading) {
    __VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({
        ...{ class: "app-global-loading app-card" },
    });
    const __VLS_0 = {}.ProgressBar;
    /** @type {[typeof __VLS_components.ProgressBar, ]} */ ;
    // @ts-ignore
    const __VLS_1 = __VLS_asFunctionalComponent(__VLS_0, new __VLS_0({
        mode: "indeterminate",
        ...{ style: {} },
    }));
    const __VLS_2 = __VLS_1({
        mode: "indeterminate",
        ...{ style: {} },
    }, ...__VLS_functionalComponentArgsRest(__VLS_1));
    __VLS_asFunctionalElement(__VLS_intrinsicElements.span, __VLS_intrinsicElements.span)({});
}
/** @type {__VLS_StyleScopedClasses['app-global-loading']} */ ;
/** @type {__VLS_StyleScopedClasses['app-card']} */ ;
var __VLS_dollars;
const __VLS_self = (await import('vue')).defineComponent({
    setup() {
        return {
            ProgressBar: ProgressBar,
            isLoading: isLoading,
        };
    },
});
export default (await import('vue')).defineComponent({
    setup() {
        return {};
    },
});
; /* PartiallyEnd: #4569/main.vue */
