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
    let errorMessage = `HTTP ${res.status} ${res.statusText}`;
    
    try {
      const contentType = res.headers.get('content-type') || '';
      if (contentType.includes('application/json')) {
        const errorData = await res.json();
        // Extract message from various error response formats
        errorMessage = errorData.message || errorData.title || errorData.error || errorMessage;
      } else {
        const text = await res.text();
        if (text) errorMessage += ': ' + text;
      }
    } catch (parseError) {
      // If parsing fails, keep the default error message
    }
    
    throw new Error(errorMessage);
  }
  
  if (res.status === 204) return null;
  const ct = res.headers.get('content-type') || '';
  if (ct.includes('application/json')) {
    return res.json();
  }
  return res.text();
}
