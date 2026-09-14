#!/usr/bin/env bash
set -euo pipefail

PACKAGE_DIR="${1:-release}"
DEPLOY_MODE="${DEPLOY_MODE:-simulation}"

echo "========================================"
echo " Turkey Showdown - Deployment"
echo "========================================"

if [ ! -d "$PACKAGE_DIR" ]; then
  echo "[ERROR] Package not found: $PACKAGE_DIR"
  exit 1
fi

if [ ! -f "$PACKAGE_DIR/website/homePage.php" ]; then
  echo "[ERROR] Website homePage.php is missing."
  exit 1
fi

if [ ! -f "$PACKAGE_DIR/website/MyGame/index.html" ]; then
  echo "[ERROR] Unity WebGL index.html is missing."
  exit 1
fi

if [ ! -d "$PACKAGE_DIR/website/PHP" ]; then
  echo "[ERROR] PHP directory is missing."
  exit 1
fi

if [ ! -d "$PACKAGE_DIR/website/HTML" ]; then
  echo "[ERROR] HTML directory is missing."
  exit 1
fi

echo "[OK] Production package validated."

if [ "$DEPLOY_MODE" = "production" ]; then
  echo "[ERROR] Production deployment is intentionally disabled."
  echo "A real server and SSH/rsync configuration must be added first."
  exit 2
fi

echo "[INFO] Deployment mode: SIMULATION"
echo "[INFO] No production VM is currently available."
echo
echo "[SIMULATION] The following deployment would be performed:"
echo "  Website -> /var/www/turkey-showdown/"
echo "  Game    -> /var/www/turkey-showdown/MyGame/"
echo "  Nginx   -> reload after successful transfer"
echo
echo "[SIMULATION] SSH/rsync transfer skipped."
echo "[SIMULATION] Nginx reload skipped."
echo
echo "========================================"
echo " Simulated deployment completed"
echo "========================================"
