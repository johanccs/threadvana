$r = az webapp deploy --name threadcraft-academy --resource-group threadcraft-rg --src-path "X:\Playground\multiple-threading-playground\deploy.zip" --type zip 2>&1
"$(Get-Date -Format 'HH:mm:ss') Exit=$LASTEXITCODE" | Out-File "X:\Playground\multiple-threading-playground\deploy_ok.txt"
$r | Out-File "X:\Playground\multiple-threading-playground\deploy_ok.txt" -Append
az webapp restart --name threadcraft-academy --resource-group threadcraft-rg 2>&1 | Out-Null
"$(Get-Date -Format 'HH:mm:ss') Restarted" | Out-File "X:\Playground\multiple-threading-playground\deploy_ok.txt" -Append
