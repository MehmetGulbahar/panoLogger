export interface PublicPanelFileResponse {
  id: string;
  category: string;
  fileName: string;
  contentType: string;
  sizeBytes: number;
  viewUrl: string;
  downloadUrl: string;
}

export interface PublicPanelDocumentsResponse {
  electricalProjectCount: number;
  maintenanceReportCount: number;
  panelDocumentCount: number;
  categoryCounts: Record<string, number>;
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
