import React from 'react';
export default function Link({ to, navigate, children }) {
  return (
    <a href={to} onClick={(e) => { e.preventDefault(); navigate(to); }}>
      {children}
    </a>
  );
}
