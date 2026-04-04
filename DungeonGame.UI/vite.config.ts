import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import { VitePWA } from 'vite-plugin-pwa';

// https://vite.dev/config/
export default defineConfig(({ command }) => ({
  base: command === 'build' ? '/dungeon-game/' : '/',
  build: {
    outDir: 'dist',
  },
  plugins: [
    react({
      babel: {
        plugins: [
          // If you have the compiler installed, you add it here:
          ['babel-plugin-react-compiler', { target: '18' }], 
        ],
      },
    }),
    VitePWA({
      registerType: 'autoUpdate',
      devOptions: {
        enabled: true,
        type: 'module',
      },
      workbox: {
        // Increase limit to 5 MiB (5 * 1024 * 1024)
        maximumFileSizeToCacheInBytes: 5242880,
        // Ensure wasm files are included in the patterns to be cached
        globPatterns: ['**/*.{js,css,html,ico,png,svg,wasm}']
      },
      manifest: {
        name: 'Dungeon Game',
        icons: [
          {
            src: 'icon-192x192.png',
            sizes: '192x192',
            type: 'image/png'
          },
          {
            src: 'icon-512x512.png',
            sizes: '512x512',
            type: 'image/png'
          }
        ]
      }
    })
  ],
}));
