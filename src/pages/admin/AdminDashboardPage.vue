<template>
  <section class="admin-page" :class="{ 'is-busy': isBusy }">
    <div v-if="isBusy" class="loading-layer" role="status" aria-live="polite">
      <i class="pi pi-spin pi-spinner" aria-hidden="true"></i>
      <span>{{ loadingLabel }}</span>
    </div>

    <div class="admin-head">
      <div>
        <p class="eyebrow">Super Admin</p>
        <h1>Sistem Yonetimi</h1>
        <p>Kullanıcılar, roller, şirket bağlantıları ve sistem izinleri.</p>
      </div>
      <UiButton variant="secondary" :disabled="isLoading" @click="loadAdminData">
        <i :class="isLoading ? 'pi pi-spin pi-spinner' : 'pi pi-refresh'" aria-hidden="true"></i>
        Yenile
      </UiButton>
    </div>

    <p v-if="errorMessage" class="admin-alert" role="alert">{{ errorMessage }}</p>

    <div class="metric-grid" aria-label="Sistem ozeti">
      <div v-for="metric in metrics" :key="metric.label" class="metric-card">
        <span class="metric-icon"><i :class="metric.icon" aria-hidden="true"></i></span>
        <div>
          <strong>{{ metric.value }}</strong>
          <span>{{ metric.label }}</span>
        </div>
      </div>
    </div>

    <div class="admin-data-grid">
      <section class="admin-section">
        <div class="section-head">
          <div>
            <h2>Bakim Raporlari</h2>
            <p>Veritabanındaki son bakım raporu kayıtları.</p>
          </div>
        </div>

        <div class="compact-list">
          <div v-if="maintenanceReports.length === 0" class="empty-panel">Bakım raporu kaydı bulunamadı.</div>
          <template v-else>
            <article v-for="report in maintenanceReports.slice(0, 6)" :key="report.id" class="compact-list-item">
              <div>
                <strong>{{ report.title }}</strong>
                <span>{{ report.projectName }} / {{ report.facilityName }} / {{ report.panelCode }}</span>
              </div>
              <time>{{ formatDate(report.reportDateUtc) }}</time>
            </article>
          </template>
        </div>
      </section>

      <section class="admin-section">
        <div class="section-head">
          <div>
            <h2>Audit Logs</h2>
            <p>Sistemde kaydedilen son denetim hareketleri.</p>
          </div>
        </div>

        <div class="compact-list">
          <div v-if="auditLogs.length === 0" class="empty-panel">Audit log kaydi bulunamadi.</div>
          <template v-else>
            <article v-for="log in auditLogs.slice(0, 6)" :key="log.id" class="compact-list-item">
              <div>
                <strong>{{ log.action }} => {{ log.panelName }}</strong>
                <span>{{ log.entityName }}{{ log.username ? ` / ${log.username}` : log.userId ? ` / ${log.userId}` : '' }}</span>
              </div>
              <time>{{ formatDate(log.occurredAtUtc) }}</time>
            </article>
          </template>
        </div>
      </section>
    </div>

    <section class="admin-section">
      <div class="section-head">
        <div>
          <h2>Kullanıcı Yetkilendirme</h2>
          <p>Rol, şirket baglantisi ve aktiflik durumunu yonetin.</p>
        </div>
       <div class="table-tools">
       
      <input v-model="searchTerm" type="search" placeholder="Kullanıcı ara" aria-label="Kullanıcı ara" />
       <button class="icon-button" type="button" title="Yeni Kullanıcı Ekle" aria-label="Yeni Kullanıcı Ekle" :disabled="isLoading" @click="openCreateUserModal">
        <i class="pi pi-plus" aria-hidden="true"></i>
      </button>
      <select v-model="statusFilter" aria-label="Durum filtresi">
        <option value="all">Tüm durumlar</option>
        <option value="active">Aktif</option>
        <option value="inactive">Pasif</option>
      </select>
    </div>
      </div>

      <div class="admin-table-wrap">
        <table class="admin-table">
          <thead>
            <tr>
              <th>Kullanıcı</th>
              <th>Şirket</th>
              <th>Roller</th>
              <th>Durum</th>
              <th>Kayıt</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            <tr v-if="filteredUsers.length === 0">
              <td colspan="6" class="empty-cell">Kayıt bulunamadı.</td>
            </tr>
            <tr v-for="user in filteredUsers" :key="user.id" :class="{ selected: selectedUser?.id === user.id }">
              <td>
                <strong>{{ user.displayName }}</strong>
                <span>{{ user.username }}</span>
              </td>
              <td>
                <strong>{{ user.projectName || 'Sistem geneli' }}</strong>
                <span>{{ user.companyCode || 'SuperAdmin' }}</span>
              </td>
              <td>
                <div class="role-stack">
                  <span v-for="role in user.roles" :key="role" class="role-chip">{{ role }}</span>
                </div>
              </td>
              <td>
                <span :class="['status-pill', user.isActive ? 'is-active' : 'is-inactive']">
                  {{ user.isActive ? 'Aktif' : 'Pasif' }}
                </span>
              </td>
              <td>{{ formatDate(user.createdAtUtc) }}</td>
              <td class="actions-cell">
                <button class="icon-button" type="button" title="Duzenle" aria-label="Duzenle" @click="selectUser(user)">
                  <i class="pi pi-pencil" aria-hidden="true"></i>
                </button>
                <button
                  class="icon-button"
                  type="button"
                  title="Sifre Degistir"
                  aria-label="Sifre Degistir"
                  :disabled="isResettingPassword"
                  @click="openPasswordResetModal(user)"
                >
                  <i class="pi pi-key" aria-hidden="true"></i>
                </button>
                <button
                  class="icon-button danger-icon-button"
                  type="button"
                  title="Kullanıcıyı Sil"
                  aria-label="Kullanıcıyı Sil"
                  :disabled="isDeletingUser || user.id === authStore.user?.id"
                  @click="confirmDeleteUser($event, user)"
                >
                  <i :class="isDeletingUser ? 'pi pi-spin pi-spinner' : 'pi pi-trash'" aria-hidden="true"></i>
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </section>

    <section v-if="selectedUser" class="admin-section">
      <div class="section-head">
        <div>
          <h2>{{ selectedUser.displayName }}</h2>
          <p>{{ selectedUser.username }}</p>
        </div>
        <button class="icon-button" type="button" title="Kapat" aria-label="Kapat" @click="clearUserSelection">
          <i class="pi pi-times" aria-hidden="true"></i>
        </button>
      </div>

      <form class="admin-form" @submit.prevent="saveSelectedUser">
        <label class="field">
          <span>Ad Soyad</span>
          <input v-model="editForm.displayName" required />
        </label>

        <label class="field">
          <span>Şirket</span>
          <select v-model="editForm.companyId">
            <option :value="null">Sistem geneli / SuperAdmin</option>
            <option v-for="company in companies" :key="company.id" :value="company.id">
              {{ company.projectName }} - {{ company.companyCode }}
            </option>
          </select>
        </label>

        <label class="toggle-row">
          <input v-model="editForm.isActive" type="checkbox" />
          <span>Aktif hesap</span>
        </label>

        <fieldset class="role-fieldset">
          <legend>Roller</legend>
          <label v-for="role in roles" :key="role.id" class="role-option">
            <input v-model="editForm.roles" type="checkbox" :value="role.name" />
            <span>
              <strong>{{ role.name }}</strong>
              <small>{{ role.description }}</small>
            </span>
          </label>
        </fieldset>

        <div class="form-actions">
          <UiButton variant="secondary" type="button" @click="clearUserSelection">Vazgeç</UiButton>
          <UiButton variant="primary" type="submit" :disabled="isSavingUser">
            <i :class="isSavingUser ? 'pi pi-spin pi-spinner' : 'pi pi-save'" aria-hidden="true"></i>
            Kaydet
          </UiButton>
        </div>
      </form>
    </section>

    <section class="admin-section">
      <div class="section-head">
        <div>
          <h2>Rol ve İzin Yönetimi</h2>
          <p>Yeni rol oluşturma veya mevcut rollerin ne yapabileceğini düzenleyin.</p>
        </div>
      </div>

      <div class="role-manager">
        <aside class="role-list">
          <button
            v-for="role in roles"
            :key="role.id"
            :class="['role-list-item', { selected: selectedRole?.id === role.id }]"
            type="button"
            @click="selectRole(role)"
          >
            <strong>{{ role.name }}</strong>
            <span>{{ role.permissions.length }} izin</span>
          </button>
        </aside>

        <div class="role-editor">
          <form class="role-create-form" @submit.prevent="createRole">
            <div>
              <h3>Yeni Rol</h3>
              <p>Rol adı harf, rakam, tire ve alt çizgi icerebilir.</p>
            </div>
            <label class="field">
              <span>Rol Adı</span>
              <input v-model="newRoleForm.name" placeholder="Orn. Auditor" />
            </label>
            <label class="field">
              <span>Açıklama</span>
              <input v-model="newRoleForm.description" placeholder="Bu rolün sorumluluğu" />
            </label>
            <UiButton variant="secondary" type="submit" :disabled="isSavingRole">
              <i :class="isSavingRole ? 'pi pi-spin pi-spinner' : 'pi pi-plus'" aria-hidden="true"></i>
              Rol Ekle
            </UiButton>
          </form>

          <form v-if="selectedRole" class="permission-editor" @submit.prevent="saveSelectedRole">
            <div class="section-head compact">
              <div>
                <h3>{{ selectedRole.name }}</h3>
                <p v-if="selectedRole.isProtected">Bu rol sistem için korunuyor.</p>
                <p v-else>Bu role ait yetkileri seçin.</p>
              </div>
              <button
                v-if="!selectedRole.isProtected"
                class="danger-button"
                type="button"
                :disabled="isDeletingRole"
                @click="confirmDeleteSelectedRole"
              >
                <i :class="isDeletingRole ? 'pi pi-spin pi-spinner' : 'pi pi-trash'" aria-hidden="true"></i>
                Rol Sil
              </button>
              <UiButton variant="primary" type="submit" :disabled="isSavingRole || selectedRole.isProtected">
                <i :class="isSavingRole ? 'pi pi-spin pi-spinner' : 'pi pi-save'" aria-hidden="true"></i>
                İzinleri Kaydet
              </UiButton>
            </div>

            <label class="field">
              <span>Açıklama</span>
              <input v-model="roleForm.description" :disabled="selectedRole.isProtected" required />
            </label>

            <div class="permission-options">
              <label v-for="permission in availablePermissions" :key="permission" class="permission-option">
                <input v-model="roleForm.permissions" type="checkbox" :value="permission" :disabled="selectedRole.isProtected" />
                <span>{{ permission }}</span>
              </label>
            </div>
          </form>

          <div v-else class="empty-panel">Düzenlemek için bir rol seçin.</div>
        </div>
      </div>
    </section>
    <div v-if="isCreateUserOpen" class="modal-backdrop" role="presentation" @click.self="closeCreateUserModal">
      <section class="user-modal" role="dialog" aria-modal="true" aria-labelledby="create-user-title">
        <div class="section-head compact">
          <div>
            <h2 id="create-user-title">Yeni Kullanıcı</h2>
            <p>Müşteriye verilecek kullanıcı adı ve geçici şifreyi oluşturun.</p>
          </div>
          <button class="icon-button" type="button" title="Kapat" aria-label="Kapat" @click="closeCreateUserModal">
            <i class="pi pi-times" aria-hidden="true"></i>
          </button>
        </div>

        <form class="admin-form" @submit.prevent="createUser">
          <label class="field">
            <span>Kullanıcı adı</span>
            <input v-model="createUserForm.username" required placeholder="ornek.kullanici" autocomplete="off" />
          </label>

          <label class="field">
            <span>Ad Soyad</span>
            <input v-model="createUserForm.displayName" required placeholder="Mehmet Gül" autocomplete="off" />
          </label>

          <label class="field">
            <span>Geçici Şifre</span>
            <input v-model="createUserForm.password" type="text" required placeholder="En az 8 karakter" autocomplete="off" />
          </label>

          <label class="field">
            <span>Şirket</span>
            <select v-model="createUserForm.companyId">
              <option :value="null">Sistem geneli / SuperAdmin</option>
              <option v-for="company in companies" :key="company.id" :value="company.id">
                {{ company.projectName }} - {{ company.companyCode }}
              </option>
            </select>
          </label>

          <fieldset class="role-fieldset">
            <legend>Roller</legend>
            <label v-for="role in roles" :key="role.id" class="role-option">
              <input v-model="createUserForm.roles" type="checkbox" :value="role.name" />
              <span>
                <strong>{{ role.name }}</strong>
                <small>{{ role.description }}</small>
              </span>
            </label>
          </fieldset>

          <div class="form-actions">
            <UiButton variant="secondary" type="button" @click="closeCreateUserModal">Vazgeç</UiButton>
            <UiButton variant="primary" type="submit" :disabled="isCreatingUser">
              <i :class="isCreatingUser ? 'pi pi-spin pi-spinner' : 'pi pi-user-plus'" aria-hidden="true"></i>
              Kullanıcı Oluştur
            </UiButton>
          </div>
        </form>
      </section>
    </div>
    <div v-if="passwordResetUser" class="modal-backdrop" role="presentation" @click.self="closePasswordResetModal">
      <section class="user-modal password-modal" role="dialog" aria-modal="true" aria-labelledby="reset-password-title">
        <div class="section-head compact">
          <div>
            <h2 id="reset-password-title">Şifre Değiştir</h2>
            <p>{{ passwordResetUser.displayName }} için yeni geçici şifre belirleyin.</p>
          </div>
          <button class="icon-button" type="button" title="Kapat" aria-label="Kapat" @click="closePasswordResetModal">
            <i class="pi pi-times" aria-hidden="true"></i>
          </button>
        </div>

        <form class="admin-form password-form" @submit.prevent="resetUserPassword">
          <label class="field">
            <span>Kullanıcı</span>
            <input :value="passwordResetUser.username" disabled />
          </label>

          <label class="field">
            <span>Yeni Şifre</span>
            <input v-model="passwordResetForm.password" type="text" required placeholder="En az 8 karakter" autocomplete="off" />
          </label>

          <p class="form-hint">Bu işlem mevcut şifreyi geri almaz; yeni şifre kaydedildikten sonra kullanıcı bu şifreyle giriş yapar.</p>

          <div class="form-actions">
            <UiButton variant="secondary" type="button" @click="closePasswordResetModal">Vazgeç</UiButton>
            <UiButton variant="primary" type="submit" :disabled="isResettingPassword || passwordResetForm.password.length < 8">
              <i :class="isResettingPassword ? 'pi pi-spin pi-spinner' : 'pi pi-key'" aria-hidden="true"></i>
              Şifreyi Kaydet
            </UiButton>
          </div>
        </form>
      </section>
    </div>
  </section>
