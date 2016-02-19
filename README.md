## NHibernate.Spatial

This is a fork of the final commit to NHibernate.Spatial at [Nhibernate Contrib][NHContrib] site made in december 2011. 
The code has been updated to compile with the latest NTS, GeoAPI and NHibernate binaries.

## Nuget Packages

NuGet packages are currently in pre-release, but stable releases for NHibernate 4.0 are expected soon (december 2014). The packages listed below are compatible with NHibernate 4.0.4 GA.

[Nhibernate.Spatial for Microsoft SQL Server](https://www.nuget.org/packages/NHibernate.Spatial.MsSql/) (Note! Assembly name has changed from MsSql2008 to MsSql)

[Nhibernate.Spatial for MySQL](https://www.nuget.org/packages/NHibernate.Spatial.MySQL/)

[Nhibernate.Spatial for PostGis](https://www.nuget.org/packages/NHibernate.Spatial.PostGis/)

## Getting Started
For configuring your application, please refer to [Configuration and Mapping](http://nhibernate.info/doc/spatial/configuration-and-mapping.html) on the NHForge Wiki. 
Furthermore you can read about [sample usage](http://nhibernate.info/doc/spatial/sample-usage.html). For an updated in-depth look at NHibernate.Spatial, you can 
read [this excellent blog post](http://build-failed.blogspot.no/2012/02/nhibernate-spatial-part-1.html) written by Pedro Sousa.

For general NHibernate help, the NHibernate community website - <http://www.nhforge.org> - has a range of resources to help you get started with basic NHibernate,
including [wikis][NHWiki], [blogs][NHWiki] and [reference documentation][NH]. 

## Request for Contributors ![Open Hub Stats](https://www.openhub.net/p/5599/widgets/project_partner_badge.gif)
All contributors are welcome. We need help improving the current dialects, getting tests to pass, identifying and fixing bugs, and keeping the project going.
Presently the Oracle Spatial dialect is excluded from the solution. The Oracle Dialect has never been functional, but with some work it could be. 
Contributors who may be able to help out with this are especially welcome. 

[NHWiki]: http://nhforge.org/wikis
[NHBlog]: http://nhforge.org/blogs/nhibernate
[NH]: http://nhforge.org/doc/nh/en/index.html
[NHContrib]: http://sourceforge.net/projects/nhcontrib/
