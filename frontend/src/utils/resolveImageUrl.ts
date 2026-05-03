const apiBaseUrl = import.meta.env.VITE_API_URL || 'http://localhost:5245/api';

const stripApiSuffix = (baseUrl: string) => baseUrl.replace(/\/api\/?$/i, '');

const assetBaseUrl = stripApiSuffix(apiBaseUrl);

export const resolveImageUrl = (url?: string | null) => {
  if (!url) return null;
  if (/^(https?:)?\/\//i.test(url) || url.startsWith('data:') || url.startsWith('blob:')) {
    return url;
  }

  const normalizedPath = url.startsWith('/') ? url : `/${url}`;
  return `${assetBaseUrl}${normalizedPath}`;
};