</template>

<script setup lang="ts">
import { computed, onMounted, reactive, ref } from 'vue';
import { useConfirm } from 'primevue/useconfirm';
import { useToast } from 'primevue/usetoast';
import { apiClient } from '@/api/client';
import { apiEndpoints } from '@/api/endpoints';
import { useAuthStore } from '@/stores';
import type {
  AdminAuditLog,
  AdminCompanyOption,
  AdminMaintenanceReport,
  AdminOverview,
  AdminRole,
  AdminUser,
  CreateAdminUserRequest,
  Permission,
  ResetAdminUserPasswordRequest,
  UpdateAdminUserRequest,
  UserRole,
} from '@/types';

const toast = useToast();
const confirm = useConfirm();
const authStore = useAuthStore();
const overview = ref<AdminOverview | null>(null);
const users = ref<AdminUser[]>([]);
const companies = ref<AdminCompanyOption[]>([]);
const roles = ref<AdminRole[]>([]);
const availablePermissions = ref<Permission[]>([]);
const auditLogs = ref<AdminAuditLog[]>([]);
const maintenanceReports = ref<AdminMaintenanceReport[]>([]);
const selectedUser = ref<AdminUser | null>(null);
const selectedRole = ref<AdminRole | null>(null);
const searchTerm = ref('');
const statusFilter = ref<'all' | 'active' | 'inactive'>('all');
const errorMessage = ref('');
const isLoading = ref(false);
const isSavingUser = ref(false);
const isCreatingUser = ref(false);
const isDeletingUser = ref(false);
const isSavingRole = ref(false);
const isDeletingRole = ref(false);
const isCreateUserOpen = ref(false);
const isResettingPassword = ref(false);
const passwordResetUser = ref<AdminUser | null>(null);

