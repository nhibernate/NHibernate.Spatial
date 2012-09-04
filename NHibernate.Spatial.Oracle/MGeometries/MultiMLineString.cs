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
using System.Collections.Generic;
using GeoAPI.Geometries;
using GisSharpBlog.NetTopologySuite.Geometries;

namespace NHibernate.Spatial.MGeometries
{
	public class MultiMLineString : MultiLineString, IMGeometry
	{
		/// <summary>
		/// Difference in M between end of one part and the beginning of the consecutive path
		/// </summary>
		private readonly double mGap;
		private bool monotone;
		private bool strictMonotone;

		/**
		 * @param MlineStrings
		 *            the <code>MLineString</code>s for this
		 *            <code>MultiMLineString</code>, or <code>null</code> or an
		 *            empty array to create the empty geometry. Elements may be
		 *            empty <code>LineString</code>s, but not <code>null</code>s.
		 */
		public MultiMLineString(MLineString[] MlineStrings, double mGap, IGeometryFactory factory)
			: base(MlineStrings, factory)
		{
			this.mGap = mGap;
			DetermineMonotone();
		}

		/**
		 * TODO Improve this, and add more unit tests
		 */
		private void DetermineMonotone()
		{
			this.monotone = true;
			this.strictMonotone = true;
			if (this.IsEmpty)
			{
				return;
			}
			MGeometryType mdir = ((MLineString)this.geometries[0]).GetMeasureDirection();
			for (int i = 0; i < this.geometries.Length; i++)
			{
				MLineString ml = (MLineString)this.geometries[i];
				// check whether mlinestrings are all pointing in same direction,
				// and
				// are monotone
				if (!ml.IsMonotone(false)
						|| (ml.GetMeasureDirection() != mdir && !(ml
								.GetMeasureDirection() == MGeometryType.Constant)))
				{
					this.monotone = false;
					break;
				}

				if (!ml.IsMonotone(true) || (ml.GetMeasureDirection() != mdir))
				{
					this.strictMonotone = false;
					break;
				}

				// check whether the geometry measures do not overlap or
				// are inconsistent with previous parts
				if (i > 0)
				{
					MLineString mlp = (MLineString)this.geometries[i - 1];
					if (mdir == MGeometryType.Increasing)
					{
						if (mlp.GetMaxM() > ml.GetMinM())
						{
							monotone = false;
						}
						else if (mlp.GetMaxM() >= ml.GetMinM())
						{
							strictMonotone = false;
						}
					}
					else
					{
						if (mlp.GetMinM() < ml.GetMaxM())
						{
							monotone = false;
						}
						else if (mlp.GetMinM() <= ml.GetMaxM())
						{
							strictMonotone = false;
						}
					}

				}

			}
			if (!monotone)
			{
				this.strictMonotone = false;
			}

		}

		protected void GeometryChangedAction()
		{
			DetermineMonotone();
		}

		public override string GeometryType
		{
			get { return "MultiMLineString"; }
		}

		public double GetMGap()
		{
			return this.mGap;
		}

		public double GetMatCoordinate(ICoordinate co, double tolerance)
		{

			if (!this.IsMonotone(false))
			{
				throw new ApplicationException("MGeometryException.OPERATION_REQUIRES_MONOTONE");
			}

			double mval = Double.NaN;
			double dist = Double.PositiveInfinity;

			IPoint p = this.Factory.CreatePoint(co);

			// find points within tolerance for GetMatCoordinate
			for (int i = 0; i < this.NumGeometries; i++)
			{
				MLineString ml = (MLineString)this.GetGeometryN(i);
				// go to next MLineString if the input point is beyond tolerance
				if (ml.Distance(p) > tolerance)
					continue;

				MCoordinate mc = ml.GetClosestPoint(co, tolerance);
				if (mc != null)
				{
					double d = mc.Distance(co);
					if (d <= tolerance && d < dist)
					{
						dist = d;
						mval = mc.M;
					}
				}
			}
			return mval;
		}

		public override Object Clone()
		{
			MultiLineString ml = (MultiLineString)base.Clone();
			return ml;
		}

		public void MeasureOnLength(bool keepBeginMeasure)
		{
			double startM = 0.0;
			for (int i = 0; i < this.NumGeometries; i++)
			{
				MLineString ml = (MLineString)this.GetGeometryN(i);
				if (i == 0)
				{
					ml.MeasureOnLength(keepBeginMeasure);
				}
				else
				{
					ml.MeasureOnLength(false);
				}
				if (startM != 0.0)
				{
					ml.ShiftMeasure(startM);
				}
				startM += ml.Length + mGap;
			}
			this.GeometryChanged();
		}

		/*
		 * (non-Javadoc)
		 * 
		 * @see org.hibernatespatial.mgeom.IMGeometry#GetCoordinateAtM(double)
		 */
		public ICoordinate GetCoordinateAtM(double m)
		{

			if (!this.IsMonotone(false))
			{
				throw new ApplicationException("MGeometryException.OPERATION_REQUIRES_MONOTONE");
			}

			ICoordinate c;
			for (int i = 0; i < this.NumGeometries; i++)
			{
				IMGeometry mg = (IMGeometry)this.GetGeometryN(i);
				c = mg.GetCoordinateAtM(m);
				if (c != null)
				{
					return c;
				}
			}
			return null;
		}

		public ICoordinateSequence[] GetCoordinatesBetween(double begin, double end)
		{

			if (!this.IsMonotone(false))
			{
				throw new ApplicationException("MGeometryException.OPERATION_REQUIRES_MONOTONE");
			}

			if (this.IsEmpty)
				return null;

			List<ICoordinateSequence> ar = new List<ICoordinateSequence>();

			for (int i = 0; i < this.NumGeometries; i++)
			{
				MLineString ml = (MLineString)this.GetGeometryN(i);
				foreach (ICoordinateSequence cs in ml.GetCoordinatesBetween(begin, end))
				{
					ar.Add(cs);
				}
			}
			return ar.ToArray();
		}

		/*
		 * (non-Javadoc)
		 * 
		 * @see org.hibernatespatial.mgeom.IMGeometry#GetMinM()
		 */
		public double GetMinM()
		{
			double minM = Double.PositiveInfinity;
			for (int i = 0; i < this.NumGeometries; i++)
			{
				MLineString ml = (MLineString)this.GetGeometryN(i);
				double d = ml.GetMinM();
				if (d < minM)
					minM = d;
			}
			return minM;
		}

		/*
		 * (non-Javadoc)
		 * 
		 * @see org.hibernatespatial.mgeom.IMGeometry#GetMaxM()
		 */
		public double GetMaxM()
		{
			double maxM = Double.NegativeInfinity;
			for (int i = 0; i < this.NumGeometries; i++)
			{
				MLineString ml = (MLineString)this.GetGeometryN(i);
				double d = ml.GetMaxM();
				if (d < maxM)
					maxM = d;
			}
			return maxM;
		}

		/*
		 * (non-Javadoc)
		 * 
		 * @see org.hibernatespatial.mgeom.IMGeometry#IsMonotone()
		 */
		public bool IsMonotone(bool strictMonotone)
		{
			return strictMonotone ? this.strictMonotone : monotone;
		}
	}
}