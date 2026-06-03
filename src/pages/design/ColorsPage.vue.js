/// <reference types="../../../node_modules/.vue-global-types/vue_3.5_0_0_0.d.ts" />
const groups = [
    {
        name: 'primary',
        label: 'Primary',
        shades: ['--primary-50', '--primary-100', '--primary-200', '--primary-300', '--primary-400', '--primary-500', '--primary-600', '--primary-700', '--primary-800', '--primary-900']
    },
    {
        name: 'neutral',
        label: 'Neutral',
        shades: ['--neutral-50', '--neutral-100', '--neutral-200', '--neutral-300', '--neutral-400', '--neutral-500', '--neutral-600', '--neutral-700', '--neutral-800', '--neutral-900']
    },
    {
        name: 'success',
        label: 'Success',
        shades: ['--success-50', '--success-100', '--success-200', '--success-300', '--success-400', '--success-500', '--success-600', '--success-700', '--success-800', '--success-900']
    },
    {
        name: 'warning',
        label: 'Warning',
        shades: ['--warning-50', '--warning-100', '--warning-200', '--warning-300', '--warning-400', '--warning-500', '--warning-600', '--warning-700', '--warning-800', '--warning-900']
    },
    {
        name: 'danger',
        label: 'Danger',
        shades: ['--danger-50', '--danger-100', '--danger-200', '--danger-300', '--danger-400', '--danger-500', '--danger-600', '--danger-700', '--danger-800', '--danger-900']
    }
];
function getHex(varName) {
    try {
        const v = getComputedStyle(document.documentElement).getPropertyValue(varName).trim();
        return v || '—';
    }
    catch (e) {
        return '—';
    }
}
debugger; /* PartiallyEnd: #3632/scriptSetup.vue */
const __VLS_ctx = {};
let __VLS_components;
let __VLS_directives;
// CSS variable injection 
// CSS variable injection end 
__VLS_asFunctionalElement(__VLS_intrinsicElements.section, __VLS_intrinsicElements.section)({
    ...{ class: "page-card app-card" },
});
__VLS_asFunctionalElement(__VLS_intrinsicElements.h1, __VLS_intrinsicElements.h1)({});
__VLS_asFunctionalElement(__VLS_intrinsicElements.p, __VLS_intrinsicElements.p)({
    ...{ class: "text-muted" },
});
__VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({
    ...{ class: "palette-grid" },
});
for (const [group] of __VLS_getVForSourceType((__VLS_ctx.groups))) {
    __VLS_asFunctionalElement(__VLS_intrinsicElements.section, __VLS_intrinsicElements.section)({
        key: (group.name),
        ...{ class: "palette-group" },
    });
    __VLS_asFunctionalElement(__VLS_intrinsicElements.h3, __VLS_intrinsicElements.h3)({
        ...{ class: "mb-1" },
    });
    (group.label);
    __VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({
        ...{ class: "palette-row" },
    });
    for (const [shade] of __VLS_getVForSourceType((group.shades))) {
        __VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({
            key: (shade),
            ...{ class: "swatch" },
        });
        __VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({
            ...{ class: "swatch__color" },
            ...{ style: ({ background: `var(${shade})` }) },
        });
        __VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({
            ...{ class: "swatch__meta" },
        });
        __VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({
            ...{ class: "swatch__name" },
        });
        (shade.replace('--', ''));
        __VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({
            ...{ class: "swatch__hex" },
        });
        (__VLS_ctx.getHex(shade));
    }
}
/** @type {__VLS_StyleScopedClasses['page-card']} */ ;
/** @type {__VLS_StyleScopedClasses['app-card']} */ ;
/** @type {__VLS_StyleScopedClasses['text-muted']} */ ;
/** @type {__VLS_StyleScopedClasses['palette-grid']} */ ;
/** @type {__VLS_StyleScopedClasses['palette-group']} */ ;
/** @type {__VLS_StyleScopedClasses['mb-1']} */ ;
/** @type {__VLS_StyleScopedClasses['palette-row']} */ ;
/** @type {__VLS_StyleScopedClasses['swatch']} */ ;
/** @type {__VLS_StyleScopedClasses['swatch__color']} */ ;
/** @type {__VLS_StyleScopedClasses['swatch__meta']} */ ;
/** @type {__VLS_StyleScopedClasses['swatch__name']} */ ;
/** @type {__VLS_StyleScopedClasses['swatch__hex']} */ ;
var __VLS_dollars;
const __VLS_self = (await import('vue')).defineComponent({
    setup() {
        return {
            groups: groups,
            getHex: getHex,
        };
    },
});
export default (await import('vue')).defineComponent({
    setup() {
        return {};
    },
});
; /* PartiallyEnd: #4569/main.vue */
