# Build context is expected to be the repository root (see docker/docker-compose.yml)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Services/ Services/
COPY Shared/ Shared/
COPY MssBase.Service/ MssBase.Service/

RUN dotnet publish MssBase.Service/MssBase.Service.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "MssBase.Service.dll"]
