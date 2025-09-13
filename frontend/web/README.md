# Planty Web (Minimal React Frontend)

Very small React + Vite frontend (no CSS) to interact with the Planty API.

## Pages
- `/` Home: lists all plants and has a button to add a new plant.
- `/plant/{id}` Plant detail: shows a single plant's properties.
- `/new` New plant form: create a plant; on success redirects to `/plant/{createdId}`.

## Architecture / Structure
```
src/
  App.jsx               # Top-level route switch
  router.js             # Tiny route parser + hook
  api/
    client.js           # fetch wrapper
    plants.js           # plant-specific API calls
  pages/
    HomePage.jsx
    PlantDetailPage.jsx
    NewPlantPage.jsx
  components/
    Link.jsx
    Loading.jsx
    ErrorMessage.jsx
  utils/
    formatDate.js       # date formatting helper
  models/
    plant.js            # JSDoc typedefs for Plant
```

Separation keeps concerns clear while staying minimal and dependency-light.

## Features Implemented
- List plants (`GET /api/plants`)
- View plant detail (`GET /api/plants/{id}`)
- Create plant (`POST /api/plants`) and redirect to detail

## Backend Assumptions
Backend runs locally on `http://localhost:5153` (from `launchSettings.json`). CORS is open via `AllowFrontend` policy.

## Run Backend
```powershell
cd backend
dotnet run --project src/Planty.API/Planty.API.csproj --launch-profile http
```

## Run Frontend
```powershell
cd frontend/web
npm install
npm run dev
```
Open: http://localhost:5173

## Environment Override (Optional)
Create `.env` in `frontend/web` if backend URL differs:
```
VITE_API_BASE=http://localhost:5153
```

## Build (Optional)
```powershell
npm run build
```
Output in `dist/`.

## Extending
Add routes: update `router.js` parse logic and add a page component.
Add new API calls: create file in `api/` or extend `plants.js`.
Add shared UI: create new component in `components/`.
Add types: extend `models/plant.js` or migrate to TypeScript.

## Notes
- Plain HTML only, intentionally no styling.
- Ensure your static hosting serves `index.html` for unknown routes (history fallback) if deploying.
