# ---------- Build stage ----------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# copy everything and restore/build/publish
COPY . .
RUN dotnet publish -c Release -o /app/publish

# ---------- Runtime (Nginx) ----------
FROM nginx:alpine AS final
# Serve static site from Nginx’s default html root
WORKDIR /usr/share/nginx/html

# Copy Blazor WASM output
COPY --from=build /app/publish/wwwroot ./

# Copy Nginx config
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Add a template we’ll transform using env vars
COPY config.template.json ./config.template.json

# Add an entrypoint hook that runs before Nginx starts (nginx image executes /docker-entrypoint.d/*)
COPY entrypoint-envsubst.sh /docker-entrypoint.d/10-envsubst.sh
RUN chmod +x /docker-entrypoint.d/10-envsubst.sh
