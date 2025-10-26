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
      fertilizationIntervalDays: data.fertilizationIntervalDays ? Number(data.fertilizationIntervalDays) : null,
      location: data.location || null
    })
  });
}

export function waterPlant(id) {
  return apiFetch(`/api/plants/${id}/water`, {
    method: 'POST'
  });
}

export function getPlantWaterings(id) {
  return apiFetch(`/api/plants/${id}/waterings`);
}

export function fertilizePlant(id) {
  return apiFetch(`/api/plants/${id}/fertilize`, {
    method: 'POST'
  });
}

export function getPlantFertilizations(id) {
  return apiFetch(`/api/plants/${id}/fertilizations`);
}

export function getPlantCareHistory(id) {
  return apiFetch(`/api/plants/${id}/care-history`);
}

export function uploadPlantPicture(id, file, notes = null) {
  const formData = new FormData();
  formData.append('file', file);
  if (notes) {
    formData.append('notes', notes);
  }
  
  return apiFetch(`/api/plants/${id}/pictures`, {
    method: 'POST',
    body: formData
  });
}
