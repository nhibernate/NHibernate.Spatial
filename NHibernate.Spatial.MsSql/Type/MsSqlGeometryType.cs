using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using GeoAPI.Geometries;
using NetTopologySuite.IO;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.Type;

namespace NHibernate.Spatial.Type
{
    [Serializable]
    public class MsSqlGeometryType : GeometryTypeBase<byte[]>
    {
        private static readonly NullableType GeometryType = new CustomGeometryType();

        public MsSqlGeometryType()
            : base(GeometryType)
        { }

        protected virtual bool IsGeography => false;

        protected override byte[] FromGeometry(object value)
        {
            var geometry = value as IGeometry;
            if (geometry == null)
            {
                return null;
            }

            SetDefaultSRID(geometry);
            var writer = new SqlServerBytesWriter { IsGeography = IsGeography };
            var bytes = writer.Write(geometry);
            return bytes;
        }

        protected override IGeometry ToGeometry(object value)
        {
            var bytes = value as byte[];
            if (bytes == null || bytes.Length == 0)
            {
                return null;
            }

            var reader = new SqlServerBytesReader { IsGeography = IsGeography };
            var geometry = reader.Read(bytes);
            SetDefaultSRID(geometry);
            return geometry;
        }

        [Serializable]
        private class CustomGeometryType : ImmutableType
        {
            public CustomGeometryType()
                : base(new BinarySqlType())
            { }

            public override string Name => ReturnedClass.Name;

            public override System.Type ReturnedClass => typeof(IGeometry);

            public override void Set(DbCommand cmd, object value, int index, ISessionImplementor session)
            {
                var parameter = (SqlParameter) cmd.Parameters[index];
                parameter.SqlDbType = SqlDbType.Binary;
                parameter.Value = value;
            }

            public override object Get(DbDataReader rs, int index, ISessionImplementor session)
            {
                var length = (int) rs.GetBytes(index, 0, null, 0, 0);
                var buffer = new byte[length];
                if (length > 0)
                {
                    rs.GetBytes(index, 0, buffer, 0, length);
                }
                return buffer;
            }

            public override object Get(DbDataReader rs, string name, ISessionImplementor session)
            {
                return Get(rs, rs.GetOrdinal(name), session);
            }
        }
    }
}
