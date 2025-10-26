import React, { useState } from 'react';
import { createPlant } from '../api/plants.js';
import { ErrorMessage } from '../components/ErrorMessage.jsx';
import { Loading } from '../components/Loading.jsx';

export function NewPlantPage({ navigate }) {
  const [data, setData] = useState({ name: '', species: '', description: '', wateringIntervalDays: 7, location: '' });
  const [creating, setCreating] = useState(false);
  const [error, setError] = useState(null);

  function submit(e) {
    e.preventDefault();
    setCreating(true);
    setError(null);
    createPlant(data)
      .then(created => navigate(`/plant/${created.id}`))
      .catch(e => setError(e.message))
      .finally(() => setCreating(false));
  }

  return (
    <div>
      <button onClick={() => navigate('/')}>Home</button>
      <h2>New Plant</h2>
      <form onSubmit={submit}>
        <div>
          <label>Name </label>
          <input value={data.name} onChange={e => setData({ ...data, name: e.target.value })} required />
        </div>
        <div>
          <label>Species </label>
            <input value={data.species} onChange={e => setData({ ...data, species: e.target.value })} />
        </div>
        <div>
          <label>Description </label>
          <input value={data.description} onChange={e => setData({ ...data, description: e.target.value })} />
        </div>
        <div>
          <label>Watering Interval Days </label>
          <input type="number" value={data.wateringIntervalDays} onChange={e => setData({ ...data, wateringIntervalDays: e.target.value })} />
        </div>
        <div>
          <label>Location </label>
          <input value={data.location} onChange={e => setData({ ...data, location: e.target.value })} />
        </div>
        <button type="submit" disabled={creating}>Create</button>
      </form>
      {creating && <Loading text="Creating..." />}
      <ErrorMessage error={error} />
    </div>
  );
}
