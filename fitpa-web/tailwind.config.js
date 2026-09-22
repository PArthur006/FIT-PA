/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/**/*.{html,ts}",
  ],
  theme: {
    extend: {
      colors: {
        ultra: {
          dark: '#0A0A0A',     // Fundo principal da tela
          card: '#141414',     // Fundo dos cartões/modal
          purple: '#9d00ff',   // Roxo neon
          orange: '#ff6b00',   // Laranja estímulo
          green: '#00e676'     // Verde sucesso
        }
      }
    },
  },
  plugins: [],
}