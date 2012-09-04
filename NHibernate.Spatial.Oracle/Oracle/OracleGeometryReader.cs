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
using System.Collections.Generic;
using System.IO;
using System.Text;
using GeoAPI.Geometries;
using GisSharpBlog.NetTopologySuite.Geometries;
using NHibernate.Spatial.MGeometries;

namespace NHibernate.Spatial.Oracle
{
	internal class OracleGeometryReader
	{
		private readonly IGeometryFactory factory;

		/// <summary>
		/// Initialize reader with a standard <c>GeometryFactory</c>. 
		/// </summary>
		public OracleGeometryReader() : this(new GeometryFactory()) { }

		/// <summary>
		/// Initialize reader with the given <c>GeometryFactory</c>.
		/// </summary>
		/// <param name="factory"></param>
		public OracleGeometryReader(IGeometryFactory factory)
		{
			this.factory = factory;
		}

		/// <summary>
		/// <c>Geometry</c> builder.
		/// </summary>
		protected virtual IGeometryFactory Factory
		{
			get { return factory; }
		}

		public IGeometry Read(SdoGeometry geometry)
		{
			return ReadGeometry(geometry);
		}

		private IGeometry ReadGeometry(SdoGeometry sdoGeom)
		{
			int dim = sdoGeom.Dimensionality;
			int lrsDim = sdoGeom.LRS;

			if (sdoGeom.Sdo_Gtype.Value == (decimal)SdoGeometryTypes.GTYPE.POINT)
			{
				return ReadPoint(sdoGeom);
			}
			if (sdoGeom.Sdo_Gtype.Value == (decimal)SdoGeometryTypes.GTYPE.LINE)
			{
				return ReadLine(dim, lrsDim, sdoGeom);
			}
			if (sdoGeom.Sdo_Gtype.Value == (decimal)SdoGeometryTypes.GTYPE.POLYGON)
			{
				return ReadPolygon(dim, lrsDim, sdoGeom);
			}
			if (sdoGeom.Sdo_Gtype.Value == (decimal)SdoGeometryTypes.GTYPE.MULTIPOINT)
			{
				return ReadMultiPoint(dim, lrsDim, sdoGeom);
			}
			if (sdoGeom.Sdo_Gtype.Value == (decimal)SdoGeometryTypes.GTYPE.MULTILINE)
			{
				return ReadMultiLine(dim, lrsDim, sdoGeom);
			}
			if (sdoGeom.Sdo_Gtype.Value == (decimal)SdoGeometryTypes.GTYPE.MULTIPOLYGON)
			{
				return ReadMultiPolygon(dim, lrsDim, sdoGeom);
			}
			if (sdoGeom.Sdo_Gtype.Value == (decimal)SdoGeometryTypes.GTYPE.COLLECTION)
			{
				return ReadGeometryCollection(dim, lrsDim, sdoGeom);
			}

			throw new ArgumentException("Type not supported: " + sdoGeom.Sdo_Gtype);
		}

		private IGeometry ReadGeometryCollection(int dim, int lrsDim, SdoGeometry sdoGeom)
		{
			List<IGeometry> geometries = new List<IGeometry>();
			foreach (SdoGeometry elemGeom in sdoGeom.getElementGeometries())
			{
				geometries.Add(ReadGeometry(elemGeom));
			}
			return factory.CreateGeometryCollection(geometries.ToArray());
		}

		private IPoint ReadPoint(SdoGeometry sdoGeom) {
			decimal[] ordinates = sdoGeom.OrdinatesArray;
			if (ordinates.Length == 0) {
				if (sdoGeom.Dimensionality == 2) {
					ordinates = new decimal[] { sdoGeom.Point.X.Value, sdoGeom.Point.Y.Value };
				} else {
					ordinates = new decimal[] { sdoGeom.Point.X.Value, sdoGeom.Point.Y.Value, sdoGeom.Point.Z.Value };
				}
			}
			ICoordinateSequence cs = ConvertOrdinateArray(ordinates, sdoGeom);
			IPoint point = factory.CreatePoint(cs);

			point.SRID = (int)sdoGeom.Sdo_Srid;
			return point;
		}

