# Render.com Deployment

## Quick Deploy

```bash
git add .
git commit -m "Deploy Vue SPA"
git push
```

Render automatically:
1. Detects Dockerfile
2. Installs Node.js
3. Builds Vue → wwwroot
4. Builds .NET
5. Deploys

**Build time:** ~3-4 minutes

## Environment Variables

Add in Render Dashboard → Environment:
```
OPENAI_API_KEY = sk-your-key-here
ASPNETCORE_ENVIRONMENT = Production
```

`DATABASE_URL` is auto-configured by Render.

**Important:** The application automatically configures cookies and HTTPS settings for production.

## Routes After Deploy

- `https://your-app.onrender.com/` → Vue SPA ✅
- `https://your-app.onrender.com/api/*` → API
- `https://your-app.onrender.com/swagger` → Swagger
- `https://your-app.onrender.com/health` → Health check

## Test Locally Before Deploy

```bash
cd src/frontend
npm install && npm run build

cd ../VibeApp.Api
dotnet run
```

Open http://localhost:5000 - should see Vue SPA.

## Troubleshooting

**Build fails:**
- Check Dockerfile has Node.js installation
- Check build logs for "npm install" step

**Frontend not showing:**
- Verify wwwroot/index.html exists in build logs
- Check `MapFallbackToFile` in Program.cs

**API works but Vue doesn't:**
- Frontend wasn't built - check Docker build logs

**Login/Register forms return 400 Bad Request:**
- Application now auto-configures cookies for HTTPS
- Cookie settings: SameSite=Lax, SecurePolicy=SameAsRequest
- Antiforgery tokens properly configured
- Should work after redeployment with latest changes

**Database migrations not applying:**
- Check logs during startup
- Migrations apply automatically in Program.cs on non-Development environment
- Verify DATABASE_URL is set correctly
