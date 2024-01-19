using NetTopologySuite.Geometries;
using NHibernate;
using NHibernate.Spatial.Criterion;
using NHibernate.Spatial.Dialect;
using NUnit.Framework;
using System;
using Tests.NHibernate.Spatial.RandomGeometries.Model;

namespace Tests.NHibernate.Spatial.RandomGeometries
{
    /// <summary>
    /// Port of MAJAS Hibernate Spatial test suite.
    /// </summary>
    public abstract class SpatialQueriesFixture : AbstractFixture
    {
        protected const string FilterString = "POLYGON((0.0 0.0, 25000.0 0.0, 25000.0 25000.0, 0.0 25000.0, 0.0 0.0))";

        protected ISession Session;
        protected Geometry Filter;

        protected override Type[] Mappings
        {
            get
            {
                return new[]
                {
                    typeof(LineStringEntity),
                    typeof(MultiLineStringEntity),
                    typeof(MultiPointEntity),
                    typeof(MultiPolygonEntity),
                    typeof(PointEntity),
                    typeof(PolygonEntity),
                };
            }
        }

        protected override bool CheckDatabaseWasCleanedOnTearDown => false;

        [Test]
        public void LineStringFiltering()
        {
            var results = Session.CreateCriteria(typeof(LineStringEntity))
                .Add(SpatialRestrictions.Filter("Geometry", Filter))
                .List();

            long count;
            using (var command = Session.Connection.CreateCommand())
            {
                command.CommandText = SqlLineStringFilter(FilterString);
                count = (long) command.ExecuteScalar();
            }

            Assert.AreEqual(count, results.Count);
        }

        [Test]
        public void PolygonFiltering()
        {
            var results = Session.CreateCriteria(typeof(PolygonEntity))
                .Add(SpatialRestrictions.Filter("Geometry", Filter))
                .List();

            long count;
            using (var command = Session.Connection.CreateCommand())
            {
                command.CommandText = SqlPolygonFilter(FilterString);
                count = (long) command.ExecuteScalar();
            }

            Assert.AreEqual(count, results.Count);
        }

        [Test]
        public void MultiLineStringFiltering()
        {
            var results = Session.CreateCriteria(typeof(MultiLineStringEntity))
                .Add(SpatialRestrictions.Filter("Geometry", Filter))
                .List();

            long count;
            using (var command = Session.Connection.CreateCommand())
            {
                command.CommandText = SqlMultiLineStringFilter(FilterString);
                count = (long) command.ExecuteScalar();
            }

            Assert.AreEqual(count, results.Count);
        }

        [Test]
        public void HqlAsTextLineString()
        {
            var results = Session
                .CreateQuery("select NHSP.AsText(l.Geometry) from LineStringEntity as l")
                .SetMaxResults(10)
                .List();
            foreach (string item in results)
            {
                Assert.IsNotNull(item);
                Assert.AreNotEqual(string.Empty, item);
            }
        }

        [Test]
        public void HqlDimensionLineString()
        {
            var results1 = Session
                .CreateQuery("select NHSP.Dimension(l.Geometry) from LineStringEntity as l where l.Geometry is not null")
                .SetMaxResults(10)
                .List();

            foreach (int dim in results1)
            {
                Assert.AreEqual(1, dim);
            }

            var results2 = Session
                .CreateQuery("select NHSP.Dimension(p.Geometry) from PolygonEntity as p where p.Geometry is not null")
                .SetMaxResults(10)
                .List();

            foreach (int dim in results2)
            {
                Assert.AreEqual(2, dim);
            }
        }

        [Test]
        public void HqlOverlapsLineString()
        {
            var results = Session
                .CreateQuery(
                    "select NHSP.Overlaps(?,l.Geometry) from LineStringEntity as l where l.Geometry is not null")
                .SetParameter(0, Filter, SpatialDialect.GeometryTypeOf(Session))
                .List();

            long countOverlapping = 0;
            foreach (bool? isOverlapped in results)
            {
                if (isOverlapped.HasValue && isOverlapped.Value)
                {
                    countOverlapping++;
                }
            }

            long count;
            using (var command = Session.Connection.CreateCommand())
            {
                command.CommandText = SqlOvelapsLineString(FilterString);
                count = (long) command.ExecuteScalar();
            }

            Assert.AreEqual(countOverlapping, count);
        }

        [Test]
        public virtual void HqlRelateLineString()
        {
            long count = Session
                .CreateQuery(
                    "select count(*) from LineStringEntity l where l.Geometry is not null and NHSP.Relate(l.Geometry, ?, 'TT*******') = true")
                .SetParameter(0, Filter, SpatialDialect.GeometryTypeOf(Session))
                .UniqueResult<long>();

            Assert.Greater((int) count, 0);
        }

