import React, { useEffect, useState } from 'react';
import { getSpecies, deleteSpecies } from '../api/species.js';
import { ErrorMessage } from '../components/ErrorMessage.jsx';

export function SpeciesPage({ navigate }) {
  const [species, setSpecies] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [deletingId, setDeletingId] = useState(null);
  const [deleteSuccess, setDeleteSuccess] = useState(null);

  function load() {
    setLoading(true);
    setError(null);
    getSpecies()
      .then(setSpecies)
      .catch(e => setError(e.message))
      .finally(() => setLoading(false));
  }

  const handleDelete = async (id, name) => {
    if (!confirm(`Are you sure you want to delete "${name}"? This will remove the species from all plants using it.`)) {
      return;
    }
    
    setDeletingId(id);
    setError(null);
    setDeleteSuccess(null);
    
    try {
      await deleteSpecies(id);
      setSpecies(prev => prev.filter(s => s.id !== id));
      setDeleteSuccess(name);
      setTimeout(() => setDeleteSuccess(null), 3000);
    } catch (e) {
      setError(e.message);
    } finally {
      setDeletingId(null);
    }
  };

  useEffect(() => { load(); }, []);

  return (
    <div>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 'var(--spacing-xl)' }}>
        <h2 style={{ fontSize: '2rem', fontWeight: '700', color: 'var(--color-primary)', display: 'flex', alignItems: 'center', gap: 'var(--spacing-sm)' }}>
          <span>🌺</span>
          Species Library
        </h2>
        <div style={{ display: 'flex', gap: 'var(--spacing-sm)' }}>
          <button onClick={() => navigate('/')} className="btn-outline btn-small">
            🏡 Home
          </button>
          <button onClick={load} disabled={loading} className="btn-outline btn-small">
            {loading ? '⏳ Loading...' : '🔄 Reload'}
          </button>
          <button onClick={() => navigate('/species/new')} className="btn-large">
            ➕ Add Species
          </button>
        </div>
      </div>
      
      {loading && (
        <div className="loading">
          <div className="spinner"></div>
          Loading species...
        </div>
      )}
      
      <ErrorMessage error={error} />
      
      {deleteSuccess && (
        <div className="message message-success">
          <span>✅</span>
          <strong>{deleteSuccess}</strong> deleted successfully!
        </div>
      )}
      
      <div style={{ display: 'grid', gap: 'var(--spacing-md)' }}>
        {species.map(s => (
          <div 
            key={s.id} 
            className="card"
            style={{ padding: 'var(--spacing-lg)' }}
          >
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
              <div 
                style={{ flex: 1, cursor: 'pointer' }}
                onClick={() => navigate(`/species/${s.id}`)}
              >
                <h3 style={{ fontSize: '1.5rem', fontWeight: '600', marginBottom: 'var(--spacing-sm)' }}>
                  {s.name}
                </h3>
                {s.description && (
                  <p style={{ color: 'var(--color-text-light)', marginBottom: 'var(--spacing-md)' }}>
                    {s.description}
                  </p>
                )}
                <div style={{ display: 'flex', gap: 'var(--spacing-lg)', fontSize: '0.95rem' }}>
                  {s.wateringIntervalDays && (
                    <div>💧 Every <strong>{s.wateringIntervalDays}</strong> days</div>
                  )}
                  {s.fertilizationIntervalDays && (
                    <div>🌿 Every <strong>{s.fertilizationIntervalDays}</strong> days</div>
                  )}
                  <div>🪴 <strong>{s.plantCount}</strong> plant{s.plantCount !== 1 ? 's' : ''}</div>
                </div>
              </div>
              <div style={{ display: 'flex', gap: 'var(--spacing-xs)' }}>
                <button
                  onClick={() => navigate(`/species/${s.id}`)}
                  className="btn-outline btn-small"
                >
                  📝 Edit
                </button>
                <button
                  onClick={(e) => {
                    e.stopPropagation();
                    handleDelete(s.id, s.name);
                  }}
                  disabled={deletingId === s.id}
                  className="btn-outline btn-small"
                  style={{ color: 'var(--color-danger)' }}
                >
                  {deletingId === s.id ? '⏳' : '🗑️'} Delete
                </button>
              </div>
            </div>
          </div>
        ))}
      </div>
      
      {species.length === 0 && !loading && (
        <div className="empty-state">
          <div className="empty-state-icon">🌺</div>
          <h3 className="empty-state-title">No species yet!</h3>
          <p className="empty-state-message">Create species to organize your plant care information 🌿</p>
          <button onClick={() => navigate('/species/new')} className="btn-large">
            ➕ Add Your First Species
          </button>
        </div>
      )}
    </div>
  );
}
