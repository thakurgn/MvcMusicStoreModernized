# Export_AlbumModule_From_SQLServer.ps1
# Purpose:
# Pull Album-module data from the legacy SQL Server MusicStore database
# running inside the Windows Server 2019 VM, and export it as CSV files
# on the Windows 11 host.
#
# Source:
# SQL Server in WinServ19 VM exposed through VirtualBox NAT port forwarding
# Host endpoint: 127.0.0.1,11433
#
# Target folder:
# G:\Projects\MigrationData
#
# Tables exported:
# 1. Genres
# 2. Artists
# 3. Albums
#
# Export order matters because Albums depends on Genres and Artists.

$migrationPassword = Read-Host "Enter migration_user SQL Server password"

$connectionString = "Server=127.0.0.1,11433;Database=MusicStore;User ID=migration_user;Password=$migrationPassword;TrustServerCertificate=True;"
$outputFolder = "G:\Projects\MigrationData"

New-Item -ItemType Directory -Force -Path $outputFolder | Out-Null

Write-Host "========================================"
Write-Host "Album Module Export Started"
Write-Host "Source: SQL Server in WinServ19 VM via 127.0.0.1:11433"
Write-Host "Target: $outputFolder"
Write-Host "========================================"

Write-Host "Exporting Genres..."

$connection = New-Object System.Data.SqlClient.SqlConnection
$connection.ConnectionString = $connectionString
$command = $connection.CreateCommand()
$command.CommandText = "SELECT GenreId, Name, Description FROM dbo.Genres ORDER BY GenreId;"
$adapter = New-Object System.Data.SqlClient.SqlDataAdapter $command
$table = New-Object System.Data.DataTable
$adapter.Fill($table) | Out-Null
$table | Export-Csv -Path "$outputFolder\genres.csv" -NoTypeInformation -Encoding UTF8
$connection.Close()

Write-Host "Exported genres.csv Rows:" $table.Rows.Count

Write-Host "Exporting Artists..."

$connection = New-Object System.Data.SqlClient.SqlConnection
$connection.ConnectionString = $connectionString
$command = $connection.CreateCommand()
$command.CommandText = "SELECT ArtistId, Name FROM dbo.Artists ORDER BY ArtistId;"
$adapter = New-Object System.Data.SqlClient.SqlDataAdapter $command
$table = New-Object System.Data.DataTable
$adapter.Fill($table) | Out-Null
$table | Export-Csv -Path "$outputFolder\artists.csv" -NoTypeInformation -Encoding UTF8
$connection.Close()

Write-Host "Exported artists.csv Rows:" $table.Rows.Count

Write-Host "Exporting Albums..."

$connection = New-Object System.Data.SqlClient.SqlConnection
$connection.ConnectionString = $connectionString
$command = $connection.CreateCommand()
$command.CommandText = "SELECT AlbumId, GenreId, ArtistId, Title, Price, AlbumArtUrl FROM dbo.Albums ORDER BY AlbumId;"
$adapter = New-Object System.Data.SqlClient.SqlDataAdapter $command
$table = New-Object System.Data.DataTable
$adapter.Fill($table) | Out-Null
$table | Export-Csv -Path "$outputFolder\albums.csv" -NoTypeInformation -Encoding UTF8
$connection.Close()

Write-Host "Exported albums.csv Rows:" $table.Rows.Count

Write-Host "========================================"
Write-Host "Album Module Export Finished"
Write-Host "Files created:"
Write-Host "$outputFolder\genres.csv"
Write-Host "$outputFolder\artists.csv"
Write-Host "$outputFolder\albums.csv"
Write-Host "========================================"