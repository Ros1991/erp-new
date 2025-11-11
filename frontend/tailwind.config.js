/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        primary: {
          50: '#e6f2ff',
          100: '#bae0ff',
          200: '#7ac7ff',
          300: '#3aa8ff',
          400: '#0a8fff',
          500: '#0071e3',
          600: '#0058b8',
          700: '#003f8f',
          800: '#002a66',
          900: '#001a40',
        },
        accent: {
          50: '#e6fff6',
          100: '#b3fce0',
          200: '#80f9ca',
          300: '#4df6b4',
          400: '#26f4a1',
          500: '#00e887',
          600: '#00ba6b',
          700: '#008b50',
          800: '#005c35',
          900: '#002e1a',
        }
      },
      fontFamily: {
        sans: ['Inter', 'system-ui', '-apple-system', 'sans-serif'],
      }
    },
  },
  plugins: [
    require('@tailwindcss/forms'),
  ],
}
