NHibernate.Spatial (pmcxs fork)
===============================

This fork tries to improve on the work started on the main [Nhibernate.Spatial]: https://github.com/suryapratap/Nhibernate.Spatial branch. 
The solution structure has been much improved over the NHibernate Contrib counterpart as all the references are now using NuGet
and the projects have been upgraded to Visual Studio 2010/2012.

The "lib" folder dependency no longer exists and the Unit-Tests have been fixed, along with some other code changes. 
This fork will be mostly targeted towards SQL Server 2008/2012 but others are on the roadmap, notably PostGIS

Versions
--------
* NHibernate 3.3.1 is currently supported.


Not working
-----------
* The Oracle projects are not ready yet, and don't compile.


NHibernate.Spatial
==================

This is not the official NH Spatial repo, this is only a copy of the final commit at [Nhibernate Contrib][NHContrib] site, 
the code has been modified slightly to get it to compile with the latest NTS, GeoAPI and Nhibernate binaries.


The NHibernate community website - <http://www.nhforge.org> - has a range of resources to help you get started,
including [wikis][NHWiki], [blogs][NHWiki] and [reference documentation][NH].

[NHWiki]: http://nhforge.org/wikis
[NHBlog]: http://nhforge.org/blogs/nhibernate
[NH]: http://nhforge.org/doc/nh/en/index.html
[NHContrib]: http://sourceforge.net/projects/nhcontrib/