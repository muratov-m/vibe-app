# Vue Frontend Integration

Vue 3 frontend integrated into ASP.NET Core backend.

## How It Works

**Development:** Vite dev server (5173) + ASP.NET Core (5000) with CORS
**Production:** Vue builds to `src/VibeApp.Api/wwwroot/`, served by ASP.NET Core

## Quick Start

### Dev Mode (2 terminals)
```bash
# Terminal 1
cd src/VibeApp.Api
dotnet run

# Terminal 2
cd src/frontend
npm install
npm run dev
```
Open: http://localhost:5173

### Production Mode
```bash
build.cmd  # or ./build.sh
cd src/VibeApp.Api
dotnet run
```
Open: http://localhost:5000

## Build Process

```
src/frontend/ → npm run build → src/VibeApp.Api/wwwroot/
```

Vite config: `build.outDir = '../VibeApp.Api/wwwroot'`

## Routing

- `/api/*` → Controllers (priority)
- `/*` → Vue SPA (fallback via `MapFallbackToFile("index.html")`)

## Deployment

Dockerfile builds frontend automatically:
1. Install Node.js
2. `npm install && npm run build`
3. Frontend in wwwroot
4. Single Docker image

Just `git push` - Render handles the rest.


