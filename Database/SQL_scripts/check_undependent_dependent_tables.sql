SELECT 
  pk.table_name AS parent_table,
  fk.table_name AS dependent_table,
  fk.constraint_name AS fk_constraint_name
FROM
  user_constraints fk
JOIN 
  user_constraints pk ON fk.r_constraint_name = pk.constraint_name
WHERE
  fk.constraint_type = 'R'
  AND fk.table_name IN ('LOCATION_TYPES', 'RETAIL_CHAINS', 'RETAIL_STORES', 'STORES_LOCATIONS', 'TRADE_ZONES', 'USA_STATES_GEOM') 
ORDER BY
  parent_table;
