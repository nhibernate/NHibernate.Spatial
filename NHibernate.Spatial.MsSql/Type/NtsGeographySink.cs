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
using Microsoft.SqlServer.Types;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;

namespace NHibernate.Spatial.Type
{
    internal class NtsGeographySink : IGeographySink110
    {
        private IGeometry geometry;
        private int srid;
        private readonly Stack<OpenGisGeographyType> types = new Stack<OpenGisGeographyType>();
        private List<Coordinate> coordinates = new List<Coordinate>();
        private readonly List<Coordinate[]> rings = new List<Coordinate[]>();
        private readonly List<IGeometry> geometries = new List<IGeometry>();
        private bool inFigure;

        public IGeometry ConstructedGeometry
        {
            get { return this.geometry; }
        }

        private void AddCoordinate(double x, double y, double? z, double? m)
        {
            Coordinate coordinate;
            if (z.HasValue)
            {
                coordinate = new Coordinate(y, x, z.Value);
            }
            else
            {
                coordinate = new Coordinate(y, x);
            }
            this.coordinates.Add(coordinate);
        }

        #region IGeometrySink Members

        public void AddLine(double x, double y, double? z, double? m)
        {
            if (!this.inFigure)
            {
                throw new ApplicationException();
            }
            AddCoordinate(x, y, z, m);
        }

        public void BeginFigure(double x, double y, double? z, double? m)
        {
            if (this.inFigure)
            {
                throw new ApplicationException();
            }
            this.coordinates = new List<Coordinate>();
            AddCoordinate(x, y, z, m);
            this.inFigure = true;
        }

        public void BeginGeography(OpenGisGeographyType type)
        {
            this.types.Push(type);
        }

        public void EndFigure()
        {
            OpenGisGeographyType type = this.types.Peek();
            if (type == OpenGisGeographyType.Polygon)
            {
                this.rings.Add(this.coordinates.ToArray());
            }
            this.inFigure = false;
        }

        public void EndGeography()
        {
            IGeometry geometry = null;

            OpenGisGeographyType type = this.types.Pop();

            switch (type)
            {
                case OpenGisGeographyType.Point:
                    geometry = BuildPoint();
                    break;

                case OpenGisGeographyType.LineString:
                    geometry = BuildLineString();
                    break;

                case OpenGisGeographyType.Polygon:
                    geometry = BuildPolygon();
                    break;

                case OpenGisGeographyType.MultiPoint:
                    geometry = BuildMultiPoint();
                    break;

                case OpenGisGeographyType.MultiLineString:
                    geometry = BuildMultiLineString();
                    break;

                case OpenGisGeographyType.MultiPolygon:
                    geometry = BuildMultiPolygon();
                    break;

                case OpenGisGeographyType.GeometryCollection:
                    geometry = BuildGeometryCollection();
                    break;
            }

            if (this.types.Count == 0)
            {
                this.geometry = geometry;
                this.geometry.SRID = this.srid;
            }
            else
            {
                this.geometries.Add(geometry);
            }
        }

        public void AddCircularArc(double x1, double y1, double? z1, double? m1, double x2, double y2, double? z2, double? m2)
        {
            throw new NotImplementedException();
        }

        private IGeometry BuildPoint()
        {
            return new Point(this.coordinates[0]);
        }

        private LineString BuildLineString()
        {
            return new LineString(this.coordinates.ToArray());
        }

        private IGeometry BuildPolygon()
        {
            if (this.rings.Count == 0)
            {
                return Polygon.Empty;
            }
            ILinearRing shell = new LinearRing(this.rings[0]);
            ILinearRing[] holes =
                this.rings.GetRange(1, this.rings.Count - 1)
                    .ConvertAll<ILinearRing>(delegate(Coordinate[] coordinates)
                    {
                        return new LinearRing(coordinates);
                    }).ToArray();
            this.rings.Clear();
            return new Polygon(shell, holes);
        }

        private IGeometry BuildMultiPoint()
        {
            IPoint[] points =
                this.geometries.ConvertAll<IPoint>(delegate(IGeometry g)
                {
                    return g as IPoint;
                }).ToArray();
            return new MultiPoint(points);
        }

        private IGeometry BuildMultiLineString()
        {
            ILineString[] lineStrings =
                this.geometries.ConvertAll<ILineString>(delegate(IGeometry g)
                {
                    return g as ILineString;
                }).ToArray();
            return new MultiLineString(lineStrings);
        }

        private IGeometry BuildMultiPolygon()
        {
            IPolygon[] polygons =
                this.geometries.ConvertAll<IPolygon>(delegate(IGeometry g)
                {
                    return g as IPolygon;
                }).ToArray();
            return new MultiPolygon(polygons);
        }

        private GeometryCollection BuildGeometryCollection()
        {
            return new GeometryCollection(this.geometries.ToArray());
        }

        public void SetSrid(int srid)
        {
            this.srid = srid;
        }

        #endregion IGeometrySink Members
    }
}