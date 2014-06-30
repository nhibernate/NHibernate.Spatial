namespace NHibernate.Spatial.Oracle
{
    public enum SDO_GTYPE
    {
        //Oracle Documentation for SDO_GTYPE.

        //This represents the last two digits in a GTYPE, where the first item is dimension(ality) and the second is LRS

        UNKNOWN_GEOMETRY = 00,
        POINT = 01,
        LINE = 02,
        CURVE = 02,
        POLYGON = 03,
        COLLECTION = 04,
        MULTIPOINT = 05,
        MULTILINE = 06,
        MULTICURVE = 06,
        MULTIPOLYGON = 07
    }
}