import { useEffect, useState } from 'react';

export function parsePath(pathname) {
  if (pathname === '/') return { name: 'home' };
  if (pathname === '/new') return { name: 'new' };
  if (pathname === '/todos') return { name: 'todos' };
  if (pathname === '/species') return { name: 'species' };
  if (pathname === '/species/new') return { name: 'species-new' };
  if (pathname === '/locations') return { name: 'locations' };
  if (pathname === '/locations/new') return { name: 'locations-new' };
  const plantMatch = pathname.match(/^\/plant\/(.+)$/);
  if (plantMatch) return { name: 'plant', id: plantMatch[1] };
  const speciesMatch = pathname.match(/^\/species\/(.+)$/);
  if (speciesMatch) return { name: 'species-detail', id: speciesMatch[1] };
  const locationMatch = pathname.match(/^\/locations\/(.+)$/);
  if (locationMatch) return { name: 'location-detail', id: locationMatch[1] };
  return { name: 'not-found' };
}

export function useRouter() {
  const [location, setLocation] = useState(() => parsePath(window.location.pathname));

  useEffect(() => {
    const onPop = () => setLocation(parsePath(window.location.pathname));
    window.addEventListener('popstate', onPop);
    return () => window.removeEventListener('popstate', onPop);
  }, []);

  function navigate(to) {
    if (to !== window.location.pathname) {
      window.history.pushState({}, '', to);
      setLocation(parsePath(to));
    }
  }

  return { route: location, navigate };
}
