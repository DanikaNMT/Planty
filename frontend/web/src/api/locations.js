import { apiFetch } from './client.js';

export function getLocations() {
  return apiFetch('/api/locations');
}

export function getLocationById(id) {
  return apiFetch(`/api/locations/${id}`);
}

export function createLocation(data) {
  return apiFetch('/api/locations', {
    method: 'POST',
    body: JSON.stringify(data)
  });
}

export function updateLocation(id, data) {
  return apiFetch(`/api/locations/${id}`, {
    method: 'PUT',
    body: JSON.stringify(data)
  });
}

export function deleteLocation(id) {
  return apiFetch(`/api/locations/${id}`, {
    method: 'DELETE'
  });
}
