import { fetchApi } from '@/lib/api/client';
import type { Point } from '@/types/chart';

export async function getSparklineData(): Promise<Point[]> {
  return fetchApi<Point[]>('/api/sparkline', { cache: 'no-store' });
}
