import {api} from "@/shared/api/apiClient.ts";

export async function uploadImage(file: File): Promise<string>{
  const formData = new FormData();
  formData.append("File", file);

  const response = await api.post<{ imageUrl: string }>("/images/upload", formData);

  return response.data.imageUrl;
};