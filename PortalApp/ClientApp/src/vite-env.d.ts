/// <reference types="vite/client" />

declare global {
    interface Window {
        config: Record<string, unknown>; // More specific than 'any'
    }

    // Use const instead of var
    const sessionStorage: Storage;
    const document: Document;
    const console: Console;
}

export {};
