# --- Etap 1: Build ---
FROM node:20-alpine AS build
WORKDIR /app

# Kopiuj package.json i zainstaluj zależności (cache layer)
COPY package*.json ./
RUN npm ci

# Kopiuj kod i zbuduj produkcyjnie
COPY . .
RUN npm run build -- --configuration production

# --- Etap 2: Nginx ---
FROM nginx:1.25-alpine
COPY --from=build /app/dist/in-league-frontend/browser /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
