/// <reference types="../../../node_modules/.vue-global-types/vue_3.5_0_0_0.d.ts" />
import { computed } from 'vue';
import { useRoute } from 'vue-router';
import mock from '@/mocks/mock-data';
import { useFileStore } from '@/stores/file-store';
const route = useRoute();
const fileStore = useFileStore();
const panel = computed(() => {
    const panelCode = String(route.params.panelCode ?? '');
    return mock.panels.find((item) => item.code === panelCode) ?? mock.panels[0];
});
const facility = computed(() => mock.facilities.find((item) => item.id === panel.value.facilityId) ?? mock.facilities[0]);
const company = computed(() => mock.companies.find((item) => item.id === facility.value.companyId) ?? mock.companies[0]);
const uploadedFileCount = computed(() => fileStore.getPanelFileCount(panel.value.id));
const documentCards = computed(() => [
    {
        key: 'electrical-project',
        title: 'Elektrik Projesi',
        description: 'Tek hat ve proje dosyaları',
        icon: 'pi pi-sitemap',
        count: 1,
    },
    {
        key: 'maintenance-report',
        title: 'Bakım Raporu',
        description: 'Periyodik bakım kayıtları',
        icon: 'pi pi-wrench',
        count: 0,
    },
    {
        key: 'panel-documents',
        title: 'Pano Dokümanları',
        description: 'Yüklenen teknik dosyalar',
        icon: 'pi pi-file',
        count: uploadedFileCount.value,
    },
]);
function downloadDocument(card) {
    const fileContent = [
        card.title,
        `Pano: ${panel.value.name}`,
        `Kod: ${panel.value.code}`,
        `Proje: ${company.value.projectName}`,
    ].join('\n');
    const blob = new Blob([fileContent], { type: 'text/plain;charset=utf-8' });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = `${panel.value.code}-${card.key}.txt`;
    link.click();
    URL.revokeObjectURL(url);
}
debugger; /* PartiallyEnd: #3632/scriptSetup.vue */
const __VLS_ctx = {};
let __VLS_components;
let __VLS_directives;
/** @type {__VLS_StyleScopedClasses['public-header__mark']} */ ;
/** @type {__VLS_StyleScopedClasses['public-header']} */ ;
/** @type {__VLS_StyleScopedClasses['public-header']} */ ;
/** @type {__VLS_StyleScopedClasses['panel-summary']} */ ;
/** @type {__VLS_StyleScopedClasses['panel-summary']} */ ;
/** @type {__VLS_StyleScopedClasses['panel-summary']} */ ;
/** @type {__VLS_StyleScopedClasses['panel-summary']} */ ;
/** @type {__VLS_StyleScopedClasses['document-card']} */ ;
/** @type {__VLS_StyleScopedClasses['document-card__icon']} */ ;
/** @type {__VLS_StyleScopedClasses['pi']} */ ;
/** @type {__VLS_StyleScopedClasses['document-card']} */ ;
/** @type {__VLS_StyleScopedClasses['document-card']} */ ;
/** @type {__VLS_StyleScopedClasses['download-button']} */ ;
/** @type {__VLS_StyleScopedClasses['download-button']} */ ;
/** @type {__VLS_StyleScopedClasses['pi']} */ ;
/** @type {__VLS_StyleScopedClasses['public-footer']} */ ;
/** @type {__VLS_StyleScopedClasses['public-panel-page']} */ ;
/** @type {__VLS_StyleScopedClasses['panel-summary']} */ ;
// CSS variable injection 
// CSS variable injection end 
__VLS_asFunctionalElement(__VLS_intrinsicElements.main, __VLS_intrinsicElements.main)({
    ...{ class: "public-panel-page" },
});
__VLS_asFunctionalElement(__VLS_intrinsicElements.section, __VLS_intrinsicElements.section)({
    ...{ class: "public-shell" },
});
__VLS_asFunctionalElement(__VLS_intrinsicElements.header, __VLS_intrinsicElements.header)({
    ...{ class: "public-header" },
});
__VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({
    ...{ class: "public-header__mark" },
});
__VLS_asFunctionalElement(__VLS_intrinsicElements.i, __VLS_intrinsicElements.i)({
    ...{ class: "pi pi-bolt" },
    'aria-hidden': "true",
});
__VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({});
__VLS_asFunctionalElement(__VLS_intrinsicElements.p, __VLS_intrinsicElements.p)({
    ...{ class: "public-eyebrow" },
});
__VLS_asFunctionalElement(__VLS_intrinsicElements.h1, __VLS_intrinsicElements.h1)({});
(__VLS_ctx.panel.name);
__VLS_asFunctionalElement(__VLS_intrinsicElements.p, __VLS_intrinsicElements.p)({});
(__VLS_ctx.panel.code);
(__VLS_ctx.facility.name);
__VLS_asFunctionalElement(__VLS_intrinsicElements.section, __VLS_intrinsicElements.section)({
    ...{ class: "panel-summary" },
});
__VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({});
__VLS_asFunctionalElement(__VLS_intrinsicElements.span, __VLS_intrinsicElements.span)({});
__VLS_asFunctionalElement(__VLS_intrinsicElements.strong, __VLS_intrinsicElements.strong)({});
(__VLS_ctx.company.projectName);
__VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({});
__VLS_asFunctionalElement(__VLS_intrinsicElements.span, __VLS_intrinsicElements.span)({});
__VLS_asFunctionalElement(__VLS_intrinsicElements.strong, __VLS_intrinsicElements.strong)({});
(__VLS_ctx.facility.city);
__VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({});
__VLS_asFunctionalElement(__VLS_intrinsicElements.span, __VLS_intrinsicElements.span)({});
__VLS_asFunctionalElement(__VLS_intrinsicElements.strong, __VLS_intrinsicElements.strong)({});
(__VLS_ctx.panel.code);
__VLS_asFunctionalElement(__VLS_intrinsicElements.section, __VLS_intrinsicElements.section)({
    ...{ class: "document-list" },
    'aria-label': "Panel dosyaları",
});
for (const [card] of __VLS_getVForSourceType((__VLS_ctx.documentCards))) {
    __VLS_asFunctionalElement(__VLS_intrinsicElements.article, __VLS_intrinsicElements.article)({
        key: (card.key),
        ...{ class: "document-card" },
    });
    __VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({
        ...{ class: "document-card__icon" },
    });
    __VLS_asFunctionalElement(__VLS_intrinsicElements.i, __VLS_intrinsicElements.i)({
        ...{ class: (card.icon) },
        'aria-hidden': "true",
    });
    __VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({
        ...{ class: "document-card__content" },
    });
    __VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({
        ...{ class: "document-card__head" },
    });
    __VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({});
    __VLS_asFunctionalElement(__VLS_intrinsicElements.h2, __VLS_intrinsicElements.h2)({});
    (card.title);
    __VLS_asFunctionalElement(__VLS_intrinsicElements.p, __VLS_intrinsicElements.p)({});
    (card.description);
    __VLS_asFunctionalElement(__VLS_intrinsicElements.span, __VLS_intrinsicElements.span)({
        ...{ class: "document-card__count" },
    });
    (card.count);
    __VLS_asFunctionalElement(__VLS_intrinsicElements.button, __VLS_intrinsicElements.button)({
        ...{ onClick: (...[$event]) => {
                __VLS_ctx.downloadDocument(card);
            } },
        ...{ class: "download-button" },
        type: "button",
        disabled: (card.count === 0),
    });
    __VLS_asFunctionalElement(__VLS_intrinsicElements.i, __VLS_intrinsicElements.i)({
        ...{ class: "pi pi-download" },
        'aria-hidden': "true",
    });
}
__VLS_asFunctionalElement(__VLS_intrinsicElements.footer, __VLS_intrinsicElements.footer)({
    ...{ class: "public-footer" },
});
__VLS_asFunctionalElement(__VLS_intrinsicElements.span, __VLS_intrinsicElements.span)({});
__VLS_asFunctionalElement(__VLS_intrinsicElements.strong, __VLS_intrinsicElements.strong)({});
(__VLS_ctx.panel.code);
/** @type {__VLS_StyleScopedClasses['public-panel-page']} */ ;
/** @type {__VLS_StyleScopedClasses['public-shell']} */ ;
/** @type {__VLS_StyleScopedClasses['public-header']} */ ;
/** @type {__VLS_StyleScopedClasses['public-header__mark']} */ ;
/** @type {__VLS_StyleScopedClasses['pi']} */ ;
/** @type {__VLS_StyleScopedClasses['pi-bolt']} */ ;
/** @type {__VLS_StyleScopedClasses['public-eyebrow']} */ ;
/** @type {__VLS_StyleScopedClasses['panel-summary']} */ ;
/** @type {__VLS_StyleScopedClasses['document-list']} */ ;
/** @type {__VLS_StyleScopedClasses['document-card']} */ ;
/** @type {__VLS_StyleScopedClasses['document-card__icon']} */ ;
/** @type {__VLS_StyleScopedClasses['document-card__content']} */ ;
/** @type {__VLS_StyleScopedClasses['document-card__head']} */ ;
/** @type {__VLS_StyleScopedClasses['document-card__count']} */ ;
/** @type {__VLS_StyleScopedClasses['download-button']} */ ;
/** @type {__VLS_StyleScopedClasses['pi']} */ ;
/** @type {__VLS_StyleScopedClasses['pi-download']} */ ;
/** @type {__VLS_StyleScopedClasses['public-footer']} */ ;
var __VLS_dollars;
const __VLS_self = (await import('vue')).defineComponent({
    setup() {
        return {
            panel: panel,
            facility: facility,
            company: company,
            documentCards: documentCards,
            downloadDocument: downloadDocument,
        };
    },
});
export default (await import('vue')).defineComponent({
    setup() {
        return {};
    },
});
; /* PartiallyEnd: #4569/main.vue */
