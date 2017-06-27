@ECHO ON

SET PGUSER=postgres
SET PGPASSWORD=Password12!
PATH=C:\Program Files\PostgreSQL\9.6\bin\;%PATH%

psql -f "%APPVEYOR_BUILD_FOLDER%\Tests.NHibernate.Spatial.PostGis20\nhsp_test.sql"

sqlcmd -S (local)\SQL2008R2SP2 -i "%APPVEYOR_BUILD_FOLDER%\Tests.NHibernate.Spatial.MsSql2008\nhsp_test.sql"
