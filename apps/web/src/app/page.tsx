import Sparkline from '@/components/Sparkline';
import { getSparklineData } from '@/services/chart.service';

export default async function HomePage() {
  const data = await getSparklineData();

  return (
    <main className="mx-auto max-w-4xl p-6">
      <h1 className="mb-2 text-2xl font-semibold">Degen</h1>
      <p className="mb-4 text-sm text-zinc-400">Smoke test: sparkline from FastAPI.</p>

      <div className="rounded-xl border border-zinc-800 bg-zinc-900/50 p-4">
        <Sparkline data={data} />
      </div>
    </main>
  );
}
