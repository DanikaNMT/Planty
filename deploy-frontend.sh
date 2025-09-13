#!/bin/bash
# /opt/planty-scripts/deploy-frontend.sh
# Script to handle frontend deployment (no sudo required)

set -e  # Exit on any error

FRONTEND_DIR="/opt/planty-frontend"
BACKUP_DIR="/opt/planty-frontend-backup"
DEPLOY_DIR="/home/deploy/deployments"
ARCHIVE_NAME="planty-frontend.tar.gz"

echo "Starting frontend deployment..."

# Check if archive exists
if [ ! -f "$DEPLOY_DIR/$ARCHIVE_NAME" ]; then
    echo "Error: Archive not found at $DEPLOY_DIR/$ARCHIVE_NAME"
    exit 1
fi

echo "Archive found: $DEPLOY_DIR/$ARCHIVE_NAME"

# Create backup of current deployment
echo "Creating backup..."
if [ -d "$BACKUP_DIR" ]; then
    chmod -R 755 $BACKUP_DIR || true
    rm -rf $BACKUP_DIR || true
fi
if [ -d "$FRONTEND_DIR" ]; then
    cp -r $FRONTEND_DIR $BACKUP_DIR || true
fi

# Extract new deployment
echo "Extracting new frontend..."
if [ -d "$FRONTEND_DIR" ]; then
    chmod -R 755 $FRONTEND_DIR || true
    rm -rf $FRONTEND_DIR || true
fi
mkdir -p $FRONTEND_DIR
tar -xzf $DEPLOY_DIR/$ARCHIVE_NAME -C $FRONTEND_DIR

# Set permissions (ensure nginx can read the files)
echo "Setting permissions..."
find $FRONTEND_DIR -type f -exec chmod 644 {} \;
find $FRONTEND_DIR -type d -exec chmod 755 {} \;

# Test nginx configuration
echo "Testing nginx configuration..."
sudo /usr/sbin/nginx -t

# Reload nginx
echo "Reloading nginx..."
sudo systemctl reload nginx

# Verify deployment
echo "Verifying deployment..."
sleep 2

HTTP_STATUS=$(curl -s -o /dev/null -w "%{http_code}" http://localhost/ || echo "000")

if [ "$HTTP_STATUS" = "200" ]; then
    echo "Deployment successful! Frontend is responding."
    # Clean up old backup if deployment successful
    rm -rf $BACKUP_DIR || true
    echo "Frontend is available at: http://$(hostname).local"
else
    echo "Deployment failed! Frontend is not responding (HTTP $HTTP_STATUS)"
    echo "Restoring backup..."
    rm -rf $FRONTEND_DIR || true
    if [ -d "$BACKUP_DIR" ]; then
        cp -r $BACKUP_DIR $FRONTEND_DIR || true
        find $FRONTEND_DIR -type f -exec chmod 644 {} \;
        find $FRONTEND_DIR -type d -exec chmod 755 {} \;
    fi
    sudo systemctl reload nginx || true
    exit 1
fi