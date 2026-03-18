# ── Build stage ──────────────────────────────────────────
FROM node:24-alpine AS build
WORKDIR /app

RUN corepack enable && corepack prepare pnpm@10.32.1 --activate

COPY package.json pnpm-lock.yaml ./
RUN pnpm install --frozen-lockfile

COPY . .
RUN pnpm run build --configuration=production

# ── Runtime stage ────────────────────────────────────────
FROM nginxinc/nginx-unprivileged:alpine AS runtime

COPY --from=build /app/dist/degen-ui/browser /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 8080
