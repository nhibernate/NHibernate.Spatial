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

namespace NHibernate.Spatial.MGeometries
{

	/**
	 * TODO: This class should be removed.
	 * 
	 * 
	 * This utility class is used to test doubles for equality
	 * 
	 * @author Didier H. Besset <p/> Adapted from "Object-oriented implementation of
	 *         numerical methods"
	 */
	public static class DoubleComparator
	{
		private static readonly int radix = ComputeRadix();

		private static readonly double machinePrecision = ComputeMachinePrecision();

		private static readonly double defaultNumericalPrecision = Math.Sqrt(machinePrecision);

		private static int ComputeRadix()
		{
			int radix = 0;
			double a = 1.0d;
			double tmp1, tmp2;
			do
			{
				a += a;
				tmp1 = a + 1.0d;
				tmp2 = tmp1 - a;
			} while (tmp2 - 1.0d != 0.0d);
			double b = 1.0d;
			while (radix == 0)
			{
				b += b;
				tmp1 = a + b;
				radix = (int)(tmp1 - a);
			}
			return radix;
		}

		public static int GetRadix()
		{
			return radix;
		}

		private static double ComputeMachinePrecision()
		{
			double floatingRadix = GetRadix();
			double inverseRadix = 1.0d / floatingRadix;
			double machinePrecision = 1.0d;
			double tmp = 1.0d + machinePrecision;
			while (tmp - 1.0d != 0.0)
			{
				machinePrecision *= inverseRadix;
				tmp = 1.0d + machinePrecision;
			}
			return machinePrecision;
		}

		public static double MachinePrecision
		{
			get { return machinePrecision; }
		}

		public static double DefaultNumericalPrecision
		{
			get { return defaultNumericalPrecision; }
		}

		public static bool Equals(double a, double b)
		{
			return Equals(a, b, defaultNumericalPrecision);
		}

		public static bool Equals(double a, double b, double precision)
		{
			double norm = Math.Max(Math.Abs(a), Math.Abs(b));
			bool result = norm < precision || Math.Abs(a - b) < precision * norm;
			return result || (Double.IsNaN(a) && Double.IsNaN(b));
		}

		public static int Compare(double a, double b)
		{
			return a < b ? -1 : (a > b ? +1 : 0);
		}
	}
}