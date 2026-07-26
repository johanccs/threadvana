$ErrorActionPreference = "Continue"
Write-Output "Deploying zip to Azure (this may take 1-2 mins)..."
$result = az webapp deploy --name threadcraft-academy --resource-group threadcraft-rg --src-path "X:\Playground\multiple-threading-playground\deploy.zip" --type zip 2>&1
Write-Output "DEPLOY EXIT: $LASTEXITCODE"
$result | Select-Object -Last 10
Write-Output "--- Restarting ---"
az webapp restart --name threadcraft-academy --resource-group threadcraft-rg 2>&1 | Out-Null
Write-Output "Deployment script finished."
