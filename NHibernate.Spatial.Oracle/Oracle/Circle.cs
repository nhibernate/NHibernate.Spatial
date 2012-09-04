/**
 * $Id: Circle.java 71 2008-01-25 19:18:02Z maesenka $
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

/**
 * This class provides operations for handling the usage of Circles and arcs in
 * Geometries.
 * 
 * Date: Oct 15, 2007
 * 
 * @author Tom Acree
 */

namespace NHibernate.Spatial.Oracle
{
	internal class Circle
	{
		private const double TWO_PI = Math.PI * 2;
		private readonly ICoordinate center = new Coordinate(0.0, 0.0);
		private readonly IPrecisionModel precisionModel = new PrecisionModel();
		private double radius;

		/**
		 * Creates a circle whose center is at the origin and whose radius is 0.
		 */
		protected Circle()
		{
		}

		/**
		 * Create a circle with a defined center and radius
		 * 
		 * @param center
		 *            The coordinate representing the center of the circle
		 * @param radius
		 *            The radius of the circle
		 */
		public Circle(ICoordinate center, double radius)
		{
			this.center = center;
			this.radius = radius;
		}

		/**
		 * Create a circle using the x/y coordinates for the center.
		 * 
		 * @param xCenter
		 *            The x coordinate of the circle's center
		 * @param yCenter
		 *            The y coordinate of the circle's center
		 * @param radius
		 *            the radius of the circle
		 */
		public Circle(double xCenter, double yCenter, double radius)
			: this(new Coordinate(xCenter, yCenter), radius)
		{
		}

		/**
		 * Creates a circle based on bounding box. It is possible for the user of
		 * this class to pass bounds to this method that do not represent a square.
		 * If this is the case, we must force the bounding rectangle to be a square.
		 * To this end, we check the box and set the side of the box to the larger
		 * dimension of the rectangle
		 *
		 * @param xLeft
		 * @param yUpper
		 * @param xRight
		 * @param yLower
		 */
		public Circle(double xLeft, double yUpper, double xRight, double yLower)
		{
			double side = Math.Min(Math.Abs(xRight - xLeft), Math.Abs(yLower - yUpper));
			center.X = Math.Min(xRight, xLeft) + side / 2;
			center.Y = Math.Min(yUpper, yLower) + side / 2;
			radius = side / 2;
		}

		/**
		 * Three point method of circle construction. All three points must be on
		 * the circumference of the circle.
		 *
		 * @param point1
		 * @param point2
		 * @param point3
		 */
		public Circle(ICoordinate point1, ICoordinate point2, ICoordinate point3)
		{
			InitThreePointCircle(point1, point2, point3);
		}

		/**
		 * Three point method of circle construction. All three points must be on
		 * the circumference of the circle.
		 * 
		 * @param x1
		 * @param y1
		 * @param x2
		 * @param y2
		 * @param x3
		 * @param y3
		 */
		public Circle(double x1, double y1, double x2, double y2, double x3, double y3)
			: this(new Coordinate(x1, y1), new Coordinate(x2, y2), new Coordinate(x3, y3))
		{
		}

		public ICoordinate Center
		{
			get { return center; }
		}

		public double Radius
		{
			get { return radius; }
		}

		/**
		 * shift the center of the circle by delta X and delta Y
		 */
		public void Shift(double deltaX, double deltaY)
		{
			center.X = center.X + deltaX;
			center.Y = center.Y + deltaY;
		}

		/**
		 * Move the circle to a new center
		 */
		public void Move(double x, double y)
		{
			center.X = x;
			center.Y = y;
		}

		/**
		 * Defines the circle based on three points. All three points must be on on
		 * the circumference of the circle, and hence, the 3 points cannot be have
		 * any pair equal, and cannot form a line. Therefore, each point given is
		 * one radius measure from the circle's center.
		 * 
		 * @param p1
		 *            A point on the desired circle
		 * @param p2
		 *            A point on the desired circle
		 * @param p3
		 *            A point on the desired circle
		 */
		private void InitThreePointCircle(ICoordinate p1, ICoordinate p2, ICoordinate p3)
		{
			double a13, b13, c13;
			double a23, b23, c23;
			double x, y, rad;

			// begin pre-calculations for linear system reduction
			a13 = 2 * (p1.X - p3.X);
			b13 = 2 * (p1.Y - p3.Y);
			c13 = (p1.Y * p1.Y - p3.Y * p3.Y) + (p1.X * p1.X - p3.X * p3.X);
			a23 = 2 * (p2.X - p3.X);
			b23 = 2 * (p2.Y - p3.Y);
			c23 = (p2.Y * p2.Y - p3.Y * p3.Y) + (p2.X * p2.X - p3.X * p3.X);
			// test to be certain we have three distinct points passed
			double smallNumber = 0.01;
			if ((Math.Abs(a13) < smallNumber && Math.Abs(b13) < smallNumber)
			    || (Math.Abs(a13) < smallNumber && Math.Abs(b13) < smallNumber))
			{
				// // points too close so set to default circle
				x = 0;
				y = 0;
				rad = 0;
			}
			else
			{
				// everything is acceptable do the y calculation
				y = (a13 * c23 - a23 * c13) / (a13 * b23 - a23 * b13);
				// x calculation
				// choose best formula for calculation
				if (Math.Abs(a13) > Math.Abs(a23))
				{
					x = (c13 - b13 * y) / a13;
				}
				else
				{
					x = (c23 - b23 * y) / a23;
				}
				// radius calculation
				rad = Math.Sqrt((x - p1.X) * (x - p1.X) + (y - p1.Y) * (y - p1.Y));
			}
			center.X = x;
			center.Y = y;
			radius = rad;
		}

		/**
		 * Given 2 points defining an arc on the circle, interpolates the circle
		 * into a collection of points that provide connected chords that
		 * approximate the arc based on the tolerance value. The tolerance value
		 * specifies the maximum distance between a chord and the circle.
		 * 
		 * @param x1
		 *            x coordinate of point 1
		 * @param y1
		 *            y coordinate of point 1
		 * @param x2
		 *            x coordinate of point 2
		 * @param y2
		 *            y coordinate of point 2
		 * @param x3
		 *            x coordinate of point 3
		 * @param y3
		 *            y coordinate of point 3
		 * @param tolerence
		 *            maximum distance between the center of the chord and the outer
		 *            edge of the circle
		 * @return an ordered list of Coordinates representing a series of chords
		 *         approximating the arc.
		 */
		public static ICoordinate[] LinearizeArc(
			double x1, double y1, 
			double x2, double y2, 
			double x3, double y3, 
			double tolerence)
		{
			ICoordinate p1 = new Coordinate(x1, y1);
			ICoordinate p2 = new Coordinate(x2, y2);
			ICoordinate p3 = new Coordinate(x3, y3);
			return new Circle(p1, p2, p3).LinearizeArc(p1, p2, p3, tolerence);
		}

		/**
		 * Given 2 points defining an arc on the circle, interpolates the circle
		 * into a collection of points that provide connected chords that
		 * approximate the arc based on the tolerance value. This method uses a
		 * tolerence value of 1/100 of the length of the radius.
		 * 
		 * @param x1
		 *            x coordinate of point 1
		 * @param y1
		 *            y coordinate of point 1
		 * @param x2
		 *            x coordinate of point 2
		 * @param y2
		 *            y coordinate of point 2
		 * @param x3
		 *            x coordinate of point 3
		 * @param y3
		 *            y coordinate of point 3
		 * @return an ordered list of Coordinates representing a series of chords
		 *         approximating the arc.
		 */
		public static ICoordinate[] LinearizeArc(
			double x1, double y1, 
			double x2, double y2, 
			double x3, double y3)
		{
			ICoordinate p1 = new Coordinate(x1, y1);
			ICoordinate p2 = new Coordinate(x2, y2);
			ICoordinate p3 = new Coordinate(x3, y3);
			Circle c = new Circle(p1, p2, p3);
			double tolerence = 0.01 * c.Radius;
			return c.LinearizeArc(p1, p2, p3, tolerence);
		}

		/**
		 * Given a circle defined by the 3 points, creates a linearized
		 * interpolation of the circle starting and ending on the first coordinate.
		 * This method uses a tolerence value of 1/100 of the length of the radius.
		 * 
		 * @param x1
		 *            x coordinate of point 1
		 * @param y1
		 *            y coordinate of point 1
		 * @param x2
		 *            x coordinate of point 2
		 * @param y2
		 *            y coordinate of point 2
		 * @param x3
		 *            x coordinate of point 3
		 * @param y3
		 *            y coordinate of point 3
		 * @return an ordered list of Coordinates representing a series of chords
		 *         approximating the arc.
		 */
		public static ICoordinate[] LinearizeCircle(
			double x1, double y1, 
			double x2, double y2, 
			double x3, double y3)
		{
			ICoordinate p1 = new Coordinate(x1, y1);
			ICoordinate p2 = new Coordinate(x2, y2);
			ICoordinate p3 = new Coordinate(x3, y3);
			Circle c = new Circle(p1, p2, p3);
			double tolerence = 0.01 * c.Radius;
			return c.LinearizeArc(p1, p2, p1, tolerence);
		}

		/**
		 * Given 2 points defining an arc on the circle, interpolates the circle
		 * into a collection of points that provide connected chords that
		 * approximate the arc based on the tolerance value. The tolerance value
		 * specifies the maximum distance between a chord and the circle.
		 * 
		 * @param p1
		 *            begin coordinate of the arc
		 * @param p2
		 *            any other point on the arc
		 * @param p3
		 *            end coordinate of the arc
		 * @param tolerence
		 *            maximum distance between the center of the chord and the outer
		 *            edge of the circle
		 * @return an ordered list of Coordinates representing a series of chords
		 *         approximating the arc.
		 */
		public ICoordinate[] LinearizeArc(
			ICoordinate p1, ICoordinate p2,
		    ICoordinate p3, double tolerence)
		{
			Arc arc = CreateArc(p1, p2, p3);
			List<ICoordinate> result = LinearizeInternal(null, arc, tolerence);
			return result.ToArray();
		}

		private static List<ICoordinate> LinearizeInternal(
			List<ICoordinate> coordinates,
		    Arc arc, double tolerence)
		{
			if (coordinates == null)
			{
				coordinates = new List<ICoordinate>();
			}
			double arcHt = arc.GetArcHeight();
			if (arcHt <= tolerence)
			{
				int lastIndex = coordinates.Count - 1;
				ICoordinate lastCoord = lastIndex >= 0 ? coordinates[lastIndex] : null;

				if (lastCoord == null || !arc.GetP1().Equals2D(lastCoord))
				{
					coordinates.Add(arc.GetP1());
					coordinates.Add(arc.GetP2());
				}
				else
				{
					coordinates.Add(arc.GetP2());
				}
			}
			else
			{
				// otherwise, split
				Arc[] splits = arc.Split();
				LinearizeInternal(coordinates, splits[0], tolerence);
				LinearizeInternal(coordinates, splits[1], tolerence);
			}
			return coordinates;
		}

