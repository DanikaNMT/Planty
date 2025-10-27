import React, { useEffect, useState } from 'react';
import { getSpeciesById, updateSpecies } from '../api/species.js';
import { getPlants } from '../api/plants.js';
import { ErrorMessage } from '../components/ErrorMessage.jsx';
import { formatDate } from '../utils/formatDate.js';

export function SpeciesDetailPage({ id, navigate }) {
  const [species, setSpecies] = useState(null);
  const [plants, setPlants] = useState([]);
  const [loading, setLoading] = useState(false);
  const [loadingPlants, setLoadingPlants] = useState(false);
  const [error, setError] = useState(null);
  const [isEditing, setIsEditing] = useState(false);
  const [saving, setSaving] = useState(false);
  const [saveSuccess, setSaveSuccess] = useState(false);
  const [editForm, setEditForm] = useState({
    name: '',
    description: '',
    wateringIntervalDays: '',
    fertilizationIntervalDays: ''
  });

  useEffect(() => {
    load();
    loadPlants();
  }, [id]);

  function load() {
    setLoading(true);
    setError(null);
    getSpeciesById(id)
      .then(data => {
        setSpecies(data);
        setEditForm({
          name: data.name || '',
          description: data.description || '',
          wateringIntervalDays: data.wateringIntervalDays || '',
          fertilizationIntervalDays: data.fertilizationIntervalDays || ''
        });
      })
      .catch(e => setError(e.message))
      .finally(() => setLoading(false));
  }

  function loadPlants() {
    setLoadingPlants(true);
    getPlants()
      .then(allPlants => {
        // Filter plants that belong to this species
        const speciesPlants = allPlants.filter(p => p.speciesId === id);
        setPlants(speciesPlants);
      })
      .catch(e => console.error('Failed to load plants:', e.message))
      .finally(() => setLoadingPlants(false));
  }

  const handleSave = async (e) => {
    e.preventDefault();
    setSaving(true);
    setError(null);
    setSaveSuccess(false);
    
    try {
      const updated = await updateSpecies(id, editForm);
      setSpecies(updated);
      setIsEditing(false);
      setSaveSuccess(true);
      setTimeout(() => setSaveSuccess(false), 3000);
    } catch (e) {
      setError(e.message);
    } finally {
      setSaving(false);
    }
  };

  const handleCancelEdit = () => {
    setIsEditing(false);
    setEditForm({
      name: species.name || '',
      description: species.description || '',
      wateringIntervalDays: species.wateringIntervalDays || '',
      fertilizationIntervalDays: species.fertilizationIntervalDays || ''
    });
  };

  if (loading) {
    return (
      <div className="loading">
        <div className="spinner"></div>
        Loading species...
      </div>
    );
  }

  if (!species) {
    return (
      <div className="empty-state">
        <div className="empty-state-icon">🌺</div>
        <h3 className="empty-state-title">Species Not Found</h3>
        <p className="empty-state-message">This species doesn't exist in your library.</p>
        <button onClick={() => navigate('/species')} className="btn-large">
          ← Back to Species
        </button>
      </div>
    );
  }

  return (
    <div>
      <div style={{ marginBottom: 'var(--spacing-lg)' }}>
        <button onClick={() => navigate('/species')} className="btn-outline">
          ← Back to Species
        </button>
      </div>
      
      {saveSuccess && (
        <div className="message message-success">
          <span>✅</span>
          Species updated successfully!
        </div>
      )}
      
      <ErrorMessage error={error} />
      
      <div className="card" style={{ marginBottom: 'var(--spacing-lg)' }}>
        {!isEditing ? (
          <div>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', marginBottom: 'var(--spacing-lg)' }}>
              <h2 style={{ fontSize: '2rem', fontWeight: '700', color: 'var(--color-primary)', display: 'flex', alignItems: 'center', gap: 'var(--spacing-sm)', flexWrap: 'wrap' }}>
                <span>🌺</span>
                {species.name}
                {species.isShared && (
                  <span style={{ 
                    display: 'inline-flex', 
                    alignItems: 'center', 
                    gap: '0.25rem', 
                    padding: '0.25rem 0.5rem',
                    backgroundColor: 'var(--color-primary-light)',
                    borderRadius: 'var(--radius-sm)',
                    fontSize: '0.875rem',
                    fontWeight: '500',
                    color: 'var(--color-primary-dark)'
                  }}>
                    <span>👥</span>
                    <span>Shared by {species.ownerName}</span>
                  </span>
                )}
              </h2>
              {!species.isShared && (
                <button onClick={() => setIsEditing(true)} className="btn-outline">
                  📝 Edit
                </button>
              )}
            </div>
            
            {species.description && (
              <div style={{ marginBottom: 'var(--spacing-lg)' }}>
                <h3 style={{ fontSize: '1.2rem', fontWeight: '600', marginBottom: 'var(--spacing-sm)' }}>
                  Description
                </h3>
                <p style={{ color: 'var(--color-text-light)' }}>{species.description}</p>
              </div>
            )}
            
            <div style={{ marginBottom: 'var(--spacing-lg)' }}>
              <h3 style={{ fontSize: '1.2rem', fontWeight: '600', marginBottom: 'var(--spacing-md)' }}>
                Care Information
              </h3>
              <div style={{ display: 'grid', gap: 'var(--spacing-md)' }}>
                {species.wateringIntervalDays ? (
                  <div style={{ display: 'flex', alignItems: 'center', gap: 'var(--spacing-sm)' }}>
                    <span style={{ fontSize: '1.5rem' }}>💧</span>
                    <div>
                      <div style={{ fontWeight: '600' }}>Watering</div>
                      <div style={{ color: 'var(--color-text-light)' }}>
                        Every {species.wateringIntervalDays} day{species.wateringIntervalDays !== 1 ? 's' : ''}
                      </div>
                    </div>
                  </div>
                ) : (
                  <div style={{ color: 'var(--color-text-light)', fontStyle: 'italic' }}>
                    💧 No watering schedule defined
                  </div>
                )}
                
                {species.fertilizationIntervalDays ? (
                  <div style={{ display: 'flex', alignItems: 'center', gap: 'var(--spacing-sm)' }}>
                    <span style={{ fontSize: '1.5rem' }}>🌿</span>
                    <div>
                      <div style={{ fontWeight: '600' }}>Fertilization</div>
                      <div style={{ color: 'var(--color-text-light)' }}>
                        Every {species.fertilizationIntervalDays} day{species.fertilizationIntervalDays !== 1 ? 's' : ''}
                      </div>
                    </div>
                  </div>
                ) : (
                  <div style={{ color: 'var(--color-text-light)', fontStyle: 'italic' }}>
                    🌿 No fertilization schedule defined
                  </div>
                )}
              </div>
            </div>
          </div>
        ) : (
          <form onSubmit={handleSave}>
            <h2 style={{ fontSize: '2rem', fontWeight: '700', color: 'var(--color-primary)', marginBottom: 'var(--spacing-xl)', display: 'flex', alignItems: 'center', gap: 'var(--spacing-sm)' }}>
              <span>📝</span>
              Edit Species
            </h2>
            
            <div className="form-group">
              <label>🌺 Species Name *</label>
              <input 
                type="text"
                value={editForm.name} 
                onChange={e => setEditForm({ ...editForm, name: e.target.value })} 
                required 
              />
            </div>
            
            <div className="form-group">
              <label>📝 Description</label>
              <textarea 
                value={editForm.description} 
                onChange={e => setEditForm({ ...editForm, description: e.target.value })} 
              />
            </div>
            
            <div className="form-group">
              <label>💧 Watering Interval (days)</label>
              <input 
                type="number" 
                value={editForm.wateringIntervalDays} 
                onChange={e => setEditForm({ ...editForm, wateringIntervalDays: e.target.value })} 
                min="1"
                max="365"
              />
            </div>
            
            <div className="form-group">
              <label>🌿 Fertilization Interval (days)</label>
              <input 
                type="number" 
                value={editForm.fertilizationIntervalDays} 
                onChange={e => setEditForm({ ...editForm, fertilizationIntervalDays: e.target.value })} 
                min="1"
                max="365"
              />
            </div>
            
            <div style={{ display: 'flex', gap: 'var(--spacing-sm)' }}>
              <button type="submit" disabled={saving} className="btn-large">
                {saving ? '⏳ Saving...' : '💾 Save Changes'}
              </button>
              <button type="button" onClick={handleCancelEdit} disabled={saving} className="btn-outline">
                ✖️ Cancel
              </button>
            </div>
          </form>
        )}
      </div>
      
      <div className="card">
        <h3 style={{ fontSize: '1.5rem', fontWeight: '600', marginBottom: 'var(--spacing-lg)' }}>
          🪴 Plants ({plants.length})
        </h3>
        
        {loadingPlants && (
          <div className="loading">
            <div className="spinner"></div>
            Loading plants...
          </div>
        )}
        
        {!loadingPlants && plants.length === 0 && (
          <div style={{ textAlign: 'center', padding: 'var(--spacing-xl)', color: 'var(--color-text-light)' }}>
            <div style={{ fontSize: '3rem', marginBottom: 'var(--spacing-sm)' }}>🌱</div>
            <p>No plants of this species yet</p>
          </div>
        )}
        
        <div style={{ display: 'grid', gap: 'var(--spacing-md)' }}>
          {plants.map(p => (
            <div 
              key={p.id} 
              style={{ 
                padding: 'var(--spacing-md)', 
                border: '1px solid var(--color-border)', 
                borderRadius: 'var(--radius-md)',
                cursor: 'pointer',
                transition: 'all 0.2s ease'
              }}
              onClick={() => navigate(`/plant/${p.id}`)}
              onMouseEnter={(e) => e.currentTarget.style.backgroundColor = 'var(--color-background)'}
              onMouseLeave={(e) => e.currentTarget.style.backgroundColor = 'transparent'}
            >
              <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                <div>
                  <h4 style={{ fontSize: '1.1rem', fontWeight: '600', marginBottom: 'var(--spacing-xs)' }}>
                    {p.name}
                  </h4>
                  <div style={{ fontSize: '0.9rem', color: 'var(--color-text-light)', display: 'flex', gap: 'var(--spacing-md)' }}>
                    {p.lastWatered && (
                      <span>💧 {formatDate(p.lastWatered)}</span>
                    )}
                    {p.lastFertilized && (
                      <span>🌿 {formatDate(p.lastFertilized)}</span>
                    )}
                    {p.locationName && (
                      <span>📍 {p.locationName}</span>
                    )}
                  </div>
                </div>
                <button className="btn-outline btn-small">
                  View →
                </button>
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}
