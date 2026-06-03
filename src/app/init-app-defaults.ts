import { setGlobalLoaderState } from '@/stores/ui-store';

export function initAppDefaults(): void {
  setGlobalLoaderState(false);
}