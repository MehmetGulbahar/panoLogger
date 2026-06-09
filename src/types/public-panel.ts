export interface PublicPanelFileResponse {
  id: string;
  category: string;
  fileName: string;
  contentType: string;
  sizeBytes: number;
  viewUrl: string;
  downloadUrl: string;
}

export interface PublicPanelFileCategoryResponse {
  key: string;
  name: string;
  description: string;
  icon: string;
  sortOrder: number;
}

export interface PublicPanelDocumentsResponse {
  electricalProjectCount: number;
  maintenanceReportCount: number;
  panelDocumentCount: number;
  categoryCounts: Record<string, number>;
  categories: PublicPanelFileCategoryResponse[];
  files: PublicPanelFileResponse[];
}

export interface PublicPanelResponse {
  panelId: string;
  panelCode: string;
  panelName: string;
  panelDescription: string;
  facilityId: string;
  facilityName: string;
  city: string;
  companyId: string;
  companyName: string;
  projectName: string;
  documents: PublicPanelDocumentsResponse;
}
