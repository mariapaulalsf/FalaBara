Write-Host "Iniciando FalaBara..." -ForegroundColor Green

Write-Host "`n1. Iniciando API..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PSScriptRoot\backend\Falabara.WebAPI'; & 'C:\Program Files\dotnet\dotnet.exe' run"

Start-Sleep -Seconds 3

Write-Host "`n2. Abrindo Frontend..." -ForegroundColor Yellow
Start-Process "chrome.exe" "file:///$PSScriptRoot/frontend/login.html"

Write-Host "`nAplicacao iniciada!" -ForegroundColor Green
Write-Host "API: http://localhost:5282" -ForegroundColor Cyan
Write-Host "Frontend: Aberto no navegador" -ForegroundColor Cyan
