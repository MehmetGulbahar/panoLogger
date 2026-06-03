<template>
  <section class="page-card app-card">
    <h1>Renk Paleti</h1>
    <p class="text-muted">Proje renk tokenlarının önizlemesi.</p>

    <div class="palette-grid">
      <section v-for="group in groups" :key="group.name" class="palette-group">
        <h3 class="mb-1">{{ group.label }}</h3>
        <div class="palette-row">
          <div
            v-for="shade in group.shades"
            :key="shade"
            class="swatch"
          >
            <div class="swatch__color" :style="{ background: `var(${shade})` }"></div>
            <div class="swatch__meta">
              <div class="swatch__name">{{ shade.replace('--','') }}</div>
              <div class="swatch__hex">{{ getHex(shade) }}</div>
            </div>
          </div>
        </div>
      </section>
    </div>
  </section>
</template>

<script setup lang="ts">
const groups = [
  {
    name: 'primary',
    label: 'Primary',
    shades: ['--primary-50','--primary-100','--primary-200','--primary-300','--primary-400','--primary-500','--primary-600','--primary-700','--primary-800','--primary-900']
  },
  {
    name: 'neutral',
    label: 'Neutral',
    shades: ['--neutral-50','--neutral-100','--neutral-200','--neutral-300','--neutral-400','--neutral-500','--neutral-600','--neutral-700','--neutral-800','--neutral-900']
  },
  {
    name: 'success',
    label: 'Success',
    shades: ['--success-50','--success-100','--success-200','--success-300','--success-400','--success-500','--success-600','--success-700','--success-800','--success-900']
  },
  {
    name: 'warning',
    label: 'Warning',
    shades: ['--warning-50','--warning-100','--warning-200','--warning-300','--warning-400','--warning-500','--warning-600','--warning-700','--warning-800','--warning-900']
  },
  {
    name: 'danger',
    label: 'Danger',
    shades: ['--danger-50','--danger-100','--danger-200','--danger-300','--danger-400','--danger-500','--danger-600','--danger-700','--danger-800','--danger-900']
  }
];

function getHex(varName: string) {
  try {
    const v = getComputedStyle(document.documentElement).getPropertyValue(varName).trim();
    return v || '—';
  } catch (e) {
    return '—';
  }
}
</script>

<style scoped>
.palette-grid {
  display:flex;
  flex-direction:column;
  gap:var(--space-lg);
  margin-top: var(--space-md);
}

.palette-group { background: transparent }
.palette-row { display:flex; gap:var(--space-md); flex-wrap:wrap }
.swatch { width:120px; display:flex; flex-direction:column; gap:var(--space-xs); align-items:flex-start; padding:var(--space-sm); border-radius:var(--radius-sm); background:var(--app-surface); border:1px solid var(--app-border) }
.swatch__color { width:100%; height:56px; border-radius:8px; box-shadow:var(--shadow-1) }
.swatch__meta { display:flex; flex-direction:column }
.swatch__name { font-weight:var(--fw-semibold); font-size:var(--type-scale-xs) }
.swatch__hex { font-size:var(--type-scale-xs); color:var(--app-text-muted) }
</style>
