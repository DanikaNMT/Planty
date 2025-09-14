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
    <div>
      <h2>{mode === 'login' ? 'Login' : 'Register'}</h2>
      <form onSubmit={handleSubmit}>
        {mode === 'register' && (
          <div>
            <label>Username: <input value={userName} onChange={e => setUserName(e.target.value)} required /></label>
          </div>
        )}
        <div>
          <label>Email: <input type="email" value={email} onChange={e => setEmail(e.target.value)} required /></label>
        </div>
        <div>
          <label>Password: <input type="password" value={password} onChange={e => setPassword(e.target.value)} required /></label>
        </div>
        <button type="submit" disabled={loading}>{loading ? 'Loading...' : (mode === 'login' ? 'Login' : 'Register')}</button>
      </form>
      {error && <div style={{color:'red'}}>{error}</div>}
      <button onClick={switchMode}>
        {mode === 'login' ? 'Need an account? Register' : 'Already have an account? Login'}
      </button>
    </div>
  );
}
