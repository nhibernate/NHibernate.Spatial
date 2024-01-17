# NHibernate.Spatial [![Build status](https://github.com/nhibernate/NHibernate.Spatial/actions/workflows/ci.yml/badge.svg)](https://github.com/nhibernate/NHibernate.Spatial/actions/workflows/ci.yml)

This is a fork of the final commit to NHibernate.Spatial at [NHibernate Contrib][NHContrib] site
made in december 2011. The code has been updated to compile with the latest NTS, GeoAPI and
NHibernate binaries.

## NuGet Packages

| Package                                                                                 | Version                                                                                                                                              |
|-----------------------------------------------------------------------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------|
| [NHibernate.Spatial.MsSql](https://www.nuget.org/packages/NHibernate.Spatial.MsSql)     | [![NuGet Status](http://img.shields.io/nuget/v/NHibernate.Spatial.MsSql.svg?style=flat)](http://www.nuget.org/packages/NHibernate.Spatial.MsSql)     |
| [NHibernate.Spatial.MySQL](https://www.nuget.org/packages/NHibernate.Spatial.MySQL)     | [![NuGet Status](http://img.shields.io/nuget/v/NHibernate.Spatial.MySQL.svg?style=flat)](http://www.nuget.org/packages/NHibernate.Spatial.MySQL)     |
| [NHibernate.Spatial.PostGis](https://www.nuget.org/packages/NHibernate.Spatial.PostGis) | [![NuGet Status](http://img.shields.io/nuget/v/NHibernate.Spatial.PostGis.svg?style=flat)](http://www.nuget.org/packages/NHibernate.Spatial.PostGis) |

## Supported Databases

| Package                    | Minimum Version                           | CI Tests                                                   |
|----------------------------|-------------------------------------------|------------------------------------------------------------|
| NHibernate.Spatial.MsSql   | SQL Server 2008                           | SQL Server 2008, SQL Server 2012                           |
| NHibernate.Spatial.MySQL   | MySQL 5.6                                 | MySQL 5.6, MySQL 5.7                                       |
| NHibernate.Spatial.PostGis | PostgreSQL 10 w/ PostGIS 2.5 <sup>1</sup> | PostgreSQL 10 w/ PostGIS 2.5, PostgreSQL 14 w/ PostGIS 3.2 |

<sup>1</sup> PostgreSQL 9.1 w/ PostGIS 2.0 or later will likely work, but are not explicitly
supported here as they are EOL (see [here](https://trac.osgeo.org/postgis/wiki/UsersWikiPostgreSQLPostGIS#PostGISSupportMatrix)).

## Getting Started

For configuring your application, please refer to [Configuration and Mapping](http://nhibernate.info/doc/spatial/configuration-and-mapping.html)
on the NHForge Wiki. Furthermore you can read about [sample usage](http://nhibernate.info/doc/spatial/sample-usage.html).
For an updated in-depth look at NHibernate.Spatial, you can read [this excellent blog
post](http://build-failed.blogspot.no/2012/02/nhibernate-spatial-part-1.html) written by Pedro
Sousa.

For general NHibernate help, the NHibernate community website - <http://nhibernate.info/> - has a
range of resources to help you get started with basic NHibernate, including [wikis][NHWiki],
[blogs][NHWiki] and [reference documentation][NH].

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
