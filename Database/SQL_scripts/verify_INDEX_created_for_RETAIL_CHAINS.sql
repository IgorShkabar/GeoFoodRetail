-- To verify that the index has indeed been successfully created and is functioning --

SELECT index_name, status 
FROM all_indexes
WHERE table_name = 'RETAIL_CHAINS'
AND owner = 'C##GIS_USER';