		private IMultiPoint ReadMultiPoint(int dim, int lrsDim, SdoGeometry sdoGeom)
		{
			Double[] ordinates = sdoGeom.getOrdinates().getOrdinateArray();
			ICoordinateSequence cs = ConvertOrdinateArray(ordinates, sdoGeom);
			IMultiPoint multipoint = factory.CreateMultiPoint(cs);
			multipoint.SRID = (int)sdoGeom.Sdo_Srid;
			return multipoint;
		}

		private ILineString ReadLine(int dim, int lrsDim, SdoGeometry sdoGeom)
		{
			bool lrs = sdoGeom.LRS > 0;
			decimal[] info = sdoGeom.ElemArray;
			ICoordinateSequence cs = null;

			int i = 0;
			while (i < info.Length)
			{
				if (info.getElementType(i).isCompound())
				{
					int numCompounds = info.getNumCompounds(i);
					cs = Add(cs, GetCompoundCSeq(i + 1, i + numCompounds, sdoGeom));
					i += 1 + numCompounds;
				}
				else
				{
					cs = Add(cs, GetElementCSeq(i, sdoGeom, false));
					i++;
				}
			}

			LineString ls =
				lrs
					? factory.createMLineString(cs)
					: factory.CreateLineString(cs);
			ls.SRID = (int)sdoGeom.Sdo_Srid;
			return ls;
		}

		private IMultiLineString ReadMultiLine(int dim, int lrsDim,
													SdoGeometry sdoGeom)
		{
			bool lrs = sdoGeom.LRS > 0;
			decimal[] info = sdoGeom.ElemArray;
			LineString[] lines =
				lrs
					? new MLineString[sdoGeom.ElemArray.Length]
					: new LineString[sdoGeom.ElemArray.Length];
			int i = 0;
			while (i < info.Length)
			{
				ICoordinateSequence cs = null;
				if (info.getElementType(i).isCompound())
				{
					int numCompounds = info.getNumCompounds(i);
					cs = Add(cs, GetCompoundCSeq(i + 1, i + numCompounds, sdoGeom));
					LineString line =
						lrs
							? factory.createMLineString(cs)
							: factory.CreateLineString(cs);
					lines[i] = line;
					i += 1 + numCompounds;
				}
				else
				{
					cs = Add(cs, GetElementCSeq(i, sdoGeom, false));
					LineString line = lrs ? (LineString)factory.createMLineString(cs) : factory.CreateLineString(cs);
					lines[i] = line;
					i++;
				}
			}

			MultiLineString mls =
				lrs
					? factory.createMultiMLineString((MLineString[])lines)
					: factory.CreateMultiLineString(lines);
			mls.SRID = (int)sdoGeom.Sdo_Srid;
			return mls;

		}

		private IGeometry ReadPolygon(int dim, int lrsDim, SdoGeometry sdoGeom) {
			LinearRing shell = null;
			LinearRing[] holes = new LinearRing[sdoGeom.getNumElements() - 1];
			decimal[] info = sdoGeom.ElemArray;
			int i = 0;
			int idxInteriorRings = 0;
			while (i < info.Length) {
				ICoordinateSequence cs = null;
				int numCompounds = 0;
				if (info.getElementType(i).isCompound()) {
					numCompounds = info.getNumCompounds(i);
					cs = Add(cs, GetCompoundCSeq(i + 1, i + numCompounds, sdoGeom));
				} else {
					cs = Add(cs, GetElementCSeq(i, sdoGeom, false));
				}
				if (info.getElementType(i).isInteriorRing()) {
					holes[idxInteriorRings] = factory
						.CreateLinearRing(cs);
					holes[idxInteriorRings].SRID = (int)sdoGeom.Sdo_Srid;
					idxInteriorRings++;
				} else {
					shell = factory.CreateLinearRing(cs);
					shell.SRID = (int)sdoGeom.Sdo_Srid;
				}
				i += 1 + numCompounds;
			}
			IPolygon polygon = factory.CreatePolygon(shell, holes);
			polygon.SRID = (int)sdoGeom.Sdo_Srid;
			return polygon;
		}

