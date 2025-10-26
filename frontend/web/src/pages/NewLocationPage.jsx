import React, { useState } from 'react';
import { createLocation } from '../api/locations.js';
import { ErrorMessage } from '../components/ErrorMessage.jsx';

export function NewLocationPage({ navigate }) {
  const [data, setData] = useState({ name: '', description: '' });
  const [creating, setCreating] = useState(false);
  const [error, setError] = useState(null);

  function submit(e) {
    e.preventDefault();
    setCreating(true);
    setError(null);
    createLocation(data)
      .then(created => navigate(`/locations/${created.id}`))
      .catch(e => setError(e.message))
      .finally(() => setCreating(false));
  }

  return (
    <div>
      <div style={{ marginBottom: 'var(--spacing-lg)' }}>
        <button onClick={() => navigate('/locations')} className="btn-outline">
          ← Back to Locations
        </button>
      </div>
      
      <div className="card" style={{ maxWidth: '600px', margin: '0 auto' }}>
        <h2 style={{ fontSize: '2rem', fontWeight: '700', color: 'var(--color-primary)', marginBottom: 'var(--spacing-xl)', display: 'flex', alignItems: 'center', gap: 'var(--spacing-sm)' }}>
          <span>📍</span>
          Add New Location
        </h2>
        
        <form onSubmit={submit}>
          <div className="form-group">
            <label>📍 Location Name *</label>
            <input 
              type="text"
              value={data.name} 
              onChange={e => setData({ ...data, name: e.target.value })} 
              placeholder="e.g., Living Room, Balcony, Garden"
              required 
              maxLength={100}
            />
          </div>
          
          <div className="form-group">
            <label>📝 Description</label>
            <textarea 
              value={data.description} 
              onChange={e => setData({ ...data, description: e.target.value })} 
              placeholder="Tell us about this location..."
              maxLength={500}
            />
          </div>
          
          <button type="submit" disabled={creating} className="btn-large" style={{ width: '100%' }}>
            {creating ? '⏳ Creating...' : '➕ Add Location'}
          </button>
        </form>
        
        {creating && (
          <div className="loading">
            <div className="spinner"></div>
            Creating location...
          </div>
        )}
        <ErrorMessage error={error} />
      </div>
    </div>
  );
}
