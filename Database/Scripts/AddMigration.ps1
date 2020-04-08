$migrationName = Read-Host 'Write name for new Migration'
dotnet ef migrations add $migrationName --startup-project ../Backend
echo "END PROGRAM"
