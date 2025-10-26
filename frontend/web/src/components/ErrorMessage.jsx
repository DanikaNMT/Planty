import React from 'react';

export function ErrorMessage({ error }) {
  if (!error) return null;
  
  return (
    <div className="message message-error">
      <span>⚠️</span>
      <strong>Error:</strong> {error}
    </div>
  );
}
