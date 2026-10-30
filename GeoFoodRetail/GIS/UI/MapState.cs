using GMap.NET.WindowsForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GeoFoodRetail.GIS.UI
{
    internal class MapState : Form
    {
        protected float _zoomLevel = 1.0f;
        protected PointF _mapOffset = new PointF(0, 0);
        protected Point _lastMousePos;
        protected bool _isDragging = false;
        protected StreamingRTree.RTreePage _usaRegionsRTree;
        protected RectangleF _mapViewport = new RectangleF(50, 50, 250, 250);
        protected int _selectedStoreIndex = -1;
        protected readonly Dictionary<string, Image> _tileCache = new Dictionary<string, Image>();
        protected static readonly HttpClient _httpClient = new HttpClient();
        protected readonly List<PointF> _storePixelPositions = new List<PointF>();
        protected readonly List<string> _storeNames = new List<string>();
        protected readonly InitStoreUpdate _storeState = new InitStoreUpdate();
        protected readonly GraphicsPath[] _usaRegionPaths = new GraphicsPath[60]; // 4
        protected string[] _regionNames = {
                "West (CA, WA, NV)",
                "Midwest (IL, TX, OH)",
                "South (TX, FL, GA)",
                "Northeast (NY, PA, MA)"
            };
        protected readonly PointF[] _usaRegionGeoCenters = new PointF[]
        {
            new PointF(37.7749f, -122.4194f),
            new PointF(41.8781f, -87.6298f),
            new PointF(29.7604f, -95.3698f),
            new PointF(40.7128f, -74.0060f)
        };
        protected decimal[] _regionDeliveryPrices = { 500.00m, 350.00m, 400.00m, 600.00m };
        protected PointF? _searchedPinLocation = null;
        protected string _searchedAddressLabel = string.Empty;
        protected Label _lblCashInfo;
        protected Label _lblSelectedInfo;
        protected Label _lblPrice;
        protected Label _lblDistanceInfo;
        protected TextBox _txtSearch;
        protected Button _btnSearch;
        protected Button _btnClearPin;
        protected Panel _rightPanel;
        protected ProgressBar _searchProgressBar;
        protected Label _lblSearchError;

        // Let the array be automatically assembled from my list of stores.
        // That way, the old methods—with their loops over .Length and their indices—will remain completely intact and perfectly happy!
        protected readonly PointF[] _storeCoordArray = GeoFoodRetail.GIS.Domain.Models.StoreData.Store
            .Select(store => new PointF((float)store.Location.Latitude, (float)store.Location.Longitude))
            .ToArray();
        protected List<string> StoreNameArr = Domain.Models.StoreData.Store
            .Select(m => m.Name)
            .ToList();
        protected GMapControl _gMapControl1;
        protected FlowLayoutPanel _mainRightContainer;
    }
}
