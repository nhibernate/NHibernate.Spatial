// Copyright 2008 - Ricardo Stuven (rstuven@gmail.com)
//
// This file is part of NHibernate.Spatial.
// NHibernate.Spatial is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// NHibernate.Spatial is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with NHibernate.Spatial; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using GeoAPI.Geometries;
using GisSharpBlog.NetTopologySuite.Algorithm;
using GisSharpBlog.NetTopologySuite.Geometries;
using NHibernate.Spatial.MGeometries;

namespace NHibernate.Spatial.Oracle
{
	internal class OracleGeometryWriter
	{
		//private readonly SqlGeometryBuilder _builder = new SqlGeometryBuilder();

		public SdoGeometry Write(IGeometry geometry)
		{
			// TODO:
			//_builder.SetSrid(geometry.SRID);
			//WriteGeometry(geometry);
			//return _builder.ConstructedGeometry;
			return null;
		}

		private SdoGeometry WriteGeometry(IGeometry geometry)
		{
			if (geometry is IPoint)
			{
				return WritePoint(geometry);
			}
			else if (geometry is ILineString)
			{
				return WriteLineString(geometry);
			}
			else if (geometry is IPolygon)
			{
				return WritePolygon(geometry);
			}
			else if (geometry is IMultiPoint)
			{
				return WriteMultiPoint(geometry);
			}
			else if (geometry is IMultiLineString)
			{
				return WriteMultiLineString(geometry);
			}
			else if (geometry is IMultiPolygon)
			{
				return WriteMultiPolygon(geometry);
			}
			else if (geometry is IGeometryCollection)
			{
				return WriteGeometryCollection(geometry);
			}
			return null;
		}

		private SdoGeometry WritePoint(IGeometry geometry)
		{
			int dim = GetCoordDimension(geometry);
			int lrsDim = GetCoordinateLrsPosition(geometry);
			bool isLrs = (lrsDim != 0);
			double?[] coord = ConvertCoordinates(geometry.Coordinates, dim, isLrs);
			SdoGeometry sdoGeometry = new SdoGeometry();
			sdoGeometry.Sdo_Gtype = (decimal)SdoGeometryTypes.GTYPE.POINT;
			sdoGeometry.Dimensionality = dim;
			sdoGeometry.LRS = lrsDim;
			sdoGeometry.Sdo_Srid = geometry.SRID;
			//sdoGeometry.Point = new SdoPoint();
			//sdoGeometry.Point.X = 0;
			//sdoGeometry.Point.Y = 0;
			//sdoGeometry.Point.Z = 0;
			sdoGeometry.ElemArray = new decimal[] { 1, (decimal)SdoGeometryTypes.ETYPE_SIMPLE.POINT, 1 };
			sdoGeometry.OrdinatesArrayOfDoubles = coord;
			return sdoGeometry;
		}

		private SdoGeometry WriteLineString(IGeometry geometry)
		{
			int dim = GetCoordDimension(geometry);
			int lrsPos = GetCoordinateLrsPosition(geometry);
			bool isLrs = lrsPos > 0;
			double?[] ordinates = ConvertCoordinates(geometry.Coordinates, dim, isLrs);
			SdoGeometry sdoGeometry = new SdoGeometry();
			sdoGeometry.Sdo_Gtype = (decimal)SdoGeometryTypes.GTYPE.LINE;
			sdoGeometry.Dimensionality = dim;
			sdoGeometry.LRS = lrsPos;
			sdoGeometry.Sdo_Srid = geometry.SRID;
			sdoGeometry.ElemArray = new decimal[] { 0, 1, (decimal)ElementType.LINE_STRAITH_SEGMENTS, 0 };
			sdoGeometry.OrdinatesArrayOfDoubles = ordinates;
			return sdoGeometry;
		}

		private SdoGeometry WriteMultiLineString(IGeometry geometry)
		{
			int dim = GetCoordDimension(geometry);
			int lrsDim = GetCoordinateLrsPosition(geometry);
			bool isLrs = (lrsDim != 0);
			SdoGeometry sdoGeometry = new SdoGeometry();
			sdoGeometry.Sdo_Gtype = (decimal)SdoGeometryTypes.GTYPE.MULTILINE;
			sdoGeometry.Dimensionality = dim;
			sdoGeometry.LRS = lrsDim;
			sdoGeometry.Sdo_Srid = geometry.SRID;
			decimal[] info = new decimal[geometry.NumGeometries * 3];
			int oordinatesOffset = 1;
			double?[] ordinates = new double?[] { };
			for (int i = 0; i < geometry.NumGeometries; i++)
			{
				info[i + 0] = oordinatesOffset;
				info[i + 1] = (decimal)ElementType.LINE_STRAITH_SEGMENTS;
				info[i + 2] = 0;
				ordinates = ConvertAddCoordinates(ordinates, geometry.GetGeometryN(i).Coordinates, dim, isLrs);
				oordinatesOffset = ordinates.Length + 1;
			}
			sdoGeometry.ElemArray = info;
			sdoGeometry.OrdinatesArrayOfDoubles = ordinates;
			return sdoGeometry;
		}

