using System;
using Oracle.DataAccess.Types;

namespace NHibernate.Spatial.Oracle
{
	public enum ElementType
	{
		UNSUPPORTED = 0,
		POINT = 1,
		ORIENTATION = 2,
		POINT_CLUSTER = 3,
		LINE_STRAITH_SEGMENTS = 4,
		LINE_ARC_SEGMENTS = 5,
		INTERIOR_RING_STRAIGHT_SEGMENTS = 6,
		EXTERIOR_RING_STRAIGHT_SEGMENTS = 7,
		INTERIOR_RING_ARC_SEGMENTS = 8,
		EXTERIOR_RING_ARC_SEGMENTS = 9,
		INTERIOR_RING_RECT = 10,
		EXTERIOR_RING_RECT = 11,
		INTERIOR_RING_CIRCLE = 12,
		EXTERIOR_RING_CIRCLE = 13,
		COMPOUND_LINE = 14,
		COMPOUND_EXTERIOR_RING = 15,
		COMPOUND_INTERIOR_RING = 16
	}

	public static class SdoGeometryTypes
	{

		//Oracle Documentation for SDO_ETYPE - SIMPLE

		//Point//Line//Polygon//exterior counterclockwise - polygon ring = 1003//interior clockwise  polygon ring = 2003

		public enum ETYPE_SIMPLE
		{
			POINT = 1, 
			LINE = 2, 
			POLYGON = 3, 
			POLYGON_EXTERIOR = 1003, 
			POLYGON_INTERIOR = 2003
		}

		//Oracle Documentation for SDO_ETYPE - COMPOUND

		//1005: exterior polygon ring (must be specified in counterclockwise order)

		//2005: interior polygon ring (must be specified in clockwise order)

		public enum ETYPE_COMPOUND
		{
			FOURDIGIT = 4, 
			POLYGON_EXTERIOR = 1005, 
			POLYGON_INTERIOR = 2005
		}

		//Oracle Documentation for SDO_GTYPE.

		//This represents the last two digits in a GTYPE, where the first item is dimension(ality) and the second is LRS

		public enum GTYPE
		{
			UNKNOWN_GEOMETRY = 00, 
			POINT = 01, 
			LINE = 02, 
			CURVE = 02, 
			POLYGON = 03, 
			COLLECTION = 04, 
			MULTIPOINT = 05, 
			MULTILINE = 06, 
			MULTICURVE = 06, 
			MULTIPOLYGON = 07
		}

		public enum DIMENSION
		{
			DIM2D = 2, 
			DIM3D = 3, 
			LRS_DIM3 = 3, 
			LRS_DIM4 = 4
		}

	}

	[OracleCustomTypeMappingAttribute("MDSYS.SDO_GEOMETRY")]
	public class SdoGeometry : OracleCustomTypeBase<SdoGeometry>
	{

		private enum OracleObjectColumns { SDO_GTYPE, SDO_SRID, SDO_POINT, SDO_ELEM_INFO, SDO_ORDINATES }

		private decimal? sdo_Gtype;

		[OracleObjectMappingAttribute(0)]
		public decimal? Sdo_Gtype
		{
			get { return sdo_Gtype; }
			set { sdo_Gtype = value; }
		}

		private decimal? sdo_Srid;

		[OracleObjectMappingAttribute(1)]
		public decimal? Sdo_Srid
		{
			get { return sdo_Srid; }
			set { sdo_Srid = value; }
		}

		private SdoPoint point;

		[OracleObjectMappingAttribute(2)]
		public SdoPoint Point
		{
			get { return point; }
			set { point = value; }
		}

		private decimal[] elemArray;

		[OracleObjectMappingAttribute(3)]
		public decimal[] ElemArray
		{
			get { return elemArray; }
			set { elemArray = value; }
		}

		private decimal[] ordinatesArray;

		[OracleObjectMappingAttribute(4)]
		public decimal[] OrdinatesArray
		{
			get { return ordinatesArray; }
			set { ordinatesArray = value; }
		}

		[OracleCustomTypeMappingAttribute("MDSYS.SDO_ELEM_INFO_ARRAY")]
		public class ElemArrayFactory : OracleArrayTypeFactoryBase<decimal> {}

		[OracleCustomTypeMappingAttribute("MDSYS.SDO_ORDINATE_ARRAY")]
		public class OrdinatesArrayFactory : OracleArrayTypeFactoryBase<decimal> {}

		public override void MapFromCustomObject()
		{
			SetValue((int)OracleObjectColumns.SDO_GTYPE, Sdo_Gtype);
			SetValue((int)OracleObjectColumns.SDO_SRID, Sdo_Srid);
			SetValue((int)OracleObjectColumns.SDO_POINT, Point);
			SetValue((int)OracleObjectColumns.SDO_ELEM_INFO, ElemArray);
			SetValue((int)OracleObjectColumns.SDO_ORDINATES, OrdinatesArray);
		}

		public override void MapToCustomObject()
		{
			Sdo_Gtype = GetValue<decimal?>((int)OracleObjectColumns.SDO_GTYPE);
			Sdo_Srid = GetValue<decimal?>((int)OracleObjectColumns.SDO_SRID);
			Point = GetValue<SdoPoint>((int)OracleObjectColumns.SDO_POINT);
			ElemArray = GetValue<decimal[]>((int)OracleObjectColumns.SDO_ELEM_INFO);
			OrdinatesArray = GetValue<decimal[]>((int)OracleObjectColumns.SDO_ORDINATES);
		}

		public int[] ElemArrayOfInts
		{
			get
			{
				return Array.ConvertAll<decimal, int>(
					this.elemArray,
					delegate(decimal x) { return Convert.ToInt32(x); });
			}
			set
			{
				if (value != null)
				{
					this.elemArray = Array.ConvertAll<int, decimal>(
						value,
						delegate(int x) { return Convert.ToDecimal(x); });
				}
				else
				{
					this.elemArray = null;
				}
			}
		}

