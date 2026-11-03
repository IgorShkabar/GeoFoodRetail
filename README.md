# GeoFoodRetail
is a system that computes distances, geolocation of retails; facilitates purchase payments, the dispensing of change, and the selection of an optimal food combination; shows functionality within a GIS environment: searching for retail locations on the USA map, the distances between them, and determining the distance to 
the user's computer; shows the coverage and an increase in the number of nearby supermarkets that fall within the user's search area.

## Screenshots:
<img width="50%" alt="search_retails" src="https://github.com/user-attachments/assets/503cc237-28ab-4697-bee7-5c4f93c934b1" />
<img width="50%" alt="retails" src="https://github.com/user-attachments/assets/13bcfaf0-8bcd-451b-b341-11052aef3f23" />
<img width="50%" alt="geocoding_api" src="https://github.com/user-attachments/assets/4f2aadc3-406c-458b-aa49-8a50085ca097" />
<img width="50%" alt="tests26" src="https://github.com/user-attachments/assets/9c2da46c-6b52-43fc-a66d-777dc6f01108" />
<img width="50%" alt="table_retail_chains" src="https://github.com/user-attachments/assets/f8ec0f9c-834a-435f-a44e-2a95ea8174f7" />

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
```markdown
<details>
```text
GeoFoodRetail/
├── GIS/                               
│   ├── Abstractions/                  
│   │   ├── IGisProjection.cs          
│   │   ├── ITileProvider.cs           
│   │   └── ISpatialIndex.cs           
│   ├── AddManyObjects/                
│   │   ├── Add50KPointsToMap.cs       
│   │   └── MapPointItem.cs            
│   ├── Domain/                        
│   │   ├── Models/                    
│   │   │   ├── GeoCoordinate.cs       
│   │   │   ├── MapMarker.cs           
│   │   │   ├── RetailStoreMarker.cs   
│   │   │   └── StoreType.cs           
│   │   └── Layers/                    
│   │       ├── DataLoader.cs          
│   │       └── VectorLayer.cs         
│   ├── Math3D/                       
│   ├── Projections/                   
│   │   └── WebMercatorProjection.cs   
│   ├── TileProviders/                 
│   │   └── OsmTileProvider.cs         
│   │   
│   ├── StreamingRTree/                
│   │   ├── RTreeNode.cs               
│   │   ├── BoundingBox.cs             
│   │   └── RTreeSpatialIndex.cs       
│   └── UI/                            
│       └── GisMapControl.cs           
│
├── MainDashboard/                     
│   ├── MainForm.cs                    
│   └── MainForm.Designer.cs           
│
├── FoodOptions/                       
|   ├── CompatibleFood.cs              
│   └── Hashes.cs                      
│
├── About/                             
├── HelpFeature/                       
├── Logging/                           
├── OSparts/                           
├── Resources/                         
├── UtilitarMethod/                    
└── Utils/                             
```
</details>
## 📂 Structure and Folder Architecture (for the Test Project)
```markdown
<details>  
```text
GeoFoodRetail.Tests/
└── GIS.Tests/                                   
   └── AddManyObjects.Tests/                
   ├── Domain.Tests/                        
   │   ├── Models.Tests/                    
   │   └── Layers.Tests/                    
   ├── Math3D.Tests/                       
   ├── Projections.Tests/                   
   ├── TileProviders.Tests/                 
   ├── StreamingRTree..Tests/                
   └── UI.Tests/                          
```
</details>

