import { api } from "@/shared/api/apiClient";
import type {PaginatedResult} from "@/shared/types";
import type { User, UserRole} from "@/features/users/types";

export async function getAllUsers(pageNumber = 1, pageSize = 10, username?: string
): Promise<PaginatedResult<User>>{
  const response = await api.get<PaginatedResult<User>>("/users", {
    params: { pageNumber, pageSize, username : username || undefined }
  });
  return response.data;
}

export async function updateUserRole(userId: number, roleName: UserRole): Promise<User> {
  const response = await api.put<User>(`/users/${userId}/role`, { roleName });
  return response.data;
}