		private IMultiPolygon ReadMultiPolygon(int dim, int lrsDim, SdoGeometry sdoGeom)
		{
			List<ILinearRing> holes = new List<ILinearRing>();
			List<IPolygon> polygons = new List<IPolygon>();
			decimal[] info = sdoGeom.ElemArray;
			ILinearRing shell = null;
			int i = 0;
			while (i < info.Length)
			{
				ICoordinateSequence cs = null;
				int numCompounds = 0;
				if (info.getElementType(i).isCompound())
				{
					numCompounds = info.getNumCompounds(i);
					cs = Add(cs, GetCompoundCSeq(i + 1, i + numCompounds, sdoGeom));
				}
				else
				{
					cs = Add(cs, GetElementCSeq(i, sdoGeom, false));
				}
				if (info.getElementType(i).isInteriorRing())
				{
					ILinearRing lr = factory.CreateLinearRing(cs);
					lr.SRID = (int)sdoGeom.Sdo_Srid;
					holes.Add(lr);
				}
				else
				{
					if (shell != null)
					{
						IPolygon polygon = factory.CreatePolygon(shell, holes.ToArray());
						polygon.SRID = (int)sdoGeom.Sdo_Srid;
						polygons.Add(polygon);
						shell = null;
					}
					shell = factory.CreateLinearRing(cs);
					shell.SRID = (int)sdoGeom.Sdo_Srid;
					holes = new List<ILinearRing>();
				}
				i += 1 + numCompounds;
			}
			if (shell != null)
			{
				IPolygon polygon = factory.CreatePolygon(shell, holes.ToArray());
				polygon.SRID = (int)sdoGeom.Sdo_Srid;
				polygons.Add(polygon);
			}
			IMultiPolygon multiPolygon = factory.CreateMultiPolygon(polygons.ToArray());
			multiPolygon.SRID = (int)sdoGeom.Sdo_Srid;
			return multiPolygon;
		}

		/**
		 * Gets the ICoordinateSequence corresponding to a compound element.
		 * 
		 * @param idxFirst
		 *            the first sub-element of the compound element
		 * @param idxLast
		 *            the last sub-element of the compound element
		 * @param sdoGeom
		 *            the SdoGeometry that holds the compound element.
		 * @return
		 */
		private ICoordinateSequence GetCompoundCSeq(int idxFirst, int idxLast,
													SdoGeometry sdoGeom)
		{
			ICoordinateSequence cs = null;
			for (int i = idxFirst; i <= idxLast; i++)
			{
				// pop off the last element as it is added with the next
				// coordinate sequence
				if (cs != null && cs.Count > 0)
				{
					ICoordinate[] coordinates = cs.ToCoordinateArray();
					ICoordinate[] newCoordinates = new ICoordinate[coordinates.Length - 1];
					Array.Copy(coordinates, 0, newCoordinates, 0, coordinates.Length - 1);
					cs = factory.CoordinateSequenceFactory.Create(newCoordinates);
				}
				cs = Add(cs, GetElementCSeq(i, sdoGeom, (i < idxLast)));
			}
			return cs;
		}

		/**
	 * Gets the ICoordinateSequence corresponding to an element.
	 * 
	 * @param i
	 * @param sdoGeom
	 * @return
	 */
		private ICoordinateSequence GetElementCSeq(int i, SdoGeometry sdoGeom, bool hasNextSE) {
			ElementType type = (ElementType)sdoGeom.ElemArray[i * 3 + 1];
			Double[] elemOrdinates = ExtractOrdinatesOfElement(i, sdoGeom, hasNextSE);

			ICoordinateSequence cs;

			bool isCircle =
				type == ElementType.INTERIOR_RING_CIRCLE ||
				type == ElementType.EXTERIOR_RING_CIRCLE;

			bool isArcSegment =
				type == ElementType.LINE_ARC_SEGMENTS ||
				type == ElementType.INTERIOR_RING_ARC_SEGMENTS ||
				type == ElementType.EXTERIOR_RING_ARC_SEGMENTS;

			bool isRect =
				type == ElementType.INTERIOR_RING_RECT ||
				type == ElementType.EXTERIOR_RING_RECT;

			bool isExteriorRing =
				type == ElementType.EXTERIOR_RING_STRAIGHT_SEGMENTS ||
				type == ElementType.EXTERIOR_RING_ARC_SEGMENTS ||
				type == ElementType.EXTERIOR_RING_RECT ||
				type == ElementType.EXTERIOR_RING_CIRCLE;


			bool isStraightSegment =
				type == ElementType.POINT ||
				type == ElementType.LINE_STRAITH_SEGMENTS ||
				type == ElementType.INTERIOR_RING_STRAIGHT_SEGMENTS ||
				type == ElementType.EXTERIOR_RING_STRAIGHT_SEGMENTS;

			if (isStraightSegment)
			{
				cs = ConvertOrdinateArray(elemOrdinates, sdoGeom);
			}
			else if ( isArcSegment || isCircle)
			{
				ICoordinate[] linearized = Linearize(elemOrdinates, sdoGeom.Dimensionality, sdoGeom.LRS > 0, isCircle);
				cs = factory.CoordinateSequenceFactory.Create(linearized);
			}
			else if (isRect)
			{
				cs = ConvertOrdinateArray(elemOrdinates, sdoGeom);
				ICoordinate ll = cs.GetCoordinate(0);
				ICoordinate ur = cs.GetCoordinate(1);
				ICoordinate lr = new Coordinate(ur.X, ll.Y);
				ICoordinate ul = new Coordinate(ll.X, ur.Y);
				if (isExteriorRing)
				{
					cs = factory.CoordinateSequenceFactory.Create(new ICoordinate[] {ll, lr, ur, ul, ll});
				}
				else
				{
					cs = factory.CoordinateSequenceFactory.Create(new ICoordinate[] {ll, ul, ur, lr, ll});
				}
			}
			else
			{
				throw new ApplicationException("Unexpected Element type in compound: " + type);
			}
			return cs;
		}