1. GIS/Abstractions: Independent of all other components. Contains only base types and interfaces.
2. GIS/Domain: Depends solely on Abstractions. Stores data regarding stores and retail chains.
3. GIS/StreamingRTree: Implements ISpatialIndex.cs from the Abstractions layer. Isolated from WinForms; handles rapid spatial point lookups (using a Grid Index or R-Tree).
4. GIS/UI (GisMapControl): Utilizes all GIS layers—projections, tile providers, and the R-Tree index—to perform frame rendering and process mouse-click (hit-test) events.
5. MainDashboard/MainForm.cs: The assembly point. Instantiates GisMapControl, attaches vector layers, and subscribes to store-selection events.
• IGisProjection: Converts geographic coordinates (Lat/Lon) into screen pixels (Web Mercator projection).
• ITileProvider: Handles asynchronous loading and caching of map tiles (OpenStreetMap).
• GisMapControl: A custom WinForms control that supports panning (dragging) and zooming (via the mouse wheel).
## Key architectural and technical challenges
I used spatial indexes (GIST) in PostGIS, which sped up geo-queries. Moved rendering to a background thread using double-buffered computations.
To optimize the rendering of 50,000+ points on the map, spatial searching has been moved to a stage preceding the actual drawing process; this ensures that the graphics engine only processes points that fall within the current screen area (bounding box/viewport). An R-Tree has been implemented for this purpose. For the efficient filtering of 50,000 points within the current screen view, the implementation—utilizing SimdAABB and ISpatialIndex<T>—includes index management, rapid insertion, updates, and a high-performance, non-allocating search for elements that intersect the screen.
Data is packed into fixed-size buffers. There are no heap allocations per frame/request, which means no freezes (GC pauses) during rendering or map processing.
## Key functionality: a list of what the program can do:
*Object lookup using a Quadtree index with O(log N) complexity.
GIS functionality: A range of implemented geospatial operations (e.g., an address geocoding pipeline).
*Implementation of the Web Mercator mathematical projection, mapping standard geographic coordinates (Lat/Lon) to the OpenStreetMap tile grid.
*Implementation of multithreading and asynchronous tile loading from the map server, ensuring the GUI thread remains responsive.
*Two-tier caching (Memory + Disk): Previously loaded tiles are stored in a disk-based cache (`TileCache/`), reducing data usage and ensuring smooth performance (60 FPS).
*Switching to an SoA (Structure of Arrays) layout allows for optimal utilization of the processor's L1/L2 caches during batch testing. The coordinates of four R-Tree bounding boxes are loaded simultaneously into a single SIMD register, enabling intersection checks against the query AABB to be performed with a single processor instruction.
*Windows' built-in geolocation services are used to determine coordinates based on an IP address, while the haversine formula—which accounts for the Earth's spherical shape—is implemented to precisely calculate the distance between points on the Earth's surface.
*Full GIS functionality has been implemented to display the precise locations of stores in the U.S. using a combination of GMap.NET and OpenStreetMap. The component is simply placed onto a form, the OpenStreetMap provider is specified, and markers (GMapMarker) are added based on exact coordinates (latitude/longitude).
## Technology Stack:
C#, WinForms, .NET, Oracle Spatial, nUnit, GMap.NET, json, SQL, NuGet packages. 
## Launch instructions:
Note: An excellent article about cloning a repository on the GitHub website itself: 
https://docs.github.com/en/repositories/creating-and-managing-repositories/cloning-a-repository

First you need to make Cloning a repository
1. Navigate to the main page of the repository.
2. Above the list of files, click  <>Code button.
3. Copy the URL for the repository.
 3.1. To clone the repository using HTTPS, under "HTTPS", click.
 3.2. To clone a repository using GitHub CLI, click GitHub CLI, then click .
4. Open Git Bash utility.
5. Change the current working directory to the location where you want the cloned directory.
6. Type  git clone, and then paste the URL you copied earlier:
   git clone https://github.com/IgorShkabar/GeoFoodRetail.git
7. Press Enter to create your local clone.

* Once the steps described above have been correctly completed, the full GeoFoodRetail repository will appear in your cloned directory. Double-clicking the .sln file will
open the GeoFoodRetail solution in Visual Studio. To get everything working properly, all you need to do is restore the NuGet packages.   

If you use Docker or Node.js make the following steps: Check if Docker or Node.js exists on you PC.
Prerequisites:
Ensure that you have the following installed on your PC:
• Git
• Docker & Docker Compose (if using compose.yml) OR Node.js / Python (depending on the project stack)

Step 1: Cloning the repository
Open a terminal (or command prompt) and clone the project to your computer:
git clone <REPOSITORY_LINK> && cd <PROJECT_FOLDER_NAME>
Example: 
git clone https://github.com/IgorShkabar/GeoFoodRetail && cd GeoFoodRetail

Step 2: Setting up environment variables (Required)
The project requires configuration data to function. Create a local .env file based on the example:
cp .env.example .env
⚠️ Important: Open the created .env file in a text editor and, if necessary, modify the secrets, ports, or database access credentials to suit your system.

Step 3: Launching the project (choose your preferred method)

Option A: Launching via Docker (Recommended)
Build and start all services (database, backend, frontend) with a single command:
docker-compose up --build
Option B: Local execution (Node.js)
If the project is run without Docker:
npm install && npm start

Step 4: Running tests
To ensure that everything is working correctly, run the tests in a new terminal window:

# for Docker:
docker-compose exec web npm test  # (or pytest)

# for Node.js:
npm test

How to open a project in Visual Studio:
1. Launch Visual Studio (or VS Code).
2. Click "Open a local folder" or "Open a project or solution."
3. Select the downloaded project folder (or the .sln file).
4. If you are using Docker within Visual Studio, ensure that the Docker extension is active, and launch the project using the Docker Compose profile.
