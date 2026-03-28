import { defineConfig } from 'vite'
import react, { reactCompilerPreset } from '@vitejs/plugin-react'
import babel from '@rolldown/plugin-babel'

// https://vite.dev/config/
export default defineConfig(({ command }) => ({
  base: command === 'build' ? '/dungeon-game/' : '/',
  build: {
    outDir: 'dist',
  },
  plugins: [
    react(),
    babel({ presets: [reactCompilerPreset()] }),
  ],
}));