const editForm = reactive<UpdateAdminUserRequest>({
  displayName: '',
  companyId: null,
  isActive: true,
  roles: [],
});

const createUserForm = reactive<CreateAdminUserRequest>({
  username: '',
  displayName: '',
  password: '',
  companyId: null,
  roles: ['Viewer'],
});

const passwordResetForm = reactive<ResetAdminUserPasswordRequest>({
  password: '',
});

const roleForm = reactive({
  description: '',
  permissions: [] as Permission[],
});

const newRoleForm = reactive({
  name: '',
  description: '',
});

const isBusy = computed(() => isLoading.value || isSavingUser.value || isCreatingUser.value || isDeletingUser.value || isResettingPassword.value || isSavingRole.value || isDeletingRole.value);
const loadingLabel = computed(() => {
  if (isSavingUser.value) return 'Kullanıcı güncelleniyor...';
  if (isCreatingUser.value) return 'Kullanıcı oluşturuluyor...';
  if (isDeletingUser.value) return 'Kullanıcı siliniyor...';
  if (isResettingPassword.value) return 'Sifre guncelleniyor...';
  if (isSavingRole.value) return 'Rol izinleri kaydediliyor...';
  if (isDeletingRole.value) return 'Rol siliniyor...';
  return 'Admin verileri yükleniyor...';
});

