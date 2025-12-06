# Vibe App Frontend

Vue 3 + Vite + Tailwind CSS single-page application for RAG-powered people search.

## ⚠️ Important: This Frontend is Integrated

This frontend **builds into** the ASP.NET Core backend (`../VibeApp.Api/wwwroot/`).

You don't deploy it separately - it's part of the main app!

## Features

- 🔍 Natural language search for user profiles
- 🎯 Hybrid search (semantic + structured filters)
- 🌍 Filter by country and startup status
- 🤖 AI-generated response summaries
- 📱 Responsive design (mobile-friendly)
- ⚡ Fast and modern UI with Tailwind CSS

## Tech Stack

- **Vue 3** - Composition API with `<script setup>`
- **Vite** - Build tool and dev server
- **Tailwind CSS** - Utility-first CSS framework
- **API** - Connects to ASP.NET Core 9 backend

---

## 🚀 Quick Start

### Development Mode (Recommended)

**Terminal 1 - Backend:**
```bash
cd ../../VibeApp.Api
dotnet run
```

**Terminal 2 - Frontend:**
```bash
npm install
npm run dev
```

Open: **http://localhost:5173**

✅ Hot reload, instant updates

---

### Production Mode (Test before deploy)

**Build frontend:**
```bash
npm install
npm run build
```

**Output:** `../VibeApp.Api/wwwroot/`

**Run backend:**
```bash
cd ../VibeApp.Api
dotnet run
```

Open: **http://localhost:5000**

✅ Same as production on Render

---

## 📁 Project Structure

```
src/frontend/
├── src/
│   ├── App.vue              # Main app component
│   ├── components/
│   │   ├── SearchForm.vue   # Search form with filters
│   │   └── ProfileCard.vue  # Profile result card
│   ├── services/
│   │   └── api.js           # RAG Search API client
│   ├── main.js              # App entry point
│   └── style.css            # Tailwind imports
├── index.html               # HTML template
├── package.json             # Dependencies
├── vite.config.js           # Build: ../VibeApp.Api/wwwroot
└── tailwind.config.js       # Tailwind config
```

---

## 🎨 Components

### SearchForm.vue
- Natural language query input
- Country filter (from API)
- Has Startup filter
- TopK results slider
- Min Similarity slider
- Generate AI Response toggle

### ProfileCard.vue
- User info (name, location)
- Similarity score badge
- Startup badge (if applicable)
- Skills tags (blue)
- Looking for tags (green)
- Contact links (Telegram, LinkedIn, Email)

### App.vue
- Layout and state management
- Loading/error states
- AI summary display
- Results grid (responsive)

---

## 🔌 API Integration

The frontend calls your ASP.NET Core API:

**Endpoint:** `POST /api/ragsearch/search`

**Request:**
```json
{
  "query": "Who here knows Rust?",
  "topK": 5,
  "generateResponse": true,
  "minSimilarity": 0.3,
  "filters": {
    "country": "Germany",
    "hasStartup": true
  }
}
```

**Response:**
```json
{
  "query": "Who here knows Rust?",
  "answer": "AI-generated summary...",
  "totalResults": 3,
  "results": [
    {
      "id": 1,
      "name": "John Doe",
      "skills": ["Rust", "Backend"],
      "similarityScore": 0.85,
      ...
    }
  ]
}
```

---

## ⚙️ Configuration

### vite.config.js

```js
export default defineConfig({
  build: {
    outDir: '../VibeApp.Api/wwwroot',  // ← Builds into backend!
    emptyOutDir: true
  },
  server: {
    proxy: {
      '/api': 'http://localhost:5000'  // ← API proxy for dev
    }
  }
})
```

### Environment Variables

Create `.env.local` to override API URL:

```env
VITE_API_BASE_URL=http://localhost:5000
```

For production, Render will use the same origin.

---

## 🏗️ Build Process

### Development
```
frontend/src/ → Vite Dev Server → http://localhost:5173
                (with HMR)
```

### Production
```
src/frontend/src/
   │
   │ npm run build
   ▼
../VibeApp.Api/wwwroot/
   ├── index.html
   └── assets/
       ├── index-[hash].js   (minified)
       └── index-[hash].css  (purged Tailwind)
```

---

## 📦 Scripts

- `npm run dev` - Start Vite dev server (port 5173)
- `npm run build` - Build to `../src/VibeApp.Api/wwwroot/`
- `npm run build:watch` - Build and watch for changes
- `npm run preview` - Preview production build

---

## 🎯 Usage Examples

### Example Queries

Try these in the search box:

- "Who here knows Rust and likes hiking?"
- "Find me someone with AI/ML experience"
- "Looking for a co-founder with marketing skills"
- "Who can help with startup fundraising in Germany?"

### Filters

- **Country**: Filter by specific country
- **Has Startup**: Show only founders or non-founders
- **Results (TopK)**: 3, 5, 10, or 20
- **Min Similarity**: 0.0 - 1.0 (higher = stricter)
- **Generate Response**: AI summary on/off

---

## 🚀 Deployment

**This frontend deploys automatically with the backend!**

When you push to Render:
1. Dockerfile installs Node.js
2. Runs `npm install && npm run build`
3. Frontend builds to wwwroot
4. ASP.NET Core serves it

**You don't deploy frontend separately!**

See: `../DEPLOY.md` for full instructions.

---

## 🐛 Troubleshooting

**"npm is not recognized"**
- Install Node.js: https://nodejs.org/ (LTS version)

**Changes not showing**
- Dev mode: Check Vite server is running
- Production: Rebuild with `npm run build`

**API not responding**
- Check backend is running on http://localhost:5000
- Check console for CORS errors (expected in dev)

**Build fails**
- Delete `node_modules` and run `npm install` again
- Check Node.js version: `node --version` (should be 18+)

---

## 📚 Documentation

- **Integration Guide:** `../docs/FRONTEND_INTEGRATION.md`
- **Deployment:** `../DEPLOY.md`
- **Solution Overview:** `../docs/SOLUTION_OVERVIEW.md`
- **Main README:** `../README.md`

---

## 🎨 Styling

Built with Tailwind CSS utility classes:

- **Primary Color:** Blue (#0ea5e9)
- **Accent:** Purple
- **Gradients:** Blue → Purple backgrounds
- **Cards:** White with shadows and hover effects
- **Responsive:** Mobile-first breakpoints

Custom colors in `tailwind.config.js`.

---

## 🔧 Customization

### Change API URL

Edit `src/services/api.js`:
```js
const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000';
```

### Add New Component

1. Create in `src/components/YourComponent.vue`
2. Import in `App.vue` or other component
3. Use in template

### Modify Styles

All styling is Tailwind utility classes. Edit component templates:
```vue
<div class="bg-blue-500 text-white p-4 rounded-lg">
  <!-- Your content -->
</div>
```

---

Built with ❤️ for Vibe Coding Hackathon


