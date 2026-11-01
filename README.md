# GeoFoodRetail
is a system that calculates distances, displays the geolocation of retails; it also facilitates purchase payments, the dispensing of change, and the selection of an optimal 
food combination.
Application shows functionality within a GIS environment: searching for retail locations on the USA map, the distances between them, and determining the distance to 
the user's computer.
The application shows the coverage and an increase in the number of nearby supermarkets that fall within the user's search area.

# What the application does: 
A desktop GIS system for searching retail locations on a map of the United States, performing calculations, and displaying the results 
within a WinForms interface. 

## Business value: The application can serve as a foundation for further development in the field of GIS solutions. The application can serve as a foundation for further 
development of GIS solutions. It provides a base that developers can build upon, allowing for expansion and customization to meet the specific needs of a given 
company.

## Application architecture: 
It consists of 3 projects: a Class Library, a WinForm project, and a test project. To adhere to Clean Architecture and SOLID principles, classes 
and interfaces have been moved into separate files, and the entire system has been organized into logical layers (Domain, Services, Infrastructure, Presentation).
