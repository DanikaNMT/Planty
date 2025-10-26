import React, { useEffect, useState } from 'react';
import { getLocationById, updateLocation, deleteLocation } from '../api/locations.js';
import { getPlants, updatePlant } from '../api/plants.js';
import { ErrorMessage } from '../components/ErrorMessage.jsx';

export function LocationDetailPage({ navigate, id }) {
  const [location, setLocation] = useState(null);
  const [loading, setLoading] = useState(true);
  const [editing, setEditing] = useState(false);
  const [editData, setEditData] = useState({ name: '', description: '' });
  const [saving, setSaving] = useState(false);
  const [deleting, setDeleting] = useState(false);
  const [error, setError] = useState(null);
  const [showAssignPlant, setShowAssignPlant] = useState(false);
  const [allPlants, setAllPlants] = useState([]);
  const [loadingPlants, setLoadingPlants] = useState(false);
  const [assigning, setAssigning] = useState(false);

  useEffect(() => {
    loadLocation();
  }, [id]);

  function loadLocation() {
    setLoading(true);
    setError(null);
    getLocationById(id)
      .then(loc => {
        setLocation(loc);
        setEditData({ name: loc.name, description: loc.description || '' });
      })
      .catch(e => setError(e.message))
      .finally(() => setLoading(false));
  }

  function handleEdit() {
    setEditing(true);
    setEditData({ name: location.name, description: location.description || '' });
  }

  function handleCancelEdit() {
    setEditing(false);
    setEditData({ name: location.name, description: location.description || '' });
  }

  function handleSave(e) {
    e.preventDefault();
    setSaving(true);
    setError(null);
    updateLocation(id, editData)
      .then(updated => {
        setLocation(prev => ({ ...prev, ...updated }));
        setEditing(false);
      })
      .catch(e => setError(e.message))
      .finally(() => setSaving(false));
  }

  function handleDelete() {
    if (!confirm('Are you sure you want to delete this location? Plants in this location will have no location assigned.')) {
      return;
    }
    
    setDeleting(true);
    setError(null);
    deleteLocation(id)
      .then(() => navigate('/locations'))
      .catch(e => setError(e.message))
      .finally(() => setDeleting(false));
  }

  function handleShowAssignPlant() {
    setShowAssignPlant(true);
    setLoadingPlants(true);
    setError(null);
    getPlants()
      .then(plants => {
        // Sort plants: no location first, then by location name
        const sorted = plants.sort((a, b) => {
          if (!a.location && !b.location) return a.name.localeCompare(b.name);
          if (!a.location) return -1;
          if (!b.location) return 1;
          if (a.location === b.location) return a.name.localeCompare(b.name);
          return a.location.localeCompare(b.location);
        });
        setAllPlants(sorted);
      })
      .catch(e => setError(e.message))
      .finally(() => setLoadingPlants(false));
  }

  function handleAssignPlant(plantId) {
    setAssigning(true);
    setError(null);
    
    // Get the plant data first
    const plant = allPlants.find(p => p.id === plantId);
    if (!plant) {
      setError('Plant not found');
      setAssigning(false);
      return;
    }
    
    // Update the plant with the new location
    updatePlant(plantId, {
      name: plant.name,
      speciesId: plant.speciesId,
      description: plant.description,
      locationId: id
    })
      .then(() => {
        // Reload the location to get updated plant list
        loadLocation();
        setShowAssignPlant(false);
        setAllPlants([]);
      })
      .catch(e => setError(e.message))
      .finally(() => setAssigning(false));
  }

  if (loading) {
    return (
      <div className="loading">
        <div className="spinner"></div>
        Loading location...
      </div>
    );
  }

  if (!location) {
    return (
      <div className="card">
        <h2 style={{ color: 'var(--color-danger)' }}>Location not found</h2>
        <button onClick={() => navigate('/locations')} className="btn-outline">
          ← Back to Locations
        </button>
      </div>
    );
  }

  return (
    <div>
      <div style={{ marginBottom: 'var(--spacing-lg)' }}>
        <button onClick={() => navigate('/locations')} className="btn-outline">
          ← Back to Locations
        </button>
      </div>
      
      <div className="card">
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'start', marginBottom: 'var(--spacing-lg)' }}>
          <h2 style={{ fontSize: '2rem', fontWeight: '700', color: 'var(--color-primary)', display: 'flex', alignItems: 'center', gap: 'var(--spacing-sm)' }}>
            <span>📍</span>
            {location.name}
            {location.isDefault && (
              <span style={{ 
                fontSize: '0.8rem', 
                backgroundColor: 'var(--color-primary)',
                color: 'white',
                padding: '4px 12px',
                borderRadius: '4px'
              }}>
                Default
              </span>
            )}
          </h2>
          {!editing && !location.isDefault && (
            <div style={{ display: 'flex', gap: 'var(--spacing-sm)' }}>
              <button onClick={handleEdit} className="btn-outline">
                ✏️ Edit
              </button>
              <button 
                onClick={handleDelete} 
                className="btn-outline"
                disabled={deleting}
                style={{ color: 'var(--color-danger)', borderColor: 'var(--color-danger)' }}
              >
                {deleting ? '⏳ Deleting...' : '🗑️ Delete'}
              </button>
            </div>
          )}
        </div>

        <ErrorMessage error={error} />

        {editing ? (
          <form onSubmit={handleSave} style={{ marginBottom: 'var(--spacing-xl)' }}>
            <div className="form-group">
              <label>📍 Location Name *</label>
              <input 
                type="text"
                value={editData.name} 
                onChange={e => setEditData({ ...editData, name: e.target.value })} 
                required 
                maxLength={100}
              />
            </div>
            
            <div className="form-group">
              <label>📝 Description</label>
              <textarea 
                value={editData.description} 
                onChange={e => setEditData({ ...editData, description: e.target.value })} 
                maxLength={500}
              />
            </div>
            
            <div style={{ display: 'flex', gap: 'var(--spacing-sm)' }}>
              <button type="submit" disabled={saving} className="btn-primary">
                {saving ? '⏳ Saving...' : '💾 Save Changes'}
              </button>
              <button type="button" onClick={handleCancelEdit} className="btn-outline">
                Cancel
              </button>
            </div>
          </form>
        ) : (
          <div style={{ marginBottom: 'var(--spacing-xl)' }}>
            {location.description && (
              <p style={{ 
                color: 'var(--color-text-light)', 
                marginBottom: 'var(--spacing-md)',
                fontSize: '1.1rem'
              }}>
                {location.description}
              </p>
            )}
          </div>
        )}

        <div style={{ 
          borderTop: '2px solid var(--color-border)', 
          paddingTop: 'var(--spacing-lg)',
          marginTop: 'var(--spacing-lg)'
        }}>
          <h3 style={{ 
            fontSize: '1.5rem', 
            fontWeight: '600', 
            marginBottom: 'var(--spacing-md)',
            display: 'flex',
            alignItems: 'center',
            gap: 'var(--spacing-sm)'
          }}>
            🪴 Plants in this Location ({location.plants?.length || 0})
          </h3>

          {(!location.plants || location.plants.length === 0) ? (
            <div style={{ textAlign: 'center', padding: 'var(--spacing-xl)', color: 'var(--color-text-light)' }}>
              <p style={{ fontSize: '1.1rem', marginBottom: 'var(--spacing-md)' }}>
                No plants in this location yet
              </p>
              <button onClick={handleShowAssignPlant} className="btn-primary">
                ➕ Assign a Plant
              </button>
            </div>
          ) : (
            <>
              <div style={{ marginBottom: 'var(--spacing-md)', textAlign: 'right' }}>
                <button onClick={handleShowAssignPlant} className="btn-outline">
                  ➕ Assign More Plants
                </button>
              </div>
              <div style={{ display: 'grid', gap: 'var(--spacing-md)' }}>
                {location.plants.map(plant => (
                  <div 
                    key={plant.id} 
                    className="card"
                    onClick={() => navigate(`/plant/${plant.id}`)}
                    style={{ 
                      cursor: 'pointer',
                      transition: 'all 0.2s',
                      border: '1px solid var(--color-border)',
                      backgroundColor: 'white'
                    }}
                    onMouseEnter={e => {
                      e.currentTarget.style.borderColor = 'var(--color-primary)';
                      e.currentTarget.style.transform = 'translateY(-2px)';
                      e.currentTarget.style.boxShadow = '0 4px 8px rgba(0,0,0,0.1)';
                    }}
                    onMouseLeave={e => {
                      e.currentTarget.style.borderColor = 'var(--color-border)';
                      e.currentTarget.style.transform = 'translateY(0)';
                      e.currentTarget.style.boxShadow = 'none';
                    }}
                  >
                    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'start' }}>
                      <div>
                        <h4 style={{ fontSize: '1.2rem', fontWeight: '600', marginBottom: 'var(--spacing-xs)' }}>
                          🪴 {plant.name}
                        </h4>
                        {plant.speciesName && (
                          <p style={{ color: 'var(--color-text-light)', fontSize: '0.95rem', marginBottom: 'var(--spacing-xs)' }}>
                            🌺 {plant.speciesName}
                          </p>
                        )}
                        <p style={{ color: 'var(--color-text-light)', fontSize: '0.9rem' }}>
                          Added {new Date(plant.dateAdded).toLocaleDateString()}
                        </p>
                      </div>
                      <span style={{ fontSize: '1.5rem', opacity: 0.5 }}>→</span>
                    </div>
                  </div>
                ))}
              </div>
            </>
          )}
        </div>
      </div>

      {/* Assign Plant Modal */}
      {showAssignPlant && (
        <div 
          style={{
            position: 'fixed',
            top: 0,
            left: 0,
            right: 0,
            bottom: 0,
            backgroundColor: 'rgba(0, 0, 0, 0.5)',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            zIndex: 1000
          }}
          onClick={() => setShowAssignPlant(false)}
        >
          <div 
            className="card"
            style={{ 
              maxWidth: '600px',
              width: '90%',
              maxHeight: '80vh',
              overflow: 'auto',
              margin: 'var(--spacing-md)'
            }}
            onClick={e => e.stopPropagation()}
          >
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 'var(--spacing-lg)' }}>
              <h3 style={{ fontSize: '1.5rem', fontWeight: '600' }}>
                ➕ Assign Plant to {location.name}
              </h3>
              <button 
                onClick={() => setShowAssignPlant(false)}
                className="btn-outline"
                style={{ padding: '8px 16px' }}
              >
                ✕
              </button>
            </div>

            <ErrorMessage error={error} />

            {loadingPlants ? (
              <div className="loading">
                <div className="spinner"></div>
                Loading plants...
              </div>
            ) : allPlants.length === 0 ? (
              <div style={{ textAlign: 'center', padding: 'var(--spacing-xl)', color: 'var(--color-text-light)' }}>
                <p style={{ marginBottom: 'var(--spacing-md)' }}>No plants available to assign.</p>
                <button onClick={() => navigate('/plant/new')} className="btn-primary">
                  ➕ Create New Plant
                </button>
              </div>
            ) : (
              <div style={{ display: 'grid', gap: 'var(--spacing-sm)' }}>
                {/* Group header for plants without location */}
                {allPlants.some(p => !p.location) && (
                  <>
                    <h4 style={{ 
                      fontSize: '1rem', 
                      fontWeight: '600', 
                      color: 'var(--color-text-light)',
                      marginTop: 'var(--spacing-md)',
                      marginBottom: 'var(--spacing-xs)'
                    }}>
                      Plants without a location
                    </h4>
                    {allPlants.filter(p => !p.location).map(plant => (
                      <div 
                        key={plant.id}
                        style={{
                          padding: 'var(--spacing-md)',
                          border: '1px solid var(--color-border)',
                          borderRadius: '8px',
                          display: 'flex',
                          justifyContent: 'space-between',
                          alignItems: 'center',
                          backgroundColor: plant.locationId === id ? '#f0f9ff' : 'white'
                        }}
                      >
                        <div>
                          <div style={{ fontWeight: '600', marginBottom: '4px' }}>
                            🪴 {plant.name}
                          </div>
                          {plant.speciesName && (
                            <div style={{ fontSize: '0.9rem', color: 'var(--color-text-light)' }}>
                              🌺 {plant.speciesName}
                            </div>
                          )}
                        </div>
                        <button
                          onClick={() => handleAssignPlant(plant.id)}
                          disabled={assigning || plant.locationId === id}
                          className="btn-primary"
                          style={{ padding: '8px 16px', fontSize: '0.9rem' }}
                        >
                          {assigning ? '⏳' : plant.locationId === id ? '✓ Here' : '➕ Assign'}
                        </button>
                      </div>
                    ))}
                  </>
                )}

                {/* Group header for plants with locations */}
                {allPlants.some(p => p.location) && (
                  <>
                    <h4 style={{ 
                      fontSize: '1rem', 
                      fontWeight: '600', 
                      color: 'var(--color-text-light)',
                      marginTop: 'var(--spacing-lg)',
                      marginBottom: 'var(--spacing-xs)'
                    }}>
                      Plants in other locations
                    </h4>
                    {allPlants.filter(p => p.location).map(plant => (
                      <div 
                        key={plant.id}
                        style={{
                          padding: 'var(--spacing-md)',
                          border: '1px solid var(--color-border)',
                          borderRadius: '8px',
                          display: 'flex',
                          justifyContent: 'space-between',
                          alignItems: 'center',
                          backgroundColor: plant.locationId === id ? '#f0f9ff' : 'white'
                        }}
                      >
                        <div>
                          <div style={{ fontWeight: '600', marginBottom: '4px' }}>
                            🪴 {plant.name}
                          </div>
                          <div style={{ fontSize: '0.9rem', color: 'var(--color-text-light)' }}>
                            {plant.speciesName && <span>🌺 {plant.speciesName} • </span>}
                            <span>📍 {plant.location}</span>
                          </div>
                        </div>
                        <button
                          onClick={() => handleAssignPlant(plant.id)}
                          disabled={assigning || plant.locationId === id}
                          className="btn-primary"
                          style={{ padding: '8px 16px', fontSize: '0.9rem' }}
                        >
                          {assigning ? '⏳' : plant.locationId === id ? '✓ Here' : '➕ Assign'}
                        </button>
                      </div>
                    ))}
                  </>
                )}
              </div>
            )}
          </div>
        </div>
      )}
    </div>
  );
}
