import React, { useState } from 'react';
import { useRouter } from './router.js';
import { HomePage } from './pages/HomePage.jsx';
import { PlantDetailPage } from './pages/PlantDetailPage.jsx';
import { NewPlantPage } from './pages/NewPlantPage.jsx';
import { TodosPage } from './pages/TodosPage.jsx';
import { AuthPage } from './pages/AuthPage.jsx';
import { SpeciesPage } from './pages/SpeciesPage.jsx';
import { NewSpeciesPage } from './pages/NewSpeciesPage.jsx';
import { SpeciesDetailPage } from './pages/SpeciesDetailPage.jsx';

export default function App() {
  const { route, navigate } = useRouter();
  const [token, setToken] = useState(() => localStorage.getItem('jwt'));
  const [authMode, setAuthMode] = useState('login');

  function handleAuth(newToken) {
    setToken(newToken);
    localStorage.setItem('jwt', newToken);
    navigate('/');
  }

  function handleLogout() {
    setToken(null);
    localStorage.removeItem('jwt');
    navigate('/');
  }

  if (!token) {
    return <AuthPage onAuth={handleAuth} mode={authMode} switchMode={() => setAuthMode(m => m === 'login' ? 'register' : 'login')} />;
  }

  let page;
  switch (route.name) {
    case 'home':
      page = <HomePage navigate={navigate} />; break;
    case 'plant':
      page = <PlantDetailPage id={route.id} navigate={navigate} />; break;
    case 'new':
      page = <NewPlantPage navigate={navigate} />; break;
    case 'todos':
      page = <TodosPage navigate={navigate} />; break;
    case 'species':
      page = <SpeciesPage navigate={navigate} />; break;
    case 'species-new':
      page = <NewSpeciesPage navigate={navigate} />; break;
    case 'species-detail':
      page = <SpeciesDetailPage id={route.id} navigate={navigate} />; break;
    default:
      page = (
        <div className="empty-state">
          <div className="empty-state-icon">🌵</div>
          <h2 className="empty-state-title">Page Not Found</h2>
          <p className="empty-state-message">Oops! This plant doesn't exist in our garden.</p>
          <button onClick={() => navigate('/')}>🏡 Go Home</button>
        </div>
      );
  }

  return (
    <div className="app-container">
      <header className="app-header">
        <h1 className="app-title">
          <span>🌱</span>
          Planty
        </h1>
        <div style={{ display: 'flex', gap: 'var(--spacing-sm)', alignItems: 'center' }}>
          <button onClick={() => navigate('/')} className="btn-outline btn-small">
            🏡 Home
          </button>
          <button onClick={() => navigate('/species')} className="btn-outline btn-small">
            🌺 Species
          </button>
          <button onClick={() => navigate('/todos')} className="btn-outline btn-small">
            ✅ Todos
          </button>
          <button onClick={handleLogout} className="btn-outline btn-small">
            👋 Logout
          </button>
        </div>
      </header>
      {page}
    </div>
  );
}
