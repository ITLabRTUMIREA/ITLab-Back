cd ..
rm -rf Migrations/
rm -rf DataBase/Migrations
dotnet ef database drop -f -v
dotnet ef migrations add EmptyDebugMigration --no-build -v
dotnet ef database update -v
git checkout DataBase/Migrations/Production/
