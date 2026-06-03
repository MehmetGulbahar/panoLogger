/// <reference types="../../../node_modules/.vue-global-types/vue_3.5_0_0_0.d.ts" />
import Message from 'primevue/message';
import { storeToRefs } from 'pinia';
import { useUiStore } from '@/stores/ui-store';
const uiStore = useUiStore();
const { globalError } = storeToRefs(uiStore);
const { resetError } = uiStore;
debugger; /* PartiallyEnd: #3632/scriptSetup.vue */
const __VLS_ctx = {};
let __VLS_components;
let __VLS_directives;
if (__VLS_ctx.globalError) {
    const __VLS_0 = {}.Message;
    /** @type {[typeof __VLS_components.Message, typeof __VLS_components.Message, ]} */ ;
    // @ts-ignore
    const __VLS_1 = __VLS_asFunctionalComponent(__VLS_0, new __VLS_0({
        ...{ 'onClose': {} },
        severity: "error",
        closable: (true),
    }));
    const __VLS_2 = __VLS_1({
        ...{ 'onClose': {} },
        severity: "error",
        closable: (true),
    }, ...__VLS_functionalComponentArgsRest(__VLS_1));
    let __VLS_4;
    let __VLS_5;
    let __VLS_6;
    const __VLS_7 = {
        onClose: (__VLS_ctx.resetError)
    };
    var __VLS_8 = {};
    __VLS_3.slots.default;
    (__VLS_ctx.globalError);
    var __VLS_3;
}
var __VLS_dollars;
const __VLS_self = (await import('vue')).defineComponent({
    setup() {
        return {
            Message: Message,
            globalError: globalError,
            resetError: resetError,
        };
    },
});
export default (await import('vue')).defineComponent({
    setup() {
        return {};
    },
});
; /* PartiallyEnd: #4569/main.vue */
