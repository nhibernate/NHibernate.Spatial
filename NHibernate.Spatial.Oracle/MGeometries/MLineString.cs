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
using System.Diagnostics;
using System.Text;
using GeoAPI.Geometries;
using GisSharpBlog.NetTopologySuite.Geometries;

namespace NHibernate.Spatial.MGeometries
{

	/**
	 * An implementation of the LineString class with the addition that the
	 * containing CoordinateSequence can carry measure. Note that this is not a
	 * strict requirement of the class, and can interact with non-measure geometries
	 * for JTS topological comparisons regardless.
	 * 
	 * @author Karel Maesen
	 */
	public class MLineString : LineString, IMGeometry
	{
		private bool monotone;
		private bool strictMonotone;

		public MLineString(ICoordinateSequence points, IGeometryFactory factory)
			: base(points, factory)
		{
			DetermineMonotone();
		}

		public override Object Clone()
		{
			ILineString ls = (ILineString)base.Clone();
			return new MLineString(ls.CoordinateSequence, this.Factory);
		}

		/**
		 * Calculates whether the measures in the CoordinateSequence are monotone
		 * and strict monotone. The strict parameter indicates whether the
		 * determination should apply the definition of "strict monotonicity" or
		 * non-strict.
		 * 
		 * @see #IsMonotone()
		 * @see #isStrictMonotone()
		 */
		private void DetermineMonotone()
		{
			this.monotone = true;
			this.strictMonotone = true;
			if (!this.IsEmpty)
			{
				double[] m = this.GetMeasures();
				// short circuit if the first value is NaN
				if (Double.IsNaN(m[0]))
				{
					this.monotone = false;
					this.strictMonotone = false;
				}
				else
				{
					int result = 0;
					int prevResult = 0;
					for (int i = 1; i < m.Length && this.monotone; i++)
					{
						result = DoubleComparator.Compare(m[i - 1], m[i]);
						this.monotone = !(result * prevResult < 0 || Double.IsNaN(m[i]));
						this.strictMonotone &= this.monotone && result != 0;
						prevResult = result;
					}
				}
			}
			// if not monotone, then certainly not strictly monotone
			Debug.Assert(!(this.strictMonotone && !this.monotone));
		}

		protected void GeometryChangedAction()
		{
			DetermineMonotone();
		}

		/**
		 * @param co
		 *            input coordinate in the neighbourhood of the MLineString
		 * @param tolerance
		 *            max. distance that co may be from this MLineString
		 * @return an MCoordinate on this MLineString with appropriate M-value
		 */
		public MCoordinate GetClosestPoint(ICoordinate co, double tolerance)
		{
			if (!this.IsMonotone(false))
			{
				throw new ApplicationException("MGeometryException.OPERATION_REQUIRES_MONOTONE");
			}

			if (!this.IsEmpty)
			{
				LineSegment seg = new LineSegment();
				ICoordinate[] coAr = this.Coordinates;
				seg.P0 = coAr[0];
				double d = 0.0;
				double projfact = 0.0;
				double minDist = Double.PositiveInfinity;
				MCoordinate mincp = null;
				for (int i = 1; i < coAr.Length; i++)
				{
					seg.P1 = coAr[i];
					ICoordinate cp = seg.ClosestPoint(co);
					d = cp.Distance(co);
					if (d <= tolerance && d <= minDist)
					{
						MCoordinate testcp = new MCoordinate(cp);
						projfact = seg.ProjectionFactor(cp);
						testcp.M = ((MCoordinate)coAr[i - 1]).M
								+ projfact
								* (((MCoordinate)coAr[i]).M - ((MCoordinate)coAr[i - 1]).M);
						if (d < minDist || testcp.M < mincp.M)
						{
							mincp = testcp;
							minDist = d;
						}
					}
					seg.P0 = seg.P1;
				}
				if (minDist > tolerance)
				{
					return null;
				}
				else
				{
					return mincp;
				}
			}
			else
			{
				return null;
			}
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
			if (this.IsEmpty)
			{
				return null;
			}
			else
			{
				double[] mval = this.GetMeasures();
				double lb = GetMinM();
				double up = GetMaxM();

				if (m < lb || m > up)
				{
					return null;
				}
				else
				{
					// determine linesegment that contains M;
					for (int i = 1; i < mval.Length; i++)
					{
						if ((mval[i - 1] <= m && m <= mval[i])
							|| (mval[i] <= m && m <= mval[i - 1]))
						{
							MCoordinate p0 = (MCoordinate)this.GetCoordinateN(i - 1);
							MCoordinate p1 = (MCoordinate)this.GetCoordinateN(i);
							// r indicates how far in this segment the M-values lies
							double r = (m - mval[i - 1]) / (mval[i] - mval[i - 1]);
							double dx = r * (p1.X - p0.X);
							double dy = r * (p1.Y - p0.Y);
							double dz = r * (p1.Z - p0.Z);
							MCoordinate nc = new MCoordinate(
								p0.X + dx,
								p0.Y + dy,
								p0.Z + dz,
								m
							);
							return nc;
						}
					}
				}
			}
			return null;
		}

