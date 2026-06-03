<template>
  <main class="public-panel-page">
    <section class="public-shell">
      <header class="public-header">
        <div class="public-header__mark">
          <i class="pi pi-bolt" aria-hidden="true"></i>
        </div>
        <div>
          <p class="public-eyebrow">PanelDocs</p>
          <h1>{{ panel.name }}</h1>
          <p>{{ panel.code }} · {{ facility.name }}</p>
        </div>
      </header>

      <section class="panel-summary">
        <div>
          <span>Proje</span>
          <strong>{{ company.projectName }}</strong>
        </div>
        <div>
          <span>Konum</span>
          <strong>{{ facility.city }}</strong>
        </div>
        <div>
          <span>Pano Kodu</span>
          <strong>{{ panel.code }}</strong>
        </div>
      </section>

      <section class="document-list" aria-label="Panel dosyaları">
        <article v-for="card in documentCards" :key="card.key" class="document-card">
          <div class="document-card__icon">
            <i :class="card.icon" aria-hidden="true"></i>
          </div>

          <div class="document-card__content">
            <div class="document-card__head">
              <div>
                <h2>{{ card.title }}</h2>
                <p>{{ card.description }}</p>
              </div>
              <span class="document-card__count">{{ card.count }} dosya</span>
            </div>

            <button class="download-button" type="button" :disabled="card.count === 0" @click="downloadDocument(card)">
              <i class="pi pi-download" aria-hidden="true"></i>
              İndir
            </button>
          </div>
        </article>
      </section>

      <footer class="public-footer">
        <span>QR erişim bağlantısı</span>
        <strong>{{ panel.code }}</strong>
      </footer>
    </section>
  </main>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useRoute } from 'vue-router';
import mock from '@/mocks/mock-data';
import { useFileStore } from '@/stores/file-store';

type PublicDocumentCard = {
  key: string;
  title: string;
  description: string;
  icon: string;
  count: number;
};

const route = useRoute();
const fileStore = useFileStore();

const panel = computed(() => {
  const panelCode = String(route.params.panelCode ?? '');
  return mock.panels.find((item) => item.code === panelCode) ?? mock.panels[0];
});

const facility = computed(() => mock.facilities.find((item) => item.id === panel.value.facilityId) ?? mock.facilities[0]);
const company = computed(() => mock.companies.find((item) => item.id === facility.value.companyId) ?? mock.companies[0]);
const uploadedFileCount = computed(() => fileStore.getPanelFileCount(panel.value.id));

const documentCards = computed<PublicDocumentCard[]>(() => [
  {
    key: 'electrical-project',
    title: 'Elektrik Projesi',
    description: 'Tek hat ve proje dosyaları',
    icon: 'pi pi-sitemap',
    count: 1,
  },
  {
    key: 'maintenance-report',
    title: 'Bakım Raporu',
    description: 'Periyodik bakım kayıtları',
    icon: 'pi pi-wrench',
    count: 0,
  },
  {
    key: 'panel-documents',
    title: 'Pano Dokümanları',
    description: 'Yüklenen teknik dosyalar',
    icon: 'pi pi-file',
    count: uploadedFileCount.value,
  },
]);

function downloadDocument(card: PublicDocumentCard) {
  const fileContent = [
    card.title,
    `Pano: ${panel.value.name}`,
    `Kod: ${panel.value.code}`,
    `Proje: ${company.value.projectName}`,
  ].join('\n');
  const blob = new Blob([fileContent], { type: 'text/plain;charset=utf-8' });
  const url = URL.createObjectURL(blob);
  const link = document.createElement('a');

  link.href = url;
  link.download = `${panel.value.code}-${card.key}.txt`;
  link.click();
  URL.revokeObjectURL(url);
}
</script>

<style scoped>
.public-panel-page {
  min-height: 100vh;
  background: #f5f7fb;
  color: var(--app-text);
  padding: 1rem;
}

