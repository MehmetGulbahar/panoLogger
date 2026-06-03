import type { App } from 'vue';

import { appServicesKey } from './app-keys';
import { appServices } from './services';

export function createAppServices(app: App): void {
  app.provide(appServicesKey, appServices);
}