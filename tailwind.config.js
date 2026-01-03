/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    './SpeedGameApp/**/*.razor',
    './SpeedGameApp/**/*.html',
    './SpeedGameApp/**/*.cshtml',
    './SpeedGameApp/**/*.cs',
  ],
  theme: {
    extend: {
      colors: {
        // Cyberpunk 2077 Theme Colors
        'cp': {
          cyan: '#00F0FF',
          pink: '#FF00FF',
          magenta: '#FF006E',
          yellow: '#FFFF00',
          gold: '#FFD700',
          green: '#00FF41',
          red: '#FF003C',
          'bg-dark': '#0A0A0A',
          'bg-mid': '#1A1A1A',
          'bg-card': '#1E1E1E',
          'text-primary': '#00FFFF',
          'text-secondary': '#B0B0B0',
        },
        // 3bstudio Theme Colors
        '3b': {
          blue: {
            DEFAULT: '#1E3A8A',
            dark: '#1E40AF',
            light: '#3B82F6',
          },
          yellow: {
            DEFAULT: '#FBBF24',
            dark: '#F59E0B',
            light: '#FCD34D',
          },
          black: '#000000',
          gray: {
            light: '#F3F4F6',
            DEFAULT: '#6B7280',
            dark: '#4B5563',
          },
        },
      },
      fontFamily: {
        // Cyberpunk fonts
        'cyberpunk': ['"Orbitron"', '"Rajdhani"', 'sans-serif'],
        'cyber-mono': ['"Roboto Mono"', '"Share Tech Mono"', 'monospace'],
        // 3bstudio fonts
        '3b-sans': ['"Inter"', '"Poppins"', 'system-ui', 'sans-serif'],
      },
      boxShadow: {
        // Neon glow effects for Cyberpunk theme
        'neon-cyan': '0 0 5px #00F0FF, 0 0 10px #00F0FF, 0 0 15px #00F0FF',
        'neon-pink': '0 0 5px #FF00FF, 0 0 10px #FF00FF, 0 0 15px #FF00FF',
        'neon-yellow': '0 0 5px #FFFF00, 0 0 10px #FFFF00, 0 0 15px #FFFF00',
        'neon-cyan-lg': '0 0 10px #00F0FF, 0 0 20px #00F0FF, 0 0 30px #00F0FF, 0 0 40px #00F0FF',
        'neon-pink-lg': '0 0 10px #FF00FF, 0 0 20px #FF00FF, 0 0 30px #FF00FF, 0 0 40px #FF00FF',
        // Professional shadows for 3bstudio
        'soft': '0 2px 8px rgba(0, 0, 0, 0.08)',
        'medium': '0 4px 12px rgba(0, 0, 0, 0.12)',
        'large': '0 8px 24px rgba(0, 0, 0, 0.16)',
      },
      backgroundImage: {
        // Cyberpunk gradients
        'cyber-gradient': 'linear-gradient(135deg, #00F0FF 0%, #FF00FF 100%)',
        'cyber-dark': 'linear-gradient(180deg, #0A0A0A 0%, #1A1A1A 100%)',
        // 3bstudio gradients
        '3b-gradient': 'linear-gradient(135deg, #1E3A8A 0%, #000000 100%)',
        '3b-wave': "url('/images/wave-yellow.svg')",
      },
      animation: {
        'pulse-neon': 'pulse-neon 2s cubic-bezier(0.4, 0, 0.6, 1) infinite',
        'glitch': 'glitch 1s linear infinite',
        'scanline': 'scanline 8s linear infinite',
      },
      keyframes: {
        'pulse-neon': {
          '0%, 100%': {
            opacity: '1',
            textShadow: '0 0 5px currentColor, 0 0 10px currentColor',
          },
          '50%': {
            opacity: '0.8',
            textShadow: '0 0 10px currentColor, 0 0 20px currentColor, 0 0 30px currentColor',
          },
        },
        'glitch': {
          '0%': { transform: 'translate(0)' },
          '20%': { transform: 'translate(-2px, 2px)' },
          '40%': { transform: 'translate(-2px, -2px)' },
          '60%': { transform: 'translate(2px, 2px)' },
          '80%': { transform: 'translate(2px, -2px)' },
          '100%': { transform: 'translate(0)' },
        },
        'scanline': {
          '0%': { transform: 'translateY(-100%)' },
          '100%': { transform: 'translateY(100%)' },
        },
      },
    },
  },
  plugins: [
    require('@tailwindcss/forms'),
    require('@tailwindcss/typography'),
  ],
}
