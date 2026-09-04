import axios from "axios";
import {getCookie} from "@/shared/utils/cookies";
import t from "@/locales/el"

export const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL
});

api.interceptors.request.use(
  (config) => {
    const token = getCookie("access_token");
    if (token && config.headers) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      console.warn(t.auth.tokenExpired);

      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);