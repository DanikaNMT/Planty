import React, { useEffect, useState } from 'react';
import { getPlant } from '../api/plants.js';
import { Loading } from '../components/Loading.jsx';
import { ErrorMessage } from '../components/ErrorMessage.jsx';
import { formatDate } from '../utils/formatDate.js';
import Link from '../components/Link.jsx';

export function PlantDetailPage({ id, navigate }) {
  const [plant, setPlant] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  useEffect(() => {
    setLoading(true);
    setError(null);
    getPlant(id)
      .then(setPlant)
      .catch(e => setError(e.message))
      .finally(() => setLoading(false));
  }, [id]);

  return (
    <div>
      <button onClick={() => navigate('/')}>Home</button>
      {loading && <Loading />}
      <ErrorMessage error={error} />
      {!loading && !plant && !error && <div>Not found.</div>}
      {plant && (
        <div>
          <h2>{plant.name}</h2>
          <div><strong>Species:</strong> {plant.species}</div>
          {plant.description && <div><strong>Description:</strong> {plant.description}</div>}
          <div><strong>Date Added:</strong> {formatDate(plant.dateAdded)}</div>
          {plant.lastWatered && <div><strong>Last Watered:</strong> {formatDate(plant.lastWatered)}</div>}
          <div><strong>Watering Interval (days):</strong> {plant.wateringIntervalDays}</div>
          {plant.location && <div><strong>Location:</strong> {plant.location}</div>}
          {plant.imageUrl && <div><strong>Image:</strong> {plant.imageUrl}</div>}
          {plant.nextWateringDue && <div><strong>Next Watering Due:</strong> {formatDate(plant.nextWateringDue)}</div>}
          <div>
            <Link to={'/'} navigate={navigate}>Back</Link>
          </div>
        </div>
      )}
    </div>
  );
}
