/* NHibernate.Spatial PostGIS Test Database Creation Script */

-- Run this script as superuser using psql, i.e.:
--    psql -h localhost -p 5432 -U postgres -f path-to-this-file.sql

CREATE ROLE nhsp_test LOGIN
  ENCRYPTED PASSWORD 'md5c7a746bb04ce57ee60350ff6a98f9ae6' -- md5('nhsp_test' + salt)
  NOSUPERUSER NOINHERIT CREATEDB CREATEROLE;

CREATE DATABASE nhsp_test
  WITH OWNER = nhsp_test
       ENCODING = 'UTF8';

\connect nhsp_test

CREATE EXTENSION postgis;

ALTER TABLE public.geometry_columns OWNER TO nhsp_test;
ALTER TABLE public.spatial_ref_sys OWNER TO nhsp_test;
