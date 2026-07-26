$base = 'X:\Playground\multiple-threading-playground\content\lessons'
$fixed = 0

$singleMap = @{
    0x82 = 0x27; 0x83 = 0x2D; 0x85 = 0x2E
    0x91 = 0x27; 0x92 = 0x27
    0x93 = 0x22; 0x94 = 0x22
    0x96 = 0x2D; 0x97 = 0x2D
}

$patterns = @(
    @{ From = @(0xC3,0xA2,0xE2,0x82,0xAC,0xE2,0x80,0x9D); To = @(0x2D) },
    @{ From = @(0xC3,0xA2,0xE2,0x82,0xAC,0xC5,0x93); To = @(0x22) },
    @{ From = @(0xC3,0xA2,0xE2,0x82,0xAC,0xC2,0x9D); To = @(0x22) },
    @{ From = @(0xC3,0xA2,0xE2,0x82,0xAC,0xE2,0x84,0xA2); To = @(0x27) },
    @{ From = @(0xC3,0xA2,0xE2,0x82,0xAC,0xC2,0xA2); To = @(0x2D) },
    @{ From = @(0xC3,0xA2,0xE2,0x82,0xAC,0xC2,0xA6); To = @(0x2E,0x2E,0x2E) }
)

function Replace-Bytes($bytes, $from, $to) {
    $result = [System.Collections.Generic.List[byte]]::new()
    $i = 0; $fl = $from.Count; $toArr = [byte[]]$to
    while ($i -le $bytes.Count - $fl) {
        $match = $true
        for ($j = 0; $j -lt $fl; $j++) {
            if ($bytes[$i + $j] -ne $from[$j]) { $match = $false; break }
        }
        if ($match) { $result.AddRange($toArr); $i += $fl }
        else { $result.Add($bytes[$i]); $i++ }
    }
    while ($i -lt $bytes.Count) { $result.Add($bytes[$i]); $i++ }
    return $result.ToArray()
}

Get-ChildItem $base -Recurse -Include '*.md','*.json' | ForEach-Object {
    $bytes = [System.IO.File]::ReadAllBytes($_.FullName)
    $changed = $false

    foreach ($p in $patterns) {
        $newBytes = Replace-Bytes $bytes $p.From $p.To
        if ($newBytes.Count -ne $bytes.Count) { $changed = $true; $bytes = $newBytes }
    }

    for ($i = 0; $i -lt $bytes.Count; $i++) {
        if ($bytes[$i] -ge 0x80 -and $bytes[$i] -le 0x9F) {
            $bytes[$i] = if ($singleMap.ContainsKey($bytes[$i])) { $singleMap[$bytes[$i]] } else { 0x20 }
            $changed = $true
        }
    }

    if ($changed) {
        [System.IO.File]::WriteAllBytes($_.FullName, $bytes)
        $fixed++
    }
}
Write-Output "Fixed $fixed files"
