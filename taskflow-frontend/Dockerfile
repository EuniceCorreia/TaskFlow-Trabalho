# ---------- Etapa 1: build do React (Vite) ----------
FROM node:22-alpine AS build
WORKDIR /app

COPY package.json package-lock.json ./
RUN npm ci

COPY . .
RUN npm run build

# ---------- Etapa 2: nginx sem root ----------
# Imagem oficial "unprivileged": roda como usuário 101 na porta 8080
FROM nginxinc/nginx-unprivileged:stable-alpine

# Atualiza os pacotes do Alpine para as versões com correções de segurança
# (a imagem base pode ter sido publicada antes das correções saírem)
USER root
RUN apk upgrade --no-cache

COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist /usr/share/nginx/html

USER 101
EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=3s \
  CMD wget -qO- http://127.0.0.1:8080/ >/dev/null || exit 1