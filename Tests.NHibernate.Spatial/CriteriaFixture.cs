using System;
using System.Collections;
using System.Linq;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.Spatial.Criterion;
using NHibernate.Spatial.Linq;
using NUnit.Framework;
using Tests.NHibernate.Spatial.Model;
using Tests.NHibernate.Spatial.OgcSfSql11Compliance.Model;

namespace Tests.NHibernate.Spatial
{
	public abstract class CriteriaFixture : AbstractFixture
	{
		protected override Type[] Mappings
		{
			get
			{
				return new Type[] { 
					typeof(Simple)
				};
			}
		}

		private ISession session;

		protected override void OnSetUp()
		{
			session = sessions.OpenSession();

			session.Save(new Simple("a point", new Point(12, 45)));
			session.Save(new Simple("a null", null));
			session.Save(new Simple("a collection empty 1", Wkt.Read("GEOMETRYCOLLECTION EMPTY")));
			session.Save(new Simple("a collection empty 2", GeometryCollection.Empty));

			session.Flush();
		}

		protected override void OnTearDown()
		{
			DeleteMappings(session);
			session.Close();
		}

		[Test]
		public void IsDirty()
		{
			Assert.IsFalse(session.IsDirty());

			var simple = session.CreateCriteria<Simple>()
				.SetMaxResults(1)
				.UniqueResult<Simple>();

			Assert.NotNull(simple);

			// New instance of the same geometry
			var wkt = simple.Geometry.AsText();
			var geometry = new WKTReader().Read(wkt);
			geometry.SRID = simple.Geometry.SRID;

			simple.Geometry = geometry;

			Assert.IsFalse(session.IsDirty());

			simple.Geometry.SRID = 12345;

			Assert.IsTrue(session.IsDirty());
		}

		[Test]
		public void RowCount()
		{
			IList results = session.CreateCriteria(typeof(Simple))
				.SetProjection(Projections.ProjectionList()
					.Add(Projections.RowCount())
					)
				.List();

			Assert.AreEqual(4, (int)(results[0]));
		}

		[Test]
		public void CountNullOrSpatialEmpty()
		{
			IList results = session.CreateCriteria(typeof (Simple))
				.Add(Restrictions.Or(
					Restrictions.IsNull("Geometry"),
					SpatialRestrictions.IsEmpty("Geometry")
				     	))
				.List();
			Assert.AreEqual(3, results.Count);
			foreach (Simple item in results)
			{
				if (item.Geometry != null)
				{
					Assert.IsTrue(item.Geometry.IsEmpty);
				}
			}
		}

		[Test]
		public void CountNullOrSpatialEmptyLambda()
		{
			IList results = session.CreateCriteria(typeof (Simple))
				.Add(Restrictions.Or(
					Restrictions.On<Simple>(o => o.Geometry).IsNull,
					SpatialRestrictions.On<Simple>(o => o.Geometry).IsEmpty
				))
			    .List();
			Assert.AreEqual(3, results.Count);
			foreach (Simple item in results)
			{
				if (item.Geometry != null)
				{
					Assert.IsTrue(item.Geometry.IsEmpty);
				}
			}
		}

		[Test]
		public void CountNullOrSpatialEmptyLinq()
		{
			bool x = session.Query<Simple>().Select(s => s.Geometry.IsEmpty).First();

			IList results =
				session.Query<Simple>()
					.Where(s => s.Geometry == null || s.Geometry.IsEmpty)
					.ToList();

			Assert.AreEqual(3, results.Count);
			foreach (Simple item in results)
			{
				if (item.Geometry != null)
				{
					Assert.IsTrue(item.Geometry.IsEmpty);
				}
			}
		}

		[Test]
		public void CountNull()
		{
			IList results = session.CreateCriteria(typeof(Simple))
				.Add(Restrictions.IsNull("Geometry"))
				.List();
			Assert.AreEqual(1, results.Count);
			foreach (Simple item in results)
			{
				Assert.IsNull(item.Geometry);
			}
		}

		[Test]
		[ExpectedException(typeof(MappingException))]
		public void CountEmpty()
		{
			IList results = session.CreateCriteria(typeof(Simple))
				.Add(Restrictions.IsEmpty("Geometry"))
				.List();
			Assert.AreEqual(0, results.Count);
		}

		[Test]
		public void CountSpatialEmpty()
		{
			IList results = session.CreateCriteria(typeof(Simple))
				.Add(SpatialRestrictions.IsEmpty("Geometry"))
				.List();
			Assert.AreEqual(2, results.Count);
			foreach (Simple item in results)
			{
				Assert.IsTrue(item.Geometry.IsEmpty);
			}
		}

		[Test]
		public void CountSpatialEmpty_()
		{
            IGeometry geom = new Point(10, 10).Buffer(100);
		    ICriteria crit = session.CreateCriteria(typeof (Simple))
                .Add(SpatialRestrictions.Intersects("Geometry", geom));
            var result = crit.List<Simple>();
		}
	}
}
