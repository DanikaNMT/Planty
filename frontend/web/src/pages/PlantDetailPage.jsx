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
      <div style={{ marginBottom: 'var(--spacing-lg)' }}>
        <Link to={'/'} navigate={navigate} style={{ fontSize: '1rem', display: 'inline-flex', alignItems: 'center', gap: 'var(--spacing-xs)' }}>
          ← Back to My Plants
        </Link>
      </div>

      {loading && (
        <div className="loading">
          <div className="spinner"></div>
          Loading plant details...
        </div>
      )}
      
      <ErrorMessage error={error} />
      
      {waterSuccess && (
        <div className="message message-success">
          <span>💧</span>
          Plant watered successfully!
        </div>
      )}
      
      {fertilizeSuccess && (
        <div className="message message-success">
          <span>🌿</span>
          Plant fertilized successfully!
        </div>
      )}

      {uploadSuccess && (
        <div className="message message-success">
          <span>📸</span>
          Picture uploaded successfully!
        </div>
      )}

      {!loading && !plant && !error && (
        <div className="empty-state">
          <div className="empty-state-icon">🔍</div>
          <h2 className="empty-state-title">Plant Not Found</h2>
          <p className="empty-state-message">This plant doesn't exist in your garden.</p>
          <button onClick={() => navigate('/')}>🏡 Go Home</button>
        </div>
      )}
      
      {plant && (
        <div>
          <div className="plant-detail-header">
            {plant.latestPictureUrl && (
              <div className="plant-detail-image">
                <img 
                  src={plant.latestPictureUrl} 
                  alt={`Latest picture of ${plant.name}`}
                  onError={(e) => { e.target.style.display = 'none'; }}
                />
              </div>
            )}
            
            <div className="plant-detail-info">
              <h2 className="plant-detail-title">🪴 {plant.name}</h2>
              
              <div className="plant-detail-meta">
                {plant.species && (
                  <div className="plant-detail-item">
                    <span className="plant-detail-label">🌺 Species:</span>
                    <span className="plant-detail-value">{plant.species}</span>
                  </div>
                )}
                {plant.description && (
                  <div className="plant-detail-item">
                    <span className="plant-detail-label">📝 Description:</span>
                    <span className="plant-detail-value">{plant.description}</span>
                  </div>
                )}
                <div className="plant-detail-item">
                  <span className="plant-detail-label">📅 Date Added:</span>
                  <span className="plant-detail-value">{formatDate(plant.dateAdded)}</span>
                </div>
                <div className="plant-detail-item">
                  <span className="plant-detail-label">💧 Last Watered:</span>
                  <span className="plant-detail-value">{plant.lastWatered ? formatDate(plant.lastWatered) : 'Never'}</span>
                </div>
                {plant.wateringIntervalDays && (
                  <div className="plant-detail-item">
                    <span className="plant-detail-label">⏰ Watering Interval:</span>
                    <span className="plant-detail-value">Every {plant.wateringIntervalDays} days</span>
                  </div>
                )}
                {plant.nextWateringDue && (
                  <div className="plant-detail-item">
                    <span className="plant-detail-label">🔔 Next Watering:</span>
                    <span className="plant-detail-value">{formatDate(plant.nextWateringDue)}</span>
                  </div>
                )}
                <div className="plant-detail-item">
                  <span className="plant-detail-label">🌿 Last Fertilized:</span>
                  <span className="plant-detail-value">{plant.lastFertilized ? formatDate(plant.lastFertilized) : 'Never'}</span>
                </div>
                {plant.fertilizationIntervalDays && (
                  <div className="plant-detail-item">
                    <span className="plant-detail-label">⏰ Fertilization Interval:</span>
                    <span className="plant-detail-value">Every {plant.fertilizationIntervalDays} days</span>
                  </div>
                )}
                {plant.nextFertilizationDue && (
                  <div className="plant-detail-item">
                    <span className="plant-detail-label">🔔 Next Fertilization:</span>
                    <span className="plant-detail-value">{formatDate(plant.nextFertilizationDue)}</span>
                  </div>
                )}
                {plant.location && (
                  <div className="plant-detail-item">
                    <span className="plant-detail-label">📍 Location:</span>
                    <span className="plant-detail-value">{plant.location}</span>
                  </div>
                )}
              </div>
              
              <div className="plant-actions">
                <button
                  onClick={handleWaterPlant}
                  disabled={watering}
                  className="btn-water btn-large"
                >
                  {watering ? '⏳ Watering...' : '💧 Water Plant'}
                </button>
                <button
                  onClick={handleFertilizePlant}
                  disabled={fertilizing}
                  className="btn-fertilizer btn-large"
                >
                  {fertilizing ? '⏳ Fertilizing...' : '🌿 Fertilize Plant'}
                </button>
                <label style={{ display: 'inline-block' }}>
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
                    className="btn-picture btn-large"
                    type="button"
                  >
                    {uploading ? '⏳ Uploading...' : '📸 Add Picture'}
                  </button>
                </label>
              </div>
            </div>
          </div>

          {/* Care History */}
          <div className="care-history">
            <h3 className="care-history-title">
              <span>📋</span>
              Care History
            </h3>
            {loadingHistory && (
              <div className="loading">
                <div className="spinner"></div>
                Loading history...
              </div>
            )}
            {careHistory.length === 0 && !loadingHistory && (
              <div className="empty-state">
                <div className="empty-state-icon">📝</div>
                <p className="empty-state-message">No care history yet. Water, fertilize, or add a picture to start tracking!</p>
              </div>
            )}
            {careHistory.length > 0 && (
              <div>
                {careHistory.map(event => (
                  <div 
                    key={event.id} 
                    className={`care-event care-event-${event.type.toLowerCase()}`}
                  >
                    <div className="care-event-header">
                      <span>
                        {event.type === 'Watering' && '💧'}
                        {event.type === 'Fertilization' && '🌿'}
                        {event.type === 'Picture' && '📸'}
                      </span>
                      <strong>{event.type}</strong>
                      <span style={{ marginLeft: 'auto', color: 'var(--color-text-light)' }}>
                        {formatDate(event.timestamp)}
                      </span>
                    </div>
                    {event.imageUrl && (
                      <div className="care-event-image">
                        <img 
                          src={event.imageUrl} 
                          alt="Plant picture"
                          style={{ maxWidth: '300px', maxHeight: '300px', objectFit: 'contain' }}
                        />
                      </div>
                    )}
                    {event.notes && (
                      <div className="care-event-notes">
                        💬 {event.notes}
                      </div>
                    )}
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>
      )}
    </div>
  );
}