		private ICoordinateSequence Add(ICoordinateSequence seq1,
		                                ICoordinateSequence seq2) {
			if (seq1 == null) {
				return seq2;
			}
			if (seq2 == null) {
				return seq1;
			}
			ICoordinate[] c1 = seq1.ToCoordinateArray();
			ICoordinate[] c2 = seq2.ToCoordinateArray();
			ICoordinate[] c3 = new Coordinate[c1.Length + c2.Length];
			Array.Copy(c1, 0, c3, 0, c1.Length);
			Array.Copy(c2, 0, c3, c1.Length, c2.Length);
			return factory.CoordinateSequenceFactory.Create(c3);
		                                }

		private Double[] ExtractOrdinatesOfElement(int element, SdoGeometry sdoGeom, bool hasNextSE)
		{
			int start = (int)sdoGeom.ElemArray[element * 3];
			if ((element * 3) < sdoGeom.ElemArray.Length - 1)
			{
				int end = (int)sdoGeom.ElemArray[(element + 1) * 3];
				// if this is a subelement of a compound geometry,
				// the last point is the first point of
				// the next subelement.
				if (hasNextSE)
				{
					end += sdoGeom.Dimensionality;
				}
				return sdoGeom.getOrdinates().getOrdinatesArray(start, end);
			}
			else
			{
				return sdoGeom.getOrdinates().getOrdinatesArray(start);
			}
		}

		private ICoordinateSequence ConvertOrdinateArray(Double[] oordinates,
														 SdoGeometry sdoGeom)
		{
			int dim = sdoGeom.Dimensionality;
			ICoordinate[] coordinates = new ICoordinate[oordinates.Length / dim];
			int zDim = sdoGeom.getZDimension() - 1;
			int lrsDim = sdoGeom.LRS - 1;
			for (int i = 0; i < coordinates.Length; i++)
			{
				if (dim == 2)
				{
					coordinates[i] = new Coordinate(
						oordinates[i * dim],
						oordinates[i * dim + 1]);
				}
				else if (dim == 3)
				{
					if (sdoGeom.LRS > 0)
					{
						coordinates[i] = MCoordinate.Create2dWithMeasure(
							oordinates[i * dim], // X
							oordinates[i * dim + 1], // Y
							oordinates[i * dim + lrsDim]); // M
					}
					else
					{
						coordinates[i] = new Coordinate(
							oordinates[i * dim], // X
							oordinates[i * dim + 1], // Y
							oordinates[i * dim + zDim]); // Z
					}
				}
				else if (dim == 4)
				{
					// This must be an LRS Geometry
					if (sdoGeom.LRS == 0)
						throw new ApplicationException(
							"4 dimensional Geometries must be LRS geometry");
					coordinates[i] = MCoordinate.Create3dWithMeasure(
						oordinates[i * dim], // X
						oordinates[i * dim + 1], // Y
						oordinates[i * dim + zDim], // Z
						oordinates[i * dim + lrsDim]); // M
				}
			}
			return factory.CoordinateSequenceFactory.Create(coordinates);
		}

