import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      '/submission': {
        target: 'http://localhost:5251',
        changeOrigin: true,
      },
      '/approval': {
        target: 'http://localhost:5251',
        changeOrigin: true,
      },
            '/payment': {
        target: 'http://localhost:5251',
        changeOrigin: true,
      },
      '/platform': {
        target: 'http://localhost:5251',
        changeOrigin: true,
      },
    },
  },
})