<template>
  <main class="login-page">
    <section class="login-shell" aria-labelledby="auth-title">
      <header class="login-header">
        <div class="login-header__mark">
          <i class="pi pi-bolt" aria-hidden="true"></i>
        </div>
        <div>
          <p class="login-eyebrow">PanelDocs</p>
          <h1 id="auth-title">{{ isRegisterMode ? 'Hesap Oluştur' : 'Giriş Yap' }}</h1>
          <p>Pano dokümantasyon paneli</p>
        </div>
      </header>

      <div class="auth-mode" role="tablist" aria-label="Kimlik doğrulama modu">
        <button type="button" role="tab" :aria-selected="!isRegisterMode" :class="{ active: !isRegisterMode }" @click="setMode('login')">
          Giriş
        </button>
        <button type="button" role="tab" :aria-selected="isRegisterMode" :class="{ active: isRegisterMode }" @click="setMode('register')">
          Kayıt Ol
        </button>
      </div>

      <form class="login-card" @submit.prevent="onSubmit">
        <label v-if="isRegisterMode" class="field">
          <span>Ad Soyad</span>
          <span class="input-wrap">
            <i class="pi pi-user" aria-hidden="true"></i>
            <input v-model="displayName" type="text" autocomplete="name" required />
          </span>
        </label>

        <label class="field">
          <span>E-posta</span>
          <span class="input-wrap">
            <i class="pi pi-envelope" aria-hidden="true"></i>
            <input v-model="email" type="email" autocomplete="email" required />
          </span>
        </label>

        <label v-if="isRegisterMode" class="field">
          <span>Şirket Kodu</span>
          <span class="input-wrap">
            <i class="pi pi-id-card" aria-hidden="true"></i>
            <input v-model="companyCode" type="text" autocomplete="organization" required placeholder="Örn. AVM-001" />
          </span>
        </label>

        <label class="field">
          <span>Şifre</span>
          <span class="input-wrap">
            <i class="pi pi-lock" aria-hidden="true"></i>
            <input v-model="password" :type="showPassword ? 'text' : 'password'" :autocomplete="isRegisterMode ? 'new-password' : 'current-password'" required />
            <button class="icon-button" type="button" :aria-label="showPassword ? 'Şifreyi gizle' : 'Şifreyi göster'" @click="showPassword = !showPassword">
              <i :class="showPassword ? 'pi pi-eye-slash' : 'pi pi-eye'" aria-hidden="true"></i>
            </button>
          </span>
        </label>

        <label v-if="isRegisterMode" class="field">
          <span>Şifre Tekrar</span>
          <span class="input-wrap">
            <i class="pi pi-lock" aria-hidden="true"></i>
            <input v-model="confirmPassword" :type="showPassword ? 'text' : 'password'" autocomplete="new-password" required />
          </span>
        </label>

        <div v-if="!isRegisterMode" class="login-options">
          <label class="remember">
            <input v-model="rememberMe" type="checkbox" />
            <span>Beni hatırla</span>
          </label>
          <a href="#" @click.prevent>Şifremi unuttum</a>
        </div>

        <p v-if="errorMessage" class="login-error">{{ errorMessage }}</p>

        <UiButton class="login-submit" type="submit" variant="primary" :disabled="isSubmitting">
          <i :class="isRegisterMode ? 'pi pi-user-plus' : 'pi pi-sign-in'" aria-hidden="true"></i>
          {{ submitLabel }}
        </UiButton>
      </form>

      <footer class="login-footer">
        <span>{{ isRegisterMode ? 'Zaten hesabınız var mı?' : 'Henüz hesabınız yok mu?' }}</span>
        <button type="button" @click="setMode(isRegisterMode ? 'login' : 'register')">
          {{ isRegisterMode ? 'Giriş Yap' : 'Kayıt Ol' }}
        </button>
      </footer>
    </section>
  </main>
</template>

<script setup lang="ts">
import axios from 'axios';
import { computed, ref } from 'vue';
import { useToast } from 'primevue/usetoast';
import { useRoute, useRouter } from 'vue-router';
import { routeNames } from '@/constants/routes';
import { useAuthStore } from '@/stores';

type AuthMode = 'login' | 'register';

const router = useRouter();
const route = useRoute();
const authStore = useAuthStore();
const toast = useToast();

const mode = ref<AuthMode>('login');
const displayName = ref('');
const email = ref('');
const companyCode = ref('');
const password = ref('');
const confirmPassword = ref('');
const rememberMe = ref(true);
const showPassword = ref(false);
const isSubmitting = ref(false);
const errorMessage = ref('');

const isRegisterMode = computed(() => mode.value === 'register');
const submitLabel = computed(() => {
  if (isSubmitting.value) {
    return isRegisterMode.value ? 'Hesap oluşturuluyor' : 'Giriş yapılıyor';
  }

  return isRegisterMode.value ? 'Hesap Oluştur' : 'Giriş Yap';
});

function setMode(nextMode: AuthMode): void {
  mode.value = nextMode;
  errorMessage.value = '';
  password.value = '';
  confirmPassword.value = '';
}

