/// <reference types="../../../node_modules/.vue-global-types/vue_3.5_0_0_0.d.ts" />
import { computed, useSlots } from 'vue';
import Card from './Card.vue';
const slots = useSlots();
const props = defineProps();
const accentColor = computed(() => props.accentColor ?? 'var(--app-primary)');
const meta = computed(() => props.meta ?? []);
const hasActions = computed(() => !!slots.actions);
debugger; /* PartiallyEnd: #3632/scriptSetup.vue */
const __VLS_ctx = {};
let __VLS_components;
let __VLS_directives;
/** @type {__VLS_StyleScopedClasses['info-card']} */ ;
/** @type {__VLS_StyleScopedClasses['info-card__title']} */ ;
// CSS variable injection 
// CSS variable injection end 
/** @type {[typeof Card, typeof Card, ]} */ ;
// @ts-ignore
const __VLS_0 = __VLS_asFunctionalComponent(Card, new Card({
    ...{ class: "info-card" },
    padding: ('md'),
}));
const __VLS_1 = __VLS_0({
    ...{ class: "info-card" },
    padding: ('md'),
}, ...__VLS_functionalComponentArgsRest(__VLS_0));
var __VLS_3 = {};
__VLS_2.slots.default;
__VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({
    ...{ class: "info-card__accent" },
    ...{ style: ({ background: __VLS_ctx.accentColor }) },
    'aria-hidden': "true",
});
__VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({
    ...{ class: "info-card__body" },
});
__VLS_asFunctionalElement(__VLS_intrinsicElements.header, __VLS_intrinsicElements.header)({
    ...{ class: "info-card__header" },
});
__VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({
    ...{ class: "info-card__title" },
});
(__VLS_ctx.title);
if (__VLS_ctx.subtitle) {
    __VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({
        ...{ class: "info-card__subtitle" },
    });
    (__VLS_ctx.subtitle);
}
__VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({
    ...{ class: "info-card__meta" },
});
var __VLS_4 = {};
for (const [m, i] of __VLS_getVForSourceType((__VLS_ctx.meta))) {
    __VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({
        ...{ class: "info-card__meta-item" },
    });
    (m);
}
if (__VLS_ctx.description) {
    __VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({
        ...{ class: "info-card__desc" },
    });
    (__VLS_ctx.description);
}
if (__VLS_ctx.hasActions) {
    __VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({
        ...{ class: "info-card__actions" },
    });
    var __VLS_6 = {};
}
var __VLS_2;
/** @type {__VLS_StyleScopedClasses['info-card']} */ ;
/** @type {__VLS_StyleScopedClasses['info-card__accent']} */ ;
/** @type {__VLS_StyleScopedClasses['info-card__body']} */ ;
/** @type {__VLS_StyleScopedClasses['info-card__header']} */ ;
/** @type {__VLS_StyleScopedClasses['info-card__title']} */ ;
/** @type {__VLS_StyleScopedClasses['info-card__subtitle']} */ ;
/** @type {__VLS_StyleScopedClasses['info-card__meta']} */ ;
/** @type {__VLS_StyleScopedClasses['info-card__meta-item']} */ ;
/** @type {__VLS_StyleScopedClasses['info-card__desc']} */ ;
/** @type {__VLS_StyleScopedClasses['info-card__actions']} */ ;
// @ts-ignore
var __VLS_5 = __VLS_4, __VLS_7 = __VLS_6;
var __VLS_dollars;
const __VLS_self = (await import('vue')).defineComponent({
    setup() {
        return {
            Card: Card,
            accentColor: accentColor,
            meta: meta,
            hasActions: hasActions,
        };
    },
    __typeProps: {},
});
const __VLS_component = (await import('vue')).defineComponent({
    setup() {
        return {};
    },
    __typeProps: {},
});
export default {};
; /* PartiallyEnd: #4569/main.vue */