const metrics = computed(() => {
  const data = overview.value;
  return [
    { label: 'Şirket', value: data?.companies ?? 0, icon: 'pi pi-folder' },
    { label: 'Tesis', value: data?.facilities ?? 0, icon: 'pi pi-building' },
    { label: 'Pano', value: data?.panels ?? 0, icon: 'pi pi-th-large' },
    { label: 'Kullanıcı', value: data?.users ?? 0, icon: 'pi pi-users' },
    { label: 'Aktif Kullanıcı', value: data?.activeUsers ?? 0, icon: 'pi pi-check-circle' },
    { label: 'Dosya', value: data?.files ?? 0, icon: 'pi pi-file' },
  ];
});

const filteredUsers = computed(() => {
  const query = searchTerm.value.trim().toLowerCase();
  return users.value.filter((user) => {
    const matchesQuery = !query
      || user.displayName.toLowerCase().includes(query)
      || user.username.toLowerCase().includes(query)
      || (user.projectName ?? '').toLowerCase().includes(query)
      || (user.companyCode ?? '').toLowerCase().includes(query);
    const matchesStatus = statusFilter.value === 'all'
      || (statusFilter.value === 'active' && user.isActive)
      || (statusFilter.value === 'inactive' && !user.isActive);
    return matchesQuery && matchesStatus;
  });
});

onMounted(() => {
  void loadAdminData();
});


async function loadAdminData(): Promise<void> {
  isLoading.value = true;
  errorMessage.value = '';
  try {
    const [
      overviewResponse,
      usersResponse,
      companiesResponse,
      rolesResponse,
      permissionsResponse,
      auditLogsResponse,
      maintenanceReportsResponse,
    ] = await Promise.all([
      apiClient.get<AdminOverview>(`${apiEndpoints.admin}/overview`),
      apiClient.get<AdminUser[]>(`${apiEndpoints.admin}/users`),
      apiClient.get<AdminCompanyOption[]>(`${apiEndpoints.admin}/companies/options`),
      apiClient.get<AdminRole[]>(`${apiEndpoints.admin}/roles`),
      apiClient.get<Permission[]>(`${apiEndpoints.admin}/permissions`),
      apiClient.get<AdminAuditLog[]>(`${apiEndpoints.admin}/audit-logs`),
      apiClient.get<AdminMaintenanceReport[]>(`${apiEndpoints.admin}/maintenance-reports`),
    ]);

    overview.value = overviewResponse.data;
    users.value = usersResponse.data;
    companies.value = companiesResponse.data;
    roles.value = rolesResponse.data;
    availablePermissions.value = permissionsResponse.data;
    auditLogs.value = auditLogsResponse.data;
    maintenanceReports.value = maintenanceReportsResponse.data;

    if (selectedUser.value) {
      const refreshedUser = users.value.find((user) => user.id === selectedUser.value?.id);
      if (refreshedUser) selectUser(refreshedUser);
    }

    if (selectedRole.value) {
      const refreshedRole = roles.value.find((role) => role.id === selectedRole.value?.id);
      if (refreshedRole) selectRole(refreshedRole);
    }
  } catch (error: any) {
    handleRequestError(error, 'Admin verileri yüklenemedi.');
  } finally {
    isLoading.value = false;
  }
}

