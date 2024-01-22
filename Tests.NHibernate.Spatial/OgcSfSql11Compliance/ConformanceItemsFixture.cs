using NetTopologySuite.Geometries;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.Spatial.Dialect;
using NHibernate.Spatial.Linq;
using NHibernate.Spatial.Metadata;
using NUnit.Framework;
using System;
using System.Linq;
using Tests.NHibernate.Spatial.OgcSfSql11Compliance.Model;

namespace Tests.NHibernate.Spatial.OgcSfSql11Compliance
{
    /// <summary>
    /// Adaptation of OGC SFSQL 1.1 Compliance Test Suite.
    ///
    /// All conformance items should be specified as HQL queries
    /// (server-side processed) and NTS methods (client-side processed).
    /// For example, see <c>ConformanceItemT11</c> and <c>ConformanceItemT11Hql</c>.
    /// </summary>
    public abstract class ConformanceItemsFixture : AbstractFixture
    {
        // Tolerance for floating point precision
        protected const double Tolerance = 0.000000000001;

        protected virtual string SpatialReferenceSystemWKT =>
            @"PROJCS[""UTM_ZONE_14N"", GEOGCS[""World Geodetic System 72"",
DATUM[""WGS_72"",  SPHEROID[""NWL_10D"", 6378135, 298.26]],
PRIMEM[""Greenwich"", 0], UNIT[""Meter"", 1.0]],
PROJECTION[""Transverse_Mercator""],
PARAMETER[""False_Easting"", 500000.0],
PARAMETER[""False_Northing"", 0.0],
PARAMETER[""Central_Meridian"", -99.0],
PARAMETER[""Scale_Factor"", 0.9996],
PARAMETER[""Latitude_of_origin"", 0.0],
UNIT[""Meter"", 1.0]]";

        protected ISession session;

        protected override Type[] Mappings
        {
            get
            {
                return new[]
                {
                    typeof(Bridge),
                    typeof(Building),
                    typeof(DividedRoute),
                    typeof(Forest),
                    typeof(Lake),
                    typeof(MapNeatline),
                    typeof(NamedPlace),
                    typeof(Pond),
                    typeof(RoadSegment),
                    typeof(Stream),
                };
            }
        }

        protected override bool CheckDatabaseWasCleanedOnTearDown => false;

        protected override void OnBeforeCreateSchema()
        {
            using (var session = sessions.OpenSession())
            {
                if (Metadata.SupportsSpatialMetadata(session, MetadataClass.SpatialReferenceSystem))
                {
                    session.Save(new SpatialReferenceSystem(101, "POSC", 32214, SpatialReferenceSystemWKT));
                    session.Flush();
                }
            }
        }

        protected override void OnAfterDropSchema()
        {
            using (var session = sessions.OpenSession())
            {
                if (Metadata.SupportsSpatialMetadata(session, MetadataClass.SpatialReferenceSystem))
                {
                    session.Delete("from SpatialReferenceSystem where SRID=101");
                    session.Flush();
                }
            }
        }

        protected override void OnTestFixtureSetUp()
        {
            using (var session = sessions.OpenSession())
            {
                // Lake
                session.Save(new Lake(101, "Blue Lake",
                    Wkt.Read("POLYGON((52 18,66 23,73 9,48 6,52 18),(59 18,67 18,67 13,59 13,59 18))")));

                // RoadSegment
                session.Save(new RoadSegment(102, "Route 5", null, 2,
                    Wkt.Read("LINESTRING( 0 18, 10 21, 16 23, 28 26, 44 31 )")));
                session.Save(new RoadSegment(103, "Route 5", "Main Street", 4,
                    Wkt.Read("LINESTRING( 44 31, 56 34, 70 38 )")));
                session.Save(new RoadSegment(104, "Route 5", null, 2,
                    Wkt.Read("LINESTRING( 70 38, 72 48 )")));
                session.Save(new RoadSegment(105, "Main Street", null, 4,
                    Wkt.Read("LINESTRING( 70 38, 84 42 )")));
                session.Save(new RoadSegment(106, "Dirt Road by Green Forest", null, 1,
                    Wkt.Read("LINESTRING( 28 26, 28 0 )")));

                // DividedRoute
                session.Save(new DividedRoute(119, "Route 75", 4,
                    Wkt.Read("MULTILINESTRING((10 48,10 21,10 0),(16 0,16 23,16 48))")));

                // Forest
                session.Save(new Forest(109, "Green Forest",
                    Wkt.Read("MULTIPOLYGON(((28 26,28 0,84 0,84 42,28 26),(52 18,66 23,73 9,48 6,52 18)),((59 18,67 18,67 13,59 13,59 18)))")));

                // Bridge
                session.Save(new Bridge(110, "Cam Bridge",
                    Wkt.Read("POINT( 44 31 )")));

                // Stream
                session.Save(new Stream(111, "Cam Stream",
                    Wkt.Read("LINESTRING( 38 48, 44 41, 41 36, 44 31, 52 18 )")));
                session.Save(new Stream(112, null,
                    Wkt.Read("LINESTRING( 76 0, 78 4, 73 9 )")));

                // Building
                session.Save(new Building(113, "123 Main Street",
                    Wkt.Read("POINT( 52 30 )"),
                    Wkt.Read("POLYGON( ( 50 31, 54 31, 54 29, 50 29, 50 31) )")));
                session.Save(new Building(114, "215 Main Street",
                    Wkt.Read("POINT( 64 33 )"),
                    Wkt.Read("POLYGON( ( 66 34, 62 34, 62 32, 66 32, 66 34) )")));

                // Pond
                session.Save(new Pond(120, null, "Stock Pond",
                    Wkt.Read("MULTIPOLYGON( ( ( 24 44, 22 42, 24 40, 24 44) ), ( ( 26 44, 26 40, 28 42, 26 44) ) )")));

                // NamedPlace
                session.Save(new NamedPlace(117, "Ashton",
                    Wkt.Read("POLYGON( ( 62 48, 84 48, 84 30, 56 30, 56 34, 62 48) )")));
                session.Save(new NamedPlace(118, "Goose Island",
                    Wkt.Read("POLYGON( ( 67 13, 67 18, 59 18, 59 13, 67 13) )")));

                // MapNeatline
                session.Save(new MapNeatline(115,
                    Wkt.Read("POLYGON( ( 0 0, 0 48, 84 48, 84 0, 0 0 ) )")));

                session.Flush();
            }
        }

        protected override void OnTestFixtureTearDown()
        {
            using (var session = sessions.OpenSession())
            {
                DeleteMappings(session);
            }
        }

        protected override void OnSetUp()
        {
            session = sessions.OpenSession();
        }

        protected override void OnTearDown()
        {
            session.Clear();
            session.Close();
            session.Dispose();
        }

        #region Queries testing functions in section 3.2.10.2 (T1 - T14)

        /// <summary>
        /// Conformance Item T1
        /// GEOMETRY_COLUMNS table/view is created/updated properly
        /// For this test we will check to see that all of the feature tables are
        /// represented by entries in the GEOMETRY_COLUMNS table/view
        ///
        /// ANSWER: lakes, road_segments, divided_routes, buildings, forests, bridges,
        ///         named_places, streams, ponds, map_neatlines
        /// *** ADAPTATION ALERT ***
        /// Since there are no quotes around the table names in the CREATE TABLEs,
        /// they will be converted to upper case in many DBMSs, and therefore, the
        /// answer to this query may be:
        /// ANSWER: LAKES, ROAD_SEGMENTS, DIVIDED_ROUTES, BUILDINGS, FORESTS, BRIDGES,
        ///         NAMED_PLACES, STREAMS, PONDS, MAP_NEATLINES
        /// *** ADAPTATION ALERT ***
        /// If the implementer made the adaptation concerning the buildings table
        /// in sqltsch.sql, then the answer here may differ slightly.
        ///
        /// Original SQL:
        /// <code>
        ///     SELECT f_table_name
        ///     FROM geometry_columns;
        /// </code>
        /// </summary>
        [Test]
        public void ConformanceItemT01Hql()
        {
            if (!Metadata.SupportsSpatialMetadata(session, MetadataClass.GeometryColumn))
            {
                Assert.Ignore("Provider does not support spatial metadata");
            }

            string[] tables =
            {
                "lakes",
                "road_segments",
                "divided_routes",
                "buildings",
                "forests",
                "bridges",
                "named_places",
                "streams",
                "ponds",
                "map_neatlines",
            };
            var results = session.CreateQuery("select distinct g.TableName from GeometryColumn g").List();

            Assert.AreEqual(tables.Length, results.Count);

            foreach (string tableName in results)
            {
                bool found = Array.IndexOf(tables, tableName) != -1;
                Assert.IsTrue(found);
            }
        }

        /// <summary>
        /// Conformance Item T2
        /// GEOMETRY_COLUMNS table/view is created/updated properly
        /// For this test we will check to see that the correct geometry columns for the
        /// streams table is represented in the GEOMETRY_COLUMNS table/view
        ///
        /// ANSWER: centerline
        /// *** ADAPTATION ALERT ***
        /// Since there are no quotes around the table name, streams, in it's CREATE TABLE,
        /// it will be converted to upper case in many DBMSs, and therefore, the WHERE
        /// clause may have to be f_table_name = 'STREAMS'.
        ///
        /// Original SQL:
        /// <code>
        ///     SELECT f_geometry_column
        ///     FROM geometry_columns
        ///     WHERE f_table_name = 'streams';
        /// </code>
        /// </summary>
        [Test]
        public void ConformanceItemT02Hql()
        {
            if (!Metadata.SupportsSpatialMetadata(session, MetadataClass.GeometryColumn))
            {
                Assert.Ignore("Provider does not support spatial metadata");
            }
            var query = session.CreateQuery(@"
				select g.Name
				from GeometryColumn g
				where g.TableName = 'streams'
				");

            string result = query.UniqueResult<string>();

            Assert.AreEqual("centerline", result);
        }

        [Test]
        public void ConformanceItemT02Linq()
        {
            if (!Metadata.SupportsSpatialMetadata(session, MetadataClass.GeometryColumn))
            {
                Assert.Ignore("Provider does not support spatial metadata");
            }
            var query =
                from g in session.Query<GeometryColumn>()
                where g.TableName == "streams"
                select g.Name;

            string result = query.Single();

            Assert.AreEqual("centerline", result);
        }

        /// <summary>
        /// Conformance Item T3
        /// GEOMETRY_COLUMNS table/view is created/updated properly
        /// For this test we will check to see that the correct coordinate dimension
        /// for the streams table is represented in the GEOMETRY_COLUMNS table/view
        ///
        /// ANSWER: 2
        /// *** ADAPTATION ALERT ***
        /// Since there are no quotes around the table name, streams, in it's CREATE TABLE,
        /// it will be converted to upper case in many DBMSs, and therefore, the WHERE
        /// clause may have to be f_table_name = 'STREAMS'.
        ///
        /// Original SQL:
        /// <code>
        ///     SELECT coord_dimension
        ///     FROM geometry_columns
        ///     WHERE f_table_name = 'streams';
        /// </code>
        /// </summary>
        [Test]
        public virtual void ConformanceItemT03Hql()
        {
            if (!Metadata.SupportsSpatialMetadata(session, MetadataClass.GeometryColumn))
            {
                Assert.Ignore("Provider does not support spatial metadata");
            }
            var query = session.CreateQuery(@"
				select g.Dimension
				from GeometryColumn g
				where g.TableName = 'streams'
				");

            int result = query.UniqueResult<int>();

            Assert.AreEqual(2, result);
        }

        [Test]
        public virtual void ConformanceItemT03Linq()
        {
            if (!Metadata.SupportsSpatialMetadata(session, MetadataClass.GeometryColumn))
            {
                Assert.Ignore("Provider does not support spatial metadata");
            }
            var query =
                from g in session.Query<GeometryColumn>()
                where g.TableName == "streams"
                select g.Dimension;

            int result = query.Single();

            Assert.AreEqual(2, result);
        }

        /// <summary>
        /// Conformance Item T4
        /// GEOMETRY_COLUMNS table/view is created/updated properly
        /// For this test we will check to see that the correct value of srid for
        /// the streams table is represented in the GEOMETRY_COLUMNS table/view
        ///
        /// ANSWER: 101
        /// *** ADAPTATION ALERT ***
        /// Since there are no quotes around the table name, streams, in it's CREATE TABLE,
        /// it will be converted to upper case in many DBMSs, and therefore, the WHERE
        /// clause may have to be f_table_name = 'STREAMS'.
        ///
        /// Original SQL:
        /// <code>
        ///     SELECT srid
        ///     FROM geometry_columns
        ///     WHERE f_table_name = 'streams';
        /// </code>
        /// </summary>
        [Test]
        public void ConformanceItemT04Hql()
        {
            if (!Metadata.SupportsSpatialMetadata(session, MetadataClass.GeometryColumn))
            {
                Assert.Ignore("Provider does not support spatial metadata");
            }
            var query = session.CreateQuery(@"
				select g.SRID
				from GeometryColumn g
				where g.TableName = 'streams'
				");

            int result = query.UniqueResult<int>();

            Assert.AreEqual(101, result);
        }

        [Test]
        public void ConformanceItemT04Linq()
        {
            if (!Metadata.SupportsSpatialMetadata(session, MetadataClass.GeometryColumn))
            {
                Assert.Ignore("Provider does not support spatial metadata");
            }
            var query =
                from g in session.Query<GeometryColumn>()
                where g.TableName == "streams"
                select g.SRID;

            int result = query.Single();

            Assert.AreEqual(101, result);
        }

        /// <summary>
        /// Conformance Item T5
        /// SPATIAL_REF_SYS table/view is created/updated properly
        /// For this test we will check to see that the correct value of srtext is
        /// represented in the SPATIAL_REF_SYS table/view
        ///
        /// ANSWER: 'PROJCS["UTM_ZONE_14N", GEOGCS["World Geodetic System 72",
        ///           DATUM["WGS_72",  SPHEROID["NWL_10D", 6378135, 298.26]],
        ///           PRIMEM["Greenwich", 0], UNIT["Meter", 1.0]],
        ///           PROJECTION["Traverse_Mercator"], PARAMETER["False_Easting", 500000.0],
        ///           PARAMETER["False_Northing", 0.0], PARAMETER["Central_Meridian", -99.0],
        ///           PARAMETER["Scale_Factor", 0.9996], PARAMETER["Latitude_of_origin", 0.0],
        ///           UNIT["Meter", 1.0]]'
        ///
        /// Original SQL:
        /// <code>
        ///     SELECT srtext
        ///     FROM SPATIAL_REF_SYS
        ///     WHERE SRID = 101;
        /// </code>
        /// </summary>
        [Test]
        public virtual void ConformanceItemT05Hql()
        {
            if (!Metadata.SupportsSpatialMetadata(session, MetadataClass.SpatialReferenceSystem))
            {
                Assert.Ignore("Provider does not support spatial metadata");
            }
            var srs = session.CreateQuery(
                    "from SpatialReferenceSystem where SRID=101")
                .UniqueResult<SpatialReferenceSystem>();

            Assert.IsNotNull(srs);
            Assert.AreEqual(SpatialReferenceSystemWKT, srs.WellKnownText);

            // Alternative syntax for identifiers:
            srs = session.CreateQuery(
                    "from SpatialReferenceSystem s where s=101")
                .UniqueResult<SpatialReferenceSystem>();

            Assert.IsNotNull(srs);
            Assert.AreEqual(SpatialReferenceSystemWKT, srs.WellKnownText);
        }

        [Test]
        public void ConformanceItemT05Linq()
        {
            if (!Metadata.SupportsSpatialMetadata(session, MetadataClass.SpatialReferenceSystem))
            {
                Assert.Ignore("Provider does not support spatial metadata");
            }
            var query =
                from s in session.Query<SpatialReferenceSystem>()
                where s.SRID == 101
                select s;

            var srs = query.Single();

            Assert.IsNotNull(srs);
            Assert.AreEqual(SpatialReferenceSystemWKT, srs.WellKnownText);
        }

        [Test]
        public void ConformanceItemT05Get()
        {
            if (!Metadata.SupportsSpatialMetadata(session, MetadataClass.SpatialReferenceSystem))
            {
                Assert.Ignore("Provider does not support spatial metadata");
            }
            var srs = session.Get<SpatialReferenceSystem>(101);
            Assert.IsNotNull(srs);
            Assert.AreEqual(SpatialReferenceSystemWKT, srs.WellKnownText);
        }

        [Test]
        public void ConformanceItemT05Criteria()
        {
            if (!Metadata.SupportsSpatialMetadata(session, MetadataClass.SpatialReferenceSystem))
            {
                Assert.Ignore("Provider does not support spatial metadata");
            }
            var srs = session.CreateCriteria(typeof(SpatialReferenceSystem))
                .Add(Restrictions.IdEq(101))
                .UniqueResult<SpatialReferenceSystem>();

            Assert.IsNotNull(srs);
            Assert.AreEqual(SpatialReferenceSystemWKT, srs.WellKnownText);
        }

        /// <summary>
        /// Conformance Item T6
        /// Dimension(g Geometry) : Integer
        /// For this test we will determine the dimension of Blue Lake.
        ///
        /// ANSWER: 2
        ///
        /// Original SQL:
        /// <code>
        ///     SELECT Dimension(shore)
        ///     FROM lakes
        ///     WHERE name = 'Blue Lake';
        /// </code>
        /// </summary>
        [Test]
        public void ConformanceItemT06()
        {
            var lake = session.CreateCriteria(typeof(Lake))
                .Add(Restrictions.Eq("Name", "Blue Lake"))
                .UniqueResult<Lake>();

            Assert.AreEqual(2, (int) lake.Shore.Dimension);
        }

        [Test]
        public void ConformanceItemT06Hql()
        {
            string query =
                @"select NHSP.Dimension(t.Shore)
				from Lake t
				where t.Name = 'Blue Lake'
				";
            int result = session.CreateQuery(query)
                .UniqueResult<int>();

            Assert.AreEqual(2, result);
        }

        [Test]
        public void ConformanceItemT06Linq()
        {
            var query =
                from t in session.Query<Lake>()
                where t.Name == "Blue Lake"
                select t.Shore.GetDimension();

            int result = query.Single();

            Assert.AreEqual(2, result);
        }

        /// <summary>
        /// Conformance Item T7
        /// GeometryType(g Geometry) : String
        /// For this test we will determine the type of Route 75.
        ///
        /// ANSWER: 9 (which corresponds to 'MULTILINESTRING')
        ///
        /// Original SQL:
        /// <code>
        ///     SELECT GeometryType(centerlines)
        ///     FROM lakes
        ///     WHERE name = 'Route 75';
        /// </code>
        /// </summary>
        /// <remarks>
        /// Correction:
        /// * Table name should be DIVIDED_ROUTES instead of LAKES
        /// </remarks>
        [Test]
        public void ConformanceItemT07()
        {
            var route = session.CreateCriteria(typeof(DividedRoute))
                .Add(Restrictions.Eq("Name", "Route 75"))
                .UniqueResult<DividedRoute>();
            Assert.AreEqual("MULTILINESTRING", route.Centerlines.GeometryType.ToUpper());
        }

        [Test]
        public virtual void ConformanceItemT07Hql()
        {
            string query =
                @"select NHSP.GeometryType(t.Centerlines)
				from DividedRoute t
				where t.Name = 'Route 75'
				";
            string result = session.CreateQuery(query)
                .UniqueResult<string>();

            Assert.AreEqual("MULTILINESTRING", result.ToUpper());
        }

        [Test]
        public virtual void ConformanceItemT07Linq()
        {
            var query =
                from t in session.Query<DividedRoute>()
                where t.Name == "Route 75"
                select t.Centerlines.GeometryType;

            string result = query.Single();

            Assert.AreEqual("MULTILINESTRING", result.ToUpper());
        }

        /// <summary>
        /// Conformance Item T8
        /// AsText(g Geometry) : String
        /// For this test we will determine the WKT representation of Goose Island.
        ///
        /// ANSWER: 'POLYGON( ( 67 13, 67 18, 59 18, 59 13, 67 13) )'
        ///
        /// Original SQL:
        /// <code>
        ///     SELECT AsText(boundary)
        ///     FROM named_places
        ///     WHERE name = 'Goose Island';
        /// </code>
        /// </summary>
        [Test]
        public void ConformanceItemT08()
        {
            var place = session.CreateCriteria(typeof(NamedPlace))
                .Add(Restrictions.Eq("Name", "Goose Island"))
                .UniqueResult<NamedPlace>();

            var expected = Wkt.Read("POLYGON( ( 67 13, 67 18, 59 18, 59 13, 67 13) )");

            Assert.IsTrue(expected.EqualsTopologically(place.Boundary));
        }

        [Test]
        public void ConformanceItemT08Hql()
        {
            string query =
                @"select t.Boundary
				from NamedPlace t
				where t.Name = 'Goose Island'
				";
            var result = session.CreateQuery(query)
                .UniqueResult<Geometry>();
            var expected = Wkt.Read("POLYGON( ( 67 13, 67 18, 59 18, 59 13, 67 13) )");

            Assert.IsTrue(expected.EqualsTopologically(result));
        }

        [Test]
        public void ConformanceItemT08Linq()
        {
            var query =
                from t in session.Query<NamedPlace>()
                where t.Name == "Goose Island"
                select t.Boundary;

            var result = query.Single();
            var expected = Wkt.Read("POLYGON( ( 67 13, 67 18, 59 18, 59 13, 67 13) )");

            Assert.IsTrue(expected.EqualsTopologically(result));
        }

        /// <summary>
        /// Conformance Item T9
        /// AsBinary(g Geometry) : Blob
        /// For this test we will determine the WKB representation of Goose Island.
        /// We will test by applying AsText to the result of PolygonFromText to the
        /// result of AsBinary.
        ///
        /// ANSWER: 'POLYGON( ( 67 13, 67 18, 59 18, 59 13, 67 13) )'
        ///
        /// Original SQL:
        /// <code>
        ///     SELECT AsText(PolygonFromWKB(AsBinary(boundary)))
        ///     FROM named_places
        ///     WHERE name = 'Goose Island';
        /// </code>
        /// </summary>
        [Test]
        public void ConformanceItemT09Hql()
        {
            string query =
                @"select NHSP.AsText(NHSP.PolyFromWKB(NHSP.AsBinary(t.Boundary), 0))
				from NamedPlace t
				where t.Name = 'Goose Island'
				";
            string result = session.CreateQuery(query)
                .UniqueResult<string>();

            var geometry = Wkt.Read(result);
            var expected = Wkt.Read("POLYGON( ( 67 13, 67 18, 59 18, 59 13, 67 13) )");

            Assert.IsTrue(expected.EqualsTopologically(geometry));
        }

        [Test]

        //[Ignore("TODO: ToPolygon")]
        public void ConformanceItemT09Linq()
        {
            var query =
                from t in session.Query<NamedPlace>()
                where t.Name == "Goose Island"
                select t.Boundary.AsBinary().ToPolygon(0);

            Geometry geometry = query.Single();
            var expected = Wkt.Read("POLYGON( ( 67 13, 67 18, 59 18, 59 13, 67 13) )");

            Assert.IsTrue(expected.EqualsTopologically(geometry));
        }

        /// <summary>
        /// Conformance Item T10
        /// SRID(g Geometry) : Integer
        /// For this test we will determine the SRID of Goose Island.
        ///
        /// ANSWER: 101
        ///
        /// Original SQL:
        /// <code>
        ///     SELECT SRID(boundary)
        ///     FROM named_places
        ///     WHERE name = 'Goose Island';
        /// </code>
        /// </summary>
        [Test]
        public void ConformanceItemT10Hql()
        {
            string query =
                @"select NHSP.SRID(t.Boundary)
				from NamedPlace t
				where t.Name = 'Goose Island'
				";
            int result = session.CreateQuery(query)
                .UniqueResult<int>();

            Assert.AreEqual(101, result);
        }

        [Test]
        public void ConformanceItemT10Linq()
        {
            var query =
                from t in session.Query<NamedPlace>()
                where t.Name == "Goose Island"
                select t.Boundary.SRID;

            int result = query.Single();

            Assert.AreEqual(101, result);
        }

        /// <summary>
        /// Conformance Item T11
        /// IsEmpty(g Geometry) : Integer
        /// For this test we will determine whether the geometry of a
        /// segment of Route 5 is empty.
        ///
        /// ANSWER: 0
        /// *** Adaptation Alert ***
        /// If the implementer provides IsEmpty as a boolean function, instead of as
        /// an INTEGER function, then:
        /// ANSWER: FALSE or 'f'
        ///
        /// Original SQL:
        /// <code>
        ///     SELECT IsEmpty(centerline)
        ///     FROM road_segments
        ///     WHERE name = 'Route 5' AND aliases = 'Main Street';
        /// </code>
        /// </summary>
        [Test]
        public void ConformanceItemT11()
        {
            var entity = session.CreateCriteria(typeof(RoadSegment))
                .Add(Restrictions.Eq("Name", "Route 5"))
                .Add(Restrictions.Eq("Aliases", "Main Street"))
                .UniqueResult<RoadSegment>();

            Assert.IsFalse(entity.Centerline.IsEmpty);
        }

        [Test]
        public void ConformanceItemT11Hql()
        {
            string query =
                @"select NHSP.IsEmpty(t.Centerline)
				from RoadSegment t
				where t.Name = 'Route 5' and t.Aliases = 'Main Street'
				";
            bool result = session.CreateQuery(query)
                .UniqueResult<bool>();

            Assert.IsFalse(result);
        }

        [Test]
        public void ConformanceItemT11Linq()
        {
            var query =
                from t in session.Query<RoadSegment>()
                where t.Name == "Route 5" && t.Aliases == "Main Street"
                select t.Centerline.IsEmpty;

            bool result = query.Single();

            Assert.IsFalse(result);
        }

        /// <summary>
        /// Conformance Item T12
        /// IsSimple(g Geometry) : Integer
        /// For this test we will determine whether the geometry of a
        /// segment of Blue Lake is simple.
        ///
        /// ANSWER: 1
        /// *** Adaptation Alert ***
        /// If the implementer provides IsSimple as a boolean function, instead of as
        /// an INTEGER function, then:
        /// ANSWER: TRUE or 't'
        ///
        /// Original SQL:
        /// <code>
        ///     SELECT IsSimple(shore)
        ///     FROM lakes
        ///     WHERE name = 'Blue Lake';
        /// </code>
        /// </summary>
        [Test]
        public void ConformanceItemT12()
        {
            var entity = session.CreateCriteria(typeof(Lake))
                .Add(Restrictions.Eq("Name", "Blue Lake"))
                .UniqueResult<Lake>();

            Assert.IsTrue(entity.Shore.IsSimple);
        }

        [Test]
        public void ConformanceItemT12Hql()
        {
            string query =
                @"select NHSP.IsSimple(t.Shore)
				from Lake t
				where t.Name = 'Blue Lake'
				";
            bool result = session.CreateQuery(query)
                .UniqueResult<bool>();

            Assert.IsTrue(result);
        }

        [Test]
        public void ConformanceItemT12Linq()
        {
            var query =
                from t in session.Query<Lake>()
                where t.Name == "Blue Lake"
                select t.Shore.IsSimple;

            bool result = query.Single();

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Conformance Item T13
        /// Boundary(g Geometry) : Geometry
        /// For this test we will determine the boundary of Goose Island.
        /// NOTE: The boundary result is as defined in 3.12.3.2 of 96-015R1.
        ///
        /// ANSWER: 'LINESTRING( 67 13, 67 18, 59 18, 59 13, 67 13 )'
        ///
        /// Original SQL:
        /// <code>
        ///     SELECT AsText(Boundary((boundary))
        ///     FROM named_places
        ///     WHERE name = 'Goose Island';
        /// </code>
        /// </summary>
        [Test]
        public void ConformanceItemT13()
        {
            var entity = session.CreateCriteria(typeof(NamedPlace))
                .Add(Restrictions.Eq("Name", "Goose Island"))
                .UniqueResult<NamedPlace>();

            var expected = Wkt.Read("LINESTRING( 67 13, 67 18, 59 18, 59 13, 67 13 )");

            Assert.IsTrue(expected.EqualsTopologically(entity.Boundary.Boundary));
        }

        [Test]
        public virtual void ConformanceItemT13Hql()
        {
            string query =
                @"select NHSP.AsText(NHSP.Boundary(t.Boundary))
				from NamedPlace t
				where t.Name = 'Goose Island'
				";
            string result = session.CreateQuery(query)
                .UniqueResult<string>();

            var geometry = Wkt.Read(result);
            var expected = Wkt.Read("LINESTRING( 67 13, 67 18, 59 18, 59 13, 67 13 )");

            Assert.IsTrue(expected.EqualsTopologically(geometry));
        }

        [Test]
        public virtual void ConformanceItemT13Linq()
        {
            var query =
                from t in session.Query<NamedPlace>()
                where t.Name == "Goose Island"
                select t.Boundary.Boundary;

            var geometry = query.Single();
            var expected = Wkt.Read("LINESTRING( 67 13, 67 18, 59 18, 59 13, 67 13 )");

            Assert.IsTrue(expected.EqualsTopologically(geometry));
        }

        /// <summary>
        /// Conformance Item T14
        /// Envelope(g Geometry) : Geometry
        /// For this test we will determine the envelope of Goose Island.
        ///
        /// ANSWER: 'POLYGON( ( 59 13, 59 18, 67 18, 67 13, 59 13) )'
        ///
        /// Original SQL:
        /// <code>
        ///     SELECT AsText(Envelope((boundary))
        ///     FROM named_places
        ///     WHERE name = 'Goose Island';
        /// </code>
        /// </summary>
        [Test]
        public void ConformanceItemT14Hql()
        {
            string query =
                @"select NHSP.AsText(NHSP.Envelope(t.Boundary))
				from NamedPlace t
				where t.Name = 'Goose Island'
				";
            string result = session.CreateQuery(query)
                .UniqueResult<string>();

            var geometry = Wkt.Read(result);
            var expected = Wkt.Read("POLYGON( ( 59 13, 59 18, 67 18, 67 13, 59 13) )");

            Assert.IsTrue(expected.EqualsTopologically(geometry));
        }

        [Test]
        public void ConformanceItemT14Linq()
        {
            var query =
                from t in session.Query<NamedPlace>()
                where t.Name == "Goose Island"
                select t.Boundary.Envelope;

            var geometry = query.Single();
            var expected = Wkt.Read("POLYGON( ( 59 13, 59 18, 67 18, 67 13, 59 13) )");

            Assert.IsTrue(expected.EqualsTopologically(geometry));
        }

        #endregion Queries testing functions in section 3.2.10.2 (T1 - T14)

        #region Queries testing functions in section 3.2.11.2 (T15 - T16)

        /// <summary>
        /// Conformance Item T15
        /// X(p Point) : Double Precision
        /// For this test we will determine the X coordinate of Cam Bridge.
        ///
        /// ANSWER: 44.00
        ///
        /// Original SQL:
        /// <code>
        ///     SELECT X(position)
        ///     FROM bridges
        ///     WHERE name = 'Bridges';
        /// </code>
        /// </summary>
        /// <remarks>
        /// Correction:
        /// * Bridge name should be 'Cam Bridge'
        /// </remarks>
        [Test]
        public void ConformanceItemT15Hql()
        {
            string query =
                @"select NHSP.X(t.Position)
				from Bridge t
				where t.Name = 'Cam Bridge'
				";
            double result = session.CreateQuery(query)
                .UniqueResult<double>();

            Assert.AreEqual(44, result);
        }

        [Test]
        public void ConformanceItemT15Linq()
        {
            var query =
                from t in session.Query<Bridge>()
                where t.Name == "Cam Bridge"
                select ((Point) t.Position).X;

            double result = query.Single();

            Assert.AreEqual(44, result);
        }

        /// <summary>
        /// Conformance Item T16
        /// Y(p Point) : Double Precision
        /// For this test we will determine the Y coordinate of Cam Bridge.
        ///
        /// ANSWER: 31.00
        ///
        /// Original SQL:
        /// <code>
        ///     SELECT Y(position)
        ///     FROM bridges
        ///     WHERE name = 'Bridges';
        /// </code>
        /// </summary>
        /// <remarks>
        /// Correction:
        /// * Bridge name should be 'Cam Bridge'
        /// </remarks>
        [Test]
        public void ConformanceItemT16Hql()
        {
            string query =
                @"select NHSP.Y(t.Position)
				from Bridge t
				where t.Name = 'Cam Bridge'
				";
            double result = session.CreateQuery(query)
                .UniqueResult<double>();

            Assert.AreEqual(31, result);
        }

        [Test]
        public void ConformanceItemT16Linq()
        {
            var query =
                from t in session.Query<Bridge>()
                where t.Name == "Cam Bridge"
                select ((Point) t.Position).Y;

            double result = query.Single();

            Assert.AreEqual(31, result);
        }

        #endregion Queries testing functions in section 3.2.11.2 (T15 - T16)

        #region Queries testing functions in section 3.2.12.2 (T17 - T21)

        /// <summary>
        /// Conformance Item T17
        /// StartPoint(c Curve) : Point
        /// For this test we will determine the start point of road segment 102.
        ///
        /// ANSWER: 'POINT( 0 18 )'
        ///
        /// Original SQL:
        /// <code>
        ///     SELECT AsText(StartPoint(centerline))
        ///     FROM road_segments
        ///     WHERE fid = 102;
        /// </code>
        /// </summary>
        [Test]
        public void ConformanceItemT17Hql()
        {
            string query =
                @"select NHSP.AsText(NHSP.StartPoint(t.Centerline))
				from RoadSegment t
				where t.Fid = 102
				";
            string result = session.CreateQuery(query)
                .UniqueResult<string>();

            var geometry = Wkt.Read(result);
            var expected = Wkt.Read("POINT( 0 18 )");

            Assert.IsTrue(expected.EqualsTopologically(geometry));
        }

        [Test]
        public void ConformanceItemT17Linq()
        {
            var query =
                from t in session.Query<RoadSegment>()
                where t.Fid == 102
                select ((LineString) t.Centerline).StartPoint;

            Geometry geometry = query.Single();
            var expected = Wkt.Read("POINT( 0 18 )");

            Assert.IsTrue(expected.EqualsTopologically(geometry));
        }

        /// <summary>
        /// Conformance Item T18
        /// EndPoint(c Curve) : Point
        /// For this test we will determine the end point of road segment 102.
        ///
        /// ANSWER: 'POINT( 44 31 )'
        ///
        /// Original SQL:
        /// <code>
        ///     SELECT AsText(EndPoint(centerline))
        ///     FROM road_segments
        ///     WHERE fid = 102;
        /// </code>
        /// </summary>
        [Test]
        public void ConformanceItemT18Hql()
        {
            string query =
                @"select NHSP.AsText(NHSP.EndPoint(t.Centerline))
				from RoadSegment t
				where t.Fid = 102
				";
            string result = session.CreateQuery(query)
                .UniqueResult<string>();

            var geometry = Wkt.Read(result);
            var expected = Wkt.Read("POINT( 44 31 )");

            Assert.IsTrue(expected.EqualsTopologically(geometry));
        }

        [Test]
        public void ConformanceItemT18Linq()
        {
            var query =
                from t in session.Query<RoadSegment>()
                where t.Fid == 102
                select ((LineString) t.Centerline).EndPoint;

            Geometry geometry = query.Single();
            var expected = Wkt.Read("POINT( 44 31 )");

            Assert.IsTrue(expected.EqualsTopologically(geometry));
        }

        /// <summary>
        /// Conformance Item T19
        /// IsClosed(c Curve) : Integer
        /// For this test we will determine the boundary of Goose Island.
        ///
        /// ANSWER: 1
        /// *** Adaptation Alert ***
        /// If the implementer provides IsClosed as a boolean function, instead of as
        /// an INTEGER function, then:
        /// ANSWER: TRUE or 't'
        ///
        /// Original SQL:
        /// <code>
        ///     SELECT IsClosed(Boundary(boundary))
        ///     FROM named_places
        ///     WHERE name = 'Goose Island';
        /// </code>
        /// </summary>
        [Test]
        public virtual void ConformanceItemT19Hql()
        {
            string query =
                @"select NHSP.IsClosed(NHSP.Boundary(t.Boundary))
				from NamedPlace t
				where t.Name = 'Goose Island'
				";
            bool result = session.CreateQuery(query)
                .UniqueResult<bool>();

            Assert.IsTrue(result);
        }

        [Test]
        public virtual void ConformanceItemT19Linq()
        {
            var query =
                from t in session.Query<NamedPlace>()
                where t.Name == "Goose Island"
                select ((LineString) t.Boundary.Boundary).IsClosed;

            bool result = query.Single();

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Conformance Item T20
        /// IsRing(c Curve) : Integer
        /// For this test we will determine the boundary of Goose Island.
        ///
        /// ANSWER: 1
        /// *** Adaptation Alert ***
        /// If the implementer provides IsRing as a boolean function, instead of as
        /// an INTEGER function, then:
        /// ANSWER: TRUE or 't'
        ///
        /// Original SQL:
        /// <code>
        ///     SELECT IsRing(Boundary(boundary))
        ///     FROM named_places
        ///     WHERE name = 'Goose Island';
        /// </code>
        /// </summary>
        [Test]
        public virtual void ConformanceItemT20Hql()
        {
            string query =
                @"select NHSP.IsRing(NHSP.Boundary(t.Boundary))
				from NamedPlace t
				where t.Name = 'Goose Island'
				";
            bool result = session.CreateQuery(query)
                .UniqueResult<bool>();

            Assert.IsTrue(result);
        }

        [Test]
        public virtual void ConformanceItemT20Linq()
        {
            var query =
                from t in session.Query<NamedPlace>()
                where t.Name == "Goose Island"
                select ((LineString) t.Boundary.Boundary).IsRing;

            bool result = query.Single();

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Conformance Item T21
        /// Length(c Curve) : Double Precision
        /// For this test we will determine the length of road segment 106.
        ///
        /// ANSWER: 26.00 (meters)
        ///
        /// Original SQL:
        /// <code>
        ///     SELECT Length(centerline)
        ///     FROM road_segments
        ///     WHERE fid = 106;
        /// </code>
        /// </summary>
        [Test]
        public void ConformanceItemT21Hql()
        {
            string query =
                @"select NHSP.Length(t.Centerline)
				from RoadSegment t
				where t.Fid = 106
				";
            double result = session.CreateQuery(query)
                .UniqueResult<double>();

            Assert.AreEqual(26, result);
        }

        [Test]
        public void ConformanceItemT21Linq()
        {
            var query =
                from t in session.Query<RoadSegment>()
                where t.Fid == 106
                select t.Centerline.Length;

            double result = query.Single();

            Assert.AreEqual(26, result);
        }

        #endregion Queries testing functions in section 3.2.12.2 (T17 - T21)

        #region Queries testing functions in section 3.2.13.2 (T22 - T23)

        /// <summary>
        /// Conformance Item T22
        /// NumPoints(l LineString) : Integer
        /// For this test we will determine the number of points in road segment 102.
        ///
        /// ANSWER: 5
        ///
        /// Original SQL:
        /// <code>
        ///     SELECT NumPoints(centerline)
        ///     FROM road_segments
        ///     WHERE fid = 102;
        /// </code>
        /// </summary>
        [Test]
        public void ConformanceItemT22Hql()
        {
            string query =
                @"select NHSP.NumPoints(t.Centerline)
				from RoadSegment t
				where t.Fid = 102
				";
            int result = session.CreateQuery(query)
                .UniqueResult<int>();

            Assert.AreEqual(5, result);
        }

        [Test]
        public void ConformanceItemT22Linq()
        {
            var query =
                from t in session.Query<RoadSegment>()
                where t.Fid == 102
                select t.Centerline.NumPoints;

            int result = query.Single();

            Assert.AreEqual(5, result);
        }

        /// <summary>
        /// Conformance Item T23
        /// PointN(l LineString, n Integer) : Point
        /// For this test we will determine the 1st point in road segment 102.
        ///
        /// ANSWER: 'POINT( 0 18 )'
        ///
        /// Original SQL:
        /// <code>
        ///     SELECT AsText(PointN(centerline, 1))
        ///     FROM road_segments
        ///     WHERE fid = 102;
        /// </code>
        /// </summary>
        [Test]
        public void ConformanceItemT23Hql()
        {
            string query =
                @"select NHSP.AsText(NHSP.PointN(t.Centerline, 1))
				from RoadSegment t
				where t.Fid = 102
				";
            string result = session.CreateQuery(query)
                .UniqueResult<string>();

            var geometry = Wkt.Read(result);
            var expected = Wkt.Read("POINT( 0 18 )");

            Assert.IsTrue(expected.EqualsTopologically(geometry));
        }

        [Test]
        public virtual void ConformanceItemT23Linq()
        {
            var query =
                from t in session.Query<RoadSegment>()
                where t.Fid == 102
                select ((LineString) t.Centerline)
                    .GetPointN(0);

            Geometry geometry = query.Single();
            var expected = Wkt.Read("POINT( 0 18 )");

            Assert.IsTrue(expected.EqualsTopologically(geometry));
        }

        #endregion Queries testing functions in section 3.2.13.2 (T22 - T23)

        #region Queries testing functions in section 3.2.14. (T24 - T26)

        /// <summary>
        /// Conformance Item T24
        /// Centroid(s Surface) : Point
        /// For this test we will determine the centroid of Goose Island.
        ///
        /// ANSWER: 'POINT( 63 15.5 )'
        ///
        /// Original SQL:
        /// <code>
        ///     SELECT AsText(Centroid(boundary))
        ///     FROM named_places
        ///     WHERE name = 'Goose Island';
        /// </code>
        /// </summary>
        [Test]
        public void ConformanceItemT24Hql()
        {
            string query =
                @"select NHSP.AsText(NHSP.Centroid(t.Boundary))
				from NamedPlace t
				where t.Name = 'Goose Island'
				";
            string result = session.CreateQuery(query)
                .UniqueResult<string>();

            var geometry = Wkt.Read(result);
            var expected = Wkt.Read("POINT( 63 15.5 )");

            Assert.IsTrue(expected.EqualsExact(geometry, Tolerance));
        }

        [Test]
        public void ConformanceItemT24Linq()
        {
            var query =
                from t in session.Query<NamedPlace>()
                where t.Name == "Goose Island"
                select t.Boundary.Centroid;

            Geometry geometry = query.Single();
            var expected = Wkt.Read("POINT( 63 15.5 )");

            Assert.IsTrue(expected.EqualsExact(geometry, Tolerance));
        }

        /// <summary>
        /// Conformance Item T25
        /// PointOnSurface(s Surface) : Point
        /// For this test we will determine a point on Goose Island.
        /// NOTE: For this test we will have to uses the Contains function
        ///       (which we don't test until later).
        ///
        /// ANSWER: 1
        /// *** Adaptation Alert ***
        /// If the implementer provides Contains as a boolean function, instead of as
        /// an INTEGER function, then:
        /// ANSWER: TRUE or 't'
        ///
        /// Original SQL:
        /// <code>
        ///     SELECT Contains(boundary, PointOnSurface(boundary))
        ///     FROM named_places
        ///     WHERE name = 'Goose Island';
        /// </code>
        /// </summary>
        [Test]
        public void ConformanceItemT25()
        {
            var entity = session.CreateCriteria(typeof(NamedPlace))
                .Add(Restrictions.Eq("Name", "Goose Island"))
                .UniqueResult<NamedPlace>();

            Assert.IsTrue(entity.Boundary.Contains(entity.Boundary.PointOnSurface));
        }

        [Test]
        public virtual void ConformanceItemT25Hql()
        {
            string query =
                @"select NHSP.Contains(t.Boundary, NHSP.PointOnSurface(t.Boundary))
				from NamedPlace t
				where t.Name = 'Goose Island'
				";
            bool result = session.CreateQuery(query)
                .UniqueResult<bool>();

            Assert.IsTrue(result);
        }

        [Test]
        public virtual void ConformanceItemT25Linq()
        {
            var query =
                from t in session.Query<NamedPlace>()
                where t.Name == "Goose Island"
                select t.Boundary.Contains(t.Boundary.PointOnSurface);

            bool result = query.Single();

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Conformance Item T26
        /// Area(s Surface) : Double Precision
        /// For this test we will determine the area of Goose Island.
        ///
        /// ANSWER: 40.00 (square meters)
        ///
        /// Original SQL:
        /// <code>
        ///     SELECT Area(boundary)
        ///     FROM named_places
        ///     WHERE name = 'Goose Island';
        /// </code>
        /// </summary>
        [Test]
        public void ConformanceItemT26Hql()
        {
            string query =
                @"select NHSP.Area(t.Boundary)
				from NamedPlace t
				where t.Name = 'Goose Island'
				";
            double result = session.CreateQuery(query)
                .UniqueResult<double>();

            Assert.AreEqual(40, result);
        }

        [Test]
        public void ConformanceItemT26Linq()
        {
            var query =
                from t in session.Query<NamedPlace>()
                where t.Name == "Goose Island"
                select t.Boundary.Area;

            double result = query.Single();

            Assert.AreEqual(40, result);
        }

        #endregion Queries testing functions in section 3.2.14. (T24 - T26)

        #region Queries testing functions in section 3.2.15.2 (T27 - T29)

        /// <summary>
        /// Conformance Item T27
        /// ExteriorRing(p Polygon) : LineString
        /// For this test we will determine the exteroir ring of Blue Lake.
        ///
        /// ANSWER: 'LINESTRING(52 18, 66 23, 73  9, 48  6, 52 18)'
        ///
        /// Original SQL:
        /// <code>
        ///     SELECT AsText(ExteriorRing(shore))
        ///     FROM lakes
        ///     WHERE name = 'Blue Lake';
        /// </code>
        /// </summary>
        [Test]
        public void ConformanceItemT27Hql()
        {
            string query =
                @"select NHSP.AsText(NHSP.ExteriorRing(t.Shore))
				from Lake t
				where t.Name = 'Blue Lake'
				";
            string result = session.CreateQuery(query)
                .UniqueResult<string>();

            var geometry = Wkt.Read(result);
            var expected = Wkt.Read("LINESTRING(52 18, 66 23, 73  9, 48  6, 52 18)");

            Assert.IsTrue(expected.EqualsTopologically(geometry));
        }

        [Test]
        public void ConformanceItemT27Linq()
        {
            var query =
                from t in session.Query<Lake>()
                where t.Name == "Blue Lake"
                select ((Polygon) t.Shore).ExteriorRing;

            Geometry geometry = query.Single();
            var expected = Wkt.Read("LINESTRING(52 18, 66 23, 73  9, 48  6, 52 18)");

            Assert.IsTrue(expected.EqualsTopologically(geometry));
        }

        /// <summary>
        /// Conformance Item T28
        /// NumInteriorRings(p Polygon) : Integer
        /// For this test we will determine the number of interior rings of Blue Lake.
        ///
        /// ANSWER: 1
        ///
        /// Original SQL:
        /// <code>
        ///     SELECT NumInteriorRings(shore)
        ///     FROM lakes
        ///     WHERE name = 'Blue Lake';
        /// </code>
        /// </summary>
        [Test]
        public void ConformanceItemT28Hql()
        {
            string query =
                @"select NHSP.NumInteriorRings(t.Shore)
				from Lake t
				where t.Name = 'Blue Lake'
				";
            int result = session.CreateQuery(query)
                .UniqueResult<int>();

            Assert.AreEqual(1, result);
        }

        [Test]
        public void ConformanceItemT28Linq()
        {
            var query =
                from t in session.Query<Lake>()
                where t.Name == "Blue Lake"
                select ((Polygon) t.Shore).NumInteriorRings;

            int result = query.Single();

            Assert.AreEqual(1, result);
        }

        /// <summary>
        /// Conformance Item T29
        /// InteriorRingN(p Polygon, n Integer) : LineString
        /// For this test we will determine the first interior ring of Blue Lake.
        ///
        /// ANSWER: 'LINESTRING(59 18, 67 18, 67 13, 59 13, 59 18)'
        ///
        /// Original SQL:
        /// <code>
        ///     SELECT AsText(InteriorRingN(shore, 1))
        ///     FROM lakes
        ///     WHERE name = 'Blue Lake';
        /// </code>
        /// </summary>
        [Test]
        public void ConformanceItemT29()
        {
            var entity = session.CreateCriteria(typeof(Lake))
                .Add(Restrictions.Eq("Name", "Blue Lake"))
                .UniqueResult<Lake>();

            Geometry geometry = (entity.Shore as Polygon).InteriorRings[0];
            var expected = Wkt.Read("LINESTRING(59 18, 67 18, 67 13, 59 13, 59 18)");

            Assert.IsTrue(expected.EqualsTopologically(geometry));
        }

        [Test]
        public void ConformanceItemT29Hql()
        {
            string query =
                @"select NHSP.AsText(NHSP.InteriorRingN(t.Shore, 1))
				from Lake t
				where t.Name = 'Blue Lake'
				";
            string result = session.CreateQuery(query)
                .UniqueResult<string>();

            var geometry = Wkt.Read(result);
            var expected = Wkt.Read("LINESTRING(59 18, 67 18, 67 13, 59 13, 59 18)");

            Assert.IsTrue(expected.EqualsTopologically(geometry));
        }

        [Test]
        public virtual void ConformanceItemT29Linq()
        {
            var query =
                from t in session.Query<Lake>()
                where t.Name == "Blue Lake"
                select ((Polygon) t.Shore).GetInteriorRingN(0);

            Geometry geometry = query.Single();
            var expected = Wkt.Read("LINESTRING(59 18, 67 18, 67 13, 59 13, 59 18)");

            Assert.IsTrue(expected.EqualsTopologically(geometry));
        }

        #endregion Queries testing functions in section 3.2.15.2 (T27 - T29)

        #region Queries testing functions in section 3.2.16.2 (T30 - T31)

        /// <summary>
        /// Conformance Item T30
        /// NumGeometries(g GeometryCollection) : Integer
        /// For this test we will determine the number of geometries in Route 75.
        ///
        /// ANSWER: 2
        ///
        /// Original SQL:
        /// <code>
        ///     SELECT NumGeometries(centerlines)
        ///     FROM divided_routes
        ///     WHERE name = 'Route 75';
        /// </code>
        /// </summary>
        [Test]
        public void ConformanceItemT30Hql()
        {
            string query =
                @"select NHSP.NumGeometries(t.Centerlines)
				from DividedRoute t
				where t.Name = 'Route 75'
				";
            int result = session.CreateQuery(query)
                .UniqueResult<int>();

            Assert.AreEqual(2, result);
        }

        [Test]
        public void ConformanceItemT30Linq()
        {
            var query =
                from t in session.Query<DividedRoute>()
                where t.Name == "Route 75"
                select t.Centerlines.NumGeometries;

            int result = query.Single();

            Assert.AreEqual(2, result);
        }

        /// <summary>
        /// Conformance Item T31
        /// GeometryN(g GeometryCollection, n Integer) : Geometry
        /// For this test we will determine the second geometry in Route 75.
        ///
        /// ANSWER: 'LINESTRING( 16 0, 16 23, 16 48 )'
        ///
        /// Original SQL:
        /// <code>
        ///     SELECT GeometryN(centerlines, 2)
        ///     FROM divided_routes
        ///     WHERE name = 'Route 75';
        /// </code>
        /// </summary>
        [Test]
        public void ConformanceItemT31()
        {
            var entity = session.CreateCriteria(typeof(DividedRoute))
                .Add(Restrictions.Eq("Name", "Route 75"))
                .UniqueResult<DividedRoute>();

            var geometry = (entity.Centerlines as MultiLineString).Geometries[1];
            var expected = Wkt.Read("LINESTRING( 16 0, 16 23, 16 48 )");

            Assert.IsTrue(expected.EqualsTopologically(geometry));
        }

        [Test]
        public void ConformanceItemT31Hql()
        {
            string query =
                @"select NHSP.AsText(NHSP.GeometryN(t.Centerlines, 2))
				from DividedRoute t
				where t.Name = 'Route 75'
				";
            string result = session.CreateQuery(query)
                .UniqueResult<string>();

            var geometry = Wkt.Read(result);
            var expected = Wkt.Read("LINESTRING( 16 0, 16 23, 16 48 )");

            Assert.IsTrue(expected.EqualsTopologically(geometry));
        }

        [Test]
        public virtual void ConformanceItemT31Linq()
        {
            var query =
                from t in session.Query<DividedRoute>()
                where t.Name == "Route 75"
                select t.Centerlines.GetGeometryN(1);

            var geometry = query.Single();
            var expected = Wkt.Read("LINESTRING( 16 0, 16 23, 16 48 )");

            Assert.IsTrue(expected.EqualsTopologically(geometry));
        }

        #endregion Queries testing functions in section 3.2.16.2 (T30 - T31)

        #region Queries testing functions in section 3.2.17.2 (T31 - T33)

        /// <summary>
        /// Conformance Item T32
        /// IsClosed(mc MultiCurve) : Integer
        /// For this test we will determine if the geometry of Route 75 is closed.
        ///
        /// ANSWER: 0
        /// *** Adaptation Alert ***
        /// If the implementer provides IsClosed as a boolean function, instead of as
        /// an INTEGER function, then:
        /// ANSWER: FALSE or 'f'
        ///
        /// Original SQL:
        /// <code>
        ///     SELECT IsClosed(centerlines)
        ///     FROM divided_routes
        ///     WHERE name = 'Route 75';
        /// </code>
        /// </summary>
        [Test]
        public void ConformanceItemT32Hql()
        {
            string query =
                @"select NHSP.IsClosed(t.Centerlines)
				from DividedRoute t
				where t.Name = 'Route 75'
				";
            bool result = session.CreateQuery(query)
                .UniqueResult<bool>();

            Assert.IsFalse(result);
        }

        [Test]
        public void ConformanceItemT32Linq()
        {
            var query =
                from t in session.Query<DividedRoute>()
                where t.Name == "Route 75"
                select ((LineString) t.Centerlines).IsClosed;

            bool result = query.Single();

            Assert.IsFalse(result);
        }

        /// <summary>
        /// Conformance Item T33
        /// Length(mc MultiCurve) : Double Precision
        /// For this test we will determine the length of Route 75.
        /// NOTE: This makes no semantic sense in our example...
        ///
        /// ANSWER: 96.00 (meters)
        ///
        /// Original SQL:
        /// <code>
        ///     SELECT Length(centerlines)
        ///     FROM divided_routes
        ///     WHERE name = 'Route 75';
        /// </code>
        /// </summary>
        [Test]
        public void ConformanceItemT33Hql()
        {
            string query =
                @"select NHSP.Length(t.Centerlines)
				from DividedRoute t
				where t.Name = 'Route 75'
				";
            double result = session.CreateQuery(query)
                .UniqueResult<double>();

            Assert.AreEqual(96, result);
        }

        [Test]
        public void ConformanceItemT33Linq()
        {
            var query =
                from t in session.Query<DividedRoute>()
                where t.Name == "Route 75"
                select t.Centerlines.Length;

            double result = query.Single();

            Assert.AreEqual(96, result);
        }

        #endregion Queries testing functions in section 3.2.17.2 (T31 - T33)

        #region Queries testing functions in section 3.2.18.2 (T34 - T36)

        /// <summary>
        /// Conformance Item T34
        /// Centroid(ms MultiSurface) : Point
        /// For this test we will determine the centroid of the ponds.
        ///
        /// ANSWER: 'POINT( 25 42 )'
        ///
        /// Original SQL:
        /// <code>
        ///     SELECT AsText(Centroid(shores))
        ///     FROM ponds
        ///     WHERE fid = 120;
        /// </code>
        /// </summary>
        [Test]
        public void ConformanceItemT34Hql()
        {
            string query =
                @"select NHSP.AsText(NHSP.Centroid(t.Shores))
				from Pond t
				where t.Fid = 120
				";
            string result = session.CreateQuery(query)
                .UniqueResult<string>();

            var geometry = Wkt.Read(result);
            var expected = Wkt.Read("POINT( 25 42 )");

            Assert.IsTrue(expected.EqualsExact(geometry, Tolerance));
        }

        [Test]
        public void ConformanceItemT34Linq()
        {
            var query =
                from t in session.Query<Pond>()
                where t.Fid == 120
                select t.Shores.Centroid;

            Geometry geometry = query.Single();
            var expected = Wkt.Read("POINT( 25 42 )");

            Assert.IsTrue(expected.EqualsExact(geometry, Tolerance));
        }

        /// <summary>
        /// Conformance Item T35
        /// PointOnSurface(ms MultiSurface) : Point
        /// For this test we will determine a point on the ponds.
        /// NOTE: For this test we will have to uses the Contains function
        ///       (which we don't test until later).
        ///
        /// ANSWER: 1
        /// *** Adaptation Alert ***
        /// If the implementer provides Contains as a boolean function, instead of as
        /// an INTEGER function, then:
        /// ANSWER: TRUE or 't'
        ///
        /// Original SQL:
        /// <code>
        ///     SELECT Contains(shores, PointOnSurface(shores))
        ///     FROM ponds
        ///     WHERE fid = 120;
        /// </code>
        /// </summary>
        [Test]
        public void ConformanceItemT35()
        {
            var entity = session.CreateCriteria(typeof(Pond))
                .Add(Restrictions.Eq("Fid", (long) 120))
                .UniqueResult<Pond>();

            Assert.IsTrue(entity.Shores.Contains(entity.Shores.PointOnSurface));
        }

        [Test]
        public virtual void ConformanceItemT35Hql()
        {
            string query =
                @"select NHSP.Contains(t.Shores, NHSP.PointOnSurface(t.Shores))
				from Pond t
				where t.Fid = 120
				";
            bool result = session.CreateQuery(query)
                .UniqueResult<bool>();

            Assert.IsTrue(result);
        }

        [Test]
        public virtual void ConformanceItemT35Linq()
        {
            var query =
                from t in session.Query<Pond>()
                where t.Fid == 120
                select t.Shores.Contains(t.Shores.PointOnSurface);

            bool result = query.Single();

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Conformance Item T36
        /// Area(ms MultiSurface) : Double Precision
        /// For this test we will determine the area of the ponds.
        ///
        /// ANSWER: 8.00 (square meters)
        ///
        /// Original SQL:
        /// <code>
        ///     SELECT Area(shores)
        ///     FROM ponds
        ///     WHERE fid = 120;
        /// </code>
        /// </summary>
        [Test]
        public void ConformanceItemT36Hql()
        {
            string query =
                @"select NHSP.Area(t.Shores)
				from Pond t
				where t.Fid = 120
				";
            double result = session.CreateQuery(query)
                .UniqueResult<double>();

            Assert.AreEqual(8, result);
        }

        [Test]
        public void ConformanceItemT36Linq()
        {
            var query =
                from t in session.Query<Pond>()
                where t.Fid == 120
                select t.Shores.Area;

            double result = query.Single();

            Assert.AreEqual(8, result);
        }

        #endregion Queries testing functions in section 3.2.18.2 (T34 - T36)

        #region Queries testing functions in section 3.2.19.2 (T37 - T45)

        /// <summary>
        /// Conformance Item T37
        /// Equals(g1 Geometry, g2 Geometry) : Integer
        /// For this test we will determine if the geometry of Goose Island is equal
        /// to the same geometry as consructed from it's WKT representation.
        ///
        /// ANSWER: 1
        /// *** Adaptation Alert ***
        /// If the implementer provides Equals as a boolean function, instead of as
        /// an INTEGER function, then:
        /// ANSWER: TRUE or 't'
        ///
        /// Original SQL:
        /// <code>
        ///     SELECT Equals(boundary, PolygonFromText('POLYGON( ( 67 13, 67 18, 59 18, 59 13, 67 13) )',1))
        ///     FROM named_places
        ///     WHERE name = 'Goose Island';
        /// </code>
        /// </summary>
        /// <remarks>
        /// Correction:
        /// * SRID changed to 101. We require SRIDs are the same for binary operations.
        /// </remarks>
        [Test]
        public void ConformanceItemT37Hql()
        {
            string query =
                @"select NHSP.Equals(t.Boundary, NHSP.PolyFromText('POLYGON( ( 67 13, 67 18, 59 18, 59 13, 67 13) )',101))
				from NamedPlace t
				where t.Name = 'Goose Island'
				";
            bool result = session.CreateQuery(query)
                .UniqueResult<bool>();

            Assert.IsTrue(result);
        }

        [Test]
        public void ConformanceItemT37Linq()
        {
            var polygon = Wkt.Read("POLYGON( ( 67 13, 67 18, 59 18, 59 13, 67 13) )");
            polygon.SRID = 101;

            var query =
                from t in session.Query<NamedPlace>()
                where t.Name == "Goose Island"
                select t.Boundary.EqualsTopologically(polygon.MappedAs(SpatialDialect.GeometryTypeOf(session)));

            bool result = query.Single();

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Conformance Item T38
        /// Disjoint(g1 Geometry, g2 Geometry) : Integer
        /// For this test we will determine if the geometry of Route 75 is disjoint
        /// from the geometry of Ashton.
        ///
        /// ANSWER: 1
        /// *** Adaptation Alert ***
        /// If the implementer provides Disjoint as a boolean function, instead of as
        /// an INTEGER function, then:
        /// ANSWER: TRUE or 't'
        ///
        /// Original SQL:
        /// <code>
        ///     SELECT Disjoint(centerlines, boundary)
        ///     FROM divided_routes, named_places
        ///     WHERE divided_routes.name = 'Route 75' AND named_places.name = 'Ashton';
        /// </code>
        /// </summary>
        [Test]
        public void ConformanceItemT38Hql()
        {
            string query =
                @"select NHSP.Disjoint(dr.Centerlines, np.Boundary)
				from DividedRoute dr, NamedPlace np
				where dr.Name = 'Route 75' and np.Name = 'Ashton'
				";
            bool result = session.CreateQuery(query)
                .UniqueResult<bool>();

            Assert.IsTrue(result);
        }

        [Test]
        public void ConformanceItemT38Linq()
        {
            var query =
                from dr in session.Query<DividedRoute>()
                from np in session.Query<NamedPlace>()
                where dr.Name == "Route 75" && np.Name == "Ashton"
                select dr.Centerlines.Disjoint(np.Boundary);

            bool result = query.Single();

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Conformance Item T39
        /// Touch(g1 Geometry, g2 Geometry) : Integer
        /// For this test we will determine if the geometry of Cam Stream touches
        /// the geometry of Blue Lake.
        ///
        /// ANSWER: 1
        /// *** Adaptation Alert ***
        /// If the implementer provides Touch as a boolean function, instead of as
        /// an INTEGER function, then:
        /// ANSWER: TRUE or 't'
        ///
        /// Original SQL:
        /// <code>
        ///     SELECT Touch(centerline, shore)
        ///     FROM streams, lakes
        ///     WHERE streams.name = 'Cam Stream' AND lakes.name = 'Blue Lake';
        /// </code>
        /// </summary>
        [Test]
        public void ConformanceItemT39Hql()
        {
            string query =
                @"select NHSP.Touches(s.Centerline, l.Shore)
				from Stream s, Lake l
				where s.Name = 'Cam Stream' and l.Name = 'Blue Lake'
				";
            bool result = session.CreateQuery(query)
                .UniqueResult<bool>();

            Assert.IsTrue(result);
        }

        [Test]
        public void ConformanceItemT39Linq()
        {
            var query =
                from s in session.Query<Stream>()
                from l in session.Query<Lake>()
                where s.Name == "Cam Stream" && l.Name == "Blue Lake"
                select s.Centerline.Touches(l.Shore);

            bool result = query.Single();

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Conformance Item T40
        /// Within(g1 Geometry, g2 Geometry) : Integer
        /// For this test we will determine if the geometry of the house at 215 Main Street
        /// is within Ashton.
        ///
        /// ANSWER: 1
        /// *** Adaptation Alert ***
        /// If the implementer provides Within as a boolean function, instead of as
        /// an INTEGER function, then:
        /// ANSWER: TRUE or 't'
        ///
        /// Original SQL:
        /// <code>
        ///     SELECT Within(boundary, footprint)
        ///     FROM named_places, buildings
        ///     WHERE named_places.name = 'Ashton' AND buildings.address = '215 Main Street';
        /// </code>
        /// </summary>
        /// <remarks>
        /// Correction:
        /// * Within arguments order changed because is wrong.
        ///   The test asks if a footprint ('215 Main Street') is within a boundary ('Ashton'),
        ///   so the correct argument order should be Within(footprint, boundary).
        /// </remarks>
        [Test]
        public virtual void ConformanceItemT40Hql()
        {
            string query =
                @"select NHSP.Within(b.Footprint, np.Boundary)
				from NamedPlace np, Building b
				where np.Name = 'Ashton' and b.Address = '215 Main Street'
				";
            bool result = session.CreateQuery(query)
                .UniqueResult<bool>();

            Assert.IsTrue(result);
        }

        [Test]
        public virtual void ConformanceItemT40Linq()
        {
            var query =
                from np in session.Query<NamedPlace>()
                from b in session.Query<Building>()
                where np.Name == "Ashton" && b.Address == "215 Main Street"
                select b.Footprint.Within(np.Boundary);

            bool result = query.Single();

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Conformance Item T41
        /// Overlap(g1 Geometry, g2 Geometry) : Integer
        /// For this test we will determine if the geometry of Green Forest overlaps
        /// the geometry of Ashton.
        ///
        /// ANSWER: 1
        /// *** Adaptation Alert ***
        /// If the implementer provides Overlap as a boolean function, instead of as
        /// an INTEGER function, then:
        /// ANSWER: TRUE or 't'
        ///
        /// Original SQL:
        /// <code>
        ///     SELECT Overlap(forest.boundary, named_places.boundary)
        ///     FROM forests, named_places
        ///     WHERE forests.name = 'Green Forest' AND named_places.name = 'Ashton';
        /// </code>
        /// </summary>
        [Test]
        public void ConformanceItemT41Hql()
        {
            string query =
                @"select NHSP.Overlaps(f.Boundary, np.Boundary)
				from Forest f, NamedPlace np
				where f.Name = 'Green Forest' and np.Name = 'Ashton'
				";
            bool result = session.CreateQuery(query)
                .UniqueResult<bool>();

            Assert.IsTrue(result);
        }

        [Test]
        public void ConformanceItemT41Linq()
        {
            var query =
                from f in session.Query<Forest>()
                from np in session.Query<NamedPlace>()
                where f.Name == "Green Forest" && np.Name == "Ashton"
                select f.Boundary.Overlaps(np.Boundary);

            bool result = query.Single();

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Conformance Item T42
        /// Cross(g1 Geometry, g2 Geometry) : Integer
        /// For this test we will determine if the geometry of road segment 102 crosses
        /// the geometry of Route 75.
        ///
        /// ANSWER: 1
        /// *** Adaptation Alert ***
        /// If the implementer provides Cross as a boolean function, instead of as
        /// an INTEGER function, then:
        /// ANSWER: TRUE or 't'
        ///
        /// Original SQL:
        /// <code>
        ///     SELECT Cross(road_segment.centerline, divided_routes.centerlines)
        ///     FROM road_segment, divided_routes
        ///     WHERE road_segment.fid = 102 AND divided_routes.name = 'Route 75';
        /// </code>
        /// </summary>
        [Test]
        public void ConformanceItemT42Hql()
        {
            string query =
                @"select NHSP.Crosses(rs.Centerline, dr.Centerlines)
				from RoadSegment rs, DividedRoute dr
				where rs.Fid = 102 and dr.Name = 'Route 75'
				";
            bool result = session.CreateQuery(query)
                .UniqueResult<bool>();

            Assert.IsTrue(result);
        }

        [Test]
        public void ConformanceItemT42Linq()
        {
            var query =
                from rs in session.Query<RoadSegment>()
                from dr in session.Query<DividedRoute>()
                where rs.Fid == 102 && dr.Name == "Route 75"
                select rs.Centerline.Crosses(dr.Centerlines);

            bool result = query.Single();

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Conformance Item T43
        /// Intersects(g1 Geometry, g2 Geometry) : Integer
        /// For this test we will determine if the geometry of road segment 102 intersects
        /// the geometry of Route 75.
        ///
        /// ANSWER: 1
        /// *** Adaptation Alert ***
        /// If the implementer provides Intersects as a boolean function, instead of as
        /// an INTEGER function, then:
        /// ANSWER: TRUE or 't'
        ///
        /// Original SQL:
        /// <code>
        ///     SELECT Intersects(road_segment.centerline, divided_routes.centerlines)
        ///     FROM road_segment, divided_routes
        ///     WHERE road_segment.fid = 102 AND divided_routes.name = 'Route 75';
        /// </code>
        /// </summary>
        [Test]
        public void ConformanceItemT43Hql()
        {
            string query =
                @"select NHSP.Intersects(rs.Centerline, dr.Centerlines)
				from RoadSegment rs, DividedRoute dr
				where rs.Fid = 102 and dr.Name = 'Route 75'
				";
            bool result = session.CreateQuery(query)
                .UniqueResult<bool>();

            Assert.IsTrue(result);
        }

        [Test]
        public void ConformanceItemT43Linq()
        {
            var query =
                from rs in session.Query<RoadSegment>()
                from dr in session.Query<DividedRoute>()
                where rs.Fid == 102 && dr.Name == "Route 75"
                select rs.Centerline.Intersects(dr.Centerlines);

            bool result = query.Single();

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Conformance Item T44
        /// Contains(g1 Geometry, g2 Geometry) : Integer
        /// For this test we will determine if the geometry of Green Forest contains
        /// the geometry of Ashton.
        ///
        /// ANSWER: 0
        /// *** Adaptation Alert ***
        /// If the implementer provides Contains as a boolean function, instead of as
        /// an INTEGER function, then:
        /// ANSWER: FALSE or 'f'
        ///
        /// Original SQL:
        /// <code>
        ///     SELECT Contains(forest.boundary, named_places.boundary)
        ///     FROM forests, named_places
        ///     WHERE forests.name = 'Green Forest' AND named_places.name = 'Ashton';
        /// </code>
        /// </summary>
        [Test]
        public void ConformanceItemT44Hql()
        {
            string query =
                @"select NHSP.Contains(f.Boundary, np.Boundary)
				from Forest f, NamedPlace np
				where f.Name = 'Green Forest' and np.Name = 'Ashton'
				";
            bool result = session.CreateQuery(query)
                .UniqueResult<bool>();

            Assert.IsFalse(result);
        }

        [Test]
        public void ConformanceItemT44Linq()
        {
            var query =
                from f in session.Query<Forest>()
                from np in session.Query<NamedPlace>()
                where f.Name == "Green Forest" && np.Name == "Ashton"
                select f.Boundary.Contains(np.Boundary);

            bool result = query.Single();

            Assert.IsFalse(result);
        }

        /// <summary>
        /// Conformance Item T45
        /// Relate(g1 Geometry, g2 Geometry, PatternMatrix String) : Integer
        /// For this test we will determine if the geometry of Green Forest relates to
        /// the geometry of Ashton using the pattern "TTTTTTTTT".
        ///
        /// ANSWER: 1
        /// *** Adaptation Alert ***
        /// If the implementer provides Relate as a boolean function, instead of as
        /// an INTEGER function, then:
        /// ANSWER: TRUE or 't'
        ///
        /// Original SQL:
        /// <code>
        ///     SELECT Relate(forest.boundary, named_places.boundary, 'TTTTTTTTT')
        ///     FROM forests, named_places
        ///     WHERE forests.name = 'Green Forest' AND named_places.name = 'Ashton';
        /// </code>
        /// </summary>
        [Test]
        public virtual void ConformanceItemT45Hql()
        {
            string query =
                @"select NHSP.Relate(f.Boundary, np.Boundary, 'TTTTTTTTT')
				from Forest f, NamedPlace np
				where f.Name = 'Green Forest' and np.Name = 'Ashton'
				";
            bool result = session.CreateQuery(query)
                .UniqueResult<bool>();

            Assert.IsTrue(result);
        }

        [Test]
        public virtual void ConformanceItemT45Linq()
        {
            var query =
                from f in session.Query<Forest>()
                from np in session.Query<NamedPlace>()
                where f.Name == "Green Forest" && np.Name == "Ashton"
                select f.Boundary.Relate(np.Boundary, "TTTTTTTTT");

            bool result = query.Single();

            Assert.IsTrue(result);
        }

        #endregion Queries testing functions in section 3.2.19.2 (T37 - T45)

        #region Queries testing functions in section 3.2.20.2 (T46)

        /// <summary>
        /// Conformance Item T46
        /// Distance(g1 Geometry, g2 Geometry) : Double Precision
        /// For this test we will determine the distance between Cam Bridge and Ashton.
        ///
        /// ANSWER: 12 (meters)
        ///
        /// Original SQL:
        /// <code>
        ///     SELECT Distance(position, boundary)
        ///     FROM bridges, named_places
        ///     WHERE bridges.name = 'Cam Bridge' AND named_places.name = 'Ashton';
        /// </code>
        /// </summary>
        [Test]
        public void ConformanceItemT46Hql()
        {
            string query =
                @"select NHSP.Distance(b.Position, np.Boundary)
				from Bridge b, NamedPlace np
				where b.Name = 'Cam Bridge' and np.Name = 'Ashton'
				";
            double result = session.CreateQuery(query)
                .UniqueResult<double>();

            Assert.AreEqual(12, result);
        }

        [Test]
        public void ConformanceItemT46Linq()
        {
            var query =
                from b in session.Query<Bridge>()
                from np in session.Query<NamedPlace>()
                where b.Name == "Cam Bridge" && np.Name == "Ashton"
                select b.Position.Distance(np.Boundary);

            double result = query.Single();

            Assert.AreEqual(12, result);
        }

        #endregion Queries testing functions in section 3.2.20.2 (T46)

        #region Queries testing functions in section 3.2.21.2 (T47 - T50)

        /// <summary>
        /// Conformance Item T47
        /// Intersection(g1 Geometry, g2 Geometry) : Geometry
        /// For this test we will determine the intersection between Cam Stream and
        /// Blue Lake.
        ///
        /// ANSWER: 'POINT( 52 18 )'
        ///
        /// Original SQL:
        /// <code>
        ///     SELECT Intersection(centerline, shore)
        ///     FROM streams, lakes
        ///     WHERE streams.name = 'Cam Stream' AND lakes.name = 'Blue Lake';
        /// </code>
        /// </summary>
        [Test]
        public void ConformanceItemT47Hql()
        {
            string query =
                @"select NHSP.AsText(NHSP.Intersection(s.Centerline, l.Shore))
				from Stream s, Lake l
				where s.Name = 'Cam Stream' and l.Name = 'Blue Lake'
				";
            string result = session.CreateQuery(query)
                .UniqueResult<string>();

            var geometry = Wkt.Read(result);
            var expected = Wkt.Read("POINT( 52 18 )");

            Assert.IsTrue(expected.EqualsTopologically(geometry));
        }

        [Test]
        public void ConformanceItemT47Linq()
        {
            var query =
                from s in session.Query<Stream>()
                from l in session.Query<Lake>()
                where s.Name == "Cam Stream" && l.Name == "Blue Lake"
                select s.Centerline.Intersection(l.Shore);

            var geometry = query.Single();
            var expected = Wkt.Read("POINT( 52 18 )");

            Assert.IsTrue(expected.EqualsTopologically(geometry));
        }

        /// <summary>
        /// Conformance Item T48
        /// Difference(g1 Geometry, g2 Geometry) : Geometry
        /// For this test we will determine the difference between Ashton and
        /// Green Forest.
        ///
        /// ANSWER: 'POLYGON( ( 56 34, 62 48, 84 48, 84 42, 56 34) )'
        /// NOTE: The order of the vertices here is arbitrary.
        ///
        /// Original SQL:
        /// <code>
        ///     SELECT Difference(named_places.boundary, forests.boundary)
        ///     FROM named_places, forests
        ///     WHERE named_places.name = 'Ashton' AND forests.name = 'Green Forest';
        /// </code>
        /// </summary>
        [Test]
        public virtual void ConformanceItemT48Hql()
        {
            string query =
                @"select NHSP.AsText(NHSP.Difference(np.Boundary, f.Boundary))
				from NamedPlace np, Forest f
				where np.Name = 'Ashton' and f.Name = 'Green Forest'
				";
            string result = session.CreateQuery(query)
                .UniqueResult<string>();

            var geometry = Wkt.Read(result);
            var expected = Wkt.Read("POLYGON( ( 56 34, 62 48, 84 48, 84 42, 56 34) )");

            Assert.IsTrue(expected.EqualsTopologically(geometry));
        }

        [Test]
        public virtual void ConformanceItemT48Linq()
        {
            var query =
                from np in session.Query<NamedPlace>()
                from f in session.Query<Forest>()
                where np.Name == "Ashton" && f.Name == "Green Forest"
                select np.Boundary.Difference(f.Boundary);

            var geometry = query.Single();
            var expected = Wkt.Read("POLYGON( ( 56 34, 62 48, 84 48, 84 42, 56 34) )");

            Assert.IsTrue(expected.EqualsTopologically(geometry));
        }

        /// <summary>
        /// Conformance Item T49
        /// Union(g1 Geometry, g2 Geometry) : Integer
        /// For this test we will determine the union of Blue Lake and Goose Island
        ///
        /// ANSWER: 'POLYGON((52 18,66 23,73 9,48 6,52 18))'
        /// NOTE: The outer ring of BLue Lake is the answer.
        ///
        /// Original SQL:
        /// <code>
        ///     SELECT Union(shore, boundary)
        ///     FROM lakes, named_places
        ///     WHERE lakes.name = 'Blue Lake' AND named_places.name = Ashton';
        /// </code>
        /// </summary>
        /// <remarks>
        /// Correction:
        /// * Place name condition changed from 'Ashton' to 'Goose Island'
        ///   in order to conform the test enunciation and correct answer.
        /// </remarks>
        [Test]
        public void ConformanceItemT49Hql()
        {
            string query =
                @"select NHSP.AsText(NHSP.Union(l.Shore, np.Boundary))
				from Lake l, NamedPlace np
				where l.Name = 'Blue Lake' and np.Name = 'Goose Island'
				";
            string result = session.CreateQuery(query)
                .UniqueResult<string>();

            var geometry = Wkt.Read(result);
            var expected = Wkt.Read("POLYGON((52 18,66 23,73 9,48 6,52 18))");

            Assert.IsTrue(expected.EqualsTopologically(geometry));
        }

        [Test]
        public void ConformanceItemT49Linq()
        {
            var query =
                from l in session.Query<Lake>()
                from np in session.Query<NamedPlace>()
                where l.Name == "Blue Lake" && np.Name == "Goose Island"
                select l.Shore.Union(np.Boundary);

            var geometry = query.Single();
            var expected = Wkt.Read("POLYGON((52 18,66 23,73 9,48 6,52 18))");

            Assert.IsTrue(expected.EqualsTopologically(geometry));
        }

        /// <summary>
        /// Conformance Item T50
        /// SymmetricDifference(g1 Geometry, g2 Geometry) : Integer
        /// For this test we will determine the symmetric difference of Blue Lake
        /// and Goose Island
        ///
        /// ANSWER: 'POLYGON((52 18,66 23,73 9,48 6,52 18))'
        /// NOTE: The outer ring of BLue Lake is the answer.
        ///
        /// Original SQL:
        /// <code>
        ///     SELECT SymmetricDifference(shore, boundary)
        ///     FROM lakes, named_places
        ///     WHERE lakes.name = 'Blue Lake' OR named_places.name = 'Ashton';
        /// </code>
        /// </summary>
        /// <remarks>
        /// Correction:
        /// * Place name condition changed from 'Ashton' to 'Goose Island'
        ///   in order to conform the test enunciation and correct answer.
        /// </remarks>
        [Test]
        public void ConformanceItemT50()
        {
            var shore = session
                .CreateCriteria(typeof(Lake), "l")
                .Add(Restrictions.Eq("l.Name", "Blue Lake"))
                .SetProjection(Projections.Property("l.Shore"))
                .UniqueResult<Geometry>();

            var boundary = session
                .CreateCriteria(typeof(NamedPlace))
                .Add(Restrictions.Eq("Name", "Goose Island"))
                .SetProjection(Projections.Property("Boundary"))
                .UniqueResult<Geometry>();

            var result = shore.SymmetricDifference(boundary);
            var expected = Wkt.Read("POLYGON((52 18,66 23,73 9,48 6,52 18))");

            Assert.IsTrue(expected.EqualsTopologically(result));
        }

        [Test]
        public void ConformanceItemT50Hql()
        {
            string query =
                @"select NHSP.AsText(NHSP.SymDifference(l.Shore, np.Boundary))
				from Lake l, NamedPlace np
				where l.Name = 'Blue Lake' and np.Name = 'Goose Island'
				";
            string result = session.CreateQuery(query)
                .UniqueResult<string>();

            var geometry = Wkt.Read(result);
            var expected = Wkt.Read("POLYGON((52 18,66 23,73 9,48 6,52 18))");

            Assert.IsTrue(expected.EqualsTopologically(geometry));
        }

        [Test]
        public void ConformanceItemT50Linq()
        {
            var query =
                from l in session.Query<Lake>()
                from np in session.Query<NamedPlace>()
                where l.Name == "Blue Lake" && np.Name == "Goose Island"
                select l.Shore.SymmetricDifference(np.Boundary);

            var geometry = query.Single();
            var expected = Wkt.Read("POLYGON((52 18,66 23,73 9,48 6,52 18))");

            Assert.IsTrue(expected.EqualsTopologically(geometry));
        }

        /// <summary>
        /// Conformance Item T51
        /// Buffer(g Geometry, d Double Precision) : Geometry
        /// For this test we will make a 15 meter buffer about Cam Bridge.
        /// NOTE: This test we count the number of buildings contained in
        ///       the buffer that is generated. This test only works because
        ///       we have a single bridge record, two building records, and
        ///       we selected the buffer size such that only one of the buildings
        ///       is contained in the buffer.
        ///
        /// ANSWER: 1
        /// *** Adaptation Alert ***
        /// If the implementer provides Contains as a boolean function, instead of as
        /// an INTEGER function, then the WHERE clause should be:
        /// WHERE Contains(Buffer(bridges.position, 15.0), buildings.footprint) = 'TRUE';
        ///   - or -
        /// WHERE Contains(Buffer(bridges.position, 15.0), buildings.footprint) = 't';
        ///
        /// Original SQL:
        /// <code>
        ///     SELECT count(*)
        ///     FROM buildings, bridges
        ///     WHERE Contains(Buffer(bridges.position, 15.0), buildings.footprint) = 1;
        /// </code>
        /// </summary>
        [Test]
        public virtual void ConformanceItemT51Hql()
        {
            string query =
                @"select count(*)
				from Building bl, Bridge br
				where NHSP.Contains(NHSP.Buffer(br.Position, 15.0), bl.Footprint) = true
				";
            long result = session.CreateQuery(query)
                .UniqueResult<long>();

            Assert.AreEqual(1, result);
        }

        [Test]
        public virtual void ConformanceItemT51Linq()
        {
            var query =
                    from bl in session.Query<Building>()
                    from br in session.Query<Bridge>()
                    where br.Position.Buffer(15.0).Contains(bl.Footprint)
                    select bl
                ;
            long result = query.LongCount();

            Assert.AreEqual(1, result);
        }

        /// <summary>
        /// Conformance Item T52
        /// ConvexHull(g Geometry) : Geometry
        /// For this test we will determine the convex hull of Blue Lake
        ///
        /// ANSWER: 'POLYGON((52 18,66 23,73 9,48 6,52 18))'
        /// NOTE: The outer ring of BLue Lake is the answer.
        ///
        /// Original SQL:
        /// <code>
        ///     SELECT ConvexHull(shore)
        ///     FROM lakes
        ///     WHERE lakes.name = 'Blue Lake';
        /// </code>
        /// </summary>
        [Test]
        public void ConformanceItemT52Hql()
        {
            string query =
                @"select NHSP.AsText(NHSP.ConvexHull(l.Shore))
				from Lake l
				where l.Name = 'Blue Lake'
				";
            string result = session.CreateQuery(query)
                .UniqueResult<string>();

            var geometry = Wkt.Read(result);
            var expected = Wkt.Read("POLYGON((52 18,66 23,73 9,48 6,52 18))");

            Assert.IsTrue(expected.EqualsTopologically(geometry));
        }

        [Test]
        public void ConformanceItemT52Linq()
        {
            var query =
                from l in session.Query<Lake>()
                where l.Name == "Blue Lake"
                select l.Shore.ConvexHull();

            var geometry = query.Single();
            var expected = Wkt.Read("POLYGON((52 18,66 23,73 9,48 6,52 18))");

            Assert.IsTrue(expected.EqualsTopologically(geometry));
        }

        #endregion Queries testing functions in section 3.2.21.2 (T47 - T50)
    }
}
