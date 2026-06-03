/// <reference types="../../../node_modules/.vue-global-types/vue_3.5_0_0_0.d.ts" />
import { computed } from 'vue';
import { useModalStore } from '@/stores/modal-store';
const store = useModalStore();
const modals = computed(() => store.modals);
function close(id) { store.close(id); }
debugger; /* PartiallyEnd: #3632/scriptSetup.vue */
const __VLS_ctx = {};
let __VLS_components;
let __VLS_directives;
// CSS variable injection 
// CSS variable injection end 
__VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({});
for (const [m] of __VLS_getVForSourceType((__VLS_ctx.modals))) {
    const __VLS_0 = {}.teleport;
    /** @type {[typeof __VLS_components.Teleport, typeof __VLS_components.teleport, typeof __VLS_components.Teleport, typeof __VLS_components.teleport, ]} */ ;
    // @ts-ignore
    const __VLS_1 = __VLS_asFunctionalComponent(__VLS_0, new __VLS_0({
        key: (m.id),
        to: "body",
    }));
    const __VLS_2 = __VLS_1({
        key: (m.id),
        to: "body",
    }, ...__VLS_functionalComponentArgsRest(__VLS_1));
    __VLS_3.slots.default;
    const __VLS_4 = ((m.component));
    // @ts-ignore
    const __VLS_5 = __VLS_asFunctionalComponent(__VLS_4, new __VLS_4({
        ...{ 'onClose': {} },
        ...(m.props),
    }));
    const __VLS_6 = __VLS_5({
        ...{ 'onClose': {} },
        ...(m.props),
    }, ...__VLS_functionalComponentArgsRest(__VLS_5));
    let __VLS_8;
    let __VLS_9;
    let __VLS_10;
    const __VLS_11 = {
        onClose: (...[$event]) => {
            __VLS_ctx.close(m.id);
        }
    };
    __VLS_7.slots.default;
    for (const [s, name] of __VLS_getVForSourceType((m.slots))) {
        {
            const { [__VLS_tryAsConstant(name)]: __VLS_thisSlot } = __VLS_7.slots;
            const __VLS_12 = ((s));
            // @ts-ignore
            const __VLS_13 = __VLS_asFunctionalComponent(__VLS_12, new __VLS_12({}));
            const __VLS_14 = __VLS_13({}, ...__VLS_functionalComponentArgsRest(__VLS_13));
        }
    }
    var __VLS_7;
    var __VLS_3;
}
var __VLS_dollars;
const __VLS_self = (await import('vue')).defineComponent({
    setup() {
        return {
            modals: modals,
            close: close,
        };
    },
});
export default (await import('vue')).defineComponent({
    setup() {
        return {};
    },
});
; /* PartiallyEnd: #4569/main.vue */
