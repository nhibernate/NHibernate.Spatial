@echo off

REM Create output directory
set solution_dir=%~dp0
set output_dir="%solution_dir%\NuGet Packages"
if not exist %output_dir% mkdir %output_dir%

REM Build NuGet packages
set options=--configuration Release --output %output_dir%
dotnet pack %options% NHibernate.Spatial
dotnet pack %options% NHibernate.Spatial.MsSql
dotnet pack %options% NHibernate.Spatial.MySQL
REM dotnet pack %options% NHibernate.Spatial.Oracle
dotnet pack %options% NHibernate.Spatial.SpatiaLite
dotnet pack %options% NHibernate.Spatial.PostGis