.public-shell {
  width: min(100%, 30rem);
  margin: 0 auto;
}

.public-header {
  display: flex;
  gap: 0.75rem;
  align-items: center;
  padding: 0.5rem 0 1rem;
}

.public-header__mark {
  width: 2.25rem;
  height: 2.25rem;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  border-radius: 10px;
  background: var(--primary-50);
  color: var(--app-primary);
  flex: 0 0 auto;
}

.public-header__mark .pi {
  font-size: 1rem;
}

.public-eyebrow {
  margin: 0 0 0.1rem;
  font-size: 0.6875rem;
  line-height: 1.2;
  color: var(--app-primary);
  font-weight: 700;
  text-transform: uppercase;
}

.public-header h1 {
  margin: 0;
  font-size: 1.25rem;
  line-height: 1.15;
  font-weight: 800;
}

.public-header p:last-child {
  margin: 0.15rem 0 0;
  font-size: 0.8125rem;
  line-height: 1.35;
  color: var(--app-text-muted);
}

.panel-summary {
  display: grid;
  grid-template-columns: 1fr;
  gap: 0.65rem;
  margin-bottom: 0.85rem;
}

.panel-summary div,
.document-card {
  background: var(--app-bg);
  border: 1px solid var(--app-border);
  border-radius: 10px;
  box-shadow: var(--app-shadow);
}

.panel-summary div {
  padding: 0.75rem 0.85rem;
}

.panel-summary span {
  display: block;
  font-size: 0.6875rem;
  line-height: 1.2;
  color: var(--app-text-muted);
}

.panel-summary strong {
  display: block;
  margin-top: 0.12rem;
  font-size: 0.8125rem;
  line-height: 1.2;
}

.document-list {
  display: grid;
  gap: 0.75rem;
}

.document-card {
  display: flex;
  gap: 0.75rem;
  padding: 0.85rem;
}

.document-card__icon {
  width: 2rem;
  height: 2rem;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  border-radius: 8px;
  background: var(--primary-50);
  color: var(--app-primary);
  flex: 0 0 auto;
}

.document-card__icon .pi {
  font-size: 0.95rem;
}

.document-card__content {
  flex: 1;
  min-width: 0;
}

.document-card__head {
  display: flex;
  justify-content: space-between;
  gap: 0.75rem;
}

.document-card h2 {
  margin: 0;
  font-size: 0.9rem;
  line-height: 1.25;
  font-weight: 800;
}

.document-card p {
  margin: 0.2rem 0 0;
  color: var(--app-text-muted);
  font-size: 0.75rem;
  line-height: 1.3;
}

.document-card__count {
  align-self: flex-start;
  border: 1px solid var(--app-border);
  border-radius: 999px;
  padding: 0.18rem 0.5rem;
  color: var(--app-text);
  font-size: 0.6875rem;
  line-height: 1.2;
  font-weight: 700;
  white-space: nowrap;
}

.download-button {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 0.4rem;
  margin-top: 0.75rem;
  min-height: 2rem;
  padding: 0.45rem 0.75rem;
  border: 0;
  border-radius: 8px;
  background: var(--app-primary);
  color: var(--color-white);
  font-size: 0.8125rem;
  line-height: 1.2;
  font-weight: 700;
  cursor: pointer;
}

.download-button:disabled {
  cursor: not-allowed;
  background: var(--app-surface-alt);
  color: var(--app-text-muted);
}

.download-button .pi {
  font-size: 0.85rem;
}

.public-footer {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 0.75rem;
  padding: 1rem 0.25rem 0;
  color: var(--app-text-muted);
  font-size: 0.75rem;
}

.public-footer strong {
  color: var(--app-text);
}

@media (min-width: 720px) {
  .public-panel-page {
    padding: 2rem;
  }

  .panel-summary {
    grid-template-columns: repeat(3, 1fr);
  }
}
</style>
