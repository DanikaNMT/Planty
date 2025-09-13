import React, { useEffect, useState } from 'react';
import { getPlants } from '../api/plants.js';
import Link from '../components/Link.jsx';
import { Loading } from '../components/Loading.jsx';
import { ErrorMessage } from '../components/ErrorMessage.jsx';
import { formatDate } from '../utils/formatDate.js';

export function HomePage({ navigate }) {
  const [plants, setPlants] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  function load() {
    setLoading(true);
    setError(null);
    getPlants()
      .then(setPlants)
      .catch(e => setError(e.message))
      .finally(() => setLoading(false));
  }

  useEffect(() => { load(); }, []);

  return (
    <div>
      <h1>Planty</h1>
      <div>
        <button onClick={load} disabled={loading}>Reload</button>
        <button onClick={() => navigate('/new')}>Add Plant</button>
      </div>
      {loading && <Loading />}
      <ErrorMessage error={error} />
      <ul>
        {plants.map(p => (
          <li key={p.id}>
            <Link to={`/plant/${p.id}`} navigate={navigate}>
              {p.name} - {p.species} {p.nextWateringDue ? `(Next: ${formatDate(p.nextWateringDue)})` : ''}
            </Link>
          </li>
        ))}
      </ul>
      {plants.length === 0 && !loading && <div>No plants yet.</div>}
    </div>
  );
}
