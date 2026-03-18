import {
  AfterViewInit,
  Component,
  ElementRef,
  Input,
  OnChanges,
  OnDestroy,
  SimpleChanges,
  ViewChild,
} from '@angular/core';
import {
  CandlestickData,
  CandlestickSeries,
  ColorType,
  createChart,
  HistogramData,
  HistogramSeries,
  IChartApi,
  ISeriesApi,
  Time,
} from 'lightweight-charts';

export interface ChartCandle {
  time: string;
  open: number;
  high: number;
  low: number;
  close: number;
  volume: number;
}

@Component({
  selector: 'app-candlestick-chart',
  standalone: true,
  templateUrl: './candlestick-chart.html',
  styleUrl: './candlestick-chart.css',
})
export class CandlestickChartComponent implements AfterViewInit, OnChanges, OnDestroy {
  @ViewChild('chartContainer', { static: true })
  chartContainer!: ElementRef<HTMLDivElement>;

  @Input() candles: ChartCandle[] = [];
  @Input() symbol = '';

  private chart!: IChartApi;
  private candleSeries!: ISeriesApi<'Candlestick'>;
  private volumeSeries!: ISeriesApi<'Histogram'>;
  private hasInitialFit = false;

  ngAfterViewInit(): void {
    this.initChart();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (!this.candleSeries || !this.volumeSeries) {
      return;
    }

    const candlesChanged = !!changes['candles'];
    const symbolChanged = !!changes['symbol'] && !changes['symbol'].firstChange;

    if (candlesChanged) {
      const shouldFit = !this.hasInitialFit || symbolChanged;
      this.updateData(shouldFit);

      if (!this.hasInitialFit) {
        this.hasInitialFit = true;
      }

      return;
    }

    if (symbolChanged && this.candles.length > 0) {
      this.updateData(true);
    }
  }

  ngOnDestroy(): void {
    this.chart?.remove();
  }

  private initChart(): void {
    const container = this.chartContainer.nativeElement;

    this.chart = createChart(container, {
      autoSize: true,
      layout: {
        background: { type: ColorType.Solid, color: '#0a0a1a' },
        textColor: '#8888aa',
        fontSize: 12,
      },
      grid: {
        vertLines: { color: '#1a1a3e' },
        horzLines: { color: '#1a1a3e' },
      },
      crosshair: {
        vertLine: { color: '#3a3a6e', labelBackgroundColor: '#1e1e3e' },
        horzLine: { color: '#3a3a6e', labelBackgroundColor: '#1e1e3e' },
      },
      rightPriceScale: {
        borderColor: '#1a1a3e',
        scaleMargins: { top: 0.1, bottom: 0.25 },
      },
      timeScale: {
        borderColor: '#1a1a3e',
        timeVisible: true,
        secondsVisible: false,
      },
    });

    this.candleSeries = this.chart.addSeries(CandlestickSeries, {
      upColor: '#00d4aa',
      downColor: '#ff4976',
      borderVisible: false,
      wickUpColor: '#00d4aa',
      wickDownColor: '#ff4976',
    });

    this.volumeSeries = this.chart.addSeries(HistogramSeries, {
      priceFormat: { type: 'volume' },
      priceScaleId: 'volume',
    });

    this.chart.priceScale('volume').applyOptions({
      scaleMargins: { top: 0.8, bottom: 0 },
    });

    if (this.candles.length > 0) {
      this.updateData(true);
      this.hasInitialFit = true;
    }
  }

  private updateData(fitContent = false): void {
    const candleData: CandlestickData[] = this.candles.map(c => ({
      time: this.parseTime(c.time),
      open: c.open,
      high: c.high,
      low: c.low,
      close: c.close,
    }));

    const volumeData: HistogramData[] = this.candles.map(c => ({
      time: this.parseTime(c.time),
      value: c.volume,
      color: this.getVolumeColor(c),
    }));

    this.candleSeries.setData(candleData);
    this.volumeSeries.setData(volumeData);

    if (fitContent) {
      this.chart.timeScale().fitContent();
    }
  }

  private parseTime(timestamp: string): Time {
    return Math.floor(new Date(timestamp).getTime() / 1000) as Time;
  }

  private getVolumeColor(candle: ChartCandle): string {
    return candle.close >= candle.open ? 'rgba(0, 212, 170, 0.3)' : 'rgba(255, 73, 118, 0.3)';
  }

  appendCandle(candle: ChartCandle): void {
    this.candleSeries.update({
      time: this.parseTime(candle.time),
      open: candle.open,
      high: candle.high,
      low: candle.low,
      close: candle.close,
    });

    this.volumeSeries.update({
      time: this.parseTime(candle.time),
      value: candle.volume,
      color: this.getVolumeColor(candle),
    });
  }
}
