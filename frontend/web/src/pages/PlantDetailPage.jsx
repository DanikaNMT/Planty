import React, { useEffect, useState } from 'react';
import { getPlant, waterPlant, getPlantWaterings } from '../api/plants.js';
import { Loading } from '../components/Loading.jsx';
import { ErrorMessage } from '../components/ErrorMessage.jsx';
import { formatDate } from '../utils/formatDate.js';
import Link from '../components/Link.jsx';

export function PlantDetailPage({ id, navigate }) {
  const [plant, setPlant] = useState(null);
  const [waterings, setWaterings] = useState([]);
  const [loading, setLoading] = useState(false);
  const [loadingWaterings, setLoadingWaterings] = useState(false);
  const [error, setError] = useState(null);
  const [watering, setWatering] = useState(false);
  const [waterSuccess, setWaterSuccess] = useState(false);

  useEffect(() => {
    setLoading(true);
    setError(null);
    getPlant(id)
      .then(setPlant)
      .catch(e => setError(e.message))
      .finally(() => setLoading(false));
    
    // Load watering history
    setLoadingWaterings(true);
    getPlantWaterings(id)
      .then(setWaterings)
      .catch(e => console.error('Failed to load watering history:', e.message))
      .finally(() => setLoadingWaterings(false));
  }, [id]);

  const handleWaterPlant = async () => {
    if (watering) return;
    
    setWatering(true);
    setError(null);
    setWaterSuccess(false);
    
    try {
      const updatedPlant = await waterPlant(id);
      setPlant(updatedPlant);
      setWaterSuccess(true);
      // Reload watering history
      const updatedWaterings = await getPlantWaterings(id);
      setWaterings(updatedWaterings);
      // Clear success message after 3 seconds
      setTimeout(() => setWaterSuccess(false), 3000);
    } catch (e) {
      setError(e.message);
    } finally {
      setWatering(false);
    }
  };

  return (
    <div>
      <div>
        <Link to={'/'} navigate={navigate}>← Back to Plants</Link>
      </div>

      {loading && <Loading />}
      <ErrorMessage error={error} />
      
      {waterSuccess && (
        <div>
          Plant watered successfully! 💧
        </div>
      )}

      {!loading && !plant && !error && <div>Not found.</div>}
      
      {plant && (
        <div>
          <h2>{plant.name}</h2>
          {plant.species && (
            <div>
              <strong>Species:</strong> {plant.species}
            </div>
          )}
          {plant.description && (
            <div>
              <strong>Description:</strong> {plant.description}
            </div>
          )}
          <div>
            <strong>Date Added:</strong> {formatDate(plant.dateAdded)}
          </div>
          <div>
            <strong>Last Watered:</strong> {plant.lastWatered ? formatDate(plant.lastWatered) : 'Never'}
          </div>
          {plant.wateringIntervalDays && (
            <div>
              <strong>Watering Interval (days):</strong> {plant.wateringIntervalDays}
            </div>
          )}
          {plant.location && (
            <div>
              <strong>Location:</strong> {plant.location}
            </div>
          )}
          {plant.imageUrl && (
            <div>
              <strong>Image:</strong> {plant.imageUrl}
            </div>
          )}
          {plant.nextWateringDue && (
            <div>
              <strong>Next Watering Due:</strong> {formatDate(plant.nextWateringDue)}
            </div>
          )}
          
          <div>
            <button
              onClick={handleWaterPlant}
              disabled={watering}
            >
              {watering ? 'Watering...' : '💧 Water Plant'}
            </button>
          </div>

          {/* Watering History */}
          <div style={{ marginTop: '30px' }}>
            <h3>Watering History</h3>
            {loadingWaterings && <p>Loading history...</p>}
            {waterings.length === 0 && !loadingWaterings && (
              <p>No watering history yet. Water your plant to start tracking!</p>
            )}
            {waterings.length > 0 && (
              <ul style={{ listStyle: 'none', padding: 0 }}>
                {waterings.map(w => (
                  <li key={w.id} style={{ marginBottom: '10px', padding: '10px', border: '1px solid #ddd', borderRadius: '4px' }}>
                    <div>
                      <strong>💧 Watered:</strong> {formatDate(w.wateredAt)}
                    </div>
                    {w.notes && (
                      <div style={{ marginTop: '5px', fontStyle: 'italic' }}>
                        Note: {w.notes}
                      </div>
                    )}
                  </li>
                ))}
              </ul>
            )}
          </div>
        </div>
      )}
    </div>
  );
}
