/// <reference types="../../../node_modules/.vue-global-types/vue_3.5_0_0_0.d.ts" />
import { computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import mock from '@/mocks/mock-data';
import { routeNames } from '@/constants/routes';
import { useFileStore } from '@/stores/file-store';
const route = useRoute();
const router = useRouter();
const facilityId = route.params.facilityId;
const facility = computed(() => {
    if (facilityId) {
        return mock.facilities.find((f) => f.id === facilityId) ?? mock.facilities[0];
    }
    return mock.facilities[0];
});
const panelCount = computed(() => mock.panels.filter((p) => p.facilityId === facility.value.id).length);
const facilityPanels = computed(() => mock.panels.filter((p) => p.facilityId === facility.value.id));
// mock shops/files — use 1 and 0 as in screenshot
const shopCount = computed(() => 1);
const fileCount = computed(() => 0);
const fileStore = useFileStore();
function goToFacilities() {
    router.push({ name: 'facilities' });
}
function getPanelFileCount(panelId) {
    return fileStore.getPanelFileCount(panelId);
}
debugger; /* PartiallyEnd: #3632/scriptSetup.vue */
const __VLS_ctx = {};
let __VLS_components;
let __VLS_directives;
/** @type {__VLS_StyleScopedClasses['project-card__title']} */ ;
/** @type {__VLS_StyleScopedClasses['btn-go']} */ ;
/** @type {__VLS_StyleScopedClasses['pi']} */ ;
/** @type {__VLS_StyleScopedClasses['project-card__stats']} */ ;
/** @type {__VLS_StyleScopedClasses['project-card--facility']} */ ;
/** @type {__VLS_StyleScopedClasses['project-card--facility']} */ ;
/** @type {__VLS_StyleScopedClasses['project-card--facility']} */ ;
/** @type {__VLS_StyleScopedClasses['project-card__panels']} */ ;
/** @type {__VLS_StyleScopedClasses['panel-row']} */ ;
/** @type {__VLS_StyleScopedClasses['panel-row']} */ ;
/** @type {__VLS_StyleScopedClasses['panel-icon']} */ ;
/** @type {__VLS_StyleScopedClasses['pi']} */ ;
/** @type {__VLS_StyleScopedClasses['panel-view-icon']} */ ;
/** @type {__VLS_StyleScopedClasses['pi']} */ ;
// CSS variable injection 
// CSS variable injection end 
__VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({
    ...{ class: "page page--facility-detail" },
});
__VLS_asFunctionalElement(__VLS_intrinsicElements.header, __VLS_intrinsicElements.header)({
    ...{ class: "page__header" },
});
__VLS_asFunctionalElement(__VLS_intrinsicElements.h1, __VLS_intrinsicElements.h1)({
    ...{ class: "page__title" },
});
(__VLS_ctx.facility.name);
__VLS_asFunctionalElement(__VLS_intrinsicElements.p, __VLS_intrinsicElements.p)({
    ...{ class: "page__subtitle" },
});
(__VLS_ctx.facility.city);
(__VLS_ctx.facility.address ? ' • ' + __VLS_ctx.facility.address.split(',')[0] : '');
__VLS_asFunctionalElement(__VLS_intrinsicElements.main, __VLS_intrinsicElements.main)({});
__VLS_asFunctionalElement(__VLS_intrinsicElements.section, __VLS_intrinsicElements.section)({
    ...{ class: "project-card card project-card--facility" },
});
__VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({
    ...{ class: "project-card__header" },
});
__VLS_asFunctionalElement(__VLS_intrinsicElements.h2, __VLS_intrinsicElements.h2)({
    ...{ class: "project-card__title" },
});
__VLS_asFunctionalElement(__VLS_intrinsicElements.i, __VLS_intrinsicElements.i)({
    ...{ class: "pi pi-building" },
    'aria-hidden': "true",
});
(__VLS_ctx.facility.name);
__VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({
    ...{ class: "project-card__grid" },
});
__VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({
    ...{ class: "grid-item" },
});
__VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({
    ...{ class: "grid-label" },
});
__VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({
    ...{ class: "grid-value" },
});
(__VLS_ctx.facility.city);
__VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({
    ...{ class: "grid-item" },
});
__VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({
    ...{ class: "grid-label" },
});
__VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({
    ...{ class: "grid-value" },
});
__VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({
    ...{ class: "grid-item" },
});
__VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({
    ...{ class: "grid-label" },
});
__VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({
    ...{ class: "grid-value" },
});
__VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({
    ...{ class: "grid-item" },
});
__VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({
    ...{ class: "grid-label" },
});
__VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({
    ...{ class: "grid-value" },
});
(__VLS_ctx.panelCount);
__VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({
    ...{ class: "project-card__panels" },
});
__VLS_asFunctionalElement(__VLS_intrinsicElements.h3, __VLS_intrinsicElements.h3)({});
__VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({
    ...{ class: "panels-list" },
});
for (const [p] of __VLS_getVForSourceType((__VLS_ctx.facilityPanels))) {
    const __VLS_0 = {}.RouterLink;
    /** @type {[typeof __VLS_components.RouterLink, typeof __VLS_components.RouterLink, ]} */ ;
    // @ts-ignore
    const __VLS_1 = __VLS_asFunctionalComponent(__VLS_0, new __VLS_0({
        key: (p.id),
        to: ({ name: __VLS_ctx.routeNames.panelDetail, params: { panelId: p.id } }),
        ...{ class: "panel-row" },
    }));
    const __VLS_2 = __VLS_1({
        key: (p.id),
        to: ({ name: __VLS_ctx.routeNames.panelDetail, params: { panelId: p.id } }),
        ...{ class: "panel-row" },
    }, ...__VLS_functionalComponentArgsRest(__VLS_1));
    __VLS_3.slots.default;
    __VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({
        ...{ class: "panel-row__left" },
    });
    __VLS_asFunctionalElement(__VLS_intrinsicElements.span, __VLS_intrinsicElements.span)({
        ...{ class: "panel-icon" },
    });
    __VLS_asFunctionalElement(__VLS_intrinsicElements.i, __VLS_intrinsicElements.i)({
        ...{ class: "pi pi-th-large" },
    });
    __VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({});
    __VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({
        ...{ class: "panel-name" },
    });
    (p.name);
    __VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({
        ...{ class: "panel-meta" },
    });
    (p.description);
    __VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({
        ...{ class: "panel-row__right" },
    });
    __VLS_asFunctionalElement(__VLS_intrinsicElements.span, __VLS_intrinsicElements.span)({
        ...{ class: "badge" },
    });
    (__VLS_ctx.getPanelFileCount(p.id));
    __VLS_asFunctionalElement(__VLS_intrinsicElements.span, __VLS_intrinsicElements.span)({
        ...{ class: "panel-view-icon" },
    });
    __VLS_asFunctionalElement(__VLS_intrinsicElements.i, __VLS_intrinsicElements.i)({
        ...{ class: "pi pi-eye" },
        'aria-hidden': "true",
    });
    var __VLS_3;
}
/** @type {__VLS_StyleScopedClasses['page']} */ ;
/** @type {__VLS_StyleScopedClasses['page--facility-detail']} */ ;
/** @type {__VLS_StyleScopedClasses['page__header']} */ ;
/** @type {__VLS_StyleScopedClasses['page__title']} */ ;
/** @type {__VLS_StyleScopedClasses['page__subtitle']} */ ;
/** @type {__VLS_StyleScopedClasses['project-card']} */ ;
/** @type {__VLS_StyleScopedClasses['card']} */ ;
/** @type {__VLS_StyleScopedClasses['project-card--facility']} */ ;
/** @type {__VLS_StyleScopedClasses['project-card__header']} */ ;
/** @type {__VLS_StyleScopedClasses['project-card__title']} */ ;
/** @type {__VLS_StyleScopedClasses['pi']} */ ;
/** @type {__VLS_StyleScopedClasses['pi-building']} */ ;
/** @type {__VLS_StyleScopedClasses['project-card__grid']} */ ;
/** @type {__VLS_StyleScopedClasses['grid-item']} */ ;
/** @type {__VLS_StyleScopedClasses['grid-label']} */ ;
/** @type {__VLS_StyleScopedClasses['grid-value']} */ ;
/** @type {__VLS_StyleScopedClasses['grid-item']} */ ;
/** @type {__VLS_StyleScopedClasses['grid-label']} */ ;
/** @type {__VLS_StyleScopedClasses['grid-value']} */ ;
/** @type {__VLS_StyleScopedClasses['grid-item']} */ ;
/** @type {__VLS_StyleScopedClasses['grid-label']} */ ;
/** @type {__VLS_StyleScopedClasses['grid-value']} */ ;
/** @type {__VLS_StyleScopedClasses['grid-item']} */ ;
/** @type {__VLS_StyleScopedClasses['grid-label']} */ ;
/** @type {__VLS_StyleScopedClasses['grid-value']} */ ;
/** @type {__VLS_StyleScopedClasses['project-card__panels']} */ ;
/** @type {__VLS_StyleScopedClasses['panels-list']} */ ;
/** @type {__VLS_StyleScopedClasses['panel-row']} */ ;
/** @type {__VLS_StyleScopedClasses['panel-row__left']} */ ;
/** @type {__VLS_StyleScopedClasses['panel-icon']} */ ;
/** @type {__VLS_StyleScopedClasses['pi']} */ ;
/** @type {__VLS_StyleScopedClasses['pi-th-large']} */ ;
/** @type {__VLS_StyleScopedClasses['panel-name']} */ ;
/** @type {__VLS_StyleScopedClasses['panel-meta']} */ ;
/** @type {__VLS_StyleScopedClasses['panel-row__right']} */ ;
/** @type {__VLS_StyleScopedClasses['badge']} */ ;
/** @type {__VLS_StyleScopedClasses['panel-view-icon']} */ ;
/** @type {__VLS_StyleScopedClasses['pi']} */ ;
/** @type {__VLS_StyleScopedClasses['pi-eye']} */ ;
var __VLS_dollars;
const __VLS_self = (await import('vue')).defineComponent({
    setup() {
        return {
            routeNames: routeNames,
            facility: facility,
            panelCount: panelCount,
            facilityPanels: facilityPanels,
            getPanelFileCount: getPanelFileCount,
        };
    },
});
export default (await import('vue')).defineComponent({
    setup() {
        return {};
    },
});
; /* PartiallyEnd: #4569/main.vue */
