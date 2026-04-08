/// <reference types="vite/client" />

// CSS module type declarations
declare module '*.css' {
  const content: string
  export default content
}