function selectUser(user: AdminUser): void {
  selectedUser.value = user;
  editForm.displayName = user.displayName;
  editForm.companyId = user.companyId ?? null;
  editForm.isActive = user.isActive;
  editForm.roles = [...user.roles];
  errorMessage.value = '';
}

function clearUserSelection(): void {
  selectedUser.value = null;
  errorMessage.value = '';
}

function openCreateUserModal(): void {
  resetCreateUserForm();
  isCreateUserOpen.value = true;
  errorMessage.value = '';
}

function closeCreateUserModal(): void {
  if (isCreatingUser.value) return;
  isCreateUserOpen.value = false;
  resetCreateUserForm();
}

function resetCreateUserForm(): void {
  createUserForm.username = '';
  createUserForm.displayName = '';
  createUserForm.password = '';
  createUserForm.companyId = null;
  createUserForm.roles = ['Viewer'];
}

function openPasswordResetModal(user: AdminUser): void {
  passwordResetUser.value = user;
  passwordResetForm.password = '';
  errorMessage.value = '';
}

function closePasswordResetModal(): void {
  if (isResettingPassword.value) return;
  passwordResetUser.value = null;
  passwordResetForm.password = '';
}

function selectRole(role: AdminRole): void {
  selectedRole.value = role;
  roleForm.description = role.description;
  roleForm.permissions = [...role.permissions];
  errorMessage.value = '';
}

async function saveSelectedUser(): Promise<void> {
  if (!selectedUser.value) return;
  isSavingUser.value = true;
  errorMessage.value = '';
  try {
    await apiClient.put(`${apiEndpoints.admin}/users/${selectedUser.value.id}`, {
      ...editForm,
      roles: editForm.roles as UserRole[],
    });
    showSuccess('Kullanıcı güncellendi', 'Rol, şirket ve durum bilgileri kaydedildi.');
    await loadAdminData();
  } catch (error: any) {
    handleRequestError(error, 'Kullanıcı güncellenemedi.');
  } finally {
    isSavingUser.value = false;
  }
}

async function createUser(): Promise<void> {
  isCreatingUser.value = true;
  errorMessage.value = '';
  try {
    const { data } = await apiClient.post<AdminUser>(`${apiEndpoints.admin}/users`, {
      ...createUserForm,
      roles: createUserForm.roles as UserRole[],
    });
    showSuccess('Kullanıcı oluşturuldu', `${data.username} kullanıcı adı hazır.`);
    isCreateUserOpen.value = false;
    resetCreateUserForm();
    await loadAdminData();
  } catch (error: any) {
    handleRequestError(error, 'Kullanıcı oluşturulamadı.');
  } finally {
    isCreatingUser.value = false;
  }
}

async function resetUserPassword(): Promise<void> {
  if (!passwordResetUser.value) return;

  isResettingPassword.value = true;
  errorMessage.value = '';
  try {
    await apiClient.put(`${apiEndpoints.admin}/users/${passwordResetUser.value.id}/password`, {
      password: passwordResetForm.password,
    });
    showSuccess('Sifre guncellendi', `${passwordResetUser.value.username} icin yeni sifre kaydedildi.`);
    passwordResetUser.value = null;
    passwordResetForm.password = '';
  } catch (error: any) {
    handleRequestError(error, 'Sifre guncellenemedi.');
  } finally {
    isResettingPassword.value = false;
  }
}

function confirmDeleteUser(event: MouseEvent, user: AdminUser): void {
  if (user.id === authStore.user?.id) return;

  confirm.require({
    target: event.currentTarget as HTMLElement,
    message: `${user.displayName} kullanıcısını silmek istiyor musunuz?`,
    icon: 'pi pi-exclamation-triangle',
    acceptLabel: 'Kullanıcıyı Sil',
    rejectLabel: 'Vazgeç',
    acceptClass: 'p-button-danger',
    accept: () => {
      void deleteUser(user);
    },
  });
}

