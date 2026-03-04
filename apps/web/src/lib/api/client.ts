const API_BASE_URL =
  process.env.NODE_ENV === 'production' ? 'http://api:8000' : 'http://127.0.0.1:8000';

export async function fetchApi<T>(endpoint: string, options: RequestInit = {}): Promise<T> {
  const url = `${API_BASE_URL}${endpoint}`;
  const response = await fetch(url, {
    ...options,
    headers: {
      'Content-Type': 'application/json',
      ...options.headers,
    },
  });

  if (!response.ok) {
    throw new Error(`HTTP Error: ${response.status} at ${url}`);
  }

  return response.json() as T;
}
