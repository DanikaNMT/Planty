import React, { useEffect, useState } from 'react';
import { getPlants, waterPlant, fertilizePlant } from '../api/plants.js';
import { ErrorMessage } from '../components/ErrorMessage.jsx';
import { TodoSection } from '../components/TodoSection.jsx';
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
      {/* Todo Section - shows upcoming tasks for next 24 hours */}
      <TodoSection navigate={navigate} onActionComplete={load} />

      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 'var(--spacing-xl)' }}>
        <h2 style={{ fontSize: '2rem', fontWeight: '700', color: 'var(--color-primary)', display: 'flex', alignItems: 'center', gap: 'var(--spacing-sm)' }}>
          <span>🪴</span>
          My Plants
        </h2>
        <div style={{ display: 'flex', gap: 'var(--spacing-sm)' }}>
          <button onClick={load} disabled={loading} className="btn-outline btn-small">
            {loading ? '⏳ Loading...' : '🔄 Reload'}
          </button>
          <button onClick={() => navigate('/new')} className="btn-large">
            ➕ Add Plant
          </button>
        </div>
      </div>
      
      {loading && (
        <div className="loading">
          <div className="spinner"></div>
          Loading your plants...
        </div>
      )}
      
      <ErrorMessage error={error} />
      
      {waterSuccess && (
        <div className="message message-success">
          <span>💧</span>
          <strong>{waterSuccess}</strong> watered successfully!
        </div>
      )}
      
      {fertilizeSuccess && (
        <div className="message message-success">
          <span>🌿</span>
          <strong>{fertilizeSuccess}</strong> fertilized successfully!
        </div>
      )}
      
      <div>
        {plants.map(p => (
          <div 
            key={p.id} 
            className="plant-card"
            onClick={() => navigate(`/plant/${p.id}`)}
            style={{ cursor: 'pointer' }}
          >
            {p.latestPictureUrl && (
              <div>
                <img 
                  src={p.latestPictureUrl} 
                  alt={p.name}
                  className="plant-card-image"
                  onError={(e) => { e.target.style.display = 'none'; }}
                />
              </div>
            )}
            <div className="plant-card-content">
              <div className="plant-card-title">
                {p.name}{p.speciesName ? ` - ${p.speciesName}` : ''}
              </div>
              <div className="plant-info">
                <div>💧 Last watered: <strong>{p.lastWatered ? formatDate(p.lastWatered) : 'Never'}</strong></div>
                {p.nextWateringDue && (
                  <div>⏰ Next watering: <strong>{formatDate(p.nextWateringDue)}</strong></div>
                )}
                <div>🌿 Last fertilized: <strong>{p.lastFertilized ? formatDate(p.lastFertilized) : 'Never'}</strong></div>
                {p.nextFertilizationDue && (
                  <div>⏰ Next fertilization: <strong>{formatDate(p.nextFertilizationDue)}</strong></div>
                )}
              </div>
              <div className="plant-actions" onClick={(e) => e.stopPropagation()}>
                <button
                  onClick={() => handleWaterPlant(p.id, p.name)}
                  disabled={wateringStates[p.id]}
                  className="btn-water"
                >
                  {wateringStates[p.id] ? '⏳ Watering...' : '💧 Water'}
                </button>
                <button
                  onClick={() => handleFertilizePlant(p.id, p.name)}
                  disabled={fertilizingStates[p.id]}
                  className="btn-fertilizer"
                >
                  {fertilizingStates[p.id] ? '⏳ Fertilizing...' : '🌿 Fertilize'}
                </button>
              </div>
            </div>
          </div>
        ))}
      </div>
      
      {plants.length === 0 && !loading && (
        <div className="empty-state">
          <div className="empty-state-icon">🌱</div>
          <h3 className="empty-state-title">No plants yet!</h3>
          <p className="empty-state-message">Add your first plant to start your garden journey 🌿</p>
          <button onClick={() => navigate('/new')} className="btn-large">
            ➕ Add Your First Plant
          </button>
        </div>
      )}
    </div>
  );
}
