REM Setup MS SQL 2008
sqlcmd -S (local)\SQL2008R2SP2 -i "%APPVEYOR_BUILD_FOLDER%\Tests.NHibernate.Spatial.MsSql2008\nhsp_test.sql"

REM Setup MS SQL 2012
sqlcmd -S (local)\SQL2012SP1 -i "%APPVEYOR_BUILD_FOLDER%\Tests.NHibernate.Spatial.MsSql2012\nhsp_test.sql"

REM Setup MySQL 5.7
SET MYSQL_PWD=Password12!
PATH=C:\Program Files\MySQL\MySQL Server 5.7\bin\;%PATH%

mysql -u root < "%APPVEYOR_BUILD_FOLDER%\Tests.NHibernate.Spatial.MySQL57\nhsp_test.sql"

REM Setup PostGIS 2
SET PGUSER=postgres
SET PGPASSWORD=Password12!
PATH=C:\Program Files\PostgreSQL\9.6\bin\;%PATH%

psql -f "%APPVEYOR_BUILD_FOLDER%\Tests.NHibernate.Spatial.PostGis20\nhsp_test.sql"
