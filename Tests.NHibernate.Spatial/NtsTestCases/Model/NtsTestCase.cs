using System;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;

namespace Tests.NHibernate.Spatial.NtsTestCases.Model
{
	[Serializable]
	public class NtsTestCase
	{
		private long id;
		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}

		private string description;
		public virtual string Description
		{
			get { return description; }
			set { description = value; }
		}

		private IGeometry geometryA = GeometryCollection.Empty;
		public virtual IGeometry GeometryA
		{
			get { return geometryA; }
			set { geometryA = value; }
		}

		private IGeometry geometryB = GeometryCollection.Empty;
		public virtual IGeometry GeometryB
		{
			get { return geometryB; }
			set { geometryB = value; }
		}

		private string operation;
		public virtual string Operation
		{
			get { return operation; }
			set { operation = value; }
		}

		private string relatePattern;
		public virtual string RelatePattern
		{
			get { return relatePattern; }
			set { relatePattern = value; }
		}

		private IGeometry geometryResult = GeometryCollection.Empty;
		public virtual IGeometry GeometryResult
		{
			get { return geometryResult; }
			set { geometryResult = value; }
		}

		private bool booleanResult;
		public virtual bool BooleanResult
		{
			get { return booleanResult; }
			set { booleanResult = value; }
		}
	}
}
