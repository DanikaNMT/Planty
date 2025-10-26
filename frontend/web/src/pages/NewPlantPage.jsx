import React, { useEffect, useState } from 'react';
import { createPlant, getLocations } from '../api/plants.js';
import { getSpecies } from '../api/species.js';
import { ErrorMessage } from '../components/ErrorMessage.jsx';

export function NewPlantPage({ navigate }) {
  const [data, setData] = useState({ name: '', speciesId: '', description: '', locationId: '' });
  const [species, setSpecies] = useState([]);
  const [locations, setLocations] = useState([]);
  const [creating, setCreating] = useState(false);
  const [error, setError] = useState(null);

  useEffect(() => {
    // Load species and locations
    getSpecies().then(setSpecies).catch(console.error);
    getLocations().then(setLocations).catch(console.error);
  }, []);

  function submit(e) {
    e.preventDefault();
    setCreating(true);
    setError(null);
    createPlant(data)
      .then(created => navigate(`/plant/${created.id}`))
      .catch(e => setError(e.message))
      .finally(() => setCreating(false));
  }

  return (
    <div>
      <div style={{ marginBottom: 'var(--spacing-lg)' }}>
        <button onClick={() => navigate('/')} className="btn-outline">
          ← Back to My Plants
        </button>
      </div>
      
      <div className="card" style={{ maxWidth: '600px', margin: '0 auto' }}>
        <h2 style={{ fontSize: '2rem', fontWeight: '700', color: 'var(--color-primary)', marginBottom: 'var(--spacing-xl)', display: 'flex', alignItems: 'center', gap: 'var(--spacing-sm)' }}>
          <span>🌱</span>
          Add New Plant
        </h2>
        
        <form onSubmit={submit}>
          <div className="form-group">
            <label>🪴 Plant Name *</label>
            <input 
              type="text"
              value={data.name} 
              onChange={e => setData({ ...data, name: e.target.value })} 
              placeholder="e.g., My Monstera, Basil, Tomato"
              required 
            />
          </div>
          
          <div className="form-group">
            <label>🌺 Species</label>
            <select 
              value={data.speciesId} 
              onChange={e => setData({ ...data, speciesId: e.target.value })}
            >
              <option value="">No species (no care schedule)</option>
              {species.map(s => (
                <option key={s.id} value={s.id}>
                  {s.name}
                  {(s.wateringIntervalDays || s.fertilizationIntervalDays) && 
                    ` (💧 ${s.wateringIntervalDays || '—'} days, 🌿 ${s.fertilizationIntervalDays || '—'} days)`
                  }
                </option>
              ))}
            </select>
            <small style={{ color: 'var(--color-text-light)', display: 'block', marginTop: 'var(--spacing-xs)' }}>
              Species defines watering and fertilization schedule. <a onClick={() => navigate('/species/new')} style={{ cursor: 'pointer', textDecoration: 'underline' }}>Create new species</a>
            </small>
          </div>
          
          <div className="form-group">
            <label>📝 Description</label>
            <textarea 
              value={data.description} 
              onChange={e => setData({ ...data, description: e.target.value })} 
              placeholder="Tell us about your plant..."
            />
          </div>
          
          <div className="form-group">
            <label>📍 Location</label>
            {locations.length === 0 ? (
              <div>
                <p style={{ color: 'var(--color-text-light)', marginBottom: 'var(--spacing-sm)' }}>
                  No locations available. <a onClick={() => navigate('/locations/new')} style={{ cursor: 'pointer', textDecoration: 'underline', color: 'var(--color-primary)' }}>Create a location</a> to organize your plants.
                </p>
              </div>
            ) : (
              <select 
                value={data.locationId} 
                onChange={e => setData({ ...data, locationId: e.target.value })}
              >
                <option value="">No location</option>
                {locations.map(l => (
                  <option key={l.id} value={l.id}>
                    {l.name}
                  </option>
                ))}
              </select>
            )}
          </div>
          
          <button type="submit" disabled={creating} className="btn-large" style={{ width: '100%' }}>
            {creating ? '⏳ Creating...' : '➕ Add Plant'}
          </button>
        </form>
        
        {creating && (
          <div className="loading">
            <div className="spinner"></div>
            Creating your plant...
          </div>
        )}
        <ErrorMessage error={error} />
      </div>
    </div>
  );
}
