using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.Spatial.Criterion;
using NHibernate.Spatial.Criterion.Lambda;
using NUnit.Framework;
using System;
using System.Collections;
using System.Linq;
using Tests.NHibernate.Spatial.Model;

namespace Tests.NHibernate.Spatial
{
	public abstract class CriteriaFixture : AbstractFixture
	{
		protected override Type[] Mappings
		{
			get
			{
				return new[] {
					typeof(Simple),
					typeof(County)
				};
			}
		}

		private ISession _session;

		protected override void OnSetUp()
		{
			_session = sessions.OpenSession();

			_session.Save(new Simple("a point", new Point(12, 45)));
			_session.Save(new Simple("a null", null));
			_session.Save(new Simple("a collection empty 1", Wkt.Read("GEOMETRYCOLLECTION EMPTY")));
			_session.Save(new Simple("a collection empty 2", GeometryCollection.Empty));

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
		public virtual void IsDirty()
		{
			Assert.IsFalse(_session.IsDirty());

			var simple = _session.CreateCriteria<Simple>()
				.SetMaxResults(1)
				.UniqueResult<Simple>();

			Assert.NotNull(simple);

			// New instance of the same geometry
			var wkt = simple.Geometry.AsText();
			var geometry = new WKTReader().Read(wkt);
			geometry.SRID = simple.Geometry.SRID;

			simple.Geometry = geometry;

			Assert.IsFalse(_session.IsDirty());

			simple.Geometry.SRID = 12345;

			Assert.IsTrue(_session.IsDirty());
		}

		[Test]
		public void RowCount()
		{
			IList results = _session.CreateCriteria(typeof(Simple))
				.SetProjection(Projections.ProjectionList()
					.Add(Projections.RowCount())
					)
				.List();

			Assert.AreEqual(4, (int)(results[0]));
		}

		[Test]
		public void CountNullOrSpatialEmpty()
		{
			IList results = _session.CreateCriteria(typeof(Simple))
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
			IList results = _session.CreateCriteria(typeof(Simple))
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
			bool x = _session.Query<Simple>().Select(s => s.Geometry.IsEmpty).First();

			IList results =
				_session.Query<Simple>()
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
		public virtual void CountNull()
		{
			IList results = _session.CreateCriteria(typeof(Simple))
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
			IList results = _session.CreateCriteria(typeof(Simple))
				.Add(Restrictions.IsEmpty("Geometry"))
				.List();
			Assert.AreEqual(0, results.Count);
		}

		[Test]
		public virtual void CountSpatialEmpty()
		{
			IList results = _session.CreateCriteria(typeof(Simple))
				.Add(SpatialRestrictions.IsEmpty("Geometry"))
				.List();
			Assert.AreEqual(2, results.Count);
			foreach (Simple item in results)
			{
				Assert.IsTrue(item.Geometry.IsEmpty);
			}
		}

		/// <summary>
		/// Gets the County whose boundaries contains the supplied point (Criteria version)
		/// </summary>
		[Test]
		public void GeometryContains()
		{
			var point = new Point(1.5, 1.5) { SRID = 32719 };

			var county = (County)_session.CreateCriteria(typeof(County))
									  .Add(SpatialRestrictions.Contains("Boundaries", point))
									  .UniqueResult();

			Assert.AreEqual("bbbb", county.Name);
		}

		/// <summary>
		/// Gets the County whose boundaries contains the supplied point (QueryOver version)
		/// </summary>
		[Test]
		public void GeometryContainsQueryOver()
		{
			var point = new Point(1.5, 1.5) { SRID = 32719 };

			var county = _session
				.QueryOver<County>()
				.WhereSpatialRestrictionOn(p => p.Boundaries).Contains(point)
				.SingleOrDefault();

			Assert.AreEqual("bbbb", county.Name);
		}

		/// <summary>
		/// Gets the counties whose boundaries are fully contained inside the supplied polygon (QueryOver version)
		/// </summary>
		[Test]
		public void GeometryWithinQueryOver()
		{
			var boundary = Wkt.Read("POLYGON((-1.0 -1.0, -1.0 3.0, 4.0 3.0, 4.0 -1.0, -1.0 -1.0))");
			boundary.SRID = 32719;

			var results = _session
				.QueryOver<County>()
				.WhereSpatialRestrictionOn(p => p.Boundaries).Within(boundary)
				.List();

			Assert.AreEqual(4, results.Count);
		}
	}
}