import React from 'react';
export function ErrorMessage({ error }) { if (!error) return null; return <div>Error: {error}</div>; }
