cd ..
$migrationListOutput = dotnet ef migrations list --configuration Release -c ProductionDbContext
$migrationName = $migrationListOutput.Split()[-1]
echo "Applying migration $migrationName"
dotnet ef database update $migrationName --configuration Release -c ProductionDbContext
echo "END PROGRAM"
Read-Host