import 'https://cdn.jsdelivr.net/npm/construct-style-sheets-polyfill';
import 'reflect-metadata'; // shim is required for class-transformer
import React from 'react';
import ReactDOM from 'react-dom/client';
import App from './App';
import { BrowserRouter } from 'react-router';
import './index.scss';

ReactDOM.createRoot(document.getElementById('root') as HTMLElement).render(
    <BrowserRouter>
        <App />
    </BrowserRouter>
);
