/**
 * $Id$
 *
 * This file is part of Hibernate Spatial, an extension to the 
 * hibernate ORM solution for geographic data. 
 *  
 * Copyright © 2007 Geovise BVBA
 * Copyright © 2007 K.U. Leuven LRD, Spatial Applications Division, Belgium
 *
 * This work was partially supported by the European Commission, 
 * under the 6th Framework Programme, contract IST-2-004688-STP.
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 *
 * For more information, visit: http://www.hibernatespatial.org/
 */

using System;
using System.Runtime.Serialization;
using System.Text;
using GeoAPI.Geometries;
using GisSharpBlog.NetTopologySuite.Geometries;

namespace NHibernate.Spatial.MGeometries
{
	/**
	 * Implements the CoordinateSequence interface. In this implementation,
	 * Coordinates returned by #toArray and #get are live -- parties that change
	 * them are actually changing the MCoordinateSequence's underlying data.
	 */

	public class MCoordinateSequence : ICoordinateSequence
	{
		private MCoordinate[] coordinates;

		public static MCoordinate[] Copy(ICoordinate[] coordinates)
		{
			MCoordinate[] copy = new MCoordinate[coordinates.Length];
			for (int i = 0; i < coordinates.Length; i++)
			{
				copy[i] = new MCoordinate(coordinates[i]);
			}
			return copy;
		}

		public static MCoordinate[] Copy(ICoordinateSequence coordSeq)
		{
			MCoordinate[] copy = new MCoordinate[coordSeq.Count];
			for (int i = 0; i < coordSeq.Count; i++)
			{
				copy[i] = new MCoordinate(coordSeq.GetCoordinate(i));
			}
			return copy;
		}

		/**
		 * Copy constructor -- simply aliases the input array, for better
		 * performance.
		 * 
		 * @param coordinates
		 */
		public MCoordinateSequence(MCoordinate[] coordinates)
		{
			this.coordinates = coordinates;
		}

		/**
		 * Constructor that makes a copy of an array of Coordinates. Always makes a
		 * copy of the input array, since the actual class of the Coordinates in the
		 * input array may be different from MCoordinate.
		 * 
		 * @param copyCoords
		 */
		public MCoordinateSequence(ICoordinate[] copyCoords)
		{
			coordinates = Copy(copyCoords);
		}

		/**
		 * Constructor that makes a copy of a CoordinateSequence.
		 * 
		 * @param coordSeq
		 */
		public MCoordinateSequence(ICoordinateSequence coordSeq)
		{
			coordinates = Copy(coordSeq);
		}

		/**
		 * Constructs a sequence of a given size, populated with new
		 * {@link MCoordinate}s.
		 * 
		 * @param size
		 *            the size of the sequence to create
		 */
		public MCoordinateSequence(int size)
		{
			coordinates = new MCoordinate[size];
			for (int i = 0; i < size; i++)
			{
				coordinates[i] = new MCoordinate();
			}
		}

		/**
		 * @see com.vividsolutions.jts.geom.CoordinateSequence#getDimension()
		 */
		public int Dimension
		{
			get { return 4; }
		}

		public ICoordinate GetCoordinate(int i)
		{
			return coordinates[i];
		}

		/**
		 * @see com.vividsolutions.jts.geom.CoordinateSequence#GetCoordinateCopy(int)
		 */
		public ICoordinate GetCoordinateCopy(int index)
		{
			return new Coordinate(coordinates[index]);
		}

		/**
		 * @see com.vividsolutions.jts.geom.CoordinateSequence#GetCoordinate(int,
		 *      com.vividsolutions.jts.geom.Coordinate)
		 */
		public void GetCoordinate(int index, ICoordinate coord)
		{
			coord.X = coordinates[index].X;
			coord.Y = coordinates[index].Y;
		}

		/**
		 * @see com.vividsolutions.jts.geom.CoordinateSequence#GetX(int)
		 */
		public double GetX(int index)
		{
			return coordinates[index].X;
		}

		/**
		 * @see com.vividsolutions.jts.geom.CoordinateSequence#GetY(int)
		 */
		public double GetY(int index)
		{
			return coordinates[index].Y;
		}

		/**
		 * @return the measure value of the coordinate in the index
		 */
		public double GetM(int index)
		{
			return coordinates[index].M;
		}

		/**
		 * @see com.vividsolutions.jts.geom.CoordinateSequence#GetOrdinate(int,int)
		 */
		public double GetOrdinate(int index, Ordinates ordinateIndex)
		{
			switch (ordinateIndex)
			{
				case Ordinates.X:
					return coordinates[index].X;
				case Ordinates.Y:
					return coordinates[index].Y;
				case Ordinates.Z:
					return coordinates[index].Z;
				case Ordinates.M:
					return coordinates[index].M;
			}
			return Double.NaN;
		}

		/**
		 * @see com.vividsolutions.jts.geom.CoordinateSequence#SetOrdinate(int,int,double)
		 */
		public void SetOrdinate(int index, Ordinates ordinateIndex, double value)
		{
			switch (ordinateIndex)
			{
				case Ordinates.X:
					coordinates[index].X = value;
					break;
				case Ordinates.Y:
					coordinates[index].Y = value;
					break;
				case Ordinates.Z:
					coordinates[index].Z = value;
					break;
				case Ordinates.M:
					coordinates[index].M = value;
					break;
				default:
					throw new ArgumentException("invalid ordinateIndex");
			}
		}

		public Object Clone()
		{
			MCoordinate[] cloneCoordinates = new MCoordinate[Count];
			for (int i = 0; i < coordinates.Length; i++)
			{
				cloneCoordinates[i] = (MCoordinate)coordinates[i].Clone();
			}

			return new MCoordinateSequence(cloneCoordinates);
		}

		public int Count
		{
			get { return coordinates.Length; }
		}

		public ICoordinate[] ToCoordinateArray()
		{
			return coordinates;
		}

		public IEnvelope ExpandEnvelope(IEnvelope env)
		{
			for (int i = 0; i < coordinates.Length; i++)
			{
				env.ExpandToInclude(coordinates[i]);
			}
			return env;
		}

		public override String ToString()
		{
			StringBuilder strBuf = new StringBuilder();
			strBuf.Append("MCoordinateSequence [");
			for (int i = 0; i < coordinates.Length; i++)
			{
				if (i > 0)
					strBuf.Append(", ");
				strBuf.Append(coordinates[i]);
			}
			strBuf.Append("]");
			return strBuf.ToString();
		}
	}
}

