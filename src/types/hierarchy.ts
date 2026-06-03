export interface HierarchyPanel {
  id: string;
  facilityId: string;
  code: string;
  name: string;
  description: string;
}

export interface HierarchyFacility {
  id: string;
  companyId: string;
  name: string;
  city: string;
  district: string;
  address: string;
  panels: HierarchyPanel[];
}

export interface HierarchyCompany {
  id: string;
  name: string;
  projectName: string;
  taxNumber: string;
  address: string;
  contactEmail: string;
  facilities: HierarchyFacility[];
}

export interface CreateSystemRequest {
  projectName: string;
  companyName: string;
  taxNumber: string;
  companyAddress: string;
  contactEmail: string;
  facilityName: string;
  city: string;
  district: string;
  facilityAddress: string;
  panelCode: string;
  panelName: string;
  panelDescription: string;
}

export interface UpdateCompanyRequest {
  projectName: string;
  companyName: string;
  taxNumber: string;
  address: string;
  contactEmail: string;
}

export interface UpdateFacilityRequest {
  name: string;
  city: string;
  district: string;
  address: string;
}

export interface CreateFacilityRequest {
  companyId: string;
  name: string;
  city: string;
  district: string;
  address: string;
}

export interface UpdatePanelRequest {
  code: string;
  name: string;
  description: string;
}

export interface CreatePanelRequest {
  companyId: string;
  facilityId: string;
  code: string;
  name: string;
  description: string;
}
