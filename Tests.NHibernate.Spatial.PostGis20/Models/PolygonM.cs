using System;
using NetTopologySuite.Geometries;

namespace Tests.NHibernate.Spatial.Models
{
    [Serializable]
    public class PolygonM
    {
        public int Id { get; set; }

        public Geometry Geom { get; set; }
    }
}
