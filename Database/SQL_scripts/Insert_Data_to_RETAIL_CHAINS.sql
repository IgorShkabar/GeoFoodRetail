--Insert Data to RETAIL_CHAINS Table--
SET DEFINE OFF;

INSERT INTO C##GIS_USER.RETAIL_CHAINS(
CHAIN_ID, CHAIN_NAME, LEGAL_NAME, HQ_COUNTRY_CODE, HQ_STATE_PROVINCE, HQ_CITY, HQ_POSTAL_CODE, HQ_HOUSE_NUMBER, 
HQ_STREET, RETAIL_TYPE, BUSINESS_MODEL, WEB_SITE, TICKER_SYMBOL, STATUS, CREATED_AT, UPDATED_AT)
VALUES(
8,
'Grocery Outlet Holding', 'Grocery Outlet Holding Corp. (GO)', 'US', 'CA', 'Emeryville, California', '94608', '5650', '5650 Hollis Street',
'Discount Closeout/Extreme-Value Grocery', 'Independently operated stores, local owner-oper.',
'www.groceryoutlet.com', 'GO', 'Publicly Traded', TO_DATE('1946', 'YYYY'), TO_DATE('June 20, 2019', 'Month DD, YYYY')
);

SET DEFINE OFF;