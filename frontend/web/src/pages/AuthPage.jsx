import React, { useState } from 'react';
import { login, register } from '../api/auth.js';

export function AuthPage({ onAuth, mode = 'login', switchMode }) {
  const [email, setEmail] = useState('');
  const [userName, setUserName] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(false);

  async function handleSubmit(e) {
    e.preventDefault();
    setError(null);
    setLoading(true);
    try {
      if (mode === 'login') {
        const result = await login(email, password);
        onAuth(result.token);
      } else {
        await register(userName, email, password);
        // Auto-login after registration
        const result = await login(email, password);
        onAuth(result.token);
      }
    } catch (e) {
      setError(e.message);
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="auth-container">
      <div className="auth-card">
        <h2 className="auth-title">
          <span>🌱</span>
          Planty
        </h2>
        <p style={{ textAlign: 'center', color: 'var(--color-text-light)', marginBottom: 'var(--spacing-xl)' }}>
          {mode === 'login' ? 'Welcome back! 👋' : 'Start your plant journey! 🌿'}
        </p>
        
        <form onSubmit={handleSubmit}>
          {mode === 'register' && (
            <div className="form-group">
              <label>👤 Username</label>
              <input 
                type="text"
                value={userName} 
                onChange={e => setUserName(e.target.value)} 
                placeholder="Choose a username"
                required 
              />
            </div>
          )}
          <div className="form-group">
            <label>📧 Email</label>
            <input 
              type="email" 
              value={email} 
              onChange={e => setEmail(e.target.value)} 
              placeholder="your@email.com"
              required 
            />
          </div>
          <div className="form-group">
            <label>🔒 Password</label>
            <input 
              type="password" 
              value={password} 
              onChange={e => setPassword(e.target.value)} 
              placeholder="Enter your password"
              required 
            />
          </div>
          
          <button type="submit" disabled={loading} className="btn-large" style={{ width: '100%', marginBottom: 'var(--spacing-md)' }}>
            {loading ? '⏳ Loading...' : (mode === 'login' ? '🔓 Login' : '🌱 Create Account')}
          </button>
        </form>
        
        {error && (
          <div className="message message-error">
            <span>⚠️</span>
            {error}
          </div>
        )}
        
        <div style={{ textAlign: 'center', marginTop: 'var(--spacing-lg)' }}>
          <button onClick={switchMode} className="btn-outline" style={{ width: '100%' }}>
            {mode === 'login' ? '🌿 Need an account? Register' : '👋 Already have an account? Login'}
          </button>
        </div>
      </div>
    </div>
  );
}
