import React, { useEffect, useState } from 'react';
import { getPlantTodos, waterPlant, fertilizePlant } from '../api/plants.js';
import { ErrorMessage } from '../components/ErrorMessage.jsx';
import { formatDate } from '../utils/formatDate.js';
import Link from '../components/Link.jsx';

export function TodosPage({ navigate }) {
  const [todos, setTodos] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [actionStates, setActionStates] = useState({});
  const [hoursAhead, setHoursAhead] = useState(168); // 7 days default
  const [successMessage, setSuccessMessage] = useState(null);

  function load() {
    setLoading(true);
    setError(null);
    getPlantTodos(hoursAhead)
      .then(setTodos)
      .catch(e => setError(e.message))
      .finally(() => setLoading(false));
  }

  const handleAction = async (todo) => {
    const key = `${todo.plantId}-${todo.actionType}`;
    if (actionStates[key]) return;
    
    setActionStates(prev => ({ ...prev, [key]: true }));
    setError(null);
    setSuccessMessage(null);
    
    try {
      if (todo.actionType === 'Water') {
        await waterPlant(todo.plantId);
      } else {
        await fertilizePlant(todo.plantId);
      }
      setSuccessMessage(`${todo.actionType === 'Water' ? '💧' : '🌿'} ${todo.plantName} ${todo.actionType.toLowerCase()}ed successfully!`);
      // Reload todos after action
      await load();
      // Clear success message after 3 seconds
      setTimeout(() => setSuccessMessage(null), 3000);
    } catch (e) {
      setError(e.message);
    } finally {
      setActionStates(prev => ({ ...prev, [key]: false }));
    }
  };

  useEffect(() => { load(); }, [hoursAhead]);

  const timeRangeOptions = [
    { value: 24, label: 'Next 24 Hours' },
    { value: 48, label: 'Next 2 Days' },
    { value: 168, label: 'Next Week' },
    { value: 336, label: 'Next 2 Weeks' },
  ];

  return (
    <div>
      <div style={{ marginBottom: 'var(--spacing-lg)' }}>
        <Link to={'/'} navigate={navigate} style={{ fontSize: '1rem', display: 'inline-flex', alignItems: 'center', gap: 'var(--spacing-xs)' }}>
          ← Back to My Plants
        </Link>
      </div>

      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 'var(--spacing-xl)', flexWrap: 'wrap', gap: 'var(--spacing-md)' }}>
        <h2 style={{ fontSize: '2rem', fontWeight: '700', color: 'var(--color-primary)', display: 'flex', alignItems: 'center', gap: 'var(--spacing-sm)' }}>
          <span>✅</span>
          Plant Care Todos
        </h2>
        <div style={{ display: 'flex', gap: 'var(--spacing-sm)', alignItems: 'center' }}>
          <label style={{ fontSize: '0.9rem', color: 'var(--color-text-light)' }}>
            Show:
          </label>
          <select 
            value={hoursAhead}
            onChange={(e) => setHoursAhead(Number(e.target.value))}
            style={{
              padding: 'var(--spacing-sm) var(--spacing-md)',
              borderRadius: 'var(--radius-md)',
              border: '2px solid var(--color-border)',
              fontSize: '1rem',
              cursor: 'pointer',
              background: 'var(--color-surface)',
              color: 'var(--color-text)',
            }}
          >
            {timeRangeOptions.map(option => (
              <option key={option.value} value={option.value}>
                {option.label}
              </option>
            ))}
          </select>
          <button onClick={load} disabled={loading} className="btn-outline btn-small">
            {loading ? '⏳ Loading...' : '🔄 Reload'}
          </button>
        </div>
      </div>

      {loading && (
        <div className="loading">
          <div className="spinner"></div>
          Loading todos...
        </div>
      )}

      <ErrorMessage error={error} />

      {successMessage && (
        <div className="message message-success" style={{ marginBottom: 'var(--spacing-lg)' }}>
          <span>✅</span>
          {successMessage}
        </div>
      )}

      {todos.length === 0 && !loading && (
        <div className="empty-state">
          <div className="empty-state-icon">🎉</div>
          <h3 className="empty-state-title">All caught up!</h3>
          <p className="empty-state-message">No upcoming care tasks for your plants in this time period. Great job! 🌿</p>
          <button onClick={() => navigate('/')} className="btn-large">
            🏡 Go to My Plants
          </button>
        </div>
      )}

      {todos.length > 0 && (
        <div className="todo-list">
          {todos.map(todo => {
            const key = `${todo.plantId}-${todo.actionType}`;
            const isOverdue = new Date(todo.dueDate) < new Date();
            const dueDate = new Date(todo.dueDate);
            const now = new Date();
            const hoursUntilDue = Math.floor((dueDate - now) / (1000 * 60 * 60));
            const daysUntilDue = Math.floor(hoursUntilDue / 24);
            
            let timeDisplay;
            if (isOverdue) {
              timeDisplay = `⚠️ Overdue - ${formatDate(todo.dueDate)}`;
            } else if (hoursUntilDue < 24) {
              timeDisplay = `⏰ In ${hoursUntilDue} hour${hoursUntilDue !== 1 ? 's' : ''}`;
            } else {
              timeDisplay = `⏰ In ${daysUntilDue} day${daysUntilDue !== 1 ? 's' : ''} - ${formatDate(todo.dueDate)}`;
            }
            
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
                      </div>
                      {todo.species && (
                        <div className="todo-card-species">{todo.species}</div>
                      )}
                    </div>
                    <div className={`todo-card-due ${isOverdue ? 'overdue' : ''}`}>
                      {timeDisplay}
                    </div>
                  </div>
                  <div className="todo-card-actions" onClick={(e) => e.stopPropagation()}>
                    <button
                      onClick={() => handleAction(todo)}
                      disabled={actionStates[key]}
                      className={todo.actionType === 'Water' ? 'btn-water' : 'btn-fertilizer'}
                    >
                      {actionStates[key] ? '⏳ Processing...' : `${todo.actionType === 'Water' ? '💧' : '🌿'} ${todo.actionType}`}
                    </button>
                  </div>
                </div>
              </div>
            );
          })}
        </div>
      )}
    </div>
  );
}
