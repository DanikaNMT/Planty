import { apiFetch } from '../api/client.js';

export function login(email, password) {
  return apiFetch('/api/auth/login', {
    method: 'POST',
    body: JSON.stringify({ email, password })
  });
}

export function register(userName, email, password) {
  return apiFetch('/api/auth/register', {
    method: 'POST',
    body: JSON.stringify({ userName, email, password })
  });
}
