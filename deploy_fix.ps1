$result = az webapp deploy --name threadcraft-academy --resource-group threadcraft-rg --src-path "X:\Playground\multiple-threading-playground\deploy.zip" --type zip 2>&1
"$(Get-Date): Exit=$LASTEXITCODE" | Out-File "X:\Playground\multiple-threading-playground\deploy_result.txt"
$result | Out-File "X:\Playground\multiple-threading-playground\deploy_result.txt" -Append
