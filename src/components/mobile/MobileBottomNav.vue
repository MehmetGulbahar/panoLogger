<template>
  <nav class="mobile-bottom-nav" role="navigation" aria-label="Mobile navigation">
    <RouterLink to="/" class="nav-item" aria-label="Ana Sayfa">
      <span class="icon pi pi-home" aria-hidden="true"></span>
      <span class="label">Ana</span>
    </RouterLink>
    <RouterLink to="/companies" class="nav-item" aria-label="Şirketler">
      <span class="icon pi pi-folder" aria-hidden="true"></span>
      <span class="label">Şirket</span>
    </RouterLink>
    <RouterLink to="/facilities" class="nav-item" aria-label="Tesisler">
      <span class="icon pi pi-building" aria-hidden="true"></span>
      <span class="label">Tesis</span>
    </RouterLink>
    <RouterLink to="/panels" class="nav-item" aria-label="Paneller">
      <span class="icon pi pi-th-large" aria-hidden="true"></span>
      <span class="label">Paneller</span>
    </RouterLink>
  </nav>
</template>

<script setup lang="ts">
</script>

<style scoped>
.mobile-bottom-nav {
  display: none;
}

@media (max-width: 960px) {
  .mobile-bottom-nav {
    display: flex;
    position: fixed;
    left: 0;
    right: 0;
    bottom: 0;
    height: 52px;
    background: var(--app-surface);
    border-top: 1px solid var(--app-border);
    align-items: center;
    justify-content: space-around;
    z-index: 1300;
    padding: 0 0.75rem;
  }

  .mobile-bottom-nav .nav-item { display:flex; flex-direction:column; align-items:center; justify-content:center; gap:3px; min-width:3.5rem; height:100%; color:var(--app-text-muted); text-decoration:none }
  .mobile-bottom-nav .nav-item.router-link-active { color:var(--app-primary); font-weight:700 }
  .mobile-bottom-nav .icon { font-size:0.95rem }
  .mobile-bottom-nav .label { font-size:0.6875rem; line-height:1.1 }
}
</style>