		private SdoGeometry WriteMultiPoint(IGeometry geometry)
		{
			int dim = GetCoordDimension(geometry);
			int lrsDim = GetCoordinateLrsPosition(geometry);
			bool isLrs = (lrsDim != 0);
			SdoGeometry sdoGeometry = new SdoGeometry();
			sdoGeometry.Sdo_Gtype = (decimal)SdoGeometryTypes.GTYPE.MULTIPOINT;
			sdoGeometry.Dimensionality = dim;
			sdoGeometry.LRS = lrsDim;
			sdoGeometry.Sdo_Srid = geometry.SRID;

			decimal[] info = new decimal[geometry.NumPoints * 3];
			int oordinatesOffset = 1;
			double?[] ordinates = new double?[0];
			for (int i = 0; i < geometry.NumPoints; i++)
			{
				//info.setElement(i, oordinatesOffset, ElementType.POINT, 0);
				info[i + 0] = oordinatesOffset;
				info[i + 1] = (decimal)ElementType.POINT;
				info[i + 2] = 0;
				ordinates = ConvertAddCoordinates(ordinates, geometry.GetGeometryN(i).Coordinates, dim, isLrs);
				oordinatesOffset = ordinates.Length + 1;
			}
			sdoGeometry.ElemArray = info;
			sdoGeometry.OrdinatesArrayOfDoubles = ordinates;
			return sdoGeometry;
		}

		private SdoGeometry WritePolygon(IGeometry geometry)
		{
			int dim = GetCoordDimension(geometry);
			int lrsPos = GetCoordinateLrsPosition(geometry);
			SdoGeometry sdoGeometry = new SdoGeometry();
			sdoGeometry.Sdo_Gtype = (decimal)SdoGeometryTypes.GTYPE.POLYGON;
			sdoGeometry.Dimensionality = dim;
			sdoGeometry.LRS = lrsPos;
			sdoGeometry.Sdo_Srid = geometry.SRID;
			AddPolygon(sdoGeometry, geometry as IPolygon);
			return sdoGeometry;
		}

		private SdoGeometry WriteMultiPolygon(IGeometry geometry)
		{
			int dim = GetCoordDimension(geometry);
			int lrsPos = GetCoordinateLrsPosition(geometry);
			SdoGeometry sdoGeometry = new SdoGeometry();
			sdoGeometry.Sdo_Gtype = (decimal)SdoGeometryTypes.GTYPE.MULTIPOLYGON;
			sdoGeometry.Dimensionality = dim;
			sdoGeometry.LRS = lrsPos;
			sdoGeometry.Sdo_Srid = geometry.SRID;
			for (int i = 0; i < geometry.NumGeometries; i++)
			{
				try
				{
					IPolygon pg = (IPolygon)geometry.GetGeometryN(i);
					AddPolygon(sdoGeometry, pg);
				}
				catch (Exception e)
				{
					throw new ApplicationException(
						"Found geometry that was not a geometry in MultiPolygon");
				}
			}
			return sdoGeometry;
		}

		private SdoGeometry WriteGeometryCollection(IGeometry geometry)
		{
			SdoGeometry[] sdoElements = new SdoGeometry[geometry.NumGeometries];
			for (int i = 0; i < geometry.NumGeometries; i++)
			{
				IGeometry sdoGeometry = geometry.GetGeometryN(i);
				sdoElements[i] = WriteGeometry(sdoGeometry);
			}
			return SdoGeometry.Join(sdoElements);
		}

		private void AddPolygon(SdoGeometry sdoGeometry, IPolygon polygon)
		{
			int numInteriorRings = polygon.NumInteriorRings;
			decimal[] info = new decimal[(numInteriorRings + 1) * 3];
			int ordinatesOffset = 1;
			if (sdoGeometry.OrdinatesArray != null)
			{
				ordinatesOffset = sdoGeometry.OrdinatesArray.Length + 1;
			}
			double?[] ordinates = new double?[] { };
			for (int i = 0; i < info.Length; i++)
			{
				ElementType et;
				ICoordinate[] coords;
				if (i == 0)
				{
					et = ElementType.EXTERIOR_RING_STRAIGHT_SEGMENTS;
					coords = polygon.ExteriorRing.Coordinates;
					if (!CGAlgorithms.IsCCW(coords))
					{
						coords = ReverseRing(coords);
					}
				}
				else
				{
					et = ElementType.INTERIOR_RING_STRAIGHT_SEGMENTS;
					coords = polygon.InteriorRings[i - 1].Coordinates;
					if (CGAlgorithms.IsCCW(coords))
					{
						coords = ReverseRing(coords);
					}
				}
				//info.setElement(i, ordinatesOffset, et, 0);
				info[i + 0] = ordinatesOffset;
				info[i + 1] = (decimal)et;
				info[i + 2] = 0;
				ordinates = ConvertAddCoordinates(ordinates, coords, sdoGeometry.Dimensionality, sdoGeometry.LRS > 0);
				ordinatesOffset = ordinates.Length + 1;
			}
			sdoGeometry.addElement(info);
			sdoGeometry.AddOrdinates(ordinates);
		}