		/*
		 * (non-Javadoc)
		 * 
		 * @see com.vividsolutions.jts.geom.Geometry#GetGeometryType()
		 */
		public String GetGeometryType()
		{
			return "MLineString";
		}

		/*
		 * (non-Javadoc)
		 * 
		 * @see com.vividsolutions.jts.geom.Geometry#GetMatCoordinate(com.vividsolutions.jts.geom.Coordinate,
		 *      double)
		 */
		public double GetMatCoordinate(ICoordinate c, double tolerance)
		{
			MCoordinate mco = this.GetClosestPoint(c, tolerance);
			if (mco == null)
			{
				return Double.NaN;
			}
			else
			{
				return (mco.M);
			}
		}

		/**
		 * get the measure of the specified coordinate
		 * 
		 * @param n
		 *            index of the coordinate
		 * @return The measure of the coordinate. If the coordinate does not exists
		 *         it returns Double.NaN
		 */
		public double GetMatN(int n)
		{
			return ((MCoordinate)(this.Coordinates[n])).M;
		}

		/*
		 * (non-Javadoc)
		 * 
		 * @see org.hibernatespatial.mgeom.IMGeometry##IMGeometry#GetMaxM()
		 */
		public double GetMaxM()
		{
			if (this.IsEmpty)
			{
				return Double.NaN;
			}
			else
			{
				double[] measures = this.GetMeasures();

				if (this.GetMeasureDirection() == MGeometryType.Increasing)
				{
					return measures[measures.Length - 1];
				}
				else if (this.GetMeasureDirection() == MGeometryType.Decreasing
						 || this.GetMeasureDirection() == MGeometryType.Constant)
				{
					return measures[0];
				}
				else
				{
					double ma = Double.NegativeInfinity;
					for (int i = 0; i < measures.Length; i++)
					{
						if (ma < measures[i])
						{
							ma = measures[i];
						}
					}
					return ma;
				}
			}
		}

		/*
		 * (non-Javadoc)
		 * 
		 * @see org.hibernatespatial.mgeom.IMGeometry#GetCoordinatesBetween(double,double)
		 */
		public ICoordinateSequence[] GetCoordinatesBetween(double fromM, double toM)
		{

			if (!this.IsMonotone(false))
			{
				throw new ApplicationException("MGeometryException.OPERATION_REQUIRES_MONOTONE");
			}

			if (this.IsEmpty || !this.IsMonotone(false))
			{
				return new MCoordinateSequence[0];
			}
			else
			{
				double[] mval = this.GetMeasures();

				// determin upper and lower boundaries for the MLineString Measures
				double lb = Math.Min(mval[0], mval[mval.Length - 1]);
				double up = Math.Max(mval[0], mval[mval.Length - 1]);

				// set fromM and toM to maximal/minimal values when they exceed
				// lowerbound-upperbound
				fromM = Math.Max(lb, Math.Min(fromM, up));
				toM = Math.Max(lb, Math.Min(toM, up));

				// if at this point the fromM and toM are equal, then return an
				// empty MCoordinateSequence
				if (DoubleComparator.Equals(fromM, toM))
				{
					return new MCoordinateSequence[0];
				}
				MCoordinate[] mcoords = (MCoordinate[])this.Coordinates;
				// ensure that we traverse the coordinate array in ascending M-order
				if (GetMeasureDirection() == MGeometryType.Decreasing)
				{
					CoordinateArrays.Reverse(mcoords);
				}

				double minM = Math.Min(fromM, toM);
				double maxM = Math.Max(fromM, toM);
				List<MCoordinate> mcolist = new List<MCoordinate>();
				for (int i = 0; i < mcoords.Length; i++)
				{
					if (mcolist.Count == 0 && mcoords[i].M >= minM)
					{
						MCoordinate mco2 = mcoords[i];
						if (DoubleComparator.Equals(mcoords[i].M, minM))
						{
							mcolist.Add(mco2);
						}
						else
						{
							MCoordinate mco1 = mcoords[i - 1];
							double r = (minM - mco1.M) / (mco2.M - mco1.M);
							Debug.Assert(DoubleComparator.Equals(mco1.M + r * (mco2.M - mco1.M), minM), "Error on assumption on r");
							MCoordinate mc = new MCoordinate(
								mco1.X + r * (mco2.X - mco1.X),
								mco1.Y + r * (mco2.Y - mco1.Y),
								mco1.Z + r * (mco2.Z - mco1.Z),
								minM);
							mcolist.Add(mc);
						}
					}
					else if (mcoords[i].M >= minM && mcoords[i].M <= maxM)
					{
						mcolist.Add(mcoords[i]);
						if (DoubleComparator.Equals(mcoords[i].M, maxM))
						{
							break;
						}
					}
					else if (mcoords[i].M > maxM)
					{
						// mcoords[i] > Math.max(fromM, toM
						Debug.Assert(i > 0, "mistaken assumption");
						MCoordinate mco2 = mcoords[i];
						MCoordinate mco1 = mcoords[i - 1];
						double r = (maxM - mco1.M) / (mco2.M - mco1.M);
						MCoordinate mc = new MCoordinate(
							mco1.X + r * (mco2.X - mco1.X),
							mco1.Y + r * (mco2.Y - mco1.Y),
							mco1.Z + r * (mco2.Z - mco1.Z),
							maxM
						);
						mcolist.Add(mc);
						break;
					}
				}
				// copy over, but only to the length of numPnts
				MCoordinate[] h = new MCoordinate[mcolist.Count];
				for (int i = 0; i < mcolist.Count; i++)
				{
					h[i] = (MCoordinate)mcolist[i];
				}

				if (!DoubleComparator.Equals(minM, fromM))
				{
					CoordinateArrays.Reverse(h);
				}

				MCoordinateSequence mc2 = new MCoordinateSequence(h);
				return new MCoordinateSequence[] { mc2 };
			}
		}

		/**
		 * todo consider refactoring to add INCREASING_STRICT and DECREASING_STRICT
		 * determine the direction of the measures w.r.t. the direction of the line
		 * 
		 * @return IMGeometry.NonMonotone<BR>
		 *         IMGeometry.Increasing<BR>
		 *         IMGeometry.Decreasing<BR>
		 *         IMGeometry.Constant
		 */
		public MGeometryType GetMeasureDirection()
		{
			if (!this.monotone)
			{
				return MGeometryType.NonMonotone;
			}
			MCoordinate c1 = (MCoordinate)this.GetCoordinateN(0);
			MCoordinate c2 = (MCoordinate)this.GetCoordinateN(this.NumPoints - 1);

			if (c1.M < c2.M)
			{
				return MGeometryType.Increasing;
			}
			else if (c1.M > c2.M)
			{
				return MGeometryType.Decreasing;
			}
			else
			{
				return MGeometryType.Constant;
			}
		}

		/**
		 * @return the array with measure-values of the vertices
		 */
		public double[] GetMeasures()
		{
			// return the measures of all vertices
			if (!this.IsEmpty)
			{
				ICoordinate[] co = this.Coordinates;
				double[] a = new double[co.Length];
				for (int i = 0; i < co.Length; i++)
				{
					a[i] = ((MCoordinate)co[i]).M;
				}
				return a;
			}
			else
			{
				return null;
			}
		}

		public double GetMinM()
		{

			if (this.IsEmpty)
			{
				return Double.NaN;
			}
			else
			{
				double[] a = this.GetMeasures();
				if (this.GetMeasureDirection() == MGeometryType.Increasing)
				{
					return a[0];
				}
				else if (this.GetMeasureDirection() == MGeometryType.Decreasing
						|| this.GetMeasureDirection() == MGeometryType.Constant)
				{
					return a[a.Length - 1];
				}
				else
				{

					double ma = Double.PositiveInfinity;
					for (int i = 0; i < a.Length; i++)
					{
						if (ma > a[i])
						{
							ma = a[i];
						}
					}
					return ma;
				}
			}
		}

		/**
		 * Assigns the first coordinate in the CoordinateSequence to the
		 * <code>beginMeasure</code> and the last coordinate in the
		 * CoordinateSequence to the <code>endMeasure</code>. Measure values for
		 * intermediate coordinates are then interpolated proportionally based on
		 * their 2d offset of the overall 2d length of the LineString.
		 * <p>
		 * If the beginMeasure and endMeasure values are equal it is assumed that
		 * all intermediate coordinates shall be the same value.
		 * 
		 * @param beginMeasure
		 *            Measure value for first coordinate
		 * @param endMeasure
		 *            Measure value for last coordinate
		 */
		public void Interpolate(double beginMeasure, double endMeasure)
		{
			if (this.IsEmpty)
			{
				return;
			}
			// interpolate with first vertex = beginMeasure; last vertex =
			// endMeasure
			ICoordinate[] coordinates = this.Coordinates;
			double length = this.Length;
			double mLength = endMeasure - beginMeasure;
			double d = 0;
			bool continuous = DoubleComparator.Equals(beginMeasure, endMeasure);
			double m = beginMeasure;
			MCoordinate prevCoord = MCoordinate.ConvertCoordinate(coordinates[0]);
			prevCoord.M = m;
			MCoordinate curCoord;
			for (int i = 1; i < coordinates.Length; i++)
			{
				curCoord = MCoordinate.ConvertCoordinate(coordinates[i]);
				if (continuous)
				{
					curCoord.M = beginMeasure;
				}
				else
				{
					d += curCoord.Distance(prevCoord);
					m = beginMeasure + (d / length) * mLength;
					curCoord.M = m;
					prevCoord = curCoord;
				}
			}
			this.GeometryChanged();
			Debug.Assert(this.IsMonotone(false), "interpolate function should always leave IMGeometry monotone");
		}

		/**
		 * Returns the measure length of the segment. This method assumes that the
		 * length of the LineString is defined by the absolute value of (last
		 * coordinate - first coordinate) in the CoordinateSequence. If either
		 * measure is not defined or the CoordinateSequence contains no coordinates,
		 * then Double.NaN is returned. If there is only 1 element in the
		 * CoordinateSequence, then 0 is returned.
		 * 
		 * @return The measure length of the LineString
		 */
		public double GetMLength()
		{
			if (CoordinateSequence.Count == 0)
			{
				return Double.NaN;
			}
			if (CoordinateSequence.Count == 1)
			{
				return 0.0D;
			}

			int lastIndex = CoordinateSequence.Count - 1;
			double begin = CoordinateSequence.GetOrdinate(0, Ordinates.M);
			double end = CoordinateSequence.GetOrdinate(lastIndex, Ordinates.M);
			return (Double.IsNaN(begin) || Double.IsNaN(end)) ? Double.NaN : Math.Abs(end - begin);
		}

		/**
		 * Indicates whether the MLineString has monotone increasing or decreasing
		 * M-values
		 * 
		 * @return <code>true if MLineString is empty or M-values are increasing (NaN) values, false otherwise</code>
		 */
		public bool IsMonotone(bool strict)
		{
			return strict ? this.strictMonotone : this.monotone;
		}

		// TODO get clear on function and implications of normalize
		// public void normalize(){
		//
		// }

		public void MeasureOnLength(bool keepBeginMeasure)
		{

			ICoordinate[] co = this.Coordinates;
			if (!this.IsEmpty)
			{
				double d = 0.0;
				MCoordinate pco = (MCoordinate)co[0];
				if (!keepBeginMeasure || Double.IsNaN(pco.M))
				{
					pco.M = 0.0d;
				}
				MCoordinate mco;
				for (int i = 1; i < co.Length; i++)
				{
					mco = (MCoordinate)co[i];
					d += mco.Distance(pco);
					mco.M = d;
					pco = mco;
				}
				this.GeometryChanged();
			}
		}

