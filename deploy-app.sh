#!/bin/bash
# /opt/planty-scripts/deploy-app.sh
# Script to handle application deployment

set -e  # Exit on any error

APP_DIR="/opt/planty-api"
BACKUP_DIR="/opt/planty-api-backup"
DEPLOY_DIR="/home/deploy/deployments"
SERVICE_NAME="planty-api"

echo "Starting deployment..."

# Stop the service
echo "Stopping service..."
systemctl stop $SERVICE_NAME || true

# Backup current deployment
echo "Creating backup..."
rm -rf $BACKUP_DIR || true
cp -r $APP_DIR $BACKUP_DIR || true

# Extract new deployment
echo "Extracting new version..."
rm -rf $APP_DIR || true
mkdir -p $APP_DIR
tar -xzf $DEPLOY_DIR/planty-api.tar.gz -C $APP_DIR

# Set permissions
echo "Setting permissions..."
chown -R planty:planty $APP_DIR
chmod +x $APP_DIR/Planty.API

# Start the service
echo "Starting service..."
systemctl start $SERVICE_NAME
systemctl enable $SERVICE_NAME

# Verify service is running
echo "Verifying deployment..."
sleep 5
if systemctl is-active --quiet $SERVICE_NAME; then
    echo "Deployment successful! Service is running."
    # Clean up old backup if deployment successful
    rm -rf $BACKUP_DIR || true
else
    echo "Deployment failed! Service is not running."
    echo "Restoring backup..."
    systemctl stop $SERVICE_NAME || true
    rm -rf $APP_DIR || true
    cp -r $BACKUP_DIR $APP_DIR || true
    chown -R planty:planty $APP_DIR || true
    chmod +x $APP_DIR/Planty.API || true
    systemctl start $SERVICE_NAME || true
    exit 1
fi