		// reverses ordinates in a coordinate array in-place
		private ICoordinate[] ReverseRing(ICoordinate[] ar) {
			for (int i = 0; i < ar.Length / 2; i++) {
				ICoordinate cs = ar[i];
				ar[i] = ar[ar.Length - 1 - i];
				ar[ar.Length - 1 - i] = cs;
			}
			return ar;
		}

		/**
	 * Linearizes arcs and circles.
	 * 
	 * @param arcOrdinates
	 *            arc or circle coordinates
	 * @param dim
	 *            coordinate dimension
	 * @param lrs
	 *            whether this is an lrs geometry
	 * @param entireCirlce
	 *            whether the whole arc should be linearized
	 * @return linearized interpolation of arcs or circle
	 */
		private ICoordinate[] Linearize(Double[] arcOrdinates, int dim, bool lrs,
		                               bool entireCirlce) {
			ICoordinate[] linearizedCoords = new ICoordinate[0];
			// CoordDim is the dimension that includes only non-measure (X,Y,Z)
			// ordinates in its value
			int coordDim = lrs ? dim - 1 : dim;
			// this only works with 2-Dimensional geometries, since we use
			// JGeometry linearization;
			if (coordDim != 2)
				throw new ArgumentException(
					"Can only linearize 2D arc segments, but geometry is "
					+ dim + "D.");
			int numOrd = dim;
			while (numOrd < arcOrdinates.Length) {
				numOrd = numOrd - dim;
				double x1 = arcOrdinates[numOrd++];
				double y1 = arcOrdinates[numOrd++];
				double m1 = lrs ? arcOrdinates[numOrd++] : Double.NaN;
				double x2 = arcOrdinates[numOrd++];
				double y2 = arcOrdinates[numOrd++];
				double m2 = lrs ? arcOrdinates[numOrd++] : Double.NaN;
				double x3 = arcOrdinates[numOrd++];
				double y3 = arcOrdinates[numOrd++];
				double m3 = lrs ? arcOrdinates[numOrd++] : Double.NaN;

				ICoordinate[] coords;
				if (entireCirlce) {
					coords = Circle.LinearizeCircle(x1, y1, x2, y2, x3, y3);
				} else {
					coords = Circle.LinearizeArc(x1, y1, x2, y2, x3, y3);
				}

				// if this is an LRS geometry, fill the measure values into
				// the linearized array
				if (lrs) {
					ICoordinate[] mcoord = new ICoordinate[coords.Length];
					int lastIndex = coords.Length - 1;
					mcoord[0] = MCoordinate.Create2dWithMeasure(x1, y1, m1);
					mcoord[lastIndex] = MCoordinate.Create2dWithMeasure(x3, y3, m3);
					// convert the middle coordinates to MCoordinate
					for (int i = 1; i < lastIndex; i++) {
						mcoord[i] = MCoordinate.ConvertCoordinate(coords[i]);
						// if we happen to split on the middle measure, then
						// assign it
						if (mcoord[i].X == x2
						    && mcoord[i].Y == y2) {
						    	((MCoordinate)mcoord[i]).M = m2;
						    }
					}
					coords = mcoord;
				}

				// if this is not the first arcsegment, the first linearized
				// point is already in linearizedArc, so disregard this.
				int resultBegin = 1;
				if (linearizedCoords.Length == 0)
					resultBegin = 0;

				int destPos = linearizedCoords.Length;
				ICoordinate[] tmpCoords = new Coordinate[linearizedCoords.Length
				                                        + coords.Length - resultBegin];
				Array.Copy(linearizedCoords, 0, tmpCoords, 0, linearizedCoords.Length);
				Array.Copy(coords, resultBegin, tmpCoords, destPos, coords.Length - resultBegin);

				linearizedCoords = tmpCoords;
			}
			return linearizedCoords;
		                               }

		//public int[] sqlTypes() {
		//    return geometryTypes;
		//}

		//public static String arrayToString(Object array) {
		//    if (array == null || Array.getLength(array) == 0) {
		//        return "()";
		//    }
		//    int length = Array.getLength(array);
		//    StringBuilder stb = new StringBuilder();
		//    stb.Append("(").Append(Array.get(array, 0));
		//    for (int i = 1; i < length; i++) {
		//        stb.Append(",").Append(Array.get(array, i));
		//    }
		//    stb.Append(")");
		//    return stb.ToString();
		//}

	}
}