import React, { useEffect, useState } from 'react';
import { getPlant, waterPlant, fertilizePlant, getPlantCareHistory, uploadPlantPicture, updatePlant, getLocations } from '../api/plants.js';
import { getSpecies } from '../api/species.js';
import { Loading } from '../components/Loading.jsx';
import { ErrorMessage } from '../components/ErrorMessage.jsx';
import { SpeciesAutocomplete } from '../components/SpeciesAutocomplete.jsx';
import { formatDate } from '../utils/formatDate.js';
import Link from '../components/Link.jsx';

// Permission helper functions based on ShareRole
// Viewer = 0, Carer = 1, Editor = 2, Owner = 3
const canView = (plant) => !plant.isShared || plant.userRole !== null;
const canCare = (plant) => !plant.isShared || (plant.userRole !== null && plant.userRole >= 1); // Carer or higher
const canEdit = (plant) => !plant.isShared || (plant.userRole !== null && plant.userRole >= 2); // Editor or higher
const canDelete = (plant) => !plant.isShared || (plant.userRole !== null && plant.userRole >= 3); // Owner only

export function PlantDetailPage({ id, navigate }) {
  const [plant, setPlant] = useState(null);
  const [careHistory, setCareHistory] = useState([]);
  const [locations, setLocations] = useState([]);
  const [species, setSpecies] = useState([]);
  const [loading, setLoading] = useState(false);
  const [loadingHistory, setLoadingHistory] = useState(false);
  const [error, setError] = useState(null);
  const [watering, setWatering] = useState(false);
  const [fertilizing, setFertilizing] = useState(false);
  const [uploading, setUploading] = useState(false);
  const [waterSuccess, setWaterSuccess] = useState(false);
  const [fertilizeSuccess, setFertilizeSuccess] = useState(false);
  const [uploadSuccess, setUploadSuccess] = useState(false);
  const [isEditing, setIsEditing] = useState(false);
  const [saving, setSaving] = useState(false);
  const [editForm, setEditForm] = useState({
    name: '',
    speciesId: '',
    description: '',
    locationId: ''
  });

  useEffect(() => {
    setLoading(true);
    setError(null);
    
    // Load plant, locations, and species in parallel
    Promise.all([
      getPlant(id),
      getLocations(),
      getSpecies()
    ])
      .then(([plant, locations, species]) => {
        setPlant(plant);
        setLocations(locations);
        setSpecies(species);
        // Initialize edit form with plant data
        setEditForm({
          name: plant.name || '',
          speciesId: plant.speciesId || '',
          description: plant.description || '',
          locationId: plant.locationId || ''
        });
      })
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

  const handleEditClick = () => {
    setIsEditing(true);
    setError(null);
  };

  const handleCancelEdit = () => {
    setIsEditing(false);
    setError(null);
    // Reset form to current plant data
    setEditForm({
      name: plant.name || '',
      species: plant.species || '',
      description: plant.description || '',
      wateringIntervalDays: plant.wateringIntervalDays || '',
      fertilizationIntervalDays: plant.fertilizationIntervalDays || '',
      locationId: plant.locationId || ''
    });
  };

  const handleSaveEdit = async () => {
    if (saving) return;
    
    setSaving(true);
    setError(null);
    
    try {
      const updatedPlant = await updatePlant(id, editForm);
      setPlant(updatedPlant);
      setIsEditing(false);
      // Update edit form with saved data
      setEditForm({
        name: updatedPlant.name || '',
        species: updatedPlant.species || '',
        description: updatedPlant.description || '',
        wateringIntervalDays: updatedPlant.wateringIntervalDays || '',
        fertilizationIntervalDays: updatedPlant.fertilizationIntervalDays || '',
        locationId: updatedPlant.locationId || ''
      });
    } catch (e) {
      setError(e.message);
    } finally {
      setSaving(false);
    }
  };

  const handleFormChange = (field, value) => {
    setEditForm(prev => ({ ...prev, [field]: value }));
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
            {plant.latestPictureUrl && !isEditing && (
              <div className="plant-detail-image">
                <img 
                  src={plant.latestPictureUrl} 
                  alt={`Latest picture of ${plant.name}`}
                  onError={(e) => { e.target.style.display = 'none'; }}
                />
              </div>
            )}
            
            <div className="plant-detail-info">
              <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', marginBottom: 'var(--spacing-md)' }}>
                {isEditing ? (
                  <div style={{ flex: 1 }}>
                    <label style={{ display: 'block', marginBottom: 'var(--spacing-xs)', fontWeight: '600', color: 'var(--color-text)' }}>
                      Plant Name *
                    </label>
                    <input
                      type="text"
                      value={editForm.name}
                      onChange={(e) => handleFormChange('name', e.target.value)}
                      placeholder="Enter plant name"
                      required
                      style={{
                        width: '100%',
                        padding: 'var(--spacing-sm) var(--spacing-md)',
                        border: '2px solid var(--color-border)',
                        borderRadius: 'var(--radius-md)',
                        fontSize: '1.5rem',
                        fontWeight: '700',
                      }}
                    />
                  </div>
                ) : (
                  <div style={{ flex: 1 }}>
                    <h2 className="plant-detail-title">🪴 {plant.name}</h2>
                    {plant.isShared && (
                      <div style={{ 
                        display: 'inline-flex', 
                        alignItems: 'center', 
                        gap: 'var(--spacing-xs)', 
                        padding: 'var(--spacing-xs) var(--spacing-sm)',
                        backgroundColor: 'var(--color-primary-light)',
                        borderRadius: 'var(--radius-sm)',
                        fontSize: '0.875rem',
                        fontWeight: '500',
                        color: 'var(--color-primary-dark)',
                        marginTop: 'var(--spacing-xs)'
                      }}>
                        <span>👥</span>
                        <span>
                          Shared by {plant.ownerName || 'another user'} · 
                          {plant.userRole === 0 && ' Viewer'}
                          {plant.userRole === 1 && ' Carer'}
                          {plant.userRole === 2 && ' Editor'}
                          {plant.userRole === 3 && ' Owner'}
                        </span>
                      </div>
                    )}
                  </div>
                )}
                {!isEditing && canEdit(plant) && (
                  <button onClick={handleEditClick} className="btn-outline btn-small">
                    ✏️ Edit
                  </button>
                )}
              </div>
              
              {isEditing ? (
                <div style={{ display: 'flex', flexDirection: 'column', gap: 'var(--spacing-md)' }}>
                  <div>
                    <label style={{ display: 'block', marginBottom: 'var(--spacing-xs)', fontWeight: '600', color: 'var(--color-text)' }}>
                      Description
                    </label>
                    <textarea
                      value={editForm.description}
                      onChange={(e) => handleFormChange('description', e.target.value)}
                      placeholder="Add notes about your plant..."
                      rows="3"
                      style={{
                        width: '100%',
                        padding: 'var(--spacing-sm) var(--spacing-md)',
                        border: '2px solid var(--color-border)',
                        borderRadius: 'var(--radius-md)',
                        fontSize: '1rem',
                        resize: 'vertical',
                      }}
                    />
                  </div>
                  <SpeciesAutocomplete 
                    species={species}
                    value={editForm.speciesId}
                    onChange={(speciesId) => handleFormChange('speciesId', speciesId)}
                    onSpeciesCreated={(newSpecies) => {
                      setSpecies([...species, newSpecies]);
                      handleFormChange('speciesId', newSpecies.id);
                    }}
                  />
                  <div>
                    <label style={{ display: 'block', marginBottom: 'var(--spacing-xs)', fontWeight: '600', color: 'var(--color-text)' }}>
                      Location
                    </label>
                    <select
                      value={editForm.locationId}
                      onChange={(e) => handleFormChange('locationId', e.target.value)}
                      style={{
                        width: '100%',
                        padding: 'var(--spacing-sm) var(--spacing-md)',
                        border: '2px solid var(--color-border)',
                        borderRadius: 'var(--radius-md)',
                        fontSize: '1rem',
                      }}
                    >
                      <option value="">No location</option>
                      {locations.map(location => (
                        <option key={location.id} value={location.id}>
                          {location.name}
                        </option>
                      ))}
                    </select>
                  </div>
                  <div style={{ display: 'flex', gap: 'var(--spacing-sm)', marginTop: 'var(--spacing-md)' }}>
                    <button
                      onClick={handleSaveEdit}
                      disabled={saving || !editForm.name.trim()}
                      className="btn-large"
                      style={{ flex: 1 }}
                    >
                      {saving ? '⏳ Saving...' : '💾 Save Changes'}
                    </button>
                    <button
                      onClick={handleCancelEdit}
                      disabled={saving}
                      className="btn-outline btn-large"
                      style={{ flex: 1 }}
                    >
                      ❌ Cancel
                    </button>
                  </div>
                </div>
              ) : (
                <>
                  <div className="plant-detail-meta">
                    {plant.speciesName && (
                      <div className="plant-detail-item">
                        <span className="plant-detail-label">🌺 Species:</span>
                        <span 
                          className="plant-detail-value" 
                          style={{ cursor: 'pointer', textDecoration: 'underline' }}
                          onClick={() => navigate(`/species/${plant.speciesId}`)}
                        >
                          {plant.speciesName}
                        </span>
                      </div>
                    )}
                    {plant.description && (
                      <div className="plant-detail-item">
                        <span className="plant-detail-label">📝 Description:</span>
                        <span className="plant-detail-value">{plant.description}</span>
                      </div>
                    )}
                    {plant.location && (
                      <div className="plant-detail-item">
                        <span className="plant-detail-label">� Location:</span>
                        <span className="plant-detail-value clickable-link" onClick={() => navigate(`/locations/${plant.locationId}`)}>{plant.location}</span>
                      </div>
                    )}
                    <div className="plant-detail-item">
                      <span className="plant-detail-label">� Date Added:</span>
                      <span className="plant-detail-value">{formatDate(plant.dateAdded)}</span>
                    </div>
                  </div>
                  
                  <div className="plant-actions">
                {canCare(plant) && (
                  <button
                    onClick={handleWaterPlant}
                    disabled={watering}
                    className="btn-water btn-large"
                  >
                    {watering ? '⏳ Watering...' : '💧 Water Plant'}
                  </button>
                )}
                {canCare(plant) && (
                  <button
                    onClick={handleFertilizePlant}
                    disabled={fertilizing}
                    className="btn-fertilizer btn-large"
                  >
                    {fertilizing ? '⏳ Fertilizing...' : '🌿 Fertilize Plant'}
                  </button>
                )}
                {canCare(plant) && (
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
                )}
              </div>
                </>
              )}
            </div>
          </div>

          {/* Care Schedule Section */}
          {!isEditing && (plant.lastWatered || plant.lastFertilized || plant.wateringIntervalDays || plant.fertilizationIntervalDays) && (
            <div className="care-schedule-section">
              <h3 style={{ 
                fontSize: '1.25rem', 
                fontWeight: '700', 
                marginBottom: 'var(--spacing-md)',
                display: 'flex',
                alignItems: 'center',
                gap: 'var(--spacing-sm)'
              }}>
                <span>📅</span>
                Care Schedule
              </h3>
              <div className="plant-detail-meta">
                <div className="plant-detail-item">
                  <span className="plant-detail-label">� Last Watered:</span>
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
              </div>
            </div>
          )}

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
                      <span style={{ marginLeft: 'auto', display: 'flex', alignItems: 'center', gap: 'var(--spacing-sm)', color: 'var(--color-text-light)' }}>
                        <span>👤 {event.userName}</span>
                        <span>•</span>
                        <span>{formatDate(event.timestamp)}</span>
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
