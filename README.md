NHibernate.Spatial (pmcxs fork)
===============================

This fork tries to improve on the work started on the main [Nhibernate.Spatial]: https://github.com/suryapratap/Nhibernate.Spatial branch. 

Notable changes:
* Fixed the remaining references. No longer dependent on "lib" folder.
* Renamed projects
* Fixed some bugs, including minor refactors
* Fixed the unit tests, including minor refactors

This fork will be mostly targeted towards SQL Server 2008/2012 but support for others RDBMS is also on the roadmap

Currently supports NHibernate 3.3.1. All the unit-tests for SQL Server 2008 pass.

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