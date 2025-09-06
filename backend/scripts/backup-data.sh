#!/bin/bash

PI_IP="192.168.1.100"  # Replace with your Pi's IP
PI_USER="pi"
BACKUP_DIR="./backups"
DATE=$(date +%Y%m%d_%H%M%S)

echo "💾 Backing up Plant App data..."

mkdir -p $BACKUP_DIR

# Backup database
scp $PI_USER@$PI_IP:/home/pi/plantapp/data/plants.db "$BACKUP_DIR/plants_$DATE.db"

# Backup any uploaded images (if you addded later)
# scp -r $PI_USER@$PI_IP:/home/pi/plantapp/data/images/ "$BACKUP_DIR/images_$DATE/"

echo "✅ Backup complete: $BACKUP_DIR/plants_$DATE.db"