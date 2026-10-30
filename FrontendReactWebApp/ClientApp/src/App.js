import React { useEffect, useRef } from 'react';
import maplibregl from 'maplibre-gl';
import 'maplibre-gl/dist/maplibre-gl.css';
import { Route } from 'react-router';
import Layout from './components/Layout';
import Home from './components/Home';
import Counter from './components/Counter';
import FetchData from './components/FetchData';

// Функция генерации 50,000 случайных точек (тяжелый слой)
const generateGeoJsonData = (count) => {
    const features = [];
    for (let i = 0; i < count; i++)
    {
        features.push({
            type: 'Feature',
            geometry: {
                type: 'Point',
                coordinates: [
                    40.7128 + (Math.random() - 0.5) * 2, // Долгота (New York и окрестности)
                    74.0060 + (Math.random() - 0.5) * 1, // Широта
                ],
            },
            properties: {
                id: i,
            },
        });
    }
    return
    {
        type: 'FeatureCollection',
        features: features,
    };
};

function App()
{
    const mapContainer = useRef(null);
    const map = useRef(null);

    useEffect(() => {
        if (map.current) return;

        // Инициализация карты (использует WebGL Canvas под капотом)
        map.current = new maplibregl.Map({
            container: mapContainer.current,
            style: 'https://maplibre.org', // Базовый бесплатный стиль
            center: [40.7128, 74.0060],
            zoom: 8,
        });

        map.current.on('load', () => {
            // 1. Добавляем источник данных (GeoJSON с 50k объектов)
            map.current.addSource('heavy-layer-source', {
                type: 'geojson',
                data: generateGeoJsonData(50000),
            });

            // 2. Добавляем WebGL слой (circle рендерится через GPU на Canvas)
            map.current.addLayer({
                id: 'heavy-points-layer',
                type: 'circle',
                source: 'heavy-layer-source',
                paint: {
                    'circle-radius': 4,
                    'circle-color': '#FF3B30',
                    'circle-opacity': 0.6,
                    'circle-stroke-width': 1,
                    'circle-stroke-color': '#FFFFFF'
                },
            });
        });

        // Интеграция с WinForms (прием сообщений из C#)
    if (window.chrome && window.chrome.webview)
    {
            window.chrome.webview.addEventListener('message', event => {
                const message = event.data;
                if (message.action === 'changeColor') {
                    // Мгновенное изменение цвета 50к точек через GPU
                    map.current.setPaintProperty('heavy-points-layer', 'circle-color', message.color);
                }
                const data = event.data;
                if (data.action === 'changeColor') {
                    const color = data.color;
                    // Моя логика изменения цвета на карте
                }
            });
        }

    return () => map.current ?.remove();
    }, []);

    return (
        <div style={{ width: '100vw', height: '100vh', position: 'relative' }}>
            <div ref={mapContainer} style={{ width: '100%', height: '100%' }} />
        </div>
    );
}

// Это было в Framework
export default () => (
  <Layout>
    <Route exact path='/' component={Home} />
    <Route path='/counter' component={Counter} />
    <Route path='/fetch-data/:startDateIndex?' component={FetchData} />
  </Layout>
);
