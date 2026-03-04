'use client';

export default function GlobalError({
  error,
  reset,
}: {
  error: Error & { digest?: string };
  reset: () => void;
}) {
  return (
    <html>
      <body>
        <h2>Fatal System Error: {error.message}</h2>

        <button onClick={() => reset()}>Hard Reset</button>
      </body>
    </html>
  );
}
