import { appServicesKey } from './app-keys';
import { appServices } from './services';
export function createAppServices(app) {
    app.provide(appServicesKey, appServices);
}
