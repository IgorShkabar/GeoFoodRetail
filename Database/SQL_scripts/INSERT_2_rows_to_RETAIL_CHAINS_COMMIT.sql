INSERT INTO C##GIS_USER.RETAIL_CHAINS(
CHAIN_ID, CHAIN_NAME, LEGAL_NAME, HQ_COUNTRY_CODE, HQ_STATE_PROVINCE, HQ_CITY, HQ_POSTAL_CODE, 
HQ_HOUSE_NUMBER, HQ_STREET, RETAIL_TYPE, BUSINESS_MODEL, WEB_SITE, TICKER_SYMBOL, STATUS, CREATED_AT, UPDATED_AT)
VALUES(
3,
'Costco',
'Costco Wholesale Corporation',	'WA',	'US',	'Issaquah, Washington',	'98027',	'999', '999 Lake Drive Street',	
'Membership-only warehouse club retail store',	'Membership-based warehouse club', 'https://www.costco.com/',	'COST',	
'ACTIVE',	
TO_DATE('September 15, 1983', 'Month DD, YYYY'),
TO_DATE('October 02, 2026', 'Month DD, YYYY'));

INSERT INTO C##GIS_USER.RETAIL_CHAINS(
CHAIN_ID, CHAIN_NAME, LEGAL_NAME, HQ_COUNTRY_CODE, HQ_STATE_PROVINCE, HQ_CITY, HQ_POSTAL_CODE, 
HQ_HOUSE_NUMBER, HQ_STREET, RETAIL_TYPE, BUSINESS_MODEL, WEB_SITE, TICKER_SYMBOL, STATUS, CREATED_AT, UPDATED_AT)
VALUES(
4,
'Target Corp.',	'Target Corp. (TGT)',	'MN',	'US',	'Minneapolis',	'55403', '1000',	'1000 Nicollet Mall',
'Membership-only warehouse club retail store',	'Upscale discount omnichannel retail',	
'https://corporate.target.com/',	'TGT (NYSE)',	'ACTIVE',	
TO_DATE('01-MAY-62', 'DD-MONTH-YY'),
TO_DATE('04-OCT-26', 'DD-MONTH-YY'));

COMMIT;
