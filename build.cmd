@echo off
REM Build script for Vibe App Frontend + Backend (Windows)

echo Building Vibe App...

REM 1. Build Frontend
echo Building Vue frontend...
cd src\frontend
call npm install
call npm run build
cd ..\..

echo Frontend built to src/VibeApp.Api/wwwroot

REM 2. Build Backend
echo Building ASP.NET Core backend...
dotnet build VibeApp.sln -c Release

echo Backend built successfully

echo Build complete!
echo.
echo To run the app:
echo   cd src\VibeApp.Api
echo   dotnet run
echo.
echo Then open: http://localhost:5000

