import AppBreadcrumbs from '@/components/navigation/AppBreadcrumbs.vue';
import AppGlobalError from '@/components/system/AppGlobalError.vue';
import AppGlobalLoading from '@/components/system/AppGlobalLoading.vue';
import Card from '@/components/ui/Card.vue';
import Button from '@/components/ui/Button.vue';
import Table from '@/components/ui/Table.vue';
import Form from '@/components/ui/Form.vue';
import ModalHost from '@/components/ui/ModalHost.vue';
import MobileDrawer from '@/components/mobile/MobileDrawer.vue';
import FloatingActionButton from '@/components/mobile/FloatingActionButton.vue';
import ResponsiveContainer from '@/components/responsive/ResponsiveContainer.vue';
import ResponsiveHidden from '@/components/responsive/ResponsiveHidden.vue';
export function registerGlobalComponents(app) {
    app.component('AppBreadcrumbs', AppBreadcrumbs);
    app.component('AppGlobalError', AppGlobalError);
    app.component('AppGlobalLoading', AppGlobalLoading);
    app.component('Card', Card);
    app.component('UiButton', Button);
    app.component('UiTable', Table);
    app.component('UiForm', Form);
    app.component('ModalHost', ModalHost);
    app.component('MobileDrawer', MobileDrawer);
    app.component('FloatingActionButton', FloatingActionButton);
    app.component('ResponsiveContainer', ResponsiveContainer);
    app.component('ResponsiveHidden', ResponsiveHidden);
}
