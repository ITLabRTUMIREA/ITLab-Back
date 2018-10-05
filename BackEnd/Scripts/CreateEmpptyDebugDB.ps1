cd ..
rm -rf Migrations/
rm -rf DataBase/Migrations
dotnet ef database drop -f
dotnet ef migrations add EmptyDebugMigration --no-build
dotnet ef database update
git checkout DataBase/Migrations/Production/
