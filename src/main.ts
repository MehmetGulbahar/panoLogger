import { createApp } from 'vue';
import PrimeVue from 'primevue/config';
import ConfirmationService from 'primevue/confirmationservice';
import ToastService from 'primevue/toastservice';
import Tooltip from 'primevue/tooltip';
import { VueQueryPlugin } from '@tanstack/vue-query';

import 'primeicons/primeicons.css';
import 'primeflex/primeflex.css';
import 'primevue/resources/primevue.min.css';
import 'primevue/resources/themes/saga-blue/theme.css';

import App from './App.vue';
import { appRouter } from './router';
import { appPinia } from './stores';
import { queryClient } from './api/query-client';
import { createAppServices } from './app/app-services';
import { registerGlobalComponents } from './app/register-global-components';
import { applyTheme } from './app/theme';
import { initAppDefaults } from './app/init-app-defaults';
import { registerPwa } from './services/pwaService';

import './assets/styles/main.css';

applyTheme();

const app = createApp(App);

app.use(appPinia);
initAppDefaults();
app.use(appRouter);
app.use(VueQueryPlugin, { queryClient });
app.use(PrimeVue, {
  ripple: true,
  inputStyle: 'outlined',
});
app.use(ConfirmationService);
app.use(ToastService);

app.directive('tooltip', Tooltip);

createAppServices(app);
registerGlobalComponents(app);

app.mount('#app');

registerPwa({
  onInstallAvailable: () => window.dispatchEvent(new CustomEvent('pwa-install-available')),
  onInstallUnavailable: () => window.dispatchEvent(new CustomEvent('pwa-install-unavailable')),
  onUpdateAvailable: () => window.dispatchEvent(new CustomEvent('pwa-update-available')),
});
