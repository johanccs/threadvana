$r = az webapp deploy --name threadcraft-academy --resource-group threadcraft-rg --src-path "X:\Playground\multiple-threading-playground\deploy.zip" --type zip 2>&1
az webapp restart --name threadcraft-academy --resource-group threadcraft-rg 2>&1 | Out-Null
"$(Get-Date -Format 'HH:mm:ss') Deploy done, exit=$LASTEXITCODE" | Out-File "X:\Playground\multiple-threading-playground\deploy_done.txt"
