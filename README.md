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
```markdown
<details>
  <summary>Click to expand the project file tree</summary>
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
```
