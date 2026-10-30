-- Table of retailers` brands --
CREATE TABLE brands(
  brand_id NUMBER PRIMARY KEY,
  brand_name VARCHAR(100) NOT NULL,
  industry_sector VARCHAR(50) NOT NULL --- Grocery, Electronics, Apparel
  );
  