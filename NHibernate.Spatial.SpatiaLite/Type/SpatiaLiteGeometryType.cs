using System;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace NHibernate.Spatial.Type
{
    [Serializable]
    public class SpatiaLiteGeometryType : GeometryTypeBase<byte[]>
    {
        private readonly GaiaGeoReader _reader;
        private readonly GaiaGeoWriter _writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpatiaLiteGeometryType"/> class.
        /// </summary>
        public SpatiaLiteGeometryType()
            : base(NHibernateUtil.BinaryBlob)
        {
            _reader = new GaiaGeoReader();
            _writer = new GaiaGeoWriter();
        }

        /// <summary>
        /// Convert a <see cref="Geometry"/> to a SpatiaLite byte array.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override byte[] FromGeometry(object value)
        {
            var geometry = value as Geometry;
            if (geometry == null)
            {
                return null;
            }

            SetDefaultSRID(geometry);

            // Ensure HandleOrdinates is set according to the ordinality of the geometry
            // https://github.com/NetTopologySuite/NetTopologySuite.IO.SpatiaLite/issues/7
            var ordinates = Ordinates.XY;
            var coordinate = geometry.Coordinate;
            if (coordinate != null && !double.IsNaN(coordinate.Z))
            {
                ordinates |= Ordinates.Z;
            }
            if (coordinate != null && !double.IsNaN(coordinate.M))
            {
                ordinates |= Ordinates.M;
            }
            _writer.HandleOrdinates = ordinates;

            return _writer.Write(geometry);
        }

        /// <summary>
        /// Convert a SpatiaLite byte array to a <see cref="Geometry"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override Geometry ToGeometry(object value)
        {
            if (!(value is byte[] bytes))
            {
                return null;
            }

            var geometry = _reader.Read(bytes);
            SetDefaultSRID(geometry);

            return geometry;
        }
    }
}
