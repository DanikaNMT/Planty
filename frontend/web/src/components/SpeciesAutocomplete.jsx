import React, { useState, useEffect, useRef } from 'react';
import { createSpecies } from '../api/species.js';
import { ErrorMessage } from './ErrorMessage.jsx';

export function SpeciesAutocomplete({ species, value, onChange, onSpeciesCreated }) {
  const [searchTerm, setSearchTerm] = useState('');
  const [showDropdown, setShowDropdown] = useState(false);
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [creating, setCreating] = useState(false);
  const [createError, setCreateError] = useState(null);
  const [newSpeciesData, setNewSpeciesData] = useState({
    name: '',
    description: '',
    wateringIntervalDays: '',
    fertilizationIntervalDays: ''
  });
  const dropdownRef = useRef(null);

  const selectedSpecies = species.find(s => s.id === value);
  const filteredSpecies = species.filter(s => 
    s.name.toLowerCase().includes(searchTerm.toLowerCase())
  );

  // Close dropdown when clicking outside
  useEffect(() => {
    function handleClickOutside(event) {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target)) {
        setShowDropdown(false);
        if (!selectedSpecies) {
          setSearchTerm('');
        }
      }
    }

    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, [selectedSpecies]);

  function handleSelect(speciesId) {
    onChange(speciesId);
    setSearchTerm('');
    setShowDropdown(false);
  }

  function handleCreateClick() {
    setNewSpeciesData({ ...newSpeciesData, name: searchTerm });
    setShowCreateModal(true);
    setShowDropdown(false);
  }

  function handleCreateSpecies(e) {
    e.preventDefault();
    setCreating(true);
    setCreateError(null);

    createSpecies(newSpeciesData)
      .then(created => {
        onSpeciesCreated(created);
        setShowCreateModal(false);
        setNewSpeciesData({ name: '', description: '', wateringIntervalDays: '', fertilizationIntervalDays: '' });
        setSearchTerm('');
      })
      .catch(e => setCreateError(e.message))
      .finally(() => setCreating(false));
  }

  return (
    <div>
      <label style={{ display: 'block', marginBottom: 'var(--spacing-xs)', fontWeight: '600', color: 'var(--color-text)' }}>
        🌺 Species
      </label>
      <div style={{ position: 'relative' }} ref={dropdownRef}>
        <input
          type="text"
          value={searchTerm || (selectedSpecies ? selectedSpecies.name : '')}
          onChange={(e) => {
            setSearchTerm(e.target.value);
            setShowDropdown(true);
          }}
          onFocus={() => setShowDropdown(true)}
          placeholder="Search or select species..."
          style={{
            width: '100%',
            padding: 'var(--spacing-sm) var(--spacing-md)',
            border: '2px solid var(--color-border)',
            borderRadius: 'var(--radius-md)',
            fontSize: '1rem',
          }}
        />
        {selectedSpecies && !searchTerm && (
          <button
            type="button"
            onClick={() => {
              onChange('');
              setSearchTerm('');
            }}
            style={{
              position: 'absolute',
              right: '8px',
              top: '50%',
              transform: 'translateY(-50%)',
              background: 'none',
              border: 'none',
              cursor: 'pointer',
              fontSize: '1.2rem',
              color: 'var(--color-text-light)'
            }}
          >
            ×
          </button>
        )}
        
        {showDropdown && (
          <div style={{
            position: 'absolute',
            top: '100%',
            left: 0,
            right: 0,
            marginTop: '4px',
            backgroundColor: 'white',
            border: '1px solid var(--color-border)',
            borderRadius: 'var(--radius-md)',
            boxShadow: '0 4px 6px rgba(0,0,0,0.1)',
            maxHeight: '250px',
            overflowY: 'auto',
            zIndex: 100
          }}>
            <div
              onClick={() => handleSelect('')}
              style={{
                padding: 'var(--spacing-sm) var(--spacing-md)',
                cursor: 'pointer',
                borderBottom: '1px solid var(--color-border)',
                fontStyle: 'italic',
                color: 'var(--color-text-light)'
              }}
              onMouseEnter={(e) => e.target.style.backgroundColor = 'var(--color-bg-light)'}
              onMouseLeave={(e) => e.target.style.backgroundColor = 'white'}
            >
              No species (no care schedule)
            </div>
            {filteredSpecies.map(s => (
              <div
                key={s.id}
                onClick={() => handleSelect(s.id)}
                style={{
                  padding: 'var(--spacing-sm) var(--spacing-md)',
                  cursor: 'pointer',
                  borderBottom: '1px solid var(--color-border)'
                }}
                onMouseEnter={(e) => e.target.style.backgroundColor = 'var(--color-bg-light)'}
                onMouseLeave={(e) => e.target.style.backgroundColor = 'white'}
              >
                <div style={{ fontWeight: '600' }}>{s.name}</div>
                {(s.wateringIntervalDays || s.fertilizationIntervalDays) && (
                  <div style={{ fontSize: '0.85rem', color: 'var(--color-text-light)', marginTop: '2px' }}>
                    💧 {s.wateringIntervalDays || '—'} days • 🌿 {s.fertilizationIntervalDays || '—'} days
                  </div>
                )}
              </div>
            ))}
            {searchTerm && filteredSpecies.length === 0 && (
              <div
                onClick={handleCreateClick}
                style={{
                  padding: 'var(--spacing-md)',
                  cursor: 'pointer',
                  color: 'var(--color-primary)',
                  fontWeight: '600',
                  textAlign: 'center'
                }}
                onMouseEnter={(e) => e.target.style.backgroundColor = 'var(--color-primary-light)'}
                onMouseLeave={(e) => e.target.style.backgroundColor = 'white'}
              >
                ➕ Create new species "{searchTerm}"
              </div>
            )}
          </div>
        )}
      </div>
      <small style={{ color: 'var(--color-text-light)', display: 'block', marginTop: 'var(--spacing-xs)' }}>
        Species defines watering and fertilization schedule
      </small>

      {/* Create Species Modal */}
      {showCreateModal && (
        <div style={{
          position: 'fixed',
          top: 0,
          left: 0,
          right: 0,
          bottom: 0,
          backgroundColor: 'rgba(0,0,0,0.5)',
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'center',
          zIndex: 1000,
          padding: 'var(--spacing-md)'
        }} onClick={() => setShowCreateModal(false)}>
          <div style={{
            backgroundColor: 'white',
            borderRadius: '12px',
            maxWidth: '500px',
            width: '100%',
            padding: 'var(--spacing-xl)',
            maxHeight: '90vh',
            overflow: 'auto'
          }} onClick={e => e.stopPropagation()}>
            <h3 style={{ marginBottom: 'var(--spacing-lg)', fontSize: '1.5rem' }}>
              Create New Species
            </h3>

            <form onSubmit={handleCreateSpecies}>
              <div className="form-group">
                <label>Species Name *</label>
                <input
                  type="text"
                  value={newSpeciesData.name}
                  onChange={(e) => setNewSpeciesData({ ...newSpeciesData, name: e.target.value })}
                  required
                  placeholder="e.g., Monstera deliciosa"
                />
              </div>

              <div className="form-group">
                <label>Description</label>
                <textarea
                  value={newSpeciesData.description}
                  onChange={(e) => setNewSpeciesData({ ...newSpeciesData, description: e.target.value })}
                  placeholder="Optional notes about this species..."
                  rows="3"
                />
              </div>

              <div className="form-group">
                <label>💧 Watering Interval (days)</label>
                <input
                  type="number"
                  value={newSpeciesData.wateringIntervalDays}
                  onChange={(e) => setNewSpeciesData({ ...newSpeciesData, wateringIntervalDays: e.target.value })}
                  placeholder="e.g., 7"
                  min="1"
                />
              </div>

              <div className="form-group">
                <label>🌿 Fertilization Interval (days)</label>
                <input
                  type="number"
                  value={newSpeciesData.fertilizationIntervalDays}
                  onChange={(e) => setNewSpeciesData({ ...newSpeciesData, fertilizationIntervalDays: e.target.value })}
                  placeholder="e.g., 14"
                  min="1"
                />
              </div>

              <ErrorMessage error={createError} />

              <div style={{ display: 'flex', gap: 'var(--spacing-sm)', marginTop: 'var(--spacing-lg)' }}>
                <button type="submit" disabled={creating || !newSpeciesData.name.trim()} className="btn-primary" style={{ flex: 1 }}>
                  {creating ? '⏳ Creating...' : '✨ Create Species'}
                </button>
                <button type="button" onClick={() => setShowCreateModal(false)} className="btn-outline">
                  Cancel
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
