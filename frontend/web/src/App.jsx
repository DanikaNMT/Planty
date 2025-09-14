import React, { useState } from 'react';
import { useRouter } from './router.js';
import { HomePage } from './pages/HomePage.jsx';
import { PlantDetailPage } from './pages/PlantDetailPage.jsx';
import { NewPlantPage } from './pages/NewPlantPage.jsx';
import { AuthPage } from './pages/AuthPage.jsx';

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
    default:
      page = (
        <div>
          <div>Not Found</div>
          <button onClick={() => navigate('/')}>Home</button>
        </div>
      );
  }

  return (
    <div>
      <div>
        <button onClick={handleLogout}>Logout</button>
      </div>
      {page}
    </div>
  );
}
