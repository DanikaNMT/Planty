import React, { useEffect, useState } from 'react';
import { getPlant, waterPlant } from '../api/plants.js';
import { Loading } from '../components/Loading.jsx';
import { ErrorMessage } from '../components/ErrorMessage.jsx';
import { formatDate } from '../utils/formatDate.js';
import Link from '../components/Link.jsx';

export function PlantDetailPage({ id, navigate }) {
  const [plant, setPlant] = useState(null);
  const [loading, setLoading] = useState(false);
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
        </div>
      )}
    </div>
  );
}
