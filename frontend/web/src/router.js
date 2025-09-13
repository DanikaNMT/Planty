import { useEffect, useState } from 'react';

export function parsePath(pathname) {
  if (pathname === '/') return { name: 'home' };
  if (pathname === '/new') return { name: 'new' };
  const plantMatch = pathname.match(/^\/plant\/(.+)$/);
  if (plantMatch) return { name: 'plant', id: plantMatch[1] };
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
