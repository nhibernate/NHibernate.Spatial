using NetTopologySuite.Geometries;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.Spatial.Criterion;
using NHibernate.Spatial.Dialect;
using NUnit.Framework;
using System;
using System.Collections;
using System.Linq;
using Tests.NHibernate.Spatial.Model;

namespace Tests.NHibernate.Spatial
{
    public abstract class ProjectionsFixture : AbstractFixture
    {
        private ISession _session;

        protected override Type[] Mappings
        {
            get
            {
                return new[]
				           {
				               typeof(County),
				               typeof(Simple)
				           };
            }
        }

        protected virtual Type GeometryType
        {
            get { return GeometryType; }
        }

        protected override void OnSetUp()
        {
            _session = sessions.OpenSession();

            _session.Save(new Simple("point 1", new Point(2.5, 1.5)));
            _session.Save(new Simple("point 2", new Point(0.5, 0.5)));
            _session.Save(new Simple("point 3", new Point(0.5, 2.5)));
            _session.Save(new Simple("point 4", new Point(1.5, 1.5)));

            _session.Save(new County("aaaa", "AA", Wkt.Read("POLYGON((1 0, 2 0, 2 1, 1 1, 1 0))")));
            _session.Save(new County("bbbb", "BB", Wkt.Read("POLYGON((1 1, 2 1, 2 2, 1 2, 1 1))")));
            _session.Save(new County("cccc", "BB", Wkt.Read("POLYGON((2 1, 3 1, 3 2, 2 2, 2 1))")));
            _session.Save(new County("dddd", "AA", Wkt.Read("POLYGON((2 0, 3 0, 3 1, 2 1, 2 0))")));
            _session.Flush();
        }

        protected override void OnTearDown()
        {
            DeleteMappings(_session);
            _session.Close();
        }

        [Test]
        public virtual void CountAndUnion()
        {
            IList results = _session.CreateCriteria(typeof(County))
                .SetProjection(Projections.ProjectionList()
                    .Add(Projections.RowCount())
                    .Add(SpatialProjections.Union("Boundaries"))
                    )
                .List();

            Assert.AreEqual(1, results.Count);

            object[] result = (object[])results[0];

            Geometry expected = Wkt.Read("POLYGON((1 0, 1 1, 1 2, 2 2, 3 2, 3 1, 3 0, 2 0, 1 0))");
            Geometry aggregated = (Geometry)result[1];

            Assert.AreEqual(4, result[0]);
            Assert.IsTrue(expected.EqualsTopologically(aggregated));
        }

        [Test]
        public virtual void CountAndUnionByState()
        {
            IList results = _session.CreateCriteria(typeof(County))
                .AddOrder(Order.Asc("State"))
                .SetProjection(Projections.ProjectionList()
                    .Add(Projections.GroupProperty("State"))
                    .Add(Projections.RowCount())
                    .Add(SpatialProjections.Union("Boundaries"))
                    )
                .List();

            CountAndUnionByState(results);
        }

        [Test]
        public virtual void CountAndUnionByStateLambda()
        {
            var results = _session.QueryOver<County>()
                .Select(
                    Projections.ProjectionList()
                        .Add(Projections.Group<County>(o => o.State))
                        .Add(Projections.RowCount())
                        .Add(SpatialProjections.Union<County>(o => o.Boundaries)))
                .OrderBy(o => o.State).Asc
                .List<object[]>();

            CountAndUnionByState((IList)results);
        }

