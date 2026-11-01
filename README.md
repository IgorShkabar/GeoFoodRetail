# GeoFoodRetail
is a system that computes distances, geolocation of retails; facilitates purchase payments, the dispensing of change, and the selection of an optimal food combination; shows functionality within a GIS environment: searching for retail locations on the USA map, the distances between them, and determining the distance to 
the user's computer; shows the coverage and an increase in the number of nearby supermarkets that fall within the user's search area.

# What the application does: 
A desktop GIS system for searching retail locations on a map of the United States, performing calculations, and displaying the results within a WinForms interface. 

## Business value: 
The application can serve as a basis for further development in the field of GIS solutions: it provides a foundation upon which developers can build, allowing for the expansion of functionality and the adaptation of the system to the specific needs of a given company.

## Application architecture: 
The solution is built using Clean Architecture and SOLID principles, split into logical layers (Domain, Services, Infrastructure, Presentation) with all classes and interfaces separated into dedicated files.
It consists of 3 projects:
• Class Library — Core business logic and infrastructure.
• WinForms App — Presentation layer / UI.
• Test Project — Unit and integration tests.
### 📂 Structure and Folder Architecture (for the GeoFoodRetail WinForms Project)
<details>
  <summary>Click to expand the project file tree</summary>
  ```text
GeoFoodRetail/
├── GIS/                               # Core GIS module (calculations, maps, layers)
│   ├── Abstractions/                  # Interfaces and architectural contracts
│   │   ├── IGisProjection.cs          # Lat/Lon <-> Pixel Conversion
│   │   ├── ITileProvider.cs           # Map tile retrieval contract
│   │   └── ISpatialIndex.cs           # Spatial search contract for objects
│   ├── AddManyObjects/                # Example of adding a huge number of points to a map
│   │   ├── Add50KPointsToMap.cs       # Logic for generating 50k points
│   │   └── MapPointItem.cs            # Auxiliary model (a point with X (longitude) and Y (latitude))
│   ├── Domain/                        # Data models and domain logic
│   │   ├── Models/                    # Entities and data types
│   │   │   ├── GeoCoordinate.cs       # Coordinate structure (Lat, Lon)
│   │   │   ├── MapMarker.cs           
│   │   │   ├── RetailStoreMarker.cs   
│   │   │   └── StoreType.cs           # Enum: Retail, Grocery, Supermarket
│   │   └── Layers/                    
│   │       ├── DataLoader.cs          # data loader class
│   │       └── VectorLayer.cs         
│   ├── Math3D/                        # Mathematics for 3D projections
│   ├── Projections/                   # Geodetic projections
│   │   └── WebMercatorProjection.cs   # Implementing the Web Mercator projection (EPSG:3857)
│   ├── TileProviders/                 # Инфраструктура загрузки и кэширования карт
│   │   └── OsmTileProvider.cs         
│   │   
│   ├── StreamingRTree/                # Spatial indexing 
│   │   ├── RTreeNode.cs               # R-tree node / Grid Index cell
│   │   ├── BoundingBox.cs             # AABB (Axis-Aligned Bounding Box)
│   │   └── RTreeSpatialIndex.cs       # Quick search for objects within the field of view O(log N)
│   └── UI/                            
│       └── GisMapControl.cs           
│
├── MainDashboard/                     # Main application interface
│   ├── MainForm.cs                    # Main WinForms Form 
│   └── MainForm.Designer.cs           
│
├── FoodOptions/                       # Business module: Food, payment, change
|   ├── CompatibleFood.cs              # Lists of food, tuple 
│   └── Hashes.cs                      # lookup collections (`HashSet<string>`)
│
├── About/                             # "About program"
├── HelpFeature/                       # HelpFeatureForm class with sizes
├── Logging/                           # A minimal concept for logging errors
├── OSparts/                           # Check up versions: OS, .NET, .NETFramework 
├── Resources/                         # Static resources (icons, .jpg, .png - files, etc.)
├── UtilitarMethod/                    # Math.Clamp micro-function
└── Utils/                             # Parses the OpenFoodFacts gzip archive
```
</details>
