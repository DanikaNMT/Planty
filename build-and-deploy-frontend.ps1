# build-and-deploy-frontend.ps1
# Script to build and deploy the React frontend to Raspberry Pi

param(
    [string]$ProjectPath = "C:\Users\arnod\repos\Planty\frontend\web",
    [string]$OutputPath = "C:\Users\arnod\repos\Planty\frontend-publish",
    [string]$PiHost = "raspberrypi.local",
    [string]$PiUser = "deploy",
    [string]$ApiUrl = "https://planty.arnodece.com"
)

Write-Host "Starting frontend build and deployment..." -ForegroundColor Green

# Clean and prepare
Write-Host "Cleaning previous builds..." -ForegroundColor Yellow
if (Test-Path $OutputPath) {
    Remove-Item $OutputPath -Recurse -Force
}
New-Item -ItemType Directory -Path $OutputPath -Force | Out-Null

# Navigate to frontend project
Set-Location $ProjectPath

# Check if node_modules exists
if (-not (Test-Path "node_modules")) {
    Write-Host "Installing dependencies..." -ForegroundColor Yellow
    npm install
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Failed to install dependencies!" -ForegroundColor Red
        exit 1
    }
} else {
    Write-Host "Dependencies already installed, updating..." -ForegroundColor Yellow
    npm ci
}

# Build the React app for production
Write-Host "Building React app for production..." -ForegroundColor Yellow
$env:VITE_API_BASE = $ApiUrl
npm run build

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}

# Copy build output to our publish directory
Write-Host "Copying build output..." -ForegroundColor Yellow
if (Test-Path "dist") {
    Copy-Item "dist\*" -Destination $OutputPath -Recurse -Force
} else {
    Write-Host "Build output directory 'dist' not found!" -ForegroundColor Red
    exit 1
}

# Create archive
Write-Host "Creating deployment archive..." -ForegroundColor Yellow
Set-Location $OutputPath

# Remove any existing archive first
if (Test-Path "planty-frontend.tar.gz") {
    Remove-Item "planty-frontend.tar.gz" -Force
}

# Create archive
tar -czf planty-frontend.tar.gz *

# Verify archive
if (Test-Path "planty-frontend.tar.gz") {
    $fileSize = (Get-Item "planty-frontend.tar.gz").Length
    Write-Host "Archive created successfully: $fileSize bytes" -ForegroundColor Green
    
    # Show contents
    Write-Host "Archive contents:" -ForegroundColor Yellow
    tar -tzf planty-frontend.tar.gz | Select-Object -First 10
} else {
    Write-Host "Failed to create archive!" -ForegroundColor Red
    exit 1
}

# Optional: Upload to Pi
$upload = Read-Host "Upload to Pi? (y/n)"
if ($upload -eq "y" -or $upload -eq "Y") {
    Write-Host "Uploading to Raspberry Pi..." -ForegroundColor Yellow
    
    # Create deployments directory if it doesn't exist
    ssh "${PiUser}@${PiHost}" "mkdir -p /home/deploy/deployments"
    
    # Upload the archive
    scp planty-frontend.tar.gz "${PiUser}@${PiHost}:/home/deploy/deployments/"
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Upload successful!" -ForegroundColor Green
        
        $deploy = Read-Host "Run deployment? (y/n)"
        if ($deploy -eq "y" -or $deploy -eq "Y") {
            Write-Host "Running frontend deployment..." -ForegroundColor Yellow
            ssh "${PiUser}@${PiHost}" "/opt/planty-scripts/deploy-frontend.sh"
            
            if ($LASTEXITCODE -eq 0) {
                Write-Host "Deployment completed successfully!" -ForegroundColor Green
                Write-Host "Frontend should now be available at: http://$PiHost" -ForegroundColor Cyan
            } else {
                Write-Host "Deployment failed!" -ForegroundColor Red
            }
        }
    } else {
        Write-Host "Upload failed!" -ForegroundColor Red
    }
}

Write-Host "Done!" -ForegroundColor Green