async function deleteUser(user: AdminUser): Promise<void> {
  if (user.id === authStore.user?.id) return;
  isDeletingUser.value = true;
  errorMessage.value = '';
  try {
    await apiClient.delete(`${apiEndpoints.admin}/users/${user.id}`);
    if (selectedUser.value?.id === user.id) {
      clearUserSelection();
    }
    showSuccess('Kullanıcı silindi', `${user.displayName} sistemden kaldirildi.`);
    await loadAdminData();
  } catch (error: any) {
    handleRequestError(error, 'Kullanıcı silinemedi.');
  } finally {
    isDeletingUser.value = false;
  }
}

async function createRole(): Promise<void> {
  isSavingRole.value = true;
  errorMessage.value = '';
  try {
    const { data } = await apiClient.post<AdminRole>(`${apiEndpoints.admin}/roles`, {
      name: newRoleForm.name,
      description: newRoleForm.description,
      permissions: [],
    });
    newRoleForm.name = '';
    newRoleForm.description = '';
    showSuccess('Rol oluşturuldu', `${data.name} rolü eklendi.`);
    await loadAdminData();
    const created = roles.value.find((role) => role.id === data.id);
    if (created) selectRole(created);
  } catch (error: any) {
    handleRequestError(error, 'Rol olusturulamadi.');
  } finally {
    isSavingRole.value = false;
  }
}

async function saveSelectedRole(): Promise<void> {
  if (!selectedRole.value || selectedRole.value.isProtected) return;
  isSavingRole.value = true;
  errorMessage.value = '';
  try {
    await apiClient.put(`${apiEndpoints.admin}/roles/${selectedRole.value.id}`, {
      description: roleForm.description,
      permissions: roleForm.permissions,
    });
    showSuccess('Rol güncellendi.', 'İzinler kaydedildi. Etkilenen kullanıcılar yeniden giriş yapmalı.');
    await loadAdminData();
  } catch (error: any) {
    handleRequestError(error, 'Rol guncellenemedi.');
  } finally {
    isSavingRole.value = false;
  }
}

function confirmDeleteSelectedRole(event: MouseEvent): void {
  if (!selectedRole.value || selectedRole.value.isProtected) return;

  const roleName = selectedRole.value.name;

  confirm.require({
    target: event.currentTarget as HTMLElement,
    message: `${roleName} rolunu silmek istiyor musunuz?`,
    icon: 'pi pi-exclamation-triangle',
    acceptLabel: 'Rolu Sil',
    rejectLabel: 'Vazgeç',
    acceptClass: 'p-button-danger',
    accept: () => {
      void deleteSelectedRole(roleName);
    },
  });
}

async function deleteSelectedRole(roleName: string): Promise<void> {
  if (!selectedRole.value || selectedRole.value.isProtected) return;
  isDeletingRole.value = true;
  errorMessage.value = '';
  try {
    await apiClient.delete(`${apiEndpoints.admin}/roles/${selectedRole.value.id}`);
    selectedRole.value = null;
    roleForm.description = '';
    roleForm.permissions = [];
    showSuccess('Rol silindi', `${roleName} rolu kaldirildi.`);
    await loadAdminData();
  } catch (error: any) {
    handleRequestError(error, 'Rol silinemedi.');
  } finally {
    isDeletingRole.value = false;
  }
}

function showSuccess(summary: string, detail: string): void {
  toast.add({ severity: 'success', summary, detail, life: 3200 });
}

function handleRequestError(error: any, fallback: string): void {
  const detail = error?.response?.data?.detail ?? fallback;
  errorMessage.value = detail;
  toast.add({ severity: 'error', summary: 'Islem basarisiz', detail, life: 4200 });
}

function formatDate(value: string): string {
  return new Intl.DateTimeFormat('tr-TR', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
  }).format(new Date(value));
}
</script>

<style scoped>
.admin-page { position:relative; display:flex; flex-direction:column; gap:0.9rem }
.admin-page.is-busy { min-height:18rem }
.loading-layer { position:absolute; inset:0; z-index:5; display:flex; align-items:center; justify-content:center; gap:0.55rem; border-radius:8px; background:rgba(248,250,252,0.72); color:var(--app-text); font-size:0.82rem; font-weight:700; backdrop-filter:blur(2px) }
.loading-layer .pi { color:var(--app-primary); font-size:1rem }
.admin-head { display:flex; align-items:flex-start; justify-content:space-between; gap:1rem }
.admin-head h1 { margin:0; font-size:1.35rem; line-height:1.2; color:var(--app-text) }
.admin-head p { margin:0.2rem 0 0; color:var(--app-text-muted); font-size:0.8125rem }
.eyebrow { margin:0 0 0.2rem !important; color:var(--app-primary) !important; font-size:0.7rem !important; font-weight:700; text-transform:uppercase; letter-spacing:0.04em }
.metric-grid { display:grid; grid-template-columns:repeat(6,minmax(0,1fr)); gap:0.65rem }
.metric-card { min-height:4rem; border:1px solid var(--app-border); border-radius:8px; background:var(--app-surface); padding:0.7rem; display:flex; align-items:center; gap:0.65rem }
.metric-icon { width:2rem; height:2rem; border-radius:8px; display:grid; place-items:center; color:var(--app-primary); background:rgba(37,99,235,0.07) }
.metric-icon .pi { font-size:0.9rem }
.metric-card strong { display:block; font-size:1.15rem; line-height:1.1; color:var(--app-text) }
.metric-card span:last-child { display:block; margin-top:0.15rem; font-size:0.75rem; color:var(--app-text-muted) }
.admin-data-grid { display:grid; grid-template-columns:repeat(2,minmax(0,1fr)); gap:0.85rem }
.admin-section { border:1px solid var(--app-border); border-radius:8px; background:var(--app-surface); padding:0.95rem }
.section-head { display:flex; align-items:flex-start; justify-content:space-between; gap:1rem; margin-bottom:0.8rem }
.section-head.compact { margin-bottom:0.6rem }
.section-head h2,
.section-head h3,
.role-create-form h3 { margin:0; font-size:0.95rem; color:var(--app-text) }
.section-head p,
.role-create-form p { margin:0.25rem 0 0; color:var(--app-text-muted); font-size:0.78rem }
.table-tools { display:flex; align-items:center; gap:0.5rem }
.table-tools input,
.table-tools select,
.field input,
.field select { height:2.2rem; border:1px solid var(--app-border); border-radius:7px; padding:0 0.65rem; background:var(--app-surface); color:var(--app-text); font-size:0.8125rem }
.field input:disabled { background:var(--app-surface-alt); color:var(--app-text-muted) }
.table-tools input { width:14rem }
.admin-table-wrap { overflow:auto; border:1px solid var(--app-border); border-radius:8px }
.admin-table { width:100%; border-collapse:collapse; min-width:780px }
.admin-table th { text-align:left; padding:0.65rem; border-bottom:1px solid var(--app-border); color:var(--app-text-muted); font-size:0.72rem; font-weight:700 }
.admin-table td { padding:0.65rem; border-bottom:1px solid rgba(15,23,42,0.06); vertical-align:middle; font-size:0.8125rem }
.admin-table tr:last-child td { border-bottom:0 }
.admin-table tr.selected td { background:#eff6ff }
.admin-table td strong { display:block; font-size:0.8125rem; color:var(--app-text) }
.admin-table td span { display:block; margin-top:0.15rem; color:var(--app-text-muted); font-size:0.72rem }
.role-stack { display:flex; flex-wrap:wrap; gap:0.25rem }
.role-chip { display:inline-flex; align-items:center; min-height:1.35rem; border:1px solid var(--app-border); border-radius:999px; padding:0 0.45rem; background:var(--app-surface-alt); color:var(--app-text); font-size:0.7rem; font-weight:600 }
.status-pill { display:inline-flex !important; align-items:center; width:max-content; min-height:1.45rem; border-radius:999px; padding:0 0.5rem; font-size:0.72rem !important; font-weight:700 }
.status-pill.is-active { background:#ecfdf5; color:#047857 }
.status-pill.is-inactive { background:#fef2f2; color:#b91c1c }
.actions-cell { text-align:right; white-space:nowrap }
.icon-button { width:1.9rem; height:1.9rem; border:0; border-radius:7px; display:inline-grid; place-items:center; background:transparent; color:var(--app-text-muted); cursor:pointer }
.icon-button:hover:not(:disabled) { background:var(--app-surface-alt); color:var(--app-primary) }
.icon-button:disabled { opacity:0.5; cursor:not-allowed }
.icon-button .pi { font-size:0.82rem }
.danger-icon-button { color:#b91c1c }
.danger-icon-button:hover:not(:disabled) { background:#fef2f2; color:#991b1b }
.danger-button { min-height:2.2rem; display:inline-flex; align-items:center; justify-content:center; gap:0.4rem; border:1px solid #fecaca; border-radius:7px; padding:0 0.7rem; background:#fef2f2; color:#b91c1c; font-size:0.8125rem; font-weight:700; cursor:pointer }
.danger-button:hover:not(:disabled) { background:#fee2e2; color:#991b1b }
.danger-button:disabled { opacity:0.65; cursor:not-allowed }
.danger-button .pi { font-size:0.78rem }
.empty-cell { text-align:center; color:var(--app-text-muted); padding:1.4rem !important }
.admin-form { display:grid; grid-template-columns:repeat(2,minmax(0,1fr)); gap:0.8rem }
.field { display:flex; flex-direction:column; gap:0.35rem; font-size:0.75rem; color:var(--app-text-muted) }
.toggle-row { display:flex; align-items:center; gap:0.5rem; min-height:2.2rem; font-size:0.8125rem; color:var(--app-text) }
.role-fieldset { grid-column:1/-1; border:1px solid var(--app-border); border-radius:8px; padding:0.75rem; display:grid; grid-template-columns:repeat(2,minmax(0,1fr)); gap:0.55rem }
.role-fieldset legend { padding:0 0.35rem; color:var(--app-text-muted); font-size:0.75rem; font-weight:700 }
.role-option { display:flex; align-items:flex-start; gap:0.5rem; border:1px solid var(--app-border); border-radius:8px; padding:0.65rem; background:var(--app-surface) }
.role-option strong { display:block; font-size:0.8rem; color:var(--app-text) }
.role-option small { display:block; margin-top:0.15rem; color:var(--app-text-muted); font-size:0.72rem; line-height:1.35 }
.form-actions { grid-column:1/-1; display:flex; justify-content:flex-end; gap:0.5rem }
.form-hint { grid-column:1/-1; margin:0; border:1px solid var(--app-border); border-radius:7px; background:var(--app-surface-alt); padding:0.6rem 0.7rem; color:var(--app-text-muted); font-size:0.76rem; line-height:1.4 }
.role-manager { display:grid; grid-template-columns:minmax(12rem,16rem) 1fr; gap:0.85rem }
.role-list { display:flex; flex-direction:column; gap:0.35rem }
.role-list-item { text-align:left; border:1px solid var(--app-border); border-radius:8px; background:var(--app-surface); padding:0.65rem; cursor:pointer }
.role-list-item:hover,
.role-list-item.selected { border-color:rgba(37,99,235,0.45); background:#eff6ff }
.role-list-item strong { display:block; font-size:0.82rem; color:var(--app-text) }
.role-list-item span { display:block; margin-top:0.15rem; color:var(--app-text-muted); font-size:0.72rem }  
.role-editor { display:flex; flex-direction:column; gap:0.75rem }
.role-create-form { display:grid; grid-template-columns:minmax(12rem, 0.8fr) minmax(12rem, 1fr) minmax(14rem, 1.3fr) auto; align-items:start; gap:0.65rem; border:1px solid var(--app-border); border-radius:8px; padding:0.75rem }
.role-create-form .ui-button { align-self:start; margin-top:1.45rem; min-height:2.2rem; white-space:nowrap }
.permission-editor { border:1px solid var(--app-border); border-radius:8px; padding:0.75rem }
.permission-options { display:grid; grid-template-columns:repeat(3,minmax(0,1fr)); gap:0.45rem; margin-top:0.75rem }
.permission-option { display:flex; align-items:center; gap:0.45rem; min-height:2.1rem; border:1px solid var(--app-border); border-radius:7px; padding:0.45rem 0.55rem; color:var(--app-text); font-size:0.75rem }
.permission-option:has(input:checked) { border-color:rgba(37,99,235,0.45); background:#eff6ff; color:var(--app-primary); font-weight:700 }
.empty-panel { display:grid; place-items:center; min-height:10rem; border:1px dashed var(--app-border); border-radius:8px; color:var(--app-text-muted); font-size:0.8rem }
.compact-list {
  display:grid;
  gap:0.45rem;
  max-height:22rem;
  overflow:auto;
  padding-right:0.2rem;
  scrollbar-width:thin;
  scrollbar-color:rgba(148,163,184,0.9) transparent;
}
.compact-list .empty-panel { min-height:7rem }
.compact-list-item { display:flex; align-items:center; justify-content:space-between; gap:0.75rem; min-height:3.2rem; border:1px solid var(--app-border); border-radius:8px; padding:0.6rem 0.7rem; background:var(--app-surface) }
.compact-list-item strong { display:block; color:var(--app-text); font-size:0.8rem; line-height:1.25 }
.compact-list-item span { display:block; margin-top:0.15rem; color:var(--app-text-muted); font-size:0.72rem; line-height:1.25 }
.compact-list-item time { flex:0 0 auto; color:var(--app-text-muted); font-size:0.72rem; white-space:nowrap }
.compact-list::-webkit-scrollbar { width:8px }
.compact-list::-webkit-scrollbar-track { background:transparent }
.compact-list::-webkit-scrollbar-thumb {
  background:rgba(148,163,184,0.85);
  border-radius:999px;
  border:2px solid transparent;
  background-clip:padding-box;
}
.compact-list::-webkit-scrollbar-thumb:hover { background:rgba(100,116,139,0.95) }
.modal-backdrop { position:fixed; inset:0; z-index:1000; display:grid; place-items:center; padding:1rem; background:rgba(15,23,42,0.36); backdrop-filter:blur(3px) }
.user-modal { width:min(100%,42rem); max-height:calc(100vh - 2rem); overflow:auto; border:1px solid var(--app-border); border-radius:8px; background:var(--app-surface); box-shadow:0 24px 70px rgba(15,23,42,0.22); padding:0.95rem }
.user-modal .admin-form { grid-template-columns:repeat(2,minmax(0,1fr)) }
.password-modal { width:min(100%,34rem) }


@media (max-width: 1200px) {
  .metric-grid { grid-template-columns:repeat(3,minmax(0,1fr)) }
  .role-create-form { grid-template-columns:1fr 1fr }
  .role-create-form > div:first-child { grid-column:1/-1 }
  .role-create-form .ui-button { width:max-content; justify-self:end }
  .permission-options { grid-template-columns:repeat(2,minmax(0,1fr)) }
}

@media (max-width: 760px) {
  .admin-head,
  .section-head,
  .table-tools { flex-direction:column; align-items:stretch }
  .metric-grid,
  .admin-data-grid,
  .admin-form,
  .role-fieldset,
  .role-manager,
  .role-create-form,
  .permission-options { grid-template-columns:1fr }
  .table-tools input { width:100% }
  .role-create-form .ui-button { width:100%; justify-self:stretch }
  .user-modal .admin-form { grid-template-columns:1fr }
}
</style>
