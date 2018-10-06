cd ..
Remove-Item -path Migrations\* -Recurse -Force
Remove-Item DataBase\Migrations* -Recurse -Force
dotnet ef database drop -f -v
dotnet ef migrations add EmptyDebugMigration --no-build -v
dotnet ef database update -v
git checkout DataBase/Migrations/Production/
cd Scripts
