<template>
  <main class="login-page">
    <section class="login-layout" aria-labelledby="auth-title">
      <ElectricalPanelScene class="login-visual" />

      <section class="login-shell">
        <header class="login-header">
          <div class="login-header__mark">
            <i class="pi pi-bolt" aria-hidden="true"></i>
          </div>
          <div>
            <p class="login-eyebrow">PanelDocs</p>
            <h1 id="auth-title">{{ 'Giriş Yap' }}</h1>
            <p>Pano dokümantasyon paneli</p>
          </div>
        </header>

        <form class="login-card" @submit.prevent="onSubmit">
          <label class="field">
            <span>Kullanıcı adı</span>
            <span class="input-wrap">
              <i class="pi pi-user" aria-hidden="true"></i>
              <input v-model="username" type="text" autocomplete="username" required />
            </span>
          </label>

          <label class="field">
            <span>Şifre</span>
            <span class="input-wrap">
              <i class="pi pi-lock" aria-hidden="true"></i>
              <input v-model="password" :type="showPassword ? 'text' : 'password'" autocomplete="current-password" required />
              <button class="icon-button" type="button" :aria-label="showPassword ? 'Şifreyi gizle' : 'Şifreyi göster'" @click="showPassword = !showPassword">
                <i :class="showPassword ? 'pi pi-eye-slash' : 'pi pi-eye'" aria-hidden="true"></i>
              </button>
            </span>
          </label>

          <div class="login-options">
            <label class="remember">
              <input v-model="rememberMe" type="checkbox" />
              <span>Beni hatırla</span>
            </label>
            <a href="#" @click.prevent>Şifremi unuttum</a>
          </div>

          <p v-if="errorMessage" class="login-error">{{ errorMessage }}</p>

          <UiButton class="login-submit" type="submit" variant="primary" :disabled="isSubmitting">
            <i class="pi pi-sign-in" aria-hidden="true"></i>
            {{ isSubmitting ? 'Giriş yapılıyor' : 'Giriş Yap' }}
          </UiButton>
        </form>
      </section>
    </section>
  </main>
</template>

<script setup lang="ts">
import axios from 'axios';
import { ref } from 'vue';
import { useToast } from 'primevue/usetoast';
import { useRoute, useRouter } from 'vue-router';
import ElectricalPanelScene from '@/components/auth/ElectricalPanelScene.vue';
import { routeNames } from '@/constants/routes';
import { useAuthStore } from '@/stores';

const router = useRouter();
const route = useRoute();
const authStore = useAuthStore();
const toast = useToast();

const username = ref('');
const password = ref('');
const rememberMe = ref(true);
const showPassword = ref(false);
const isSubmitting = ref(false);
const errorMessage = ref('');

async function onSubmit(): Promise<void> {
  errorMessage.value = '';

  if (!username.value.trim() || !password.value.trim()) {
    errorMessage.value = 'Kullanıcı adı ve şifre zorunludur.';
    return;
  }

  isSubmitting.value = true;

  try {
    await authStore.login(username.value.trim(), password.value);
    toast.add({
      severity: 'success',
      summary: 'Giriş başarılı',
      detail: 'Hesabınıza güvenli şekilde giriş yaptınız.',
      life: 3000,
    });

    const redirect = typeof route.query.redirect === 'string' ? route.query.redirect : undefined;
    await router.push(redirect ?? { name: routeNames.dashboard });
  } catch (error) {
    const backendMessage = axios.isAxiosError(error) && typeof error.response?.data?.detail === 'string'
      ? error.response.data.detail
      : null;

    errorMessage.value = backendMessage ?? 'Giriş yapılamadı. Kullanıcı adı veya şifre hatalı.';
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
  padding: clamp(1rem, 3vw, 2rem);
}

.login-layout {
  width: min(100%, 61rem);
  min-height: min(35.5rem, calc(100vh - 4rem));
  display: grid;
  grid-template-columns: minmax(28rem, 35rem) minmax(21rem, 24rem);
  align-items: center;
  gap: clamp(1rem, 3vw, 2rem);
}

.login-shell {
  width: 100%;
}

.login-visual {
  min-height: min(30rem, calc(100vh - 5rem));
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
.login-options a {
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

.login-options a {
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

@media (max-width: 900px) {
  .login-layout {
    min-height: auto;
    grid-template-columns: 1fr;
    width: min(100%, 30rem);
  }

  .login-visual {
    min-height: 18.5rem;
    order: -1;
  }
}

@media (max-width: 560px) {
  .login-page {
    place-items: start center;
    padding: 0.85rem;
  }

  .login-layout {
    gap: 0.8rem;
  }

  .login-visual {
    min-height: 17.5rem;
  }
}
</style>
