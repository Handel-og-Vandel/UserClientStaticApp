#!/bin/sh
set -eu

TEMPLATE=/usr/share/nginx/html/config.template.json
TARGET=/usr/share/nginx/html/config.json

if [ -f "$TEMPLATE" ]; then
  # Substitute only the variables we expect; unset stays empty unless you provide defaults in the template
  envsubst '${USER_SERVICE_API_BASE} ${ASPNETCORE_ENVIRONMENT}' < "$TEMPLATE" > "$TARGET"
  echo "Generated /config.json from environment:"
  cat "$TARGET"
fi
