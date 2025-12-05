# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY src/VibeApp.Api/VibeApp.Api.csproj src/VibeApp.Api/
RUN dotnet restore "src/VibeApp.Api/VibeApp.Api.csproj"

# Copy everything else and build
COPY src/VibeApp.Api/ src/VibeApp.Api/
WORKDIR /src/src/VibeApp.Api
RUN dotnet build "VibeApp.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "VibeApp.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VibeApp.Api.dll"]

