export interface PanelFileResponse {
  id: string;
  panelId: string;
  category: string;
  fileName: string;
  contentType: string;
  sizeBytes: number;
  createdAtUtc: string;
}

export interface FileDownloadResponse {
  signedUrl: string;
  expiresAtUtc: string;
}
