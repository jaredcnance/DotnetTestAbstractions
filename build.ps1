function Get-Version-Suffix-From-Tag {
    $tag = $env:APPVEYOR_REPO_TAG_NAME
    $split = $tag -split "-"
    $suffix = $split[1..2]
    $final = $suffix -join "-"
    return $final
}

function CheckLastExitCode {
    param ([int[]]$SuccessCodes = @(0), [scriptblock]$CleanupScript = $null)

    if ($SuccessCodes -notcontains $LastExitCode) {
        $msg = "EXE RETURNED EXIT CODE $LastExitCode"
        throw $msg
    }
}

$revision = @{ $true = $env:APPVEYOR_BUILD_NUMBER; $false = 1 }[$env:APPVEYOR_BUILD_NUMBER -ne $NULL];
$revision = "{0:D4}" -f [convert]::ToInt32($revision, 10)

dotnet restore

dotnet test .\examples\WebApp.Tests\WebApp.Tests.csproj
CheckLastExitCode

dotnet build .\src\DotnetTestAbstractions -c Release
CheckLastExitCode

Write-Output "APPVEYOR_REPO_TAG: $env:APPVEYOR_REPO_TAG"

If ($env:APPVEYOR_REPO_TAG -eq $true) {
    $revision = Get-Version-Suffix-From-Tag
    Write-Output "VERSION-SUFFIX: $revision"

    IF ([string]::IsNullOrWhitespace($revision)) {
        Write-Output "RUNNING dotnet pack .\src\DotnetTestAbstractions -c Release -o .\artifacts"
        dotnet pack .\src\DotnetTestAbstractions -c Release -o .\artifacts
        CheckLastExitCode
    }
    Else {
        Write-Output "RUNNING dotnet pack .\src\DotnetTestAbstractions -c Release -o .\artifacts --version-suffix=$revision"
        dotnet pack .\src\DotnetTestAbstractions -c Release -o .\artifacts --version-suffix=$revision 
        CheckLastExitCode
    }
}
Else { 
    Write-Output "VERSION-SUFFIX: alpha1-$revision"
    Write-Output "RUNNING dotnet pack .\src\DotnetTestAbstractions -c Release -o .\artifacts --version-suffix=alpha1-$revision"
    dotnet pack .\src\DotnetTestAbstractions -c Release -o .\artifacts --version-suffix=alpha1-$revision 
    CheckLastExitCode
}