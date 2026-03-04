import type { Metadata } from 'next';
import localFont from 'next/font/local';
import './globals.css';

const geistSans = localFont({
  src: '../fonts/Geist[wght].woff2',
  variable: '--font-geist-sans',
  weight: '100 900',
  preload: false,
});

const geistMono = localFont({
  src: '../fonts/GeistMono[wght].woff2',
  variable: '--font-geist-mono',
  weight: '100 900',
  preload: false,
});

export const metadata: Metadata = {
  title: 'Degen',
  description: 'Degen app',
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en">
      <body className={`${geistSans.variable} ${geistMono.variable} font-sans antialiased`}>
        {children}
      </body>
    </html>
  );
}
