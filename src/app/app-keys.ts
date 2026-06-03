import type { InjectionKey } from 'vue';

import type { AppServices } from './types';

export const appServicesKey: InjectionKey<AppServices> = Symbol('app-services');