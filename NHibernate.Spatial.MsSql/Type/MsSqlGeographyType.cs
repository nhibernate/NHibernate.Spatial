using System;

namespace NHibernate.Spatial.Type
{
    [Serializable]
    public class MsSqlGeographyType : MsSqlGeometryType
    {
        protected override bool IsGeography => true;
    }
}
