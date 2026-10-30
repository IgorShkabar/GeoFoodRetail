/** 
 * Copyright (c) 2024-26 Igor Shkabar. All rights reserved.
 */
using GeoFoodRetail.GIS.UI;
using GMap.NET;
using GMap.NET.WindowsForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GeoFoodRetail.GIS.StreamingRTree
{
    internal class MatchedStores
    {
        private GMapControl _gMapControl;
        private RectangleF _queryAABB = new RectangleF(5, 5, 10, 10);
        List<StoreItem> matchedStores = new List<StoreItem>();        
        int totalHitsCount = 0;

        private const int MinQuerySize = 12;
        private const float ScrollDelta = 10f;

        public MatchedStores(List<StoreItem> retaiList)
        {
            matchedStores = retaiList;
        }       

        internal void UpdateMatchedStores(int mouseX, int mouseY, List<StoreItem> retailList)
        {
            // 1. Получаем географические границы рамки _queryAABB
            // Левый верхний угол рамки
            PointLatLng topLeftGeo = _gMapControl.FromLocalToLatLng((int)_queryAABB.X, (int)_queryAABB.Y);
            // Правый нижний угол рамки
            PointLatLng bottomRightGeo = _gMapControl.FromLocalToLatLng((int)(_queryAABB.X + _queryAABB.Width), (int)(_queryAABB.Y + _queryAABB.Height));

            // 2. Фильтруем список точек, проверяя попадание в гео-границы
            // (Внимание: у Latitude верхняя точка больше нижней, у Longitude наоборот)
            matchedStores = retailList.Where(store =>
                store.Longitude >= topLeftGeo.Lng && store.Longitude <= bottomRightGeo.Lng &&
                store.Latitude <= topLeftGeo.Lat && store.Latitude >= bottomRightGeo.Lat
            ).ToList();

            totalHitsCount = matchedStores.Count;
        }

        internal void GMapControl_MouseMove(object sender, MouseEventArgs e)
        {
            // Центрируем рамку поиска (AABB) относительно текущего положения курсора на карте
            _queryAABB.X = e.X - _queryAABB.Width / 2f;
            _queryAABB.Y = e.Y - _queryAABB.Height / 2f;

            // Метод для захвата точек, попавших в рамку (координаты e.X и e.Y)
            UpdateMatchedStores(e.X, e.Y, null);

            // Принудительно перерисовываем карту (вызовет GMapControl1_Paint)
            _gMapControl.Invalidate();
        }

        internal void GMapControl_MouseWheel(object sender, MouseEventArgs e)
        {
            // Изменение размера области поиска колесиком мыши
            float delta = e.Delta > 0 ? ScrollDelta : -ScrollDelta;
            _queryAABB.Width = Math.Max(MinQuerySize, _queryAABB.Width + delta);
            _queryAABB.Height = Math.Max(MinQuerySize, _queryAABB.Height + delta);

            // Корректируем координаты, чтобы рамка оставалась центрированной после изменения размера
            _queryAABB.X = e.X - _queryAABB.Width / 2f;
            _queryAABB.Y = e.Y - _queryAABB.Height / 2f;

            UpdateMatchedStores(e.X, e.Y, null);
            _gMapControl.Invalidate();
        }
    }
}