        [Test]
        public void HqlIntersectsLineString()
        {
            var results = Session
                .CreateQuery(
                    "select NHSP.Intersects(?,l.Geometry) from LineStringEntity as l where l.Geometry is not null")
                .SetParameter(0, Filter, SpatialDialect.GeometryTypeOf(Session))
                .List();

            long intersects = 0;
            foreach (bool b in results)
            {
                if (b)
                {
                    intersects++;
                }
            }

            long altIntersects = Session
                .CreateQuery("select count(*) from LineStringEntity as l where NHSP.Intersects(l.Geometry, ?) = true")
                .SetParameter(0, Filter, SpatialDialect.GeometryTypeOf(Session))
                .UniqueResult<long>();

            Assert.AreEqual(intersects, altIntersects);

            long count;
            using (var command = Session.Connection.CreateCommand())
            {
                command.CommandText = SqlIntersectsLineString(FilterString);
                count = (long) command.ExecuteScalar();
            }

            Assert.AreEqual(intersects, count);

            results = Session
                .CreateQuery("from LineStringEntity as l where NHSP.Intersects(?,l.Geometry) = true")
                .SetParameter(0, Filter, SpatialDialect.GeometryTypeOf(Session))
                .List();

            Assert.AreEqual(count, results.Count);
        }

        [Test]
        public virtual void HqlSRID()
        {
            var results = Session
                .CreateQuery("select NHSP.SRID(l.Geometry) from LineStringEntity as l where l.Geometry is not null")
                .List();

            foreach (object item in results)
            {
                int srid = (int) item;
                Assert.AreEqual(4326, srid);
            }
        }

        [Test]
        public virtual void HqlGeometryType()
        {
            var results = Session
                .CreateQuery(
                    "select NHSP.GeometryType(l.Geometry) from LineStringEntity as l where l.Geometry is not null")
                .List();

            foreach (object item in results)
            {
                string gt = (string) item;
                Assert.AreEqual("LINESTRING", gt.ToUpper());
            }

            results = Session
                .CreateQuery("select NHSP.GeometryType(p.Geometry) from PolygonEntity as p where p.Geometry is not null")
                .List();

            foreach (object item in results)
            {
                string gt = (string) item;
                Assert.AreEqual("POLYGON", gt.ToUpper());
            }
        }

        [Test]
        public void HqlEnvelope()
        {
            HqlEnvelope("LineStringEntity");
            HqlEnvelope("PolygonEntity");
        }

        [Test]
        public void HqlIsEmpty()
        {
            var results = Session
                .CreateQuery(
                    "select l.Id, NHSP.IsEmpty(l.Geometry) from LineStringEntity as l where l.Geometry is not null")
                .List();

            var query = SqlIsEmptyLineString(Session);

            foreach (object[] item in results)
            {
                long id = (long) item[0];
                bool isEmpty = (bool) item[1];
                query.SetInt64(0, id);
                bool expected = query.UniqueResult<bool>();
                Assert.AreEqual(expected, isEmpty);
            }
        }

        [Test]
        public void HqlIsSimple()
        {
            var results = Session
                .CreateQuery(
                    "select l.Id, NHSP.IsSimple(l.Geometry) from LineStringEntity as l where l.Geometry is not null")
                .List();

            var query = SqlIsSimpleLineString(Session);

            foreach (object[] item in results)
            {
                long id = (long) item[0];
                bool isSimple = (bool) item[1];
                query.SetInt64(0, id);
                bool expected = query.UniqueResult<bool>();
                Assert.AreEqual(expected, isSimple);
            }
        }

        [Test]
        public virtual void HqlBoundary()
        {
            var results = Session
                .CreateQuery(
                    "select p.Geometry, NHSP.Boundary(p.Geometry) from PolygonEntity as p where p.Geometry is not null")
                .List();
            foreach (object[] item in results)
            {
                var geom = (Geometry) item[0];
                var bound = (Geometry) item[1];
                Assert.IsTrue(geom.Boundary.EqualsTopologically(bound));
            }
        }

        [Test]
        public void HqlAsBinary()
        {
            var results = Session
                .CreateQuery(
                    "select l.Id, NHSP.AsBinary(l.Geometry) from LineStringEntity as l where l.Geometry is not null")
                .List();

            var query = SqlAsBinaryLineString(Session);

            foreach (object[] item in results)
            {
                long id = (long) item[0];
                byte[] wkb = (byte[]) item[1];
                query.SetInt64(0, id);
                byte[] expected = query.UniqueResult<byte[]>();
                Assert.AreEqual(expected, wkb);
            }
        }

