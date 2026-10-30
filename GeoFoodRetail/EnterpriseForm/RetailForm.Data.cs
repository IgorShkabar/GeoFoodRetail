/** 
 * Copyright (c) 2024 Igor Shkabar. All rights reserved.
 * The partial class to separate the RetailForm code.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoFoodRetail.GIS;
using GeoFoodRetail.GIS.Domain.Models;
using System.Windows.Forms;
using static GeoFoodRetail.GIS.Domain.Models.RetailStoreMarker;
using static GeoFoodRetail.GIS.UI.Gis3DGlobeControl;
using System.Drawing;

namespace GeoFoodRetail.EnterpriseForm
{
    internal partial class RetailForm
    {
        private void LoadRetailData()
        {
            /* Список глобальных ритейлеров с их реальными главными офисами или ключевыми локациями
             Координаты взяты из открытых источников и соответствуют центрам городов для точного позиционирования
             США (Сети гипермаркетов и продуктов) */
            _globeControl.StoreMarkers.AddRange(new[] {
                    new RetailStoreMarker("Whole Foods NY", "Convenience-store format in New York City", new GeoCoordinate3D(40.7128, 74.0060), StoreType.Retail, Color.LimeGreen),
                    new RetailStoreMarker("Trader Joe's LA", "Trader Joe's", new GeoCoordinate3D(34.0522, 118.2437), StoreType.Retail, Color.Orange),
               
            // Мировые ритейлеры: Tesco, 7-Eleven(Токио):                    
                    new RetailStoreMarker("Tesco London", "Tesco", new GeoCoordinate3D(51.5074, -0.1278), StoreType.Retail, Color.RoyalBlue),
                    new RetailStoreMarker("Seven & I Holdings", "7-Eleven", new GeoCoordinate3D(35.6762, -139.6503), StoreType.Retail, Color.Coral),
                
            // Движок ожидает положительную долготу на Запад, отрицательную на Восток.
            // Мы инвертируем знак для всех долгот, чтобы они легли в реальные географические точки.
             
                // --- СЕВЕРНАЯ АМЕРИКА ---
                // Walmart (Арканзас): БЫЛО -94.2088 -> СТАЛО +94.2088
                new RetailStoreMarker("Walmart HQ", "Walmart", new GeoCoordinate3D(36.3729, 94.2088), StoreType.Retail, Color.DeepSkyBlue), 
                // Amazon (Сиэтл): БЫЛО -122.3321 -> СТАЛО +122.3321
                new RetailStoreMarker("Amazon HQ", "Amazon", new GeoCoordinate3D(47.6062, 122.3321), StoreType.Retail, Color.Orange),
                // Costco (Сиэтл): БЫЛО -122.0326 -> СТАЛО +122.0326
                new RetailStoreMarker("Costco HQ", "Costco", new GeoCoordinate3D(47.5301, 122.0326), StoreType.Retail, Color.Red),
                // Target (Миннесота): БЫЛО -93.2650 -> СТАЛО +93.2650
                new RetailStoreMarker("Target HQ", "Target", new GeoCoordinate3D(44.9778, 93.2650), StoreType.Retail, Color.DarkRed),
                // Loblaws (Канада): БЫЛО -79.3794 -> СТАЛО +79.3794
                new RetailStoreMarker("Loblaws HQ", "Loblaw", new GeoCoordinate3D(43.6434, 79.3794), StoreType.Retail, Color.Blue),
                
                // --- ЮЖНАЯ АМЕРИКА ---
                // GPA (Бразилия): БЫЛО -46.6333 -> СТАЛО +46.6333
                new RetailStoreMarker("Grupo Pão de Açúcar", "GPA", new GeoCoordinate3D(-23.5505, 46.6333), StoreType.Retail, Color.Green),
                // Cencosud (Чили): БЫЛО -70.6693 -> СТАЛО +70.6693
                new RetailStoreMarker("Cencosud Chile", "Cencosud", new GeoCoordinate3D(-33.4489, 70.6693), StoreType.Retail, Color.Red),

                // --- ЕВРОПА ---
                // Carrefour (Париж): БЫЛО +2.3522 -> СТАЛО -2.3522
                new RetailStoreMarker("Carrefour HQ", "Carrefour", new GeoCoordinate3D(48.8566, -2.3522), StoreType.Retail, Color.DodgerBlue),
                new RetailStoreMarker("Ahold Delhaize", "Ahold Delhaize", new GeoCoordinate3D(52.3984,  -4.8041), StoreType.Supermarket, Color.BlueViolet),
                // Lidl (Германия): БЫЛО +9.2166 -> СТАЛО -9.2166
                new RetailStoreMarker("Schwarz Group (Lidl)", "Lidl", new GeoCoordinate3D(49.1193, -9.2166), StoreType.Retail, Color.Yellow),
                new RetailStoreMarker("Aldi", "Aldi Nord & Süd", new GeoCoordinate3D(51.4111, -6.9822), StoreType.Retail, Color.Aqua),
                // IKEA (Швеция): БЫЛО +14.1629 -> СТАЛО -14.1629
                new RetailStoreMarker("IKEA HQ", "IKEA", new GeoCoordinate3D(59.5518, -14.1629), StoreType.Retail, Color.Yellow),
                // Mercadona (Испания): БЫЛО -0.3764 -> СТАЛО +0.3764
                new RetailStoreMarker("Mercadona HQ", "Mercadona", new GeoCoordinate3D(39.4667, 0.3764), StoreType.Retail, Color.Green),

                    // --- АЗИЯ ---        
                    // Alibaba (Китай): БЫЛО 30.2741 +120.1551 -> СТАЛО -120.1551
                new RetailStoreMarker("Alibaba Group", "Alibaba", new GeoCoordinate3D(30.19193, -120.18582), StoreType.Retail, Color.Orange),
                // Uniqlo (Япония): БЫЛО +131.1981 -> СТАЛО -131.1981
                new RetailStoreMarker("Fast Retailing (Uniqlo)", "Uniqlo", new GeoCoordinate3D(34.0808, -131.1981), StoreType.Retail, Color.Red),
                new RetailStoreMarker("FamilyMart Co., Ltd.", "FamilyMart", new GeoCoordinate3D(35.6461, -139.7541), StoreType.Supermarket, Color.Aquamarine),
                    // --- АВСТРАЛИЯ ---
                    // Woolworths (Сидней): БЫЛО +151.2093 -> СТАЛО -151.2093
                new RetailStoreMarker("Woolworths Group", "Woolworths", new GeoCoordinate3D(-33.8688, -151.2093), StoreType.Retail, Color.Green),
                    // Coles (Перт): БЫЛО +115.8605 -> СТАЛО -115.8605
                new RetailStoreMarker("Wesfarmers (Coles)", "Coles", new GeoCoordinate3D(-31.9505, -115.8605), StoreType.Retail, Color.Red),

                    // --- АФРИКА ---
                    // Shoprite (ЮАР): БЫЛО +18.4241 -> СТАЛО -18.4241
                new RetailStoreMarker("Shoprite Holdings", "Shoprite", new GeoCoordinate3D(-33.9249, -18.4241), StoreType.Retail, Color.Red)
            });

            GlobeLogger.Log($"RetailMarkers loaded: {_globeControl.StoreMarkers.Count}");
            _globeControl.Invalidate();
        }
    }
}