		public double?[] OrdinatesArrayOfDoubles
		{
			get
			{
				return Array.ConvertAll<decimal, double?>(
					this.ordinatesArray,
					delegate(decimal x) { return (double?)Convert.ToDouble(x); });
			}
			set
			{
				if (value != null)
				{
					this.ordinatesArray = Array.ConvertAll<double?, decimal>(
						value,
						delegate(double? x) { return Convert.ToDecimal(x); });
				}
				else
				{
					this.ordinatesArray = null;
				}
			}
		}

		private int _Dimensionality;
		public int Dimensionality
		{
			get { return _Dimensionality; }
			set { _Dimensionality = value; }
		}

		private int _LRS;
		public int LRS
		{
			get { return _LRS; }
			set { _LRS = value; }
		}

		private int _GeometryType;
		public int GeometryType
		{
			get { return _GeometryType; }
			set { _GeometryType = value; }
		}

		public int PropertiesFromGTYPE()
		{
			if ((int)this.sdo_Gtype != 0)
			{
				int v = (int)this.sdo_Gtype;
				int dim = v / 1000;
				Dimensionality = dim;
				v -= dim * 1000;
				int lrsDim = v / 100;
				LRS = lrsDim;
				v -= lrsDim * 100;
				GeometryType = v;
				return (Dimensionality * 1000) + (LRS * 100) + GeometryType;
			}
			return 0;
		}

		public int PropertiesToGTYPE()
		{
			int v = Dimensionality * 1000;
			v = v + (LRS * 100);
			v = v + GeometryType;
			this.sdo_Gtype = System.Convert.ToDecimal(v);
			return v;
		}

		#region Ported from Hibernate Spatial SDOGeometryType.java

		public void addElement(decimal[] element)
		{
			decimal[] newTriplets = new decimal[this.elemArray.Length + element.Length];
			Array.Copy(this.elemArray, 0, newTriplets, 0, this.elemArray.Length);
			Array.Copy(element, 0, newTriplets, this.elemArray.Length, element.Length);
			this.elemArray = newTriplets;
		}

		//public void addOrdinates(double?[] ordinatesToAdd)
		//{
		//    decimal[] newOrdinates = new decimal[this.ordinatesArray.Length + ordinatesToAdd.Length];
		//    Array.Copy(this.ordinatesArray, 0, newOrdinates, 0, this.ordinatesArray.Length);
		//    Array.Copy(ordinatesToAdd, 0, newOrdinates, this.ordinatesArray.Length, ordinatesToAdd.Length);
		//    this.ordinatesArray = newOrdinates;
		//}

		//private void addOrdinates(decimal[] ordinatesToAdd)
		//{
		//    decimal[] newOrdinates = new decimal[this.ordinatesArray.Length + ordinatesToAdd.Length];
		//    Array.Copy(this.ordinatesArray, 0, newOrdinates, 0, this.ordinatesArray.Length);
		//    Array.Copy(ordinatesToAdd, 0, newOrdinates, this.ordinatesArray.Length, ordinatesToAdd.Length);
		//    this.ordinatesArray = newOrdinates;
		//}

		public void AddOrdinates<T>(T[] ordinatesToAdd)
		{
			decimal[] newOrdinates = new decimal[this.ordinatesArray.Length + ordinatesToAdd.Length];
			Array.Copy(this.ordinatesArray, 0, newOrdinates, 0, this.ordinatesArray.Length);
			Array.Copy(ordinatesToAdd, 0, newOrdinates, this.ordinatesArray.Length, ordinatesToAdd.Length);
			this.ordinatesArray = newOrdinates;
		}

		/**
		* This joins an array of SDO_GEOMETRIES to a SDO_GEOMETRY of type
		* COLLECTION
		* 
		* @param sdoElements
		* @return
		*/
		public static SdoGeometry Join(SdoGeometry[] sdoElements)
		{
			SdoGeometry sdoCollection = new SdoGeometry();
			sdoCollection.Sdo_Gtype = (decimal)SdoGeometryTypes.GTYPE.COLLECTION;
			if (sdoElements == null || sdoElements.Length == 0)
			{
				sdoCollection.Dimensionality = 2;
				sdoCollection.LRS = 0;
			}
			else
			{
				SdoGeometry firstElement = sdoElements[0];
				int dim = firstElement.Dimensionality;
				int lrsDim = firstElement.LRS;
				sdoCollection.Dimensionality = dim;
				sdoCollection.LRS = lrsDim;
				int ordinatesOffset = 1;
				for (int i = 0; i < sdoElements.Length; i++)
				{
					decimal[] element = sdoElements[i].ElemArray;
					decimal[] ordinates = sdoElements[i].OrdinatesArray;
					if (element != null && element.Length > 0)
					{
						//int shift = ordinatesOffset - element.getOrdinatesOffset(0);
						int shift = ordinatesOffset - (int)element[0];
						ShiftOrdinateOffset(element, shift);
						sdoCollection.addElement(element);
						sdoCollection.AddOrdinates(ordinates);
						ordinatesOffset += ordinates.Length;
					}
				}
			}
			return sdoCollection;
		}

		private static void ShiftOrdinateOffset(decimal[] elemInfo, int offset)
		{
			for (int i = 0; i < elemInfo.Length; i++)
			{
				//int newOffset = elemInfo.getOrdinatesOffset(i) + offset;
				//elemInfo.setOrdinatesOffset(i, newOffset);
				elemInfo[i*3] = elemInfo[i*3] + offset;
			}
		}

		#endregion

	}
}