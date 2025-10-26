const API_BASE = import.meta.env.VITE_API_BASE || 'http://localhost:5153';

export async function apiFetch(path, options = {}) {
  const token = localStorage.getItem('jwt');
  
  // Don't set Content-Type for FormData (browser will set it with boundary)
  const headers = { ...(options.headers || {}) };
  if (!(options.body instanceof FormData)) {
    headers['Content-Type'] = 'application/json';
  }
  
  if (token) headers['Authorization'] = `Bearer ${token}`;
  
  const res = await fetch(`${API_BASE}${path}`, {
    headers,
    ...options
  });
  if (!res.ok) {
    let details = '';
    try { details = await res.text(); } catch {}
    throw new Error(`HTTP ${res.status} ${res.statusText}${details ? ': ' + details : ''}`);
  }
  if (res.status === 204) return null;
  const ct = res.headers.get('content-type') || '';
  if (ct.includes('application/json')) {
    return res.json();
  }
  return res.text();
}
