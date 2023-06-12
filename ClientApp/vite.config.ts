import { fileURLToPath, URL } from 'node:url'

import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [vue()],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url))
    }
  },
   server: {
    proxy: {
      '/list': {
        target: 'http://localhost:5089/',
//        changeOrigin: true,
        rewrite: (path) => {console.log(path); return path.replace(/^:.*\//, ':44470/');},
      },
      '/list/compiled': {
        target: 'http://localhost:5089/',
//        changeOrigin: true,
        rewrite: (path) => {console.log(path); return path.replace(/^:.*\//, ':44470/');},
      }
    }
   }
})