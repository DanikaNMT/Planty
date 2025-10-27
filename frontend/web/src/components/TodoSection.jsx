import React, { useEffect, useState } from 'react';
import { getPlantTodos, waterPlant, fertilizePlant } from '../api/plants.js';
import { ErrorMessage } from './ErrorMessage.jsx';
import { formatDate } from '../utils/formatDate.js';

export function TodoSection({ navigate, onActionComplete }) {
  const [todos, setTodos] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [actionStates, setActionStates] = useState({});

  function load() {
    setLoading(true);
    setError(null);
    getPlantTodos(24)
      .then(setTodos)
      .catch(e => setError(e.message))
      .finally(() => setLoading(false));
  }

  const handleAction = async (todo) => {
    const key = `${todo.plantId}-${todo.actionType}`;
    if (actionStates[key]) return;
    
    setActionStates(prev => ({ ...prev, [key]: true }));
    setError(null);
    
    try {
      if (todo.actionType === 'Water') {
        await waterPlant(todo.plantId);
      } else {
        await fertilizePlant(todo.plantId);
      }
      // Reload todos after action
      await load();
      // Notify parent to refresh plant list
      if (onActionComplete) {
        onActionComplete();
      }
    } catch (e) {
      setError(e.message);
    } finally {
      setActionStates(prev => ({ ...prev, [key]: false }));
    }
  };

  useEffect(() => { load(); }, []);

  if (loading && todos.length === 0) {
    return (
      <div className="todo-section">
        <div className="loading">
          <div className="spinner"></div>
          Loading todos...
        </div>
      </div>
    );
  }

  if (todos.length === 0) {
    return null; // Don't show section if no todos
  }

  return (
    <div className="todo-section">
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 'var(--spacing-md)' }}>
        <h2 style={{ fontSize: '1.5rem', fontWeight: '700', color: 'var(--color-primary)', display: 'flex', alignItems: 'center', gap: 'var(--spacing-sm)' }}>
          <span>✅</span>
          Upcoming Todos (Next 24 Hours)
        </h2>
        <button 
          onClick={() => navigate('/todos')} 
          className="btn-outline btn-small"
        >
          📅 View All
        </button>
      </div>

      <ErrorMessage error={error} />

      <div className="todo-list">
        {todos.map(todo => {
          const key = `${todo.plantId}-${todo.actionType}`;
          const isOverdue = new Date(todo.dueDate) < new Date();
          
          return (
            <div 
              key={key}
              className={`todo-card ${isOverdue ? 'todo-overdue' : ''}`}
              onClick={() => navigate(`/plant/${todo.plantId}`)}
              style={{ cursor: 'pointer' }}
            >
              {todo.latestPictureUrl && (
                <div className="todo-card-image">
                  <img 
                    src={todo.latestPictureUrl} 
                    alt={todo.plantName}
                    onError={(e) => { e.target.style.display = 'none'; }}
                  />
                </div>
              )}
              <div className="todo-card-content">
                <div className="todo-card-header">
                  <div>
                    <div className="todo-card-title">
                      {todo.actionType === 'Water' ? '💧' : '🌿'} {todo.actionType} {todo.plantName}
                      {todo.isShared && (
                        <span style={{ 
                          display: 'inline-flex', 
                          alignItems: 'center', 
                          gap: '0.25rem', 
                          marginLeft: '0.5rem',
                          padding: '0.125rem 0.375rem',
                          backgroundColor: 'var(--color-primary-light)',
                          borderRadius: 'var(--radius-sm)',
                          fontSize: '0.7rem',
                          fontWeight: '500',
                          color: 'var(--color-primary-dark)'
                        }}>
                          <span>👥</span>
                          <span>
                            {todo.userRole === 0 && 'Viewer'}
                            {todo.userRole === 1 && 'Carer'}
                            {todo.userRole === 2 && 'Editor'}
                            {todo.userRole === 3 && 'Owner'}
                          </span>
                        </span>
                      )}
                    </div>
                    {todo.species && (
                      <div className="todo-card-species">{todo.species}</div>
                    )}
                  </div>
                  <div className={`todo-card-due ${isOverdue ? 'overdue' : ''}`}>
                    {isOverdue ? '⚠️ Overdue' : '⏰'} {formatDate(todo.dueDate)}
                  </div>
                </div>
                <div className="todo-card-actions" onClick={(e) => e.stopPropagation()}>
                  {/* Only show action button if user has Carer role or higher (or not shared) */}
                  {(!todo.isShared || (todo.userRole !== null && todo.userRole >= 1)) && (
                    <button
                      onClick={() => handleAction(todo)}
                      disabled={actionStates[key]}
                      className={todo.actionType === 'Water' ? 'btn-water' : 'btn-fertilizer'}
                    >
                      {actionStates[key] ? '⏳ Processing...' : `${todo.actionType === 'Water' ? '💧' : '🌿'} ${todo.actionType}`}
                    </button>
                  )}
                </div>
              </div>
            </div>
          );
        })}
      </div>
    </div>
  );
}
