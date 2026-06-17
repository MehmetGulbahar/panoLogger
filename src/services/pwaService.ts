type BeforeInstallPromptEvent = Event & {
  prompt: () => Promise<void>;
  userChoice: Promise<{ outcome: 'accepted' | 'dismissed'; platform: string }>;
};

type PwaRegistrationOptions = {
  onInstallAvailable?: () => void;
  onInstallUnavailable?: () => void;
  onUpdateAvailable?: (registration: ServiceWorkerRegistration) => void;
};

let deferredInstallPrompt: BeforeInstallPromptEvent | null = null;
let activeRegistration: ServiceWorkerRegistration | null = null;

export function isPwaSupported() {
  return typeof window !== 'undefined' && 'serviceWorker' in navigator;
}

export function canRequestNotifications() {
  return typeof window !== 'undefined' && 'Notification' in window;
}

export function getNotificationPermission() {
  if (!canRequestNotifications()) return 'unsupported';

  return Notification.permission;
}

export async function requestNotificationPermission() {
  if (!canRequestNotifications()) return 'unsupported';
  if (Notification.permission !== 'default') return Notification.permission;

  return Notification.requestPermission();
}

export function hasInstallPrompt() {
  return deferredInstallPrompt !== null;
}

export async function promptInstall() {
  if (!deferredInstallPrompt) return 'unavailable';

  const promptEvent = deferredInstallPrompt;
  deferredInstallPrompt = null;
  await promptEvent.prompt();
  const choice = await promptEvent.userChoice;

  return choice.outcome;
}

export async function registerPwa(options: PwaRegistrationOptions = {}) {
  if (!isPwaSupported()) {
    options.onInstallUnavailable?.();
    return null;
  }

  window.addEventListener('beforeinstallprompt', (event) => {
    event.preventDefault();
    deferredInstallPrompt = event as BeforeInstallPromptEvent;
    options.onInstallAvailable?.();
  });

  window.addEventListener('appinstalled', () => {
    deferredInstallPrompt = null;
    options.onInstallUnavailable?.();
  });

  try {
    activeRegistration = await navigator.serviceWorker.register('/sw.js', { scope: '/' });

    activeRegistration.addEventListener('updatefound', () => {
      const installingWorker = activeRegistration?.installing;
      if (!installingWorker) return;

      installingWorker.addEventListener('statechange', () => {
        if (installingWorker.state === 'installed' && navigator.serviceWorker.controller) {
          options.onUpdateAvailable?.(activeRegistration as ServiceWorkerRegistration);
        }
      });
    });

    return activeRegistration;
  } catch {
    // Registration must fail silently in unsupported or restricted environments.
    options.onInstallUnavailable?.();
    return null;
  }
}

export function activateWaitingServiceWorker(registration = activeRegistration) {
  if (!registration?.waiting) return false;

  registration.waiting.postMessage({ type: 'SKIP_WAITING' });
  return true;
}
