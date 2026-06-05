import { defineStore } from 'pinia';
import { computed, ref } from 'vue';

import { apiClient } from '@/api/client';
import { apiEndpoints } from '@/api/endpoints';
import type {
  CreateSystemRequest,
  CreateFacilityRequest,
  CreatePanelRequest,
  HierarchyCompany,
  UpdateCompanyRequest,
  UpdateFacilityRequest,
  UpdatePanelRequest,
} from '@/types';

export const useHierarchyStore = defineStore('hierarchy', () => {
  const companies = ref<HierarchyCompany[]>([]);
  const isLoading = ref(false);
  const isLoaded = ref(false);

  const facilities = computed(() => companies.value.flatMap(company => company.facilities));
  const panels = computed(() => facilities.value.flatMap(facility => facility.panels));

  async function load(force = false): Promise<void> {
    if (isLoaded.value && !force) {
      return;
    }

    isLoading.value = true;
    try {
      const { data } = await apiClient.get<HierarchyCompany[]>(`${apiEndpoints.hierarchy}/`);
      companies.value = data;
      isLoaded.value = true;
    } finally {
      isLoading.value = false;
    }
  }

  async function createSystem(request: CreateSystemRequest): Promise<HierarchyCompany> {
    const { data } = await apiClient.post<HierarchyCompany>(`${apiEndpoints.hierarchy}/systems`, request);
    companies.value = [...companies.value, data].sort((a, b) => a.projectName.localeCompare(b.projectName, 'tr'));
    return data;
  }

  async function updateCompany(companyId: string, request: UpdateCompanyRequest): Promise<void> {
    await apiClient.put(`${apiEndpoints.hierarchy}/companies/${companyId}`, request);
    await load(true);
  }

  async function deleteCompany(companyId: string): Promise<void> {
    await apiClient.delete(`${apiEndpoints.hierarchy}/companies/${companyId}`);
    companies.value = companies.value.filter(company => company.id !== companyId);
  }

  async function updateFacility(facilityId: string, request: UpdateFacilityRequest): Promise<void> {
    await apiClient.put(`${apiEndpoints.hierarchy}/facilities/${facilityId}`, request);
    await load(true);
  }

  async function createFacility(request: CreateFacilityRequest): Promise<void> {
    await apiClient.post(`${apiEndpoints.hierarchy}/facilities`, request);
    await load(true);
  }

  async function deleteFacility(facilityId: string): Promise<void> {
    await apiClient.delete(`${apiEndpoints.hierarchy}/facilities/${facilityId}`);
    await load(true);
  }

  async function updatePanel(panelId: string, request: UpdatePanelRequest): Promise<void> {
    await apiClient.put(`${apiEndpoints.hierarchy}/panels/${panelId}`, request);
    await load(true);
  }

  async function createPanel(request: CreatePanelRequest): Promise<void> {
    await apiClient.post(`${apiEndpoints.hierarchy}/panels`, request);
    await load(true);
  }

  async function deletePanel(panelId: string): Promise<void> {
    await apiClient.delete(`${apiEndpoints.hierarchy}/panels/${panelId}`);
    await load(true);
  }

  function reset(): void {
    companies.value = [];
    isLoaded.value = false;
    isLoading.value = false;
  }

  return {
    companies,
    facilities,
    panels,
    isLoading,
    isLoaded,
    load,
    createSystem,
    updateCompany,
    deleteCompany,
    updateFacility,
    createFacility,
    deleteFacility,
    updatePanel,
    createPanel,
    deletePanel,
    reset,
  };
});
