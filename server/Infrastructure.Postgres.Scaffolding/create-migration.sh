#Uses the EF Core CLI: Must have it installed beforehand
dotnet ef migrations remove --startup-project ../Startup/Startup.csproj
dotnet ef migrations add Initial --startup-project ../Startup/Startup.csproj