        [Test]
        public void HqlDistance()
        {
            var results = Session
                .CreateQuery(
                    @"
					select NHSP.Distance(l.Geometry, ?), l.Geometry
					from LineStringEntity as l
					where l.Geometry is not null")
                .SetParameter(0, Filter, SpatialDialect.GeometryTypeOf(Session))
                .SetMaxResults(100)
                .List();
            foreach (object[] item in results)
            {
                double distance = (double) item[0];
                var geom = (Geometry) item[1];
                Assert.AreEqual(geom.Distance(Filter), distance, 0.003);
            }
        }

        [Test]
        public void HqlDistanceMin()
        {
            const double minDistance = 40000;

            var results = Session
                .CreateQuery(
                    @"
					select NHSP.Distance(l.Geometry, :filter), l.Geometry
					from LineStringEntity as l
					where l.Geometry is not null
					and NHSP.Distance(l.Geometry, :filter) > :minDistance
					order by NHSP.Distance(l.Geometry, :filter)")
                .SetParameter("filter", Filter, SpatialDialect.GeometryTypeOf(Session))
                .SetParameter("minDistance", minDistance)
                .SetMaxResults(100)
                .List();

            Assert.IsNotEmpty(results);
            foreach (object[] item in results)
            {
                double distance = (double) item[0];
                Assert.Greater(distance, minDistance);
                var geom = (Geometry) item[1];
                Assert.AreEqual(geom.Distance(Filter), distance, 0.003);
            }
        }

        [Test]
        public void HqlBuffer()
        {
            const double distance = 10.0;

            var results = Session
                .CreateQuery(
                    "select p.Geometry, NHSP.Buffer(p.Geometry, ?) from PolygonEntity as p where p.Geometry is not null")
                .SetDouble(0, distance)
                .SetMaxResults(100)
                .List();

            int count = 0;
            foreach (object[] item in results)
            {
                var geom = (Geometry) item[0];
                var buffer = (Geometry) item[1];
                var ntsBuffer = geom.Buffer(distance);

                buffer.Normalize();
                ntsBuffer.Normalize();

                if (IsApproximateCoincident(ntsBuffer, buffer, 0.05))
                {
                    count++;
                }
            }
            Assert.Greater(count, 0);
        }

        [Test]
        public void HqlConvexHull()
        {
            var results = Session
                .CreateQuery(
                    "select m.Geometry, NHSP.ConvexHull(m.Geometry) from MultiLineStringEntity as m where m.Geometry is not null")
                .SetMaxResults(100)
                .List();

            int count = 0;
            foreach (object[] item in results)
            {
                var geom = (Geometry) item[0];
                var cvh = (Geometry) item[1];
                var ntsCvh = geom.ConvexHull();

                Assert.IsTrue(cvh.Contains(geom));

                cvh.Normalize();
                ntsCvh.Normalize();

                if (ntsCvh.EqualsExact(cvh, 0.5))
                {
                    count++;
                }
            }
            Assert.Greater(count, 0);
        }

        [Test]
        public void HqlDifference()
        {
            var results = Session
                .CreateQuery(
                    "select e.Geometry, NHSP.Difference(e.Geometry, ?) from PolygonEntity as e where e.Geometry is not null")
                .SetParameter(0, Filter, SpatialDialect.GeometryTypeOf(Session))
                .SetMaxResults(100)
                .List();

            int count = 0;
            foreach (object[] item in results)
            {
                var geom = (Geometry) item[0];
                var diff = (Geometry) item[1];

                // some databases give a null object if the difference is the
                // null-set
                if (diff == null || diff.IsEmpty)
                {
                    continue;
                }

                diff.Normalize();
                var ntsDiff = geom.Difference(Filter);
                ntsDiff.Normalize();

                if (ntsDiff.EqualsExact(diff, 0.5))
                {
                    count++;
                }
            }
            Assert.Greater(count, 0);
        }

        [Test]
        public void HqlIntersection()
        {
            var results = Session
                .CreateQuery(
                    "select e.Geometry, NHSP.Intersection(e.Geometry, ?) from PolygonEntity as e where e.Geometry is not null")
                .SetParameter(0, Filter, SpatialDialect.GeometryTypeOf(Session))
                .SetMaxResults(100)
                .List();

            int count = 0;
            foreach (object[] item in results)
            {
                var geom = (Geometry) item[0];
                var intersect = (Geometry) item[1];

                // some databases give a null object if the difference is the
                // null-set
                if (intersect == null || intersect.IsEmpty)
                {
                    continue;
                }

                intersect.Normalize();
                var ntsIntersect = geom.Intersection(Filter);
                ntsIntersect.Normalize();

                if (ntsIntersect.EqualsExact(intersect, 0.5))
                {
                    count++;
                }
            }
            Assert.Greater(count, 0);
        }

        [Test]
        public void HqlSymDifference()
        {
            var results = Session
                .CreateQuery(
                    "select e.Geometry, NHSP.SymDifference(e.Geometry, ?) from PolygonEntity as e where e.Geometry is not null")
                .SetParameter(0, Filter, SpatialDialect.GeometryTypeOf(Session))
                .SetMaxResults(100)
                .List();

            int count = 0;
            foreach (object[] item in results)
            {
                var geom = (Geometry) item[0];
                var symDiff = (Geometry) item[1];

                // some databases give a null object if the difference is the
                // null-set
                if (symDiff == null || symDiff.IsEmpty)
                {
                    continue;
                }

                symDiff.Normalize();
                var ntsSymDiff = geom.SymmetricDifference(Filter);
                ntsSymDiff.Normalize();

                if (ntsSymDiff.EqualsExact(symDiff, 0.5))
                {
                    count++;
                }
            }
            Assert.Greater(count, 0);
        }

        [Test]
        public void HqlUnion()
        {
            var results = Session
                .CreateQuery(
                    "select e.Geometry, NHSP.Union(e.Geometry, ?) from PolygonEntity as e where e.Geometry is not null")
                .SetParameter(0, Filter, SpatialDialect.GeometryTypeOf(Session))
                .SetMaxResults(100)
                .List();

            int count = 0;
            foreach (object[] item in results)
            {
                var geom = (Geometry) item[0];
                var union = (Geometry) item[1];

                // some databases give a null object if the difference is the
                // null-set
                if (union == null || union.IsEmpty)
                {
                    continue;
                }

                union.Normalize();
                var ntsUnion = geom.Union(Filter);
                ntsUnion.Normalize();

                if (ntsUnion.EqualsExact(union, 0.5))
                {
                    count++;
                }
            }
            Assert.Greater(count, 0);
        }

        protected override void OnTestFixtureSetUp()
        {
            const int srid = 4326;
            DataGenerator.Generate(sessions, srid);

            Filter = Wkt.Read(FilterString);
            Filter.SRID = srid;
        }

        protected override void OnTestFixtureTearDown()
        {
            using (var session = sessions.OpenSession())
            {
                DeleteMappings(session);
                session.Close();
            }
        }

        protected override void OnSetUp()
        {
            Session = sessions.OpenSession();
        }

        protected override void OnTearDown()
        {
            Session.Clear();
            Session.Close();
        }

        protected abstract string SqlLineStringFilter(string filterString);

        protected abstract string SqlPolygonFilter(string filterString);

        protected abstract string SqlMultiLineStringFilter(string filterString);

        protected abstract string SqlOvelapsLineString(string filterString);

        protected abstract string SqlIntersectsLineString(string filterString);

        protected abstract ISQLQuery SqlIsEmptyLineString(ISession session);

        protected abstract ISQLQuery SqlIsSimpleLineString(ISession session);

        protected abstract ISQLQuery SqlAsBinaryLineString(ISession session);

        private static bool IsApproximateCoincident(Geometry g1, Geometry g2, double tolerance)
        {
            Geometry symdiff;
            if (g1.Dimension < Dimension.Surface && g2.Dimension < Dimension.Surface)
            {
                g1 = g1.Buffer(tolerance);
                g2 = g2.Buffer(tolerance);
                symdiff = g1.SymmetricDifference(g2).Buffer(tolerance);
            }
            else
            {
                symdiff = g1.SymmetricDifference(g2);
            }
            double relError = symdiff.Area/(g1.Area + g2.Area);
            return relError < tolerance;
        }

        private void HqlEnvelope(string entityName)
        {
            var results = Session
                .CreateQuery("select NHSP.Envelope(e.Geometry), e.Geometry from "
                             + entityName + " as e where e.Geometry is not null")
                .SetMaxResults(10)
                .List();
            foreach (object[] item in results)
            {
                var env = (Geometry) item[0];
                var g = (Geometry) item[1];
                Assert.IsTrue(g.Envelope.EqualsTopologically(env));
            }
        }
    }
}
