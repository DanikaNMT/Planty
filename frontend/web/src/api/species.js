import { apiFetch } from './client.js';

export function getSpecies() {
  return apiFetch('/api/species');
}

export function getSpeciesById(id) {
  return apiFetch(`/api/species/${id}`);
}

export function createSpecies(data) {
  return apiFetch('/api/species', {
    method: 'POST',
    body: JSON.stringify({
      name: data.name,
      description: data.description || null,
      wateringIntervalDays: data.wateringIntervalDays ? Number(data.wateringIntervalDays) : null,
      fertilizationIntervalDays: data.fertilizationIntervalDays ? Number(data.fertilizationIntervalDays) : null
    })
  });
}

export function updateSpecies(id, data) {
  return apiFetch(`/api/species/${id}`, {
    method: 'PUT',
    body: JSON.stringify({
      name: data.name,
      description: data.description || null,
      wateringIntervalDays: data.wateringIntervalDays ? Number(data.wateringIntervalDays) : null,
      fertilizationIntervalDays: data.fertilizationIntervalDays ? Number(data.fertilizationIntervalDays) : null
    })
  });
}

export function deleteSpecies(id) {
  return apiFetch(`/api/species/${id}`, {
    method: 'DELETE'
  });
}
