-- Checking a standard index on the STORE_ID field --
-- To check for the existence of an index on a specific table column, 
-- use the system views ALL_IND_COLUMNS or USER_IND_COLUMNS. --

SELECT index_owner, index_name, table_name, column_name, column_position
FROM all_ind_columns
WHERE table_name = 'RETAIL_CHAINS'
AND column_name = 'CHAIN_ID';