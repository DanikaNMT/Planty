#!/bin/bash

# Configuration
PI_IP="192.168.1.100"  # Replace with your Pi's IP
PI_USER="pi"
APP_NAME="plantapp"
LOCAL_PUBLISH_DIR="./publish"
REMOTE_DIR="/home/pi/plantapp"

echo "🌱 Starting Plant App deployment to Raspberry Pi..."

# Build and publish for ARM64
echo "📦 Building and publishing for ARM64..."
dotnet publish src/PlantApp.API/PlantApp.API.csproj \
  -c Release \
  -r linux-arm64 \
  --self-contained \
  -o $LOCAL_PUBLISH_DIR \
  /p:PublishSingleFile=true \
  /p:PublishTrimmed=true

if [ $? -ne 0 ]; then
    echo "❌ Build failed!"
    exit 1
fi

# Create data directory structure
mkdir -p $LOCAL_PUBLISH_DIR/data

# Copy files to Pi
echo "🚀 Deploying to Raspberry Pi ($PI_IP)..."
ssh $PI_USER@$PI_IP "mkdir -p $REMOTE_DIR/data"
scp -r $LOCAL_PUBLISH_DIR/* $PI_USER@$PI_IP:$REMOTE_DIR/

# Set permissions
ssh $PI_USER@$PI_IP "chmod +x $REMOTE_DIR/PlantApp.API"

# Create or update systemd service
echo "⚙️  Setting up systemd service..."
cat << EOF > /tmp/plantapp.service
[Unit]
Description=Plant App Backend API
After=network.target

[Service]
Type=notify
User=$PI_USER
Group=$PI_USER
WorkingDirectory=$REMOTE_DIR
ExecStart=$REMOTE_DIR/PlantApp.API
Restart=always
RestartSec=10
SyslogIdentifier=plantapp
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://0.0.0.0:5000
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
EOF

scp /tmp/plantapp.service $PI_USER@$PI_IP:/tmp/
ssh $PI_USER@$PI_IP "sudo mv /tmp/plantapp.service /etc/systemd/system/ && sudo systemctl daemon-reload"

# Start the service
echo "🎯 Starting Plant App service..."
ssh $PI_USER@$PI_IP "sudo systemctl enable $APP_NAME && sudo systemctl restart $APP_NAME"

# Check status
echo "📊 Checking service status..."
ssh $PI_USER@$PI_IP "sudo systemctl status $APP_NAME --no-pager"

echo "✅ Deployment complete!"
echo "🌐 Your Plant App should be running at: http://$PI_IP:5000"
echo "📖 API Documentation: http://$PI_IP:5000/swagger"

# Clean up local publish directory
rm -rf $LOCAL_PUBLISH_DIR
rm /tmp/plantapp.service

# scripts/setup-dev.sh
#!/bin/bash

echo "🌱 Setting up Plant App development environment..."

# Restore packages
echo "📦 Restoring NuGet packages..."
dotnet restore

# Create initial migration
echo "🗄️  Creating initial database migration..."
cd src/PlantApp.API
dotnet ef migrations add InitialCreate --context PlantDbContext

# Update database
echo "📊 Updating database..."
dotnet ef database update --context PlantDbContext

echo "✅ Development environment setup complete!"
echo "🚀 Run 'dotnet run' in src/PlantApp.API to start the API"
echo "🧪 Run 'dotnet test' from the root to run all tests"