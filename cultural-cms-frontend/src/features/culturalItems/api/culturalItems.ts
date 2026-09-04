import type {AuditLog, CulturalItem, ItemSearchParams} from "@/features/culturalItems/types/domain";
import {api} from "@/shared/api/apiClient";
import type {CulturalItemFormData} from "@/features/culturalItems/schemas/culturalItem.ts";
import type {PaginatedResult} from "@/shared/types";

export async function getCulturalItemById(id: number): Promise<CulturalItem> {
  const response = await api.get<CulturalItem>(`/cultural-items/${id}`);
  return response.data;
}

export async function createCulturalItem(data: CulturalItemFormData): Promise<CulturalItem> {
  const response = await api.post<CulturalItem>("/cultural-items", data);
  return response.data;
}

export async function updateItem(id: number, data: CulturalItemFormData): Promise<void> {
  await api.put<CulturalItem>(`/cultural-items/${id}`, data);
}

export async function deleteCulturalItem(id: number): Promise<void> {
  await api.delete<CulturalItem>(`/cultural-items/${id}`);
}

export async function submitItemForReview(id: number): Promise<void> {
  await api.post<CulturalItem>(`/cultural-items/${id}/submit`);
}

export async function approveItem(id: number): Promise<void> {
  await api.post<CulturalItem>(`/cultural-items/${id}/approve`);
}

export async function rejectItem(id: number): Promise<void> {
  await api.post<CulturalItem>(`/cultural-items/${id}/reject`);
}

export async function searchMyItems(query: ItemSearchParams): Promise<PaginatedResult<CulturalItem>> {
  const response =
    await api.get<PaginatedResult<CulturalItem>>("/cultural-items/my-items", { params: query });
  return response.data;
}

export async function searchCulturalItems(query: ItemSearchParams): Promise<PaginatedResult<CulturalItem>> {
  const response =
    await api.get<PaginatedResult<CulturalItem>>("/cultural-items/search", { params: query });
  return response.data;
}

export async function searchCulturalItemsAllStatuses(query: ItemSearchParams): Promise<PaginatedResult<CulturalItem>> {
  const response =
    await api.get<PaginatedResult<CulturalItem>>("/cultural-items/search/all", { params: query });
  return response.data;
}

export async function getAuditLogs(itemId: number): Promise<AuditLog[]> {
  const response = await api.get<AuditLog[]>(`/cultural-items/${itemId}/audit-logs`);
  return response.data;
}