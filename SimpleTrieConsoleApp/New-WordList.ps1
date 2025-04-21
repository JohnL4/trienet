<#
.SYNOPSIS
    Create a list of unique words, like filenames.
#>
[CmdletBinding( SupportsShouldProcess)]
param(
    # How many words to create
    [Parameter( Mandatory=$true)]
    [int]
    $Count
)

Write-Verbose ("Hi, creating {0} words" -f $Count)

$alphabet = [char[]] ( "abcdefghijklmnopqrstuvwxyz0123456789" -split "" | ? {$_ -ne ""})
$alphabetLength = $alphabet.Count

Write-Verbose ("Alphabet length: {0}" -f $alphabetLength)

$wordLength = [Math]::Ceiling( [Math]::Log( $Count)/[Math]::Log($alphabet.Length))

Write-Verbose ("Word length: {0}" -f $wordLength)

$charIndices = @()
for ($i = 0; $i -lt $wordLength; $i++) {
    $charIndices += 0
}

$chars = @()    #  = New-Object "System.Collection.Generic.List[char]" # [char[]]($charIndices | % {$alphabet[ $_]})
foreach ($charIndex in $charIndices) {
    $chars += $alphabet[ $charIndex]
    # $chars.Add( $alphabet[ $charIndex])
}

for ($i = 0; $i -lt $Count; $i++) {
    $word = -join $chars
    Write-Verbose ("{0}`t:{1}" -f $i,$word)
    Write-Output $word
    # 'a' -- 97, 'z' -- 122, '0' -- 48, '9' -- 57
    $carry = $true
    $j = $wordLength - 1
    while ($carry) {
        if ($j -lt 0) { throw "Ran off left end of array" }
        $charIndices[ $j]++
        if ($charIndices[$j] -ge $alphabetLength) {
            $charIndices[ $j] = 0
        } else {
            $carry = $false
        }
        $chars[$j] = $alphabet[ $charIndices[$j]]
        $j--
    }
    if (($i % 10000) -eq 0) {
        Write-Host -NoNewline "."
    }
}