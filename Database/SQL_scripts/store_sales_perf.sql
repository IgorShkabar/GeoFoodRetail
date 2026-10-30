-- Table of annual sales, that depends on a store existance --
CREATE TABLE c##gis_user.store_sales_perf (
    sale_id NUMBER PRIMARY KEY,
    store_id NUMBER REFERENCES c##gis_user.retail_stores(store_id) ON DELETE CASCADE,
    year_period NUMBER,
    revenue_usd NUMERIC(12, 2)
);
