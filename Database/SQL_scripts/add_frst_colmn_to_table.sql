--To add the 1-st column to the Table Location_types:--
--make the existed columns invisible--
ALTER TABLE C##GIS_USER.LOCATION_TYPES MODIFY (col1 INVISIBLE, col2 INVISIBLE, col3 INVISIBLE);

--add the Location_types_ID column--
ALTER TABLE C##GIS_USER.LOCATION_TYPES ADD(location_types_ID NUMBER);

--Restoring visibility to the old columns (they will be positioned after the new one)--
ALTER TABLE C##GIS_USER.LOCATION_TYPES MODIFY (type_code VISIBLE, type_name VISIBLE, description VISIBLE);


