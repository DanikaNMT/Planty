# build-and-deploy.ps1
# Script to build and create deployment artifact locally

param(
    [string]$ProjectPath = "C:\Users\arnod\repos\Planty\backend\src\Planty.API",
    [string]$OutputPath = "C:\Users\arnod\repos\Planty\publish",
    [string]$PiHost = "raspberrypi.local",
    [string]$PiUser = "deploy"
)

Write-Host "Starting local build and deployment..." -ForegroundColor Green

# Clean and prepare
Write-Host "Cleaning previous builds..." -ForegroundColor Yellow
if (Test-Path $OutputPath) {
    Remove-Item $OutputPath -Recurse -Force
}
New-Item -ItemType Directory -Path $OutputPath -Force | Out-Null

# Navigate to backend folder
Set-Location "C:\Users\arnod\repos\Planty\backend"

# Clean all projects first
Write-Host "Cleaning solution..." -ForegroundColor Yellow
dotnet clean --configuration Release

# Navigate to project
Set-Location $ProjectPath

# Build and publish in one step to ensure everything is rebuilt
Write-Host "Publishing for Linux ARM (32-bit)..." -ForegroundColor Yellow
dotnet publish --configuration Release --output $OutputPath --runtime linux-arm --self-contained true

Write-Host "Publishing migration tool..." -ForegroundColor Yellow
Set-Location "C:\Users\arnod\repos\Planty\backend\src\Planty.MigrationTool"
dotnet publish --configuration Release --output $OutputPath --runtime linux-arm --self-contained true

# Config files are already in the output directory from the API publish

# Create archive
Write-Host "Creating deployment archive..." -ForegroundColor Yellow
Set-Location $OutputPath

# Remove any existing archive first
if (Test-Path "planty-api.tar.gz") {
    Remove-Item "planty-api.tar.gz" -Force
}

# Create archive excluding any .gz files
tar -czf planty-api.tar.gz --exclude="*.gz" *

# Verify archive
if (Test-Path "planty-api.tar.gz") {
    $fileSize = (Get-Item "planty-api.tar.gz").Length
    Write-Host "Archive created successfully: $fileSize bytes" -ForegroundColor Green
    
    # Show contents
    Write-Host "Archive contents:" -ForegroundColor Yellow
    tar -tzf planty-api.tar.gz | Select-Object -First 10
} else {
    Write-Host "Failed to create archive!" -ForegroundColor Red
    exit 1
}

# Optional: Upload to Pi
$upload = Read-Host "Upload to Pi? (y/n)"
if ($upload -eq "y" -or $upload -eq "Y") {
    Write-Host "Uploading to Raspberry Pi..." -ForegroundColor Yellow
    scp planty-api.tar.gz "${PiUser}@${PiHost}:/home/deploy/deployments/"
    
    $deploy = Read-Host "Run deployment? (y/n)"
    if ($deploy -eq "y" -or $deploy -eq "Y") {
        Write-Host "Running deployment script..." -ForegroundColor Yellow
        ssh "${PiUser}@${PiHost}" "sudo /opt/planty-scripts/deploy-app.sh"
    }
}

Write-Host "Done!" -ForegroundColor Green