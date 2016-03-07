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

using GeoAPI.Geometries;
using NetTopologySuite.Algorithm;
using NHibernate.Spatial.MGeometries;
using System;

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

            //return null;
            var geom = this.WriteGeometry(geometry);
            geom.Sdo_Srid = geometry.SRID;
            return geom;
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
            var dim = this.GetCoordDimension(geometry);
            var lrsDim = this.GetCoordinateLrsPosition(geometry);
            var isLrs = (lrsDim != 0);
            var coord = this.ConvertCoordinates(geometry.Coordinates, dim, isLrs);
            var sdoGeometry = new SdoGeometry
                                  {
                                      GeometryType = SDO_GTYPE.POINT,
                                      Dimensionality = dim,
                                      LRS = lrsDim,
                                      Sdo_Srid = geometry.SRID,
                                      ElemArray = new[] { 1, (decimal)SDO_ETYPE.ETYPE_SIMPLE_POINT, 1 },
                                      OrdinatesArray = coord
                                  };
            return sdoGeometry;
        }

        private SdoGeometry WriteLineString(IGeometry geometry)
        {
            var dim = this.GetCoordDimension(geometry);
            var lrsPos = this.GetCoordinateLrsPosition(geometry);
            var isLrs = lrsPos > 0;
            var ordinates = this.ConvertCoordinates(geometry.Coordinates, dim, isLrs);
            var elementType = ElementType.LINE_STRAITH_SEGMENTS;
            var sdoGeometry = new SdoGeometry
                                  {
                                      GeometryType = SDO_GTYPE.LINE,
                                      Dimensionality = dim,
                                      LRS = lrsPos,
                                      Sdo_Srid = geometry.SRID,
                                      ElemArray = new[] { 1, elementType.EType(), elementType.Interpretation() },
                                      OrdinatesArray = ordinates
                                  };
            return sdoGeometry;
        }

        private SdoGeometry WriteMultiLineString(IGeometry geometry)
        {
            var dim = this.GetCoordDimension(geometry);
            var lrsDim = this.GetCoordinateLrsPosition(geometry);
            var isLrs = (lrsDim != 0);
            var sdoGeometry = new SdoGeometry
                                  {
                                      GeometryType = SDO_GTYPE.MULTILINE,
                                      Dimensionality = dim,
                                      LRS = lrsDim,
                                      Sdo_Srid = geometry.SRID
                                  };
            var info = new decimal[geometry.NumGeometries * 3];
            var oordinatesOffset = 1;
            var ordinates = new decimal[] { };
            var elementType = ElementType.LINE_STRAITH_SEGMENTS;
            for (var i = 0; i < geometry.NumGeometries; i++)
            {
                info[i + 0] = oordinatesOffset;
                info[i + 1] = elementType.EType();
                info[i + 2] = elementType.Interpretation();
                ordinates = this.ConvertAddCoordinates(ordinates, geometry.GetGeometryN(i).Coordinates, dim, isLrs);
                oordinatesOffset = ordinates.Length + 1;
            }
            sdoGeometry.ElemArray = info;
            sdoGeometry.OrdinatesArray = ordinates;
            return sdoGeometry;
        }

        private SdoGeometry WriteMultiPoint(IGeometry geometry)
        {
            var dim = this.GetCoordDimension(geometry);
            var lrsDim = this.GetCoordinateLrsPosition(geometry);
            var isLrs = (lrsDim != 0);
            var sdoGeometry = new SdoGeometry
                                  {
                                      GeometryType = SDO_GTYPE.MULTIPOINT,
                                      Dimensionality = dim,
                                      LRS = lrsDim,
                                      Sdo_Srid = geometry.SRID
                                  };

            var info = new decimal[geometry.NumPoints * 3];
            var oordinatesOffset = 1;
            var ordinates = new decimal[0];
            var elementType = ElementType.POINT;
            for (int i = 0; i < geometry.NumPoints; i++)
            {
                //info.setElement(i, oordinatesOffset, ElementType.POINT, 0);
                info[i + 0] = oordinatesOffset;
                info[i + 1] = elementType.EType();
                info[i + 2] = elementType.Interpretation();
                ordinates = this.ConvertAddCoordinates(ordinates, geometry.GetGeometryN(i).Coordinates, dim, isLrs);
                oordinatesOffset = ordinates.Length + 1;
            }
            sdoGeometry.ElemArray = info;
            sdoGeometry.OrdinatesArray = ordinates;
            return sdoGeometry;
        }

        private SdoGeometry WritePolygon(IGeometry geometry)
        {
            var dim = this.GetCoordDimension(geometry);
            var lrsPos = this.GetCoordinateLrsPosition(geometry);
            var sdoGeometry = new SdoGeometry
                                  {
                                      GeometryType = SDO_GTYPE.POLYGON,
                                      Dimensionality = dim,
                                      LRS = lrsPos,
                                      Sdo_Srid = geometry.SRID
                                  };
            this.AddPolygon(sdoGeometry, geometry as IPolygon);
            return sdoGeometry;
        }

        private SdoGeometry WriteMultiPolygon(IGeometry geometry)
        {
            var dim = this.GetCoordDimension(geometry);
            var lrsPos = this.GetCoordinateLrsPosition(geometry);
            var sdoGeometry = new SdoGeometry
                                  {
                                      GeometryType = SDO_GTYPE.MULTIPOLYGON,
                                      Dimensionality = dim,
                                      LRS = lrsPos,
                                      Sdo_Srid = geometry.SRID
                                  };
            for (int i = 0; i < geometry.NumGeometries; i++)
            {
                try
                {
                    var pg = (IPolygon)geometry.GetGeometryN(i);
                    this.AddPolygon(sdoGeometry, pg);
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
            var sdoElements = new SdoGeometry[geometry.NumGeometries];
            for (var i = 0; i < geometry.NumGeometries; i++)
            {
                var sdoGeometry = geometry.GetGeometryN(i);
                sdoElements[i] = this.WriteGeometry(sdoGeometry);
            }
            return SdoGeometry.Join(sdoElements);
        }

        private void AddPolygon(SdoGeometry sdoGeometry, IPolygon polygon)
        {
            int numInteriorRings = polygon.NumInteriorRings;
            var info = new decimal[(numInteriorRings + 1) * 3];
            int ordinatesPreviousOffset = 0;
            if (sdoGeometry.OrdinatesArray != null)
            {
                ordinatesPreviousOffset = sdoGeometry.OrdinatesArray.Length;
            }
            int ordinatesOffset = ordinatesPreviousOffset + 1;
            var ordinates = new decimal[] { };
            for (int i = 0; i < info.Length; i = i + 3)
            {
                ElementType et;
                Coordinate[] coords;
                if (i == 0)
                {
                    et = ElementType.EXTERIOR_RING_STRAIGHT_SEGMENTS;
                    coords = polygon.ExteriorRing.Coordinates;
                    
                    // 1003: exterior polygon ring (must be specified in counterclockwise order)
                    if (!CGAlgorithms.IsCCW(coords))
                    {
                        coords = this.ReverseRing(coords);
                    }
                }
                else
                {
                    et = ElementType.INTERIOR_RING_STRAIGHT_SEGMENTS;
                    coords = polygon.InteriorRings[i - 1].Coordinates;
                    
                    // 2003: interior polygon ring (must be specified in clockwise order)
                    if (CGAlgorithms.IsCCW(coords))
                    {
                        coords = this.ReverseRing(coords);
                    }
                }
                //info.setElement(i, ordinatesOffset, et, 0);
                info[i + 0] = ordinatesOffset;
                info[i + 1] = et.EType();
                info[i + 2] = et.Interpretation();
                ordinates = this.ConvertAddCoordinates(ordinates, coords, sdoGeometry.Dimensionality, sdoGeometry.LRS > 0);
                ordinatesOffset = ordinatesPreviousOffset + ordinates.Length + 1;
            }
            sdoGeometry.addElement(info);
            sdoGeometry.AddOrdinates(ordinates);
        }

        // reverses ordinates in a coordinate array in-place
        private Coordinate[] ReverseRing(Coordinate[] ar)
        {
            for (int i = 0; i < ar.Length / 2; i++)
            {
                Coordinate cs = ar[i];
                ar[i] = ar[ar.Length - 1 - i];
                ar[ar.Length - 1 - i] = cs;
            }
            return ar;
        }

        private decimal[] ConvertAddCoordinates(decimal[] ordinates, Coordinate[] coordinates, int dim, bool isLrs)
        {
            var no = this.ConvertCoordinates(coordinates, dim, isLrs);
            var newordinates = new decimal[ordinates.Length + no.Length];
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

        private decimal[] ConvertCoordinates(Coordinate[] coordinates, int dim, bool isLrs)
        {
            if (dim > 4)
            {
                throw new ArgumentException("Dim parameter value cannot be greater than 4");
            }
            var converted = new decimal[coordinates.Length * dim];
            for (int i = 0; i < coordinates.Length; i++)
            {
                Coordinate c = coordinates[i];
                MCoordinate cm = c as MCoordinate;

                // set the X and Y values
                converted[i * dim] = this.ToDecimal(c.X);
                converted[i * dim + 1] = this.ToDecimal(c.Y);
                if (dim == 3)
                {
                    converted[i * dim + 2] = isLrs ? this.ToDecimal(cm.M) : this.ToDecimal(c.Z);
                    converted[i * dim + 2] = this.ToDecimal(c.Z);
                }
                else if (dim == 4)
                {
                    converted[i * dim + 2] = this.ToDecimal(c.Z);
                    converted[i * dim + 3] = this.ToDecimal(cm.M);
                }
            }
            return converted;
        }

        /**
         * This method converts a double primitive to a decimal instance, but
         * treats a Double.NaN value as null.
         *
         * @param d
         *            the value to be converted
         * @return A Double instance of d, Null if the parameter is Double.NaN
         */

        private decimal ToDecimal(double d)
        {
            return double.IsNaN(d) ? new decimal() : Convert.ToDecimal(d);
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
            Coordinate c = geom.Coordinate;
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