import React, { useEffect, useState } from 'react';
import { getPlant, waterPlant, fertilizePlant, getPlantCareHistory, uploadPlantPicture } from '../api/plants.js';
import { Loading } from '../components/Loading.jsx';
import { ErrorMessage } from '../components/ErrorMessage.jsx';
import { formatDate } from '../utils/formatDate.js';
import Link from '../components/Link.jsx';

export function PlantDetailPage({ id, navigate }) {
  const [plant, setPlant] = useState(null);
  const [careHistory, setCareHistory] = useState([]);
  const [loading, setLoading] = useState(false);
  const [loadingHistory, setLoadingHistory] = useState(false);
  const [error, setError] = useState(null);
  const [watering, setWatering] = useState(false);
  const [fertilizing, setFertilizing] = useState(false);
  const [uploading, setUploading] = useState(false);
  const [waterSuccess, setWaterSuccess] = useState(false);
  const [fertilizeSuccess, setFertilizeSuccess] = useState(false);
  const [uploadSuccess, setUploadSuccess] = useState(false);

  useEffect(() => {
    setLoading(true);
    setError(null);
    getPlant(id)
      .then(setPlant)
      .catch(e => setError(e.message))
      .finally(() => setLoading(false));
    
    // Load care history
    setLoadingHistory(true);
    getPlantCareHistory(id)
      .then(setCareHistory)
      .catch(e => console.error('Failed to load care history:', e.message))
      .finally(() => setLoadingHistory(false));
  }, [id]);

  const handleWaterPlant = async () => {
    if (watering) return;
    
    setWatering(true);
    setError(null);
    setWaterSuccess(false);
    setFertilizeSuccess(false);
    
    try {
      const updatedPlant = await waterPlant(id);
      setPlant(updatedPlant);
      setWaterSuccess(true);
      // Reload care history
      const updatedHistory = await getPlantCareHistory(id);
      setCareHistory(updatedHistory);
      // Clear success message after 3 seconds
      setTimeout(() => setWaterSuccess(false), 3000);
    } catch (e) {
      setError(e.message);
    } finally {
      setWatering(false);
    }
  };

  const handleFertilizePlant = async () => {
    if (fertilizing) return;
    
    setFertilizing(true);
    setError(null);
    setWaterSuccess(false);
    setFertilizeSuccess(false);
    
    try {
      const updatedPlant = await fertilizePlant(id);
      setPlant(updatedPlant);
      setFertilizeSuccess(true);
      // Reload care history
      const updatedHistory = await getPlantCareHistory(id);
      setCareHistory(updatedHistory);
      // Clear success message after 3 seconds
      setTimeout(() => setFertilizeSuccess(false), 3000);
    } catch (e) {
      setError(e.message);
    } finally {
      setFertilizing(false);
    }
  };

  const handlePictureUpload = async (event) => {
    const file = event.target.files?.[0];
    if (!file) return;
    
    setUploading(true);
    setError(null);
    setWaterSuccess(false);
    setFertilizeSuccess(false);
    setUploadSuccess(false);
    
    try {
      await uploadPlantPicture(id, file);
      setUploadSuccess(true);
      // Reload plant and care history
      const [updatedPlant, updatedHistory] = await Promise.all([
        getPlant(id),
        getPlantCareHistory(id)
      ]);
      setPlant(updatedPlant);
      setCareHistory(updatedHistory);
      // Clear success message after 3 seconds
      setTimeout(() => setUploadSuccess(false), 3000);
      // Reset file input
      event.target.value = '';
    } catch (e) {
      setError(e.message);
    } finally {
      setUploading(false);
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
      
      {fertilizeSuccess && (
        <div>
          Plant fertilized successfully! 🌿
        </div>
      )}

      {uploadSuccess && (
        <div>
          Picture uploaded successfully! 📸
        </div>
      )}

      {!loading && !plant && !error && <div>Not found.</div>}
      
      {plant && (
        <div>
          <h2>{plant.name}</h2>
          
          {/* Latest Picture Display */}
          {plant.latestPictureUrl && (
            <div style={{ marginBottom: '20px' }}>
              <img 
                src={plant.latestPictureUrl} 
                alt={`Latest picture of ${plant.name}`}
                style={{ maxWidth: '400px', maxHeight: '400px', objectFit: 'contain', borderRadius: '8px', border: '1px solid #ddd' }}
                onError={(e) => { e.target.style.display = 'none'; }}
              />
            </div>
          )}
          
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
            <strong>Last Fertilized:</strong> {plant.lastFertilized ? formatDate(plant.lastFertilized) : 'Never'}
          </div>
          {plant.fertilizationIntervalDays && (
            <div>
              <strong>Fertilization Interval (days):</strong> {plant.fertilizationIntervalDays}
            </div>
          )}
          {plant.nextFertilizationDue && (
            <div>
              <strong>Next Fertilization Due:</strong> {formatDate(plant.nextFertilizationDue)}
            </div>
          )}
          
          <div>
            <button
              onClick={handleWaterPlant}
              disabled={watering}
            >
              {watering ? 'Watering...' : '💧 Water Plant'}
            </button>
            <button
              onClick={handleFertilizePlant}
              disabled={fertilizing}
              style={{ marginLeft: '10px' }}
            >
              {fertilizing ? 'Fertilizing...' : '🌿 Fertilize Plant'}
            </button>
            <label style={{ marginLeft: '10px', display: 'inline-block' }}>
              <input
                type="file"
                accept="image/*"
                onChange={handlePictureUpload}
                disabled={uploading}
                style={{ display: 'none' }}
                id="picture-upload"
              />
              <button
                onClick={() => document.getElementById('picture-upload').click()}
                disabled={uploading}
                style={{ cursor: 'pointer' }}
              >
                {uploading ? 'Uploading...' : '📸 Add Picture'}
              </button>
            </label>
          </div>

          {/* Care History */}
          <div style={{ marginTop: '30px' }}>
            <h3>Care History</h3>
            {loadingHistory && <p>Loading history...</p>}
            {careHistory.length === 0 && !loadingHistory && (
              <p>No care history yet. Water, fertilize, or add a picture to start tracking!</p>
            )}
            {careHistory.length > 0 && (
              <ul style={{ listStyle: 'none', padding: 0 }}>
                {careHistory.map(event => (
                  <li key={event.id} style={{ marginBottom: '15px', padding: '10px', border: '1px solid #ddd', borderRadius: '4px' }}>
                    <div>
                      <strong>
                        {event.type === 'Watering' && '💧'}
                        {event.type === 'Fertilization' && '🌿'}
                        {event.type === 'Picture' && '📸'}
                        {' '}{event.type}:
                      </strong> {formatDate(event.timestamp)}
                    </div>
                    {event.imageUrl && (
                      <div style={{ marginTop: '10px' }}>
                        <img 
                          src={event.imageUrl} 
                          alt="Plant picture"
                          style={{ maxWidth: '300px', maxHeight: '300px', objectFit: 'contain', borderRadius: '4px' }}
                        />
                      </div>
                    )}
                    {event.notes && (
                      <div style={{ marginTop: '5px', fontStyle: 'italic' }}>
                        Note: {event.notes}
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
