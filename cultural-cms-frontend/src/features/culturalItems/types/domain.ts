export type MetadataItem = {
  key: string;
  value: string;
};

export type CulturalItem = {
  id: number;
  title: string;
  description: string;
  category: string;
  historicalPeriod: string;
  status: string;
  viewCount: number;
  createdById: number;
  imageUrl?: string;
  createdAt: string;
  updatedAt?: string;
  metadata: MetadataItem[];
};

export type AuditLog = {
  id: number;
  action: string;
  entityName: string;
  entityId: number;
  userId: number;
  username: string;
  timestamp: string;
  oldValues?: string;
  newValues?: string;
  changedColumns?: string;
};

export type ItemSearchParams = {
  searchTerm?: string;
  category?: string;
  historicalPeriod?: string;
  status?: string;
  sortBy?: string;
  sortOrder?: string;
  pageNumber?: number;
  pageSize?: number;
  metadataKey?: string;
  metadataValue?: string;
};