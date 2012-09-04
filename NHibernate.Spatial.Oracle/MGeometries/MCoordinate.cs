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
using GeoAPI.Geometries;
using GisSharpBlog.NetTopologySuite.Geometries;

namespace NHibernate.Spatial.MGeometries
{
	/**
	 * This coordinate class supports 4D coordinates, where the first 3 measures
	 * (x,y,z) are coordinates in a 3 dimensional space (cartesian for example), and
	 * the fourth is a measure value used for linear referencing. Note that the
	 * measure value is independent of whether the (x,y,z) values are used. For
	 * example, the z value can not be used while the measure value is used. <p/>
	 * While this class extends the Coordinate class, it can be used seamlessly as a
	 * substitute in the event that the Measure value is not used. In these cases
	 * the Measure value shall simply be Double.NaN
	 * 
	 * @see com.vividsolutions.jts.geom.Coordinate
	 */
	public class MCoordinate : Coordinate
	{
		public double M;

		/**
		 * Default constructor
		 */

		public MCoordinate()
			: base()
		{
			M = Double.NaN;
		}

		public MCoordinate(double x, double y, double z, double m)
			: base(x, y, z)
		{
			this.M = m;
		}

		public MCoordinate(double x, double y)
			: base(x, y)
		{
			M = Double.NaN;
		}

		public MCoordinate(ICoordinate coord)
			: base(coord)
		{
			M = coord is MCoordinate ? ((MCoordinate)coord).M : Double.NaN;
		}

		public MCoordinate(MCoordinate coord)
			: base(coord)
		{
			M = coord.M;
		}

		/**
	 * TODO: I'd like to see this method added to the base Coordinate class
	 * Returns the ordinate value specified in this Coordinate instance. The
	 * index of the desired ordinates are specified in the CoordinateSequence
	 * class; hence CoodinateSequence.X returns the x ordinate,
	 * CoodinateSequence.Y the y ordinate, CoodinateSequence.Z the z ordinate,
	 * and CoodinateSequence.M the M ordinate. Note that the dimension may not
	 * imply the desired ordinate in the case where one is using a 2 dimensional
	 * geometry with a measure value. Therefore, these constants are highly
	 * recommended.
	 * 
	 * @param ordinateIndex
	 *            the desired ordinate index.
	 * @return the value of stored in the ordinate index. Incorrect or unused
	 *         indexes shall return Double.NaN
	 */

		public double GetOrdinate(Ordinates ordinateIndex)
		{
			switch (ordinateIndex)
			{
				case Ordinates.X:
					return this.X;
				case Ordinates.Y:
					return this.Y;
				case Ordinates.Z:
					return this.Z;
				case Ordinates.M:
					return M;
			}
			return Double.NaN;
		}

		/**
	 * TODO: I'd like to see this method added to the base Coordinate class Sets
	 * the value for a given ordinate. This should be specified using the
	 * CoordinateSequence ordinate index constants.
	 * 
	 * @param ordinateIndex
	 *            the desired ordinate index.
	 * @param value
	 *            the new ordinate value
	 * @throws IllegalArgumentException
	 *             if the ordinateIndex value is incorrect
	 * @see #GetOrdinate(int)
	 */

		public void SetOrdinate(Ordinates ordinateIndex, double value)
		{
			switch (ordinateIndex)
			{
				case Ordinates.X:
					this.X = value;
					break;
				case Ordinates.Y:
					this.Y = value;
					break;
				case Ordinates.Z:
					this.Z = value;
					break;
				case Ordinates.M:
					M = value;
					break;
				default:
					throw new ArgumentException("ordinateIndex");
			}
		}

		public bool Equals2DWithMeasure(Coordinate other)
		{
			bool result = this.Equals2D(other);
			if (result)
			{
				MCoordinate mc = ConvertCoordinate(other);
				result = (DoubleComparator.Compare(M, mc.M) == 0);
			}
			return result;
		}

		public bool Equals3DWithMeasure(Coordinate other)
		{
			bool result = this.Equals3D(other);
			if (result)
			{
				MCoordinate mc = ConvertCoordinate(other);
				result = (DoubleComparator.Compare(M, mc.M) == 0);
			}
			return result;
		}

		/*
	 * Default equality is now equality in 2D-plane. This is required to remain
	 * consistent with JTS.
	 * 
	 * TODO:check whether this method is still needed.
	 * 
	 * (non-Javadoc)
	 * 
	 * @see com.vividsolutions.jts.geom.Coordinate#Equals(java.lang.Object)
	 */

		public override bool Equals(Object other)
		{
			if (other is Coordinate)
			{
				return Equals2D((Coordinate)other);
			}
			else
			{
				return false;
			}
		}

		public override string ToString()
		{
			return "(" + X + "," + Y + "," + Z + "," + " M=" + M + ")";
		}

		/**
	 * Converts a standard Coordinate instance to an MCoordinate instance. If
	 * coordinate is already an instance of an MCoordinate, then it is simply
	 * returned. In cases where it is converted, the measure value of the
	 * coordinate is initialized to Double.NaN.
	 * 
	 * @param coordinate
	 *            The coordinate to be converted
	 * @return an instance of MCoordinate corresponding to the
	 *         <code>coordinate</code> parameter
	 */

		public static MCoordinate ConvertCoordinate(ICoordinate coordinate)
		{
			if (coordinate == null)
			{
				return null;
			}
			if (coordinate is MCoordinate)
			return (MCoordinate)coordinate;
			return new MCoordinate(coordinate);
		}

		/**
	 * A convenience method for creating a MCoordinate instance where there are
	 * only 2 coordinates and an lrs measure value. The z value of the
	 * coordinate shall be set to Double.NaN
	 * 
	 * @param x
	 *            the x coordinate value
	 * @param y
	 *            the y coordinate value
	 * @param M
	 *            the lrs measure value
	 * @return The constructed MCoordinate value
	 */

		public static MCoordinate Create2dWithMeasure(double x, double y, double m)
		{
			return new MCoordinate(x, y, Double.NaN, m);
		}

		/**
	 * A convenience method for creating a MCoordinate instance where there are
	 * only 2 coordinates and an lrs measure value. The z and M value of the
	 * coordinate shall be set to Double.NaN
	 * 
	 * @param x
	 *            the x coordinate value
	 * @param y
	 *            the y coordinate value
	 * @return The constructed MCoordinate value
	 */

		public static MCoordinate Create2d(double x, double y)
		{
			return new MCoordinate(x, y, Double.NaN, Double.NaN);
		}

		/**
	 * A convenience method for creating a MCoordinate instance where there are
	 * 3 coordinates and an lrs measure value.
	 * 
	 * @param x
	 *            the x coordinate value
	 * @param y
	 *            the y coordinate value
	 * @param z
	 *            the z coordinate value
	 * @param M
	 *            the lrs measure value
	 * @return The constructed MCoordinate value
	 */

		public static MCoordinate Create3dWithMeasure(double x, double y, double z,
		                                              double m)
		{
			return new MCoordinate(x, y, z, m);
		}

		/**
	 * A convenience method for creating a MCoordinate instance where there are
	 * 3 coordinates but no lrs measure value. The M value of the coordinate
	 * shall be set to Double.NaN
	 * 
	 * @param x
	 *            the x coordinate value
	 * @param y
	 *            the y coordinate value
	 * @param z
	 *            the z coordinate value
	 * @return The constructed MCoordinate value
	 */

		public static MCoordinate Create3d(double x, double y, double z)
		{
			return new MCoordinate(x, y, z, Double.NaN);
		}
	}
}