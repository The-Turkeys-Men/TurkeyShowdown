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

if [ ! -f "$PACKAGE_DIR/game/index.html" ]; then
  echo "[ERROR] Unity WebGL index.html is missing."
  exit 1
fi

if [ ! -d "$PACKAGE_DIR/website" ]; then
  echo "[ERROR] Website directory is missing."
  exit 1
fi

echo "[OK] Production package validated."

if [ "$DEPLOY_MODE" = "production" ]; then
  echo "[ERROR] Production deployment is intentionally disabled."
  echo "Configure a real server and SSH secrets before enabling it."
  exit 2
fi

echo "[INFO] Deployment mode: SIMULATION"
echo "[INFO] A production server is not currently available."
echo "[SIMULATION] The following files would be transferred:"
echo "             $PACKAGE_DIR/game     -> /var/www/turkey-showdown/game"
echo "             $PACKAGE_DIR/website  -> /var/www/turkey-showdown/website"
echo "[SIMULATION] SSH transfer skipped."
echo "[SIMULATION] Nginx reload skipped."

echo "========================================"
echo " Simulated deployment completed"
echo "========================================"
