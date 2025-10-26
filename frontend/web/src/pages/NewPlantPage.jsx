import React, { useState } from 'react';
import { createPlant } from '../api/plants.js';
import { ErrorMessage } from '../components/ErrorMessage.jsx';
import { Loading } from '../components/Loading.jsx';

export function NewPlantPage({ navigate }) {
  const [data, setData] = useState({ name: '', species: '', description: '', wateringIntervalDays: 7, fertilizationIntervalDays: '', location: '' });
  const [creating, setCreating] = useState(false);
  const [error, setError] = useState(null);

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
              placeholder="e.g., Monstera, Basil, Tomato"
              required 
            />
          </div>
          
          <div className="form-group">
            <label>🌺 Species</label>
            <input 
              type="text"
              value={data.species} 
              onChange={e => setData({ ...data, species: e.target.value })} 
              placeholder="e.g., Monstera deliciosa"
            />
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
            <label>💧 Watering Interval (days)</label>
            <input 
              type="number" 
              value={data.wateringIntervalDays} 
              onChange={e => setData({ ...data, wateringIntervalDays: e.target.value })} 
              min="1"
              placeholder="7"
            />
            <small style={{ color: 'var(--color-text-light)', display: 'block', marginTop: 'var(--spacing-xs)' }}>
              How often does this plant need water?
            </small>
          </div>
          
          <div className="form-group">
            <label>🌿 Fertilization Interval (days)</label>
            <input 
              type="number" 
              value={data.fertilizationIntervalDays} 
              onChange={e => setData({ ...data, fertilizationIntervalDays: e.target.value })} 
              min="1"
              placeholder="30"
            />
            <small style={{ color: 'var(--color-text-light)', display: 'block', marginTop: 'var(--spacing-xs)' }}>
              How often should you fertilize?
            </small>
          </div>
          
          <div className="form-group">
            <label>📍 Location</label>
            <input 
              type="text"
              value={data.location} 
              onChange={e => setData({ ...data, location: e.target.value })} 
              placeholder="e.g., Living room, Balcony, Kitchen"
            />
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
