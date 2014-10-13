namespace NHibernate.Spatial.Oracle
{
    public enum SDO_ETYPE
    {
        //Oracle Documentation for SDO_ETYPE - SIMPLE

        //Point//Line//Polygon//exterior counterclockwise - polygon ring = 1003//interior clockwise  polygon ring = 2003

        //public enum ETYPE_SIMPLE
        // {
        ETYPE_SIMPLE_POINT = 1,

        ETYPE_SIMPLE_LINE = 2,
        ETYPE_SIMPLE_POLYGON = 3,
        ETYPE_SIMPLE_POLYGON_EXTERIOR = 1003,
        ETYPE_SIMPLE_POLYGON_INTERIOR = 2003,
        //}

        //Oracle Documentation for SDO_ETYPE - COMPOUND

        //1005: exterior polygon ring (must be specified in counterclockwise order)

        //2005: interior polygon ring (must be specified in clockwise order)

        //public enum ETYPE_COMPOUND
        //{
        ETYPE_COMPOUND_FOURDIGIT = 4,

        ETYPE_COMPOUND_POLYGON_EXTERIOR = 1005,
        ETYPE_COMPOUND_POLYGON_INTERIOR = 2005
        //}
    }
}