		/**
		 * This method reverses the measures assigned to the Coordinates in the
		 * CoordinateSequence without modifying the positional (x,y,z) values.
		 */
		public void ReverseMeasures()
		{
			if (!this.IsEmpty)
			{
				double[] m = this.GetMeasures();
				MCoordinate[] coar = Array.ConvertAll<ICoordinate, MCoordinate>(
					this.Coordinates,
					delegate(ICoordinate x) { return (MCoordinate)x; });
				for (int i = 0; i < m.Length; i++)
				{
					double nv = m[m.Length - 1 - i];
					coar[i].M = nv;
				}
				this.GeometryChanged();
			}
		}

		public void SetMeasureAtIndex(int index, double m)
		{
			CoordinateSequence.SetOrdinate(index, Ordinates.M, m);
			this.GeometryChanged();
		}

		/**
		 * Shift all measures by the amount parameter. A negative amount shall
		 * subtract the amount from the measure. Note that this can make for
		 * negative measures.
		 * 
		 * @param amount
		 *            the positive or negative amount by which to shift the measures
		 *            in the CoordinateSequence.
		 */
		public void ShiftMeasure(double amount)
		{
			ICoordinate[] coordinates = this.Coordinates;
			MCoordinate mco;
			if (!this.IsEmpty)
			{
				for (int i = 0; i < coordinates.Length; i++)
				{
					mco = (MCoordinate)coordinates[i];
					mco.M = mco.M + amount;
				}
			}
			this.GeometryChanged();
		}

		/*
		 * (non-Javadoc)
		 * 
		 * @see java.lang.Object#toString()
		 */
		public override String ToString()
		{
			ICoordinate[] ar = this.Coordinates;
			StringBuilder buf = new StringBuilder(ar.Length * 17 * 3);
			for (int i = 0; i < ar.Length; i++)
			{
				buf.Append(ar[i].X);
				buf.Append(" ");
				buf.Append(ar[i].Y);
				buf.Append(" ");
				buf.Append(((MCoordinate)ar[i]).M);
				buf.Append("\n");
			}
			return buf.ToString();
		}

		public MLineString UnionM(MLineString l)
		{

			if (!this.monotone || !l.monotone)
			{
				throw new ApplicationException("MGeometryException.OPERATION_REQUIRES_MONOTONE");
			}
			ICoordinate[] linecoar = l.Coordinates;
			if (l.GetMeasureDirection() == MGeometryType.Decreasing)
			{
				CoordinateArrays.Reverse(linecoar);
			}
			ICoordinate[] thiscoar = this.Coordinates;
			if (this.GetMeasureDirection() == MGeometryType.Decreasing)
			{
				CoordinateArrays.Reverse(thiscoar);
			}

			// either the last coordinate in thiscoar Equals the first in linecoar;
			// or the last in linecoar Equals the first in thiscoar;
			MCoordinate lasttco = (MCoordinate)thiscoar[thiscoar.Length - 1];
			MCoordinate firsttco = (MCoordinate)thiscoar[0];
			MCoordinate lastlco = (MCoordinate)linecoar[linecoar.Length - 1];
			MCoordinate firstlco = (MCoordinate)linecoar[0];

			MCoordinate[] newcoar = new MCoordinate[thiscoar.Length
					+ linecoar.Length - 1];
			if (lasttco.Equals2D(firstlco)
					&& DoubleComparator.Equals(lasttco.M, firstlco.M))
			{
				Array.Copy(thiscoar, 0, newcoar, 0, thiscoar.Length);
				Array.Copy(linecoar, 1, newcoar, thiscoar.Length,
						linecoar.Length - 1);
			}
			else if (lastlco.Equals2D(firsttco)
					&& DoubleComparator.Equals(lastlco.M, firsttco.M))
			{
				Array.Copy(linecoar, 0, newcoar, 0, linecoar.Length);
				Array.Copy(thiscoar, 1, newcoar, linecoar.Length,
						thiscoar.Length - 1);
			}
			else
			{
				throw new ApplicationException("MGeometryException.UNIONM_ON_DISJOINT_MLINESTRINGS");
			}

			ICoordinateSequence mcs = this.Factory.CoordinateSequenceFactory.Create(newcoar);
			MLineString returnmlinestring = new MLineString(mcs, this.Factory);
			Debug.Assert(returnmlinestring.IsMonotone(false), "new UnionM-ed MLineString is not monotone");
			return returnmlinestring;
		}
	}
}
