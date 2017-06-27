@ECHO ON

REM Install PostGIS 2.3.2
curl -L -O http://download.osgeo.org/postgis/windows/pg96/postgis-bundle-pg96-2.3.2x64.zip
7z x postgis-bundle-pg96-2.3.2x64.zip
xcopy /e /y /q postgis-bundle-pg96-2.3.2x64 "C:\Program Files\PostgreSQL\9.6"
