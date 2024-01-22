# NHibernate.Spatial [![Build status](https://github.com/nhibernate/NHibernate.Spatial/actions/workflows/ci.yml/badge.svg)](https://github.com/nhibernate/NHibernate.Spatial/actions/workflows/ci.yml)

This is a fork of the final commit to NHibernate.Spatial at [NHibernate Contrib][NHContrib] site
made in december 2011. The code has been updated to compile with the latest NTS, GeoAPI and
NHibernate binaries.

## NuGet Packages

| Package                                                                                       | Version                                                                                                                                                    |
|-----------------------------------------------------------------------------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------|
| [NHibernate.Spatial.MsSql](https://www.nuget.org/packages/NHibernate.Spatial.MsSql)           | [![NuGet Status](http://img.shields.io/nuget/v/NHibernate.Spatial.MsSql.svg?style=flat)](http://www.nuget.org/packages/NHibernate.Spatial.MsSql)           |
| [NHibernate.Spatial.MySQL](https://www.nuget.org/packages/NHibernate.Spatial.MySQL)           | [![NuGet Status](http://img.shields.io/nuget/v/NHibernate.Spatial.MySQL.svg?style=flat)](http://www.nuget.org/packages/NHibernate.Spatial.MySQL)           |
| [NHibernate.Spatial.PostGis](https://www.nuget.org/packages/NHibernate.Spatial.PostGis)       | [![NuGet Status](http://img.shields.io/nuget/v/NHibernate.Spatial.PostGis.svg?style=flat)](http://www.nuget.org/packages/NHibernate.Spatial.PostGis)       |
| [NHibernate.Spatial.SpatiaLite](https://www.nuget.org/packages/NHibernate.Spatial.SpatiaLite) | [![NuGet Status](http://img.shields.io/nuget/v/NHibernate.Spatial.SpatiaLite.svg?style=flat)](http://www.nuget.org/packages/NHibernate.Spatial.SpatiaLite) |

## Supported Databases

| Package                       | Dialects                 | CI Tests                                                                              |
|-------------------------------|--------------------------|---------------------------------------------------------------------------------------|
| NHibernate.Spatial.MsSql      | SQL Server 2012          | SQL Server 2017, SQL Server 2019, SQL Server 2022                                     |
| NHibernate.Spatial.MySQL      | MySQL 5.7, MySQL 8.0     | MySQL 5.7, MySQL 8.0, MySQL 8.3                                                       |
| NHibernate.Spatial.PostGis    | PostGIS 2.0, PostGIS 3.0 | PostGIS 2.5 (PostgreSQL 12), PostGIS 3.0 (PostgreSQL 12), PostGIS 3.4 (PostgreSQL 16) |
| NHibernate.Spatial.SpatiaLite | SpatiaLite               | SpatiaLite 4, SpatiaLite 5                                                            |

## Getting Started

For configuring your application, please refer to [Configuration and Mapping](http://nhibernate.info/doc/spatial/configuration-and-mapping.html)
on the NHForge Wiki. Furthermore you can read about [sample usage](http://nhibernate.info/doc/spatial/sample-usage.html).
For an updated in-depth look at NHibernate.Spatial, you can read [this excellent blog
post](http://build-failed.blogspot.no/2012/02/nhibernate-spatial-part-1.html) written by Pedro
Sousa.

For general NHibernate help, the NHibernate community website - <http://nhibernate.info/> - has a
range of resources to help you get started with basic NHibernate, including [wikis][NHWiki],
[blogs][NHWiki] and [reference documentation][NH].

### SpatiaLite

In order to use the SpatiaLite dialect, the `mod_spatialite.[dll|so]` library must be available
on the `PATH`. To assist with cases where `mod_spatialite` cannot be installed on the system `PATH`,
the SpatiaLite driver prepends `<path to NHibernate.Spatial.SpatiaLite.dll>/lib/spatialite` to the
`PATH`.

https://www.gaia-gis.it/fossil/libspatialite/wiki?name=mod_spatialite

## Discussion Forum

Discussion is made through the [NHibernate.Spatial Mailing List](https://groups.google.com/forum/#!forum/nhibernate-spatial).
Feel free to join in with suggestions or thoughts!

## Request for Contributors

All contributors are welcome. We need help improving the current dialects, getting tests to pass,
identifying and fixing bugs, and keeping the project going.

## Credits

Many thanks to the following individuals, organisations and projects whose work is so important to
the success of NHibernate.Spatial (in no particular order):

* [NHibernate][] - Parent library
* [NUnit][] - unit-testing
* [GitHub][] and [SourceForge][] - source code hosting
* [Log4net][] - logging, by the [Apache Software Foundation][]
* [JetBrains][] - [ReSharper][] licences for NHibernate.Spatial developers
* [NetTopologySuite][] and [GeoAPI.Net][] - Spatial Libraries

[NUnit]: http://www.nunit.org
[GitHub]: http://www.github.com
[SourceForge]: http://www.sourceforge.net
[Log4net]: http://logging.apache.org/log4net
[Apache Software Foundation]: http://www.apache.org
[JetBrains]: http://www.jetbrains.com
[ReSharper]: http://www.jetbrains.com/resharper
[GeoAPI.Net]: https://github.com/NetTopologySuite/GeoAPI
[NetTopologySuite]: https://github.com/NetTopologySuite/NetTopologySuite
[NHibernate]: https://github.com/nhibernate/nhibernate-core
[NHWiki]: http://nhforge.org/wikis
[NH]: http://nhibernate.info/doc/nh/en/index.html
[NHContrib]: http://sourceforge.net/projects/nhcontrib/
