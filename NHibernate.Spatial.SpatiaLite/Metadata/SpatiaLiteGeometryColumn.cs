using System;

namespace NHibernate.Spatial.Metadata
{
    public class SpatiaLiteGeometryColumn : GeometryColumn
    {
        /// <summary>
        /// Gets or sets the geometry subtype.
        /// </summary>
        /// <value>The subtype.</value>
        public override string Subtype
        {
            get
            {
                switch (GeometryType)
                {
                    case 1:
                        return "POINT";
                    case 2:
                        return "LINESTRING";
                    case 3:
                        return "POLYGON";
                    case 4:
                        return "MULTIPOINT";
                    case 5:
                        return "MULTILINESTRING";
                    case 6:
                        return "MULTIPOLYGON";
                    case 7:
                        return "GEOMETRYCOLLECTION";
                }
                throw new Exception("Should never reach here");
            }
            set
            {
                switch (value)
                {
                    case "POINT":
                        GeometryType = 1;
                        break;
                    case "LINESTRING":
                        GeometryType = 2;
                        break;
                    case "POLYGON":
                        GeometryType = 3;
                        break;
                    case "MULTIPOINT":
                        GeometryType = 4;
                        break;
                    case "MULTILINESTRING":
                        GeometryType = 5;
                        break;
                    case "MULTIPOLYGON":
                        GeometryType = 6;
                        break;
                    case "GEOMETRYCOLLECTION":
                        GeometryType = 7;
                        break;
                    default:
                        throw new Exception("Should never reach here");
                }
            }
        }

        /// <summary>
        /// Gets or sets the integer value defining the <see cref="Subtype"/>.
        /// </summary>
        public virtual int GeometryType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating which (if any) spatial index is enabled.
        /// </summary>
        public virtual int SpatialIndex { get; set; }
    }
}
