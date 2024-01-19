/* NHibernate.Spatial MySQL 8.0 Test Database Creation Script */

-- Run this script as superuser using mysql, i.e.:
--    mysql -P 3307 -u root -p"password" < path-to-this-file.sql

CREATE USER 'nhsp_test'@'%' IDENTIFIED BY 'nhsp_test';

CREATE DATABASE IF NOT EXISTS `nhsp_test` DEFAULT CHARACTER SET utf8 COLLATE utf8_unicode_ci;

GRANT ALL PRIVILEGES ON `nhsp_test`.* TO 'nhsp_test'@'%';

-- Required to be able to insert/delete custom SRS; see:
-- https://dev.mysql.com/doc/refman/8.0/en/create-spatial-reference-system.html
GRANT SUPER ON *.* TO 'nhsp_test'@'%';
