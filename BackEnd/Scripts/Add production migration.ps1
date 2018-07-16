cd ..
$migrationName = Read-Host 'Write name for new production Migration'
dotnet ef migrations add $migrationName -o DataBase/Migrations/Production --configuration Release
echo "END PROGRAM"
Read-Host
