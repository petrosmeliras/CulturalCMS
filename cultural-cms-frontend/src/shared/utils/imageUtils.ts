export function getImageUrl(imageUrl: string | undefined | null): string | null {
  const cleanUrl= imageUrl?.trim();
  if (!cleanUrl) {
    return null;
  }

  const API_BASE = import.meta.env.VITE_API_URL?.replace('/api/v1', '') || 'http://localhost:8080';

  if (cleanUrl.startsWith('/')) {
    return `${API_BASE}${cleanUrl}`;
  }
  return cleanUrl;
}