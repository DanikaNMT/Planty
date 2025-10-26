import React, { useState } from 'react';
import { createSpecies } from '../api/species.js';
import { ErrorMessage } from '../components/ErrorMessage.jsx';

export function NewSpeciesPage({ navigate }) {
  const [data, setData] = useState({ 
    name: '', 
    description: '', 
    wateringIntervalDays: '', 
    fertilizationIntervalDays: '' 
  });
  const [creating, setCreating] = useState(false);
  const [error, setError] = useState(null);

  function submit(e) {
    e.preventDefault();
    setCreating(true);
    setError(null);
    createSpecies(data)
      .then(created => navigate(`/species/${created.id}`))
      .catch(e => setError(e.message))
      .finally(() => setCreating(false));
  }

  return (
    <div>
      <div style={{ marginBottom: 'var(--spacing-lg)' }}>
        <button onClick={() => navigate('/species')} className="btn-outline">
          ← Back to Species
        </button>
      </div>
      
      <div className="card" style={{ maxWidth: '600px', margin: '0 auto' }}>
        <h2 style={{ fontSize: '2rem', fontWeight: '700', color: 'var(--color-primary)', marginBottom: 'var(--spacing-xl)', display: 'flex', alignItems: 'center', gap: 'var(--spacing-sm)' }}>
          <span>🌺</span>
          Add New Species
        </h2>
        
        <form onSubmit={submit}>
          <div className="form-group">
            <label>🌺 Species Name *</label>
            <input 
              type="text"
              value={data.name} 
              onChange={e => setData({ ...data, name: e.target.value })} 
              placeholder="e.g., Monstera deliciosa, Basil"
              required 
            />
          </div>
          
          <div className="form-group">
            <label>📝 Description</label>
            <textarea 
              value={data.description} 
              onChange={e => setData({ ...data, description: e.target.value })} 
              placeholder="Care tips and characteristics..."
            />
          </div>
          
          <div className="form-group">
            <label>💧 Watering Interval (days)</label>
            <input 
              type="number" 
              value={data.wateringIntervalDays} 
              onChange={e => setData({ ...data, wateringIntervalDays: e.target.value })} 
              min="1"
              max="365"
              placeholder="7"
            />
            <small style={{ color: 'var(--color-text-light)', display: 'block', marginTop: 'var(--spacing-xs)' }}>
              How often does this species need water?
            </small>
          </div>
          
          <div className="form-group">
            <label>🌿 Fertilization Interval (days)</label>
            <input 
              type="number" 
              value={data.fertilizationIntervalDays} 
              onChange={e => setData({ ...data, fertilizationIntervalDays: e.target.value })} 
              min="1"
              max="365"
              placeholder="30"
            />
            <small style={{ color: 'var(--color-text-light)', display: 'block', marginTop: 'var(--spacing-xs)' }}>
              How often should you fertilize?
            </small>
          </div>
          
          <button type="submit" disabled={creating} className="btn-large" style={{ width: '100%' }}>
            {creating ? '⏳ Creating...' : '➕ Add Species'}
          </button>
        </form>
        
        {creating && (
          <div className="loading">
            <div className="spinner"></div>
            Creating species...
          </div>
        )}
        <ErrorMessage error={error} />
      </div>
    </div>
  );
}
