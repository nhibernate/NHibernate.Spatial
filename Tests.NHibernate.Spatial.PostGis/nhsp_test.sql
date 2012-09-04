/* NHibernate.Spatial PostGIS Test Database Creation Script */

-- CREATE DATABASE cannot be executed from a multi-command script
-- so select each command separately and execute it.

CREATE ROLE nhsp_test LOGIN
  ENCRYPTED PASSWORD 'md5c7a746bb04ce57ee60350ff6a98f9ae6' -- md5('nhsp_test' + salt)
  NOSUPERUSER NOINHERIT CREATEDB CREATEROLE;

CREATE DATABASE nhsp_test
  WITH OWNER = nhsp_test
       ENCODING = 'UTF8'
       TEMPLATE=postgis;

-- Change current connection to nhsp_test before runing the following:

ALTER TABLE nhsp_test.public.geometry_columns OWNER TO nhsp_test;
ALTER TABLE nhsp_test.public.spatial_ref_sys OWNER TO nhsp_test;
