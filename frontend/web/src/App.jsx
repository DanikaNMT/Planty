import React from 'react';
import { useRouter } from './router.js';
import { HomePage } from './pages/HomePage.jsx';
import { PlantDetailPage } from './pages/PlantDetailPage.jsx';
import { NewPlantPage } from './pages/NewPlantPage.jsx';

export default function App() {
  const { route, navigate } = useRouter();

  switch (route.name) {
    case 'home':
      return <HomePage navigate={navigate} />;
    case 'plant':
      return <PlantDetailPage id={route.id} navigate={navigate} />;
    case 'new':
      return <NewPlantPage navigate={navigate} />;
    default:
      return (
        <div>
          <div>Not Found</div>
          <button onClick={() => navigate('/')}>Home</button>
        </div>
      );
  }
}
