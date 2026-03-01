# MssBase.Service

Entity Framework Helpers

Add A Migration:

- Open a terminal in the root of your solution and run:

dotnet ef migrations add InitialMigration \
  --project Services/Security/Data.Security/Data.Security.csproj \
  --startup-project MssBase.Service/MssBase.Service.csproj \
  --output-dir Migrations

--project points to your data project (where SecurityDBContext lives).
--startup-project points to your API project (where configuration is loaded).
--output-dir specifies where to put the migration files (relative to the data project).

 Update the Database

 - To apply the migration to your database:
  
  dotnet ef database update \
  --project Services/Security/Data.Security/Data.Security.csproj \
  --startup-project MssBase.Service/MssBase.Service.csproj

  Drop Database: (DANGER - Not For Production)

  dotnet ef database drop --project Services/Security/Data.Security/Data.Security.csproj --startup-project MssBase.Service/MssBase.Service.csproj
