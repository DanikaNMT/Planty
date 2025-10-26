import React, { useEffect, useState } from 'react';
import { getPlants, waterPlant, fertilizePlant } from '../api/plants.js';
import Link from '../components/Link.jsx';
import { Loading } from '../components/Loading.jsx';
import { ErrorMessage } from '../components/ErrorMessage.jsx';
import { formatDate } from '../utils/formatDate.js';

export function HomePage({ navigate }) {
  const [plants, setPlants] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [wateringStates, setWateringStates] = useState({});
  const [fertilizingStates, setFertilizingStates] = useState({});
  const [waterSuccess, setWaterSuccess] = useState(null);
  const [fertilizeSuccess, setFertilizeSuccess] = useState(null);

  function load() {
    setLoading(true);
    setError(null);
    getPlants()
      .then(setPlants)
      .catch(e => setError(e.message))
      .finally(() => setLoading(false));
  }

  const handleWaterPlant = async (plantId, plantName) => {
    if (wateringStates[plantId]) return;
    
    setWateringStates(prev => ({ ...prev, [plantId]: true }));
    setError(null);
    setWaterSuccess(null);
    setFertilizeSuccess(null);
    
    try {
      const updatedPlant = await waterPlant(plantId);
      // Update the plant in the list
      setPlants(prev => prev.map(p => p.id === plantId ? updatedPlant : p));
      setWaterSuccess(plantName);
      // Clear success message after 3 seconds
      setTimeout(() => setWaterSuccess(null), 3000);
    } catch (e) {
      setError(e.message);
    } finally {
      setWateringStates(prev => ({ ...prev, [plantId]: false }));
    }
  };

  const handleFertilizePlant = async (plantId, plantName) => {
    if (fertilizingStates[plantId]) return;
    
    setFertilizingStates(prev => ({ ...prev, [plantId]: true }));
    setError(null);
    setWaterSuccess(null);
    setFertilizeSuccess(null);
    
    try {
      const updatedPlant = await fertilizePlant(plantId);
      // Update the plant in the list
      setPlants(prev => prev.map(p => p.id === plantId ? updatedPlant : p));
      setFertilizeSuccess(plantName);
      // Clear success message after 3 seconds
      setTimeout(() => setFertilizeSuccess(null), 3000);
    } catch (e) {
      setError(e.message);
    } finally {
      setFertilizingStates(prev => ({ ...prev, [plantId]: false }));
    }
  };

  useEffect(() => { load(); }, []);

  return (
    <div>
      <h1>Planty</h1>
      <div>
        <button onClick={load} disabled={loading}>
          {loading ? 'Loading...' : 'Reload'}
        </button>
        <button onClick={() => navigate('/new')}>
          Add Plant
        </button>
      </div>
      
      {loading && <Loading />}
      <ErrorMessage error={error} />
      
      {waterSuccess && (
        <div>
          {waterSuccess} watered successfully! 💧
        </div>
      )}
      
      {fertilizeSuccess && (
        <div>
          {fertilizeSuccess} fertilized successfully! 🌿
        </div>
      )}
      
      <div>
        {plants.map(p => (
          <div key={p.id} style={{ display: 'flex', gap: '20px', alignItems: 'flex-start', marginBottom: '20px', padding: '15px', border: '1px solid #ddd', borderRadius: '8px' }}>
            {p.latestPictureUrl && (
              <div style={{ flexShrink: 0 }}>
                <img 
                  src={p.latestPictureUrl} 
                  alt={p.name}
                  style={{ width: '100px', height: '100px', objectFit: 'cover', borderRadius: '8px' }}
                  onError={(e) => { e.target.style.display = 'none'; }}
                />
              </div>
            )}
            <div style={{ flex: 1 }}>
              <Link to={`/plant/${p.id}`} navigate={navigate}>
                {p.name}{p.species ? ` - ${p.species}` : ''}
              </Link>
              <div>
                Last watered: {p.lastWatered ? formatDate(p.lastWatered) : 'Never'}
              </div>
              {p.nextWateringDue && (
                <div>
                  Next watering: {formatDate(p.nextWateringDue)}
                </div>
              )}
              <div>
                Last fertilized: {p.lastFertilized ? formatDate(p.lastFertilized) : 'Never'}
              </div>
              {p.nextFertilizationDue && (
                <div>
                  Next fertilization: {formatDate(p.nextFertilizationDue)}
                </div>
              )}
              <div style={{ marginTop: '10px' }}>
                <button
                  onClick={() => handleWaterPlant(p.id, p.name)}
                  disabled={wateringStates[p.id]}
                >
                  {wateringStates[p.id] ? 'Watering...' : '💧 Water'}
                </button>
                <button
                  onClick={() => handleFertilizePlant(p.id, p.name)}
                  disabled={fertilizingStates[p.id]}
                  style={{ marginLeft: '10px' }}
                >
                  {fertilizingStates[p.id] ? 'Fertilizing...' : '🌿 Fertilize'}
                </button>
              </div>
            </div>
          </div>
        ))}
      </div>
      
      {plants.length === 0 && !loading && (
        <div>
          No plants yet. Add your first plant to get started! 🌱
        </div>
      )}
    </div>
  );
}
