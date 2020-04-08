$migrationName = Read-Host 'Press enter to drop current database'
dotnet ef database drop -f --startup-project ../Backend