FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY backend/API/JobMesh.Api/*.csproj ./JobMesh.Api/

WORKDIR /src/JobMesh.Api

RUN dotnet restore

COPY backend/API/JobMesh.Api/ .

RUN dotnet publish \
    -c Release \
    -o /app/publish \
    -p UseAppHost=false


FROM mcr.microsoft.com/dotnet/aspnet:9.0

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "JobMesh.Api.dll"]