async function onSubmit(): Promise<void> {
  errorMessage.value = '';

  if (!email.value.trim() || !password.value.trim()) {
    errorMessage.value = 'E-posta ve şifre zorunludur.';
    return;
  }

  if (isRegisterMode.value && displayName.value.trim().length < 2) {
    errorMessage.value = 'Ad soyad en az 2 karakter olmalıdır.';
    return;
  }

  if (isRegisterMode.value && !companyCode.value.trim()) {
    errorMessage.value = 'Şirket kodu zorunludur.';
    return;
  }

  if (isRegisterMode.value && password.value.length < 8) {
    errorMessage.value = 'Şifre en az 8 karakter olmalıdır.';
    return;
  }

  if (isRegisterMode.value && password.value !== confirmPassword.value) {
    errorMessage.value = 'Şifreler eşleşmiyor.';
    return;
  }

  isSubmitting.value = true;

  try {
    if (isRegisterMode.value) {
      await authStore.register(displayName.value.trim(), email.value.trim(), password.value, companyCode.value.trim());
      toast.add({
        severity: 'success',
        summary: 'Hesap oluşturuldu',
        detail: 'Kaydınız tamamlandı. PanoDocs ortamına hoş geldiniz.',
        life: 3500,
      });
    } else {
      await authStore.login(email.value.trim(), password.value);
      toast.add({
        severity: 'success',
        summary: 'Giriş başarılı',
        detail: 'Hesabınıza güvenli şekilde giriş yaptınız.',
        life: 3000,
      });
    }

    const redirect = typeof route.query.redirect === 'string' ? route.query.redirect : undefined;
    await router.push(redirect ?? { name: routeNames.dashboard });
  } catch (error) {
    const backendMessage = axios.isAxiosError(error) && typeof error.response?.data?.detail === 'string'
      ? error.response.data.detail
      : null;

    errorMessage.value = backendMessage ?? (isRegisterMode.value
      ? 'Hesap oluşturulamadı. E-posta daha önce kullanılmış olabilir.'
      : 'Giriş yapılamadı. E-posta veya şifre hatalı.');
  } finally {
    isSubmitting.value = false;
  }
}
</script>

<style scoped>
.login-page {
  min-height: 100vh;
  display: grid;
  place-items: center;
  background: #f5f7fb;
  color: var(--app-text);
  padding: 1rem;
}

.login-shell {
  width: min(100%, 24rem);
}

.login-header {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  margin-bottom: 1rem;
}

.login-header__mark {
  width: 2.25rem;
  height: 2.25rem;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  border-radius: 8px;
  background: var(--primary-50);
  color: var(--app-primary);
  flex: 0 0 auto;
}

.login-header__mark .pi {
  font-size: 1rem;
}

.login-eyebrow {
  margin: 0 0 0.1rem;
  font-size: 0.6875rem;
  line-height: 1.2;
  color: var(--app-primary);
  font-weight: 700;
  text-transform: uppercase;
}

.login-header h1 {
  margin: 0;
  font-size: 1.25rem;
  line-height: 1.15;
  font-weight: 800;
}

.login-header p:last-child {
  margin: 0.15rem 0 0;
  color: var(--app-text-muted);
  font-size: 0.8125rem;
  line-height: 1.35;
}

.auth-mode {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 0.2rem;
  margin-bottom: 0.65rem;
  padding: 0.2rem;
  border: 1px solid var(--app-border);
  border-radius: 8px;
  background: var(--app-surface-alt);
}

.auth-mode button {
  min-height: 2rem;
  border: 0;
  border-radius: 6px;
  background: transparent;
  color: var(--app-text-muted);
  font-size: 0.75rem;
  font-weight: 700;
  cursor: pointer;
}

.auth-mode button.active {
  background: var(--app-bg);
  color: var(--app-primary);
  box-shadow: var(--app-shadow);
}

.login-card {
  display: flex;
  flex-direction: column;
  gap: 0.85rem;
  padding: 1rem;
  background: var(--app-bg);
  border: 1px solid var(--app-border);
  border-radius: 8px;
  box-shadow: var(--app-shadow);
}

.field {
  display: grid;
  gap: 0.35rem;
}

.field > span:first-child,
.remember,
.login-options a,
.login-footer {
  font-size: 0.75rem;
  line-height: 1.25;
}

.field > span:first-child {
  color: var(--app-text-muted);
  font-weight: 600;
}

.input-wrap {
  display: flex;
  align-items: center;
  gap: 0.55rem;
  min-height: 2.35rem;
  padding: 0 0.7rem;
  border: 1px solid var(--app-border);
  border-radius: 8px;
  background: var(--app-bg);
}

.input-wrap:focus-within {
  border-color: var(--app-primary);
  box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.1);
}

.input-wrap > .pi {
  color: var(--app-text-muted);
  font-size: 0.85rem;
  flex: 0 0 auto;
}

.input-wrap input:not([type='checkbox']) {
  width: 100%;
  min-width: 0;
  border: 0;
  outline: 0;
  background: transparent;
  color: var(--app-text);
  font: inherit;
  font-size: 0.8125rem;
}

.icon-button {
  width: 1.8rem;
  height: 1.8rem;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  border: 0;
  border-radius: 7px;
  background: transparent;
  color: var(--app-text-muted);
  cursor: pointer;
  flex: 0 0 auto;
}

.icon-button .pi {
  font-size: 0.85rem;
}

.login-options {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 0.75rem;
}

.remember {
  display: inline-flex;
  align-items: center;
  gap: 0.45rem;
  color: var(--app-text-muted);
}

.remember input {
  width: 0.9rem;
  height: 0.9rem;
  accent-color: var(--app-primary);
}

.login-options a,
.login-footer button {
  font-weight: 700;
}

.login-error {
  margin: 0;
  padding: 0.55rem 0.65rem;
  border-radius: 8px;
  background: #fff1f2;
  color: var(--app-danger);
  font-size: 0.75rem;
  line-height: 1.3;
}

.login-submit {
  width: 100%;
  min-height: 2.35rem;
}

.login-footer {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 0.4rem;
  margin-top: 0.85rem;
  color: var(--app-text-muted);
}

.login-footer button {
  border: 0;
  padding: 0;
  background: transparent;
  color: var(--app-primary);
  cursor: pointer;
  font-size: inherit;
}
</style>
