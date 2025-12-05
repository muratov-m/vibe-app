# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution file and all csproj files for restore
COPY VibeApp.sln .
COPY src/VibeApp.Core/VibeApp.Core.csproj src/VibeApp.Core/
COPY src/VibeApp.Data/VibeApp.Data.csproj src/VibeApp.Data/
COPY src/VibeApp.Api/VibeApp.Api.csproj src/VibeApp.Api/
RUN dotnet restore "VibeApp.sln"

# Copy everything else and build
COPY src/VibeApp.Core/ src/VibeApp.Core/
COPY src/VibeApp.Data/ src/VibeApp.Data/
COPY src/VibeApp.Api/ src/VibeApp.Api/
RUN dotnet build "VibeApp.sln" -c Release --no-restore

# Publish stage
FROM build AS publish
WORKDIR /src/src/VibeApp.Api
RUN dotnet publish "VibeApp.Api.csproj" -c Release -o /app/publish --no-restore /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VibeApp.Api.dll"]

