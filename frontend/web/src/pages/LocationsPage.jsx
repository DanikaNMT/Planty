import React, { useEffect, useState } from 'react';
import { getLocations } from '../api/locations.js';
import { ErrorMessage } from '../components/ErrorMessage.jsx';

export function LocationsPage({ navigate }) {
  const [locations, setLocations] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    loadLocations();
  }, []);

  function loadLocations() {
    setLoading(true);
    setError(null);
    getLocations()
      .then(setLocations)
      .catch(e => setError(e.message))
      .finally(() => setLoading(false));
  }

  return (
    <div>
      <div style={{ marginBottom: 'var(--spacing-lg)', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <button onClick={() => navigate('/')} className="btn-outline">
          ← Back to My Plants
        </button>
        <button onClick={() => navigate('/locations/new')} className="btn-primary">
          ➕ New Location
        </button>
      </div>
      
      <div className="card">
        <h2 style={{ fontSize: '2rem', fontWeight: '700', color: 'var(--color-primary)', marginBottom: 'var(--spacing-xl)', display: 'flex', alignItems: 'center', gap: 'var(--spacing-sm)' }}>
          <span>📍</span>
          My Locations
        </h2>
        
        {loading && (
          <div className="loading">
            <div className="spinner"></div>
            Loading locations...
          </div>
        )}
        
        <ErrorMessage error={error} />
        
        {!loading && !error && locations.length === 0 && (
          <div style={{ textAlign: 'center', padding: 'var(--spacing-xl)' }}>
            <p style={{ fontSize: '1.2rem', color: 'var(--color-text-light)', marginBottom: 'var(--spacing-md)' }}>
              📍 No locations yet
            </p>
            <p style={{ marginBottom: 'var(--spacing-lg)' }}>
              Create locations to organize your plants by room, garden area, or any other grouping.
            </p>
            <button onClick={() => navigate('/locations/new')} className="btn-primary">
              ➕ Create Your First Location
            </button>
          </div>
        )}
        
        {!loading && !error && locations.length > 0 && (
          <div style={{ display: 'grid', gap: 'var(--spacing-md)' }}>
            {locations.map(location => (
              <div 
                key={location.id} 
                className="card"
                onClick={() => navigate(`/locations/${location.id}`)}
                style={{ 
                  cursor: 'pointer',
                  transition: 'all 0.2s',
                  border: '1px solid var(--color-border)',
                  backgroundColor: location.isDefault ? 'var(--color-bg-secondary)' : 'white'
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
                  <div style={{ flex: 1 }}>
                    <h3 style={{ 
                      fontSize: '1.3rem', 
                      fontWeight: '600', 
                      marginBottom: 'var(--spacing-xs)',
                      display: 'flex',
                      alignItems: 'center',
                      gap: 'var(--spacing-sm)'
                    }}>
                      📍 {location.name}
                      {location.isDefault && (
                        <span style={{ 
                          fontSize: '0.8rem', 
                          backgroundColor: 'var(--color-primary)',
                          color: 'white',
                          padding: '2px 8px',
                          borderRadius: '4px'
                        }}>
                          Default
                        </span>
                      )}
                      {location.isShared && (
                        <span style={{ 
                          fontSize: '0.8rem', 
                          backgroundColor: 'var(--color-primary-light)',
                          color: 'var(--color-primary-dark)',
                          padding: '2px 8px',
                          borderRadius: '4px',
                          display: 'inline-flex',
                          alignItems: 'center',
                          gap: '4px'
                        }}>
                          <span>👥</span>
                          <span>
                            {location.userRole === 0 && 'Viewer'}
                            {location.userRole === 1 && 'Carer'}
                            {location.userRole === 2 && 'Editor'}
                            {location.userRole === 3 && 'Owner'}
                          </span>
                        </span>
                      )}
                    </h3>
                    {location.description && (
                      <p style={{ 
                        color: 'var(--color-text-light)', 
                        marginBottom: 'var(--spacing-sm)',
                        fontSize: '0.95rem'
                      }}>
                        {location.description}
                      </p>
                    )}
                    <div style={{ 
                      display: 'flex', 
                      alignItems: 'center', 
                      gap: 'var(--spacing-md)',
                      fontSize: '0.9rem',
                      color: 'var(--color-text-light)'
                    }}>
                      <span>🪴 {location.plantCount} {location.plantCount === 1 ? 'plant' : 'plants'}</span>
                    </div>
                  </div>
                  <span style={{ fontSize: '1.5rem', opacity: 0.5 }}>→</span>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}
