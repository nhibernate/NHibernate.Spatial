/* NHibernate.Spatial MS SQL Server 2008 Test Database Creation Script */

-- Run this script using sqlcmd, i.e.:
--    sqlcmd -S (local)\SQLSERVER2008 -i path-to-this-file.sql

CREATE LOGIN nhsp_test WITH PASSWORD = 'nhsp_test', CHECK_POLICY = OFF;

CREATE DATABASE nhsp_test;
GO
USE nhsp_test;

EXEC sp_changedbowner 'nhsp_test';
