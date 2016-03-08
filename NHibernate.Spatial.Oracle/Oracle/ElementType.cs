namespace NHibernate.Spatial.Oracle
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Implementation of table 2.2 at
    /// http://docs.oracle.com/cd/B19306_01/appdev.102/b14255/sdo_objrelschema.htm#BGHDGCCE
    /// </summary>
    public enum ElementType
    {
        [SdoElementType(0, 0)] UNSUPPORTED,
        [SdoElementType(1, 1)] POINT,
        [SdoElementType(1, 0)] ORIENTATION,
        [SdoElementType(1, true)] POINT_CLUSTER,
        [SdoElementType(2, 1)] LINE_STRAITH_SEGMENTS,
        [SdoElementType(2, 2)] LINE_ARC_SEGMENTS,
        [SdoElementType(2003, 1)] INTERIOR_RING_STRAIGHT_SEGMENTS,
        [SdoElementType(1003, 1)] EXTERIOR_RING_STRAIGHT_SEGMENTS,
        [SdoElementType(2003, 2)] INTERIOR_RING_ARC_SEGMENTS,
        [SdoElementType(1003, 2)] EXTERIOR_RING_ARC_SEGMENTS,
        [SdoElementType(2003, 3)] INTERIOR_RING_RECT,
        [SdoElementType(1003, 3)] EXTERIOR_RING_RECT,
        [SdoElementType(2003, 4)] INTERIOR_RING_CIRCLE,
        [SdoElementType(1003, 4)] EXTERIOR_RING_CIRCLE,
        [SdoElementType(4, true)] COMPOUND_LINE,
        [SdoElementType(1005, true)] COMPOUND_EXTERIOR_RING,
        [SdoElementType(2005, true)] COMPOUND_INTERIOR_RING
    }

    class SdoElementType : Attribute
    {
        internal SdoElementType(int eType, int interpretation)
        {
            this.EType = eType;
            this.Interpretation = interpretation;
        }
        internal SdoElementType(int eType, bool compound)
        {
            this.EType = eType;
            this.Compound = compound;
            this.Interpretation = 2;
        }
        public decimal EType { get; private set; }
        public decimal Interpretation { get; private set; }
        public bool Compound { get; private set; }
    }

    public static class ElementTypes
    {
        public static decimal EType(this ElementType elementType)
        {
            var attr = GetAttr(elementType);
            return attr.EType;
        }

        public static decimal Interpretation(this ElementType elementType)
        {
            var attr = GetAttr(elementType);
            return attr.Interpretation;
        }

        public static bool IsCompound(this ElementType elementType)
        {
            var attr = GetAttr(elementType);
            return attr.Compound;
        }

        public static bool IsLine(this ElementType elementType)
        {
            var attr = GetAttr(elementType);
            return (attr.EType == 2 || attr.EType == 4);
        }

        public static bool IsInteriorRing(this ElementType elementType)
        {
            var attr = GetAttr(elementType);
            return (attr.EType == 2003 || attr.EType == 2005);
        }

        public static bool IsExteriorRing(this ElementType elementType)
        {
            var attr = GetAttr(elementType);
            return (attr.EType == 1003 || attr.EType == 1005);
        }

        public static bool IsStraightSegment(this ElementType elementType)
        {
            var attr = GetAttr(elementType);
            return attr.Interpretation == 1;
        }

        public static bool IsArcSegment(this ElementType elementType)
        {
            var attr = GetAttr(elementType);
            return attr.Interpretation == 2;
        }

        public static bool IsRect(this ElementType elementType)
        {
            var attr = GetAttr(elementType);
            return attr.Interpretation == 3;
        }

        public static bool IsCircle(this ElementType elementType)
        {
            var attr = GetAttr(elementType);
            return attr.Interpretation == 4;
        }

        public static ElementType ParseType(decimal etype, decimal interpretation) {
            
            foreach (ElementType t in Enum.GetValues(typeof(ElementType))) {
                if (t.EType() == etype) {
                    if (t.IsCompound()
                            || t.Interpretation() == interpretation) {
                        return t;
                    }
                }
            }
            throw new Exception(
                    "Can't determine ElementType from etype:" + etype
                            + " and interp.:" + interpretation);
        }

        private static SdoElementType GetAttr(ElementType elementType)
        {
            return (SdoElementType)Attribute.GetCustomAttribute(ForValue(elementType), typeof(SdoElementType));
        }

        private static MemberInfo ForValue(ElementType p)
        {
            return typeof(ElementType).GetField(Enum.GetName(typeof(ElementType), p));
        }

    }
}