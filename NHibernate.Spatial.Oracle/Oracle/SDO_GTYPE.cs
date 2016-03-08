namespace NHibernate.Spatial.Oracle
{
    public enum SDO_GTYPE
    {
        //Oracle Documentation for SDO_GTYPE.

        //This represents the last two digits in a GTYPE, where the first item is dimension(ality) and the second is LRS

        UNKNOWN_GEOMETRY = 0,
        POINT = 1,
        LINE = 2,
        CURVE = 2,
        POLYGON = 3,
        COLLECTION = 4,
        MULTIPOINT = 5,
        MULTILINE = 6,
        MULTICURVE = 6,
        MULTIPOLYGON = 7
    }
}