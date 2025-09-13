import { apiFetch } from './client.js';

export function getPlants() {
  return apiFetch('/api/plants');
}

export function getPlant(id) {
  return apiFetch(`/api/plants/${id}`);
}

export function createPlant(data) {
  return apiFetch('/api/plants', {
    method: 'POST',
    body: JSON.stringify({
      name: data.name,
      species: data.species,
      description: data.description || null,
      wateringIntervalDays: Number(data.wateringIntervalDays) || 0,
      location: data.location || null
    })
  });
}
