param (
    [string]$OldName = "MyApp",
    [string]$NewName
)

function Replace-SolutionName {
    param (
        [string]$Path,
        [string]$OldName,
        [string]$NewName
    )

    # Get all files and directories in the current path
    $files = Get-ChildItem -LiteralPath $Path -File
    $directories = Get-ChildItem -LiteralPath $Path -Directory | Where-Object { $_.Name -notmatch '^(bin|obj|\.)' }

    # Process files
    foreach ($file in $files) {
        # Replace occurrences in file contents
        $fileContent = Get-Content -LiteralPath $file.FullName -Raw
        $newContent = $fileContent -replace $OldName, $NewName
        if ($fileContent.Trim() -ne $newContent.Trim()) {
            Write-Host "Updating content in $($file.FullName)"
            Set-Content -Path $file.FullName -Value $newContent.Trim() -Force
        }

        # Replace occurrences in file name
        $newFileName = $file.Name -replace $OldName, $NewName
        if ($file.Name -ne $newFileName) {
            Write-Host "Renaming $($file.FullName) to $($file.DirectoryName)\$newFileName"
            Move-Item -LiteralPath $file.FullName -Destination (Join-Path -Path $file.DirectoryName -ChildPath $newFileName) -Force
        }
    }

    # Process directories
    foreach ($directory in $directories) {
        # Recursively process subdirectories
        Replace-SolutionName -Path $directory.FullName -OldName $OldName -NewName $NewName

        # Replace occurrences in directory name
        $newDirectoryName = $directory.Name -replace $OldName, $NewName
        if ($directory.Name -ne $newDirectoryName) {
            Write-Host "Renaming $($directory.FullName) to $($directory.Parent.FullName)\$newDirectoryName"
            Move-Item -LiteralPath $directory.FullName -Destination (Join-Path -Path $directory.Parent.FullName -ChildPath $newDirectoryName) -Force
        }
    }
}

$Path = Get-Location

Replace-SolutionName -Path $Path -OldName $OldName -NewName $NewName

Read-Host "Press Enter to continue..."