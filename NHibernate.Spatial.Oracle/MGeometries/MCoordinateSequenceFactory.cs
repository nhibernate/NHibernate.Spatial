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
using System.Runtime.Serialization;
using GeoAPI.Geometries;
using GisSharpBlog.NetTopologySuite.Geometries;

namespace NHibernate.Spatial.MGeometries
{

	/**
	 * Creates MCoordinateSequenceFactory internally represented as an array of
	 * {@link MCoordinate}s.
	 */
	public class MCoordinateSequenceFactory : ICoordinateSequenceFactory
	{
		private static MCoordinateSequenceFactory instance = new MCoordinateSequenceFactory();

		private MCoordinateSequenceFactory()
		{
		}

		/**
		 * Returns the singleton instance of MCoordinateSequenceFactory
		 */
		public static MCoordinateSequenceFactory Instance
		{
			get { return instance; }
		}

		/**
		 * Returns an MCoordinateSequence based on the given array -- the array is
		 * used directly if it is an instance of MCoordinate[]; otherwise it is
		 * copied.
		 */
		public ICoordinateSequence Create(ICoordinate[] coordinates)
		{
			if (coordinates is MCoordinate[])
			{
				return new MCoordinateSequence((MCoordinate[])coordinates);
			}
			else
			{
				return new MCoordinateSequence(coordinates);
			}
		}

		public ICoordinateSequence Create(ICoordinateSequence coordSeq)
		{
			return new MCoordinateSequence(coordSeq);
		}

		/**
		 * Creates a MCoordinateSequence instance initialized to the size parameter.
		 * Note that the dimension argument is ignored.
		 * 
		 * @see com.vividsolutions.jts.geom.CoordinateSequenceFactory#create(int,int)
		 */
		public ICoordinateSequence Create(int size, int dimension)
		{
			return (ICoordinateSequence)new MCoordinateSequence(size);
		}

	}
}