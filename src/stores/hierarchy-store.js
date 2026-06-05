import { defineStore } from 'pinia';
import { computed, ref } from 'vue';
import { apiClient } from '@/api/client';
import { apiEndpoints } from '@/api/endpoints';
export const useHierarchyStore = defineStore('hierarchy', () => {
    const companies = ref([]);
    const isLoading = ref(false);
    const isLoaded = ref(false);
    const facilities = computed(() => companies.value.flatMap(company => company.facilities));
    const panels = computed(() => facilities.value.flatMap(facility => facility.panels));
    async function load(force = false) {
        if (isLoaded.value && !force)
            return;
        isLoading.value = true;
        try {
            const { data } = await apiClient.get(`${apiEndpoints.hierarchy}/`);
            companies.value = data;
            isLoaded.value = true;
        }
        finally {
            isLoading.value = false;
        }
    }
    async function createSystem(request) {
        const { data } = await apiClient.post(`${apiEndpoints.hierarchy}/systems`, request);
        companies.value = [...companies.value, data].sort((a, b) => a.projectName.localeCompare(b.projectName, 'tr'));
        return data;
    }
    async function updateCompany(companyId, request) {
        await apiClient.put(`${apiEndpoints.hierarchy}/companies/${companyId}`, request);
        await load(true);
    }
    async function deleteCompany(companyId) {
        await apiClient.delete(`${apiEndpoints.hierarchy}/companies/${companyId}`);
        companies.value = companies.value.filter(company => company.id !== companyId);
    }
    async function updateFacility(facilityId, request) {
        await apiClient.put(`${apiEndpoints.hierarchy}/facilities/${facilityId}`, request);
        await load(true);
    }
    async function createFacility(request) {
        await apiClient.post(`${apiEndpoints.hierarchy}/facilities`, request);
        await load(true);
    }
    async function deleteFacility(facilityId) {
        await apiClient.delete(`${apiEndpoints.hierarchy}/facilities/${facilityId}`);
        await load(true);
    }
    async function updatePanel(panelId, request) {
        await apiClient.put(`${apiEndpoints.hierarchy}/panels/${panelId}`, request);
        await load(true);
    }
    async function createPanel(request) {
        await apiClient.post(`${apiEndpoints.hierarchy}/panels`, request);
        await load(true);
    }
    async function deletePanel(panelId) {
        await apiClient.delete(`${apiEndpoints.hierarchy}/panels/${panelId}`);
        await load(true);
    }
    function reset() {
        companies.value = [];
        isLoaded.value = false;
        isLoading.value = false;
    }
    return { companies, facilities, panels, isLoading, isLoaded, load, createSystem, updateCompany, deleteCompany, updateFacility, createFacility, deleteFacility, updatePanel, createPanel, deletePanel, reset };
});