		// reverses ordinates in a coordinate array in-place
		private ICoordinate[] ReverseRing(ICoordinate[] ar)
		{
			for (int i = 0; i < ar.Length / 2; i++)
			{
				ICoordinate cs = ar[i];
				ar[i] = ar[ar.Length - 1 - i];
				ar[ar.Length - 1 - i] = cs;
			}
			return ar;
		}

		private double?[] ConvertAddCoordinates(double?[] ordinates,
		                                        ICoordinate[] coordinates, int dim, bool isLrs)
		{
			double?[] no = ConvertCoordinates(coordinates, dim, isLrs);
			double?[] newordinates = new double?[ordinates.Length + no.Length];
			Array.Copy(ordinates, 0, newordinates, 0, ordinates.Length);
			Array.Copy(no, 0, newordinates, ordinates.Length, no.Length);
			return newordinates;
		}

		/**
		 * Convert the coordinates to a double array for purposes of persisting them
		 * to the database. Note that Double.NaN values are to be converted to null
		 * values in the array.
		 * 
		 * @param coordinates
		 *            Coordinates to be converted to the array
		 * @param dim
		 *            Coordinate dimension
		 * @param isLrs
		 *            true if the coordinates contain measures
		 * @return
		 */
		private double?[] ConvertCoordinates(ICoordinate[] coordinates, int dim, bool isLrs)
		{
			if (dim > 4)
			{
				throw new ArgumentException("Dim parameter value cannot be greater than 4");
			}
			double?[] converted = new double?[coordinates.Length * dim];
			for (int i = 0; i < coordinates.Length; i++)
			{
				ICoordinate c = coordinates[i];
				MCoordinate cm = c as MCoordinate;

				// set the X and Y values
				converted[i * dim] = ToDouble(c.X);
				converted[i * dim + 1] = ToDouble(c.Y);
				if (dim == 3)
				{
					converted[i * dim + 2] = isLrs ? ToDouble(cm.M) : ToDouble(c.Z);
					converted[i * dim + 2] = ToDouble(c.Z);
				}
				else if (dim == 4)
				{
					converted[i * dim + 2] = ToDouble(c.Z);
					converted[i * dim + 3] = ToDouble(cm.M);
				}
			}
			return converted;
		}

		/**
		 * This method converts a double primitive to a Double wrapper instance, but
		 * treats a Double.NaN value as null.
		 * 
		 * @param d
		 *            the value to be converted
		 * @return A Double instance of d, Null if the parameter is Double.NaN
		 */
		private double? ToDouble(double d)
		{
			return double.IsNaN(d) ? (double?)null : d;
		}

		/**
		 * Returns the lrs measure position for purposes of building the gType for
		 * an oracle geometry. At this point and time, I'll have to assume that the
		 * measure is always put at the end of the ordinate tuple, even though it
		 * technically wouldn't have to. This method bases its decision on whether
		 * the first coordinate has a measure value, as measure are required for the
		 * very first and last measure in a CoordinateSequence. If there is no
		 * measure value, 0 is returned.
		 * 
		 * @param geom
		 *            and instance of the Geometry class from which the lrs position
		 *            is being extracted.
		 * @return the lrs position for the SdoGeometry.SDO_GTYPE
		 */
		private int GetCoordinateLrsPosition(IGeometry geom)
		{
			MCoordinate c = geom.Coordinate as MCoordinate;
			int measurePos = 0;
			if (c != null && !Double.IsNaN(c.M))
			{
				measurePos = (Double.IsNaN(c.Z)) ? 3 : 4;
			}
			return measurePos;
		}

		public SdoGeometry ConstructedGeometry
		{
			get
			{
				//return _builder.ConstructedGeometry;
				return null;
			}
		}

		/**
		 * Return the dimension required for building the gType in the SdoGeometry
		 * object. Has support for LRS type geometries.
		 * 
		 * @param geom
		 *            and instance of the Geometry class from which the dimension is
		 *            being extracted.
		 * @return number of dimensions for purposes of creating the
		 *         SdoGeometry.SDO_GTYPE
		 */
		private int GetCoordDimension(IGeometry geom)
		{
			// This is awkward, I have to create an MCoordinate to discover what the
			// dimension is.
			// This shall be cleaner if MCoordinate.GetOrdinate(int ordinateIndex)
			// is moved to the Coordinate class
			ICoordinate c = geom.Coordinate;
			int d = 0;
			if (c != null)
			{
				if (!Double.IsNaN(c.X))
					d++;
				if (!Double.IsNaN(c.Y))
					d++;
				if (!Double.IsNaN(c.Z))
					d++;
				if (c is MCoordinate && !Double.IsNaN((c as MCoordinate).M))
					d++;
			}
			return d;
		}


 	 
	}
}