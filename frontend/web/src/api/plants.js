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
      species: data.species || null,
      description: data.description || null,
      wateringIntervalDays: data.wateringIntervalDays ? Number(data.wateringIntervalDays) : null,
      location: data.location || null
    })
  });
}

export function waterPlant(id) {
  return apiFetch(`/api/plants/${id}/water`, {
    method: 'POST'
  });
}