        private static void CountAndUnionByState(IList results)
        {
            Assert.AreEqual(2, results.Count);

            object[] resultAA = (object[])results[0];
            object[] resultBB = (object[])results[1];

            int countAA = (int)resultAA[1];
            int countBB = (int)resultBB[1];
            Geometry aggregatedAA = (Geometry)resultAA[2];
            Geometry aggregatedBB = (Geometry)resultBB[2];

            Geometry expectedAA = Wkt.Read("POLYGON((1 0, 1 1, 3 1, 3 0, 1 0))");
            Geometry expectedBB = Wkt.Read("POLYGON((1 1, 1 2, 3 2, 3 1, 1 1))");

            Assert.AreEqual(2, countAA);
            Assert.AreEqual(2, countBB);
            Assert.IsTrue(expectedAA.EqualsTopologically(aggregatedAA));
            Assert.IsTrue(expectedBB.EqualsTopologically(aggregatedBB));
        }

        [Test]
        public virtual void EnvelopeAll()
        {
            IList results = _session.CreateCriteria(typeof(County))
                .SetProjection(SpatialProjections.Envelope("Boundaries"))
                .List();

            Assert.AreEqual(1, results.Count);

            var aggregated = (Geometry)results[0];
            var expected = new Envelope(1, 3, 0, 2);

            Assert.IsTrue(expected.Equals(aggregated.EnvelopeInternal));
        }

        [Test]
        public virtual void CollectAll()
        {
            IList results = _session.CreateCriteria(typeof(County))
                .SetProjection(SpatialProjections.Collect("Boundaries"))
                .List();

            Assert.AreEqual(1, results.Count);

            var aggregated = (Geometry)results[0];

            Assert.AreEqual(4, aggregated.NumGeometries);
            //Assert.AreEqual("GEOMETRYCOLLECTION", aggregated.GeometryType);
        }

        [Test]
        public virtual void IntersectionAll()
        {
            IList results = _session.CreateCriteria(typeof(County))
                .SetProjection(SpatialProjections.Intersection("Boundaries"))
                .List();

            Assert.AreEqual(1, results.Count);

            Geometry aggregated = (Geometry)results[0];
            Geometry expected = new Point(2, 1);

            Assert.IsTrue(expected.EqualsTopologically(aggregated));
        }

        /// <summary>
        /// Gets all the points sorted by the distance to a certain point
        /// </summary>
        [Test]
        public void OrderByDistanceHql()
        {
            var point = new Point(0.0, 0.0) { SRID = 4326 };

            var result = _session.CreateQuery(
                @"select p from Simple p
                  order by NHSP.Distance(p.Geometry, :point)")
                .SetParameter("point", point, SpatialDialect.GeometryTypeOf(_session))
                .List<Simple>();

            Assert.That(result[0].Description, Is.EqualTo("point 2"));
            Assert.That(result[1].Description, Is.EqualTo("point 4"));
            Assert.That(result[2].Description, Is.EqualTo("point 3"));
            Assert.That(result[3].Description, Is.EqualTo("point 1"));
        }

        [Test]
        public void OrderByDistanceQueryOver()
        {
            Geometry point = new Point(0.0, 0.0) { SRID = 4326 };

            Simple simple = null;

            var result = _session.QueryOver(() => simple)
                .OrderBy(SpatialProjections.Distance<Simple>(s => s.Geometry, point)).Asc
                .List();

            Assert.That(result[0].Description, Is.EqualTo("point 2"));
            Assert.That(result[1].Description, Is.EqualTo("point 4"));
            Assert.That(result[2].Description, Is.EqualTo("point 3"));
            Assert.That(result[3].Description, Is.EqualTo("point 1"));
        }

        [Test]
        public void OrderByDistanceLinq()
        {
            Geometry point = new Point(0.0, 0.0) { SRID = 4326 };

            var result = _session.Query<Simple>()
                .OrderBy(s => s.Geometry.Distance(point.MappedAs(SpatialDialect.GeometryTypeOf(_session))))
                .ToList();

            Assert.That(result[0].Description, Is.EqualTo("point 2"));
            Assert.That(result[1].Description, Is.EqualTo("point 4"));
            Assert.That(result[2].Description, Is.EqualTo("point 3"));
            Assert.That(result[3].Description, Is.EqualTo("point 1"));
        }
    }
}