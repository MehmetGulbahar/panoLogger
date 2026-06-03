export interface IdNameDto {
  id: string;
  name: string;
}

export interface AuditFieldsDto {
  createdAt: string;
  createdBy?: string | null;
  updatedAt?: string | null;
  updatedBy?: string | null;
}