		public bool Equals(Circle obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}
			if (ReferenceEquals(this, obj))
			{
				return true;
			}
			if (obj.radius != radius)
			{
				return false;
			}
			if (center != null
					? !center.Equals2D(obj.center)
					: obj.center != null)
			{
				return false;
			}
			return true;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}
			if (ReferenceEquals(this, obj))
			{
				return true;
			}
			if (obj.GetType() != typeof(Circle))
			{
				return false;
			}
			return Equals((Circle)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (center.GetHashCode() * 397) ^ radius.GetHashCode();
			}
		}

		public override string ToString()
		{
			return "Circle with Radius = " + radius
				   + " and a center at the coordinates (" + center.X + ", "
				   + center.Y + ")";
		}

		/**
		 * Returns the angle of the point from the center and the horizontal line
		 * from the center.
		 * 
		 * @param p
		 *            a point in space
		 * @return The angle of the point from the center of the circle
		 */
		public double GetAngle(ICoordinate p)
		{
			double dx = p.X - center.X;
			double dy = p.Y - center.Y;

			//return Math.Atan2(dy, dx); // ??

			double angle;

			if (dx == 0.0)
			{
				if (dy == 0.0)
				{
					angle = 0.0;
				}
				else if (dy > 0.0)
				{
					angle = Math.PI / 2.0;
				}
				else
				{
					angle = (Math.PI * 3.0) / 2.0;
				}
			}
			else if (dy == 0.0)
			{
				angle = dx > 0.0 ? 0.0 : Math.PI;
			}
			else
			{
				if (dx < 0.0)
				{
					angle = Math.Atan(dy / dx) + Math.PI;
				}
				else if (dy < 0.0)
				{
					angle = Math.Atan(dy / dx) + (2 * Math.PI);
				}
				else
				{
					angle = Math.Atan(dy / dx);
				}
			}
			return angle;
		}

		public ICoordinate GetPoint(double angle)
		{
			double x = Math.Cos(angle) * radius;
			x = x + center.X;
			x = precisionModel.MakePrecise(x);

			double y = Math.Sin(angle) * radius;
			y = y + center.Y;
			y = precisionModel.MakePrecise(y);

			return new Coordinate(x, y);
		}

		/**
		 * @param p
		 *            A point in space
		 * @return The distance the point is from the center of the circle
		 */
		public double DistanceFromCenter(ICoordinate p)
		{
			return Math.Abs(center.Distance(p));
		}

		public Arc CreateArc(ICoordinate p1, ICoordinate p2, ICoordinate p3)
		{
			return new Arc(this, p1, p2, p3);
		}

		/**
		 * Returns an angle between 0 and 2*PI. For example, 4*PI would get returned
		 * as 2*PI since they are equivalent.
		 * 
		 * @param angle
		 *            an angle in radians to normalize
		 * @return an angle between 0 and 2*PI
		 */
		public static double NormalizeAngle(double angle)
		{
			const double maxRadians = 2 * Math.PI;
			if (angle >= 0 && angle <= maxRadians)
			{
				return angle;
			}
			if (angle < 0)
			{
				return maxRadians - Math.Abs(angle);
			}
			else
			{
				return angle % maxRadians;
			}
		}

		/**
		 * Returns the angle between the angles a1 and a2 in radians. Angle is
		 * calculated in the counterclockwise direction.
		 * 
		 * @param a1
		 *            first angle
		 * @param a2
		 *            second angle
		 * @return the angle between a1 and a2 in the clockwise direction
		 */
		public static double SubtractAngles(double a1, double a2)
		{
			if (a1 < a2)
			{
				return a2 - a1;
			}
			else
			{
				return TWO_PI - Math.Abs(a2 - a1);
			}
		}

		#region Nested type: Arc

		public class Arc
		{
			private readonly Circle circle;

			private readonly bool clockwise;
			private readonly ICoordinate p1;

			private readonly double p1Angle;
			private readonly ICoordinate p2;

			private readonly double p2Angle;
			private double arcAngle; // angle in radians

			internal Arc(Circle circle, ICoordinate p1, ICoordinate midPt, ICoordinate p2)
			{
				this.circle = circle;
				this.p1 = p1;
				this.p2 = p2;
				p1Angle = circle.GetAngle(p1);
				// See if this arc covers the whole circle
				if (p1.Equals2D(p2))
				{
					p2Angle = TWO_PI + p1Angle;
					arcAngle = TWO_PI;
				}
				else
				{
					p2Angle = circle.GetAngle(p2);
					double midPtAngle = circle.GetAngle(midPt);

					// determine the direction
					double ccDegrees = SubtractAngles(p1Angle,
					                                  midPtAngle)
					                   + SubtractAngles(midPtAngle, p2Angle);

					if (ccDegrees < TWO_PI)
					{
						clockwise = false;
						arcAngle = ccDegrees;
					}
					else
					{
						clockwise = true;
						arcAngle = TWO_PI - ccDegrees;
					}
				}
			}

			private Arc(Circle circle, ICoordinate p1, ICoordinate p2, bool isClockwise)
			{
				this.p1 = p1;
				this.p2 = p2;
				clockwise = isClockwise;
				p1Angle = circle.GetAngle(p1);
				if (p1.Equals2D(p2))
				{
					p2Angle = TWO_PI + p1Angle;
				}
				else
				{
					p2Angle = circle.GetAngle(p2);
				}
				DetermineArcAngle();
			}

			private void DetermineArcAngle()
			{
				double diff;
				if (p1.Equals2D(p2))
				{
					diff = TWO_PI;
				}
				else if (clockwise)
				{
					diff = p1Angle - p2Angle;
				}
				else
				{
					diff = p2Angle - p1Angle;
				}
				arcAngle = NormalizeAngle(diff);
			}

			/**
			 * given a an arc defined from p1 to p2 existing on this circle, returns
			 * the height of the arc. This height is defined as the distance from
			 * the center of a chord defined by (p1, p2) and the outer edge of the
			 * circle.
			 * 
			 * @return the arc height
			 */
			public double GetArcHeight()
			{
				ICoordinate chordCenterPt = GetChordCenterPoint();
				double dist = circle.DistanceFromCenter(chordCenterPt);
				if (arcAngle > Math.PI)
				{
					return circle.Radius + dist;
				}
				else
				{
					return circle.Radius - dist;
				}
			}

			public ICoordinate GetChordCenterPoint()
			{
				double centerX = p1.X + (p2.X - p1.X) / 2;
				double centerY = p1.Y + (p2.Y - p1.Y) / 2;
				return new Coordinate(centerX, centerY);
			}

			public Arc[] Split()
			{
				int directionFactor = IsClockwise() ? -1 : 1;
				double angleOffset = directionFactor * (arcAngle / 2);

				double midAngle = p1Angle + angleOffset;
				ICoordinate newMidPoint = circle.GetPoint(midAngle);

				Arc arc1 = new Arc(circle, p1, newMidPoint, IsClockwise());
				Arc arc2 = new Arc(circle, newMidPoint, p2, IsClockwise());
				return new Arc[] {arc1, arc2};
			}

			public ICoordinate GetP1()
			{
				return p1;
			}

			public ICoordinate GetP2()
			{
				return p2;
			}

			public double GetArcAngle()
			{
				return arcAngle;
			}

			public double GetArcAngleDegrees()
			{
				return arcAngle * 180 / Math.PI;
			}

			public double GetP1Angle()
			{
				return p1Angle;
			}

			public double GetP2Angle()
			{
				return p2Angle;
			}

			public bool IsClockwise()
			{
				return clockwise;
			}

			public override string ToString()
			{
				return "P1: " + p1 + " P2: " + p2 + " clockwise: " + clockwise;
			}
		}

		#endregion

	}
}