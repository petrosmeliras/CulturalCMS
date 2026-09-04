import {api} from "@/shared/api/apiClient.ts";
import type {LoginResponse} from "../types";
import type {LoginFields, UserSignupFormData} from "../schemas/auth";

export async function login(credentials: LoginFields): Promise<LoginResponse> {
  const response = await api.post<LoginResponse>("/auth/login", credentials);
  return response.data;
}
export async function registerUser(data: UserSignupFormData):Promise<void> {
  await api.post("/auth/register", data);
}