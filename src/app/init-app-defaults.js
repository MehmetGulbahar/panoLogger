import { setGlobalLoaderState } from '@/stores/ui-store';
export function initAppDefaults() {
    setGlobalLoaderState(false);
}
