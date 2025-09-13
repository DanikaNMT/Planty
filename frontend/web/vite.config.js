import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    proxy: {
      '/api': {
        target: 'http://localhost:5153',
        changeOrigin: true
      }
    }
  },
  // Define environment variables for the build
  define: {
    __API_URL__: JSON.stringify(process.env.VITE_API_URL || 'http://localhost:5153')
  }
});
