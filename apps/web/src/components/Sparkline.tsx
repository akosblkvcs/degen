'use client';

import { useEffect, useRef } from 'react';
import { createChart, AreaSeries, ColorType, type Time } from 'lightweight-charts';
import type { Point } from '@/types/chart';

type SparklineProps = {
  data: Point[];
};

export default function Sparkline({ data }: SparklineProps) {
  const ref = useRef<HTMLDivElement | null>(null);

  useEffect(() => {
    if (!ref.current) return;

    const chart = createChart(ref.current, {
      width: ref.current.clientWidth,
      height: 140,
      layout: {
        attributionLogo: false,
        background: { type: ColorType.Solid, color: 'transparent' },
        textColor: 'transparent',
      },
      grid: { vertLines: { visible: false }, horzLines: { visible: false } },
      leftPriceScale: { visible: false },
      rightPriceScale: { visible: false },
      timeScale: { visible: false, borderVisible: false },
      crosshair: { vertLine: { visible: false }, horzLine: { visible: false } },
      handleScroll: false,
      handleScale: false,
    });

    const series = chart.addSeries(AreaSeries, {
      lineWidth: 2,
      lineColor: 'rgba(110, 231, 183, 1)',
      topColor: 'rgba(110, 231, 183, 0.35)',
      bottomColor: 'rgba(110, 231, 183, 0.0)',
      priceLineVisible: false,
      lastValueVisible: false,
    });

    series.setData(
      data.map((p) => ({
        time: p.time as unknown as Time,
        value: p.value,
      })),
    );

    chart.timeScale().fitContent();

    const onResize = () => {
      if (!ref.current) return;
      chart.applyOptions({ width: ref.current.clientWidth });
    };

    window.addEventListener('resize', onResize);

    return () => {
      window.removeEventListener('resize', onResize);
      chart.remove();
    };
  }, [data]);

  return <div ref={ref} className="w-full overflow-hidden rounded-lg" />;
}
