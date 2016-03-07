using Oracle.DataAccess.Types;
using System;

namespace NHibernate.Spatial.Oracle
{
    /// <summary>
    /// Oracle UDT for SDO_GEOMETRY. Seems to be taken from https://code.google.com/p/tf-net/wiki/OracleSdoGeometryAsUdt
    /// Follows http://docs.oracle.com/cd/B19306_01/appdev.102/b14255/sdo_objrelschema.htm
    /// </summary>
    [OracleCustomTypeMappingAttribute("MDSYS.SDO_GEOMETRY")]
    public class SdoGeometry : OracleCustomTypeBase<SdoGeometry>
    {
        private enum OracleObjectColumns { SDO_GTYPE, SDO_SRID, SDO_POINT, SDO_ELEM_INFO, SDO_ORDINATES }

        [OracleObjectMappingAttribute(0)]
        public decimal? Sdo_Gtype { get; private set; }

        [OracleObjectMappingAttribute(1)]
        public decimal? Sdo_Srid { get; set; }

        [OracleObjectMappingAttribute(2)]
        public SdoPoint Point { get; set; }

        [OracleObjectMappingAttribute(3)]
        public decimal[] ElemArray { get; set; }

        [OracleObjectMappingAttribute(4)]
        public decimal[] OrdinatesArray { get; set; }

        [OracleCustomTypeMappingAttribute("MDSYS.SDO_ELEM_INFO_ARRAY")]
        public class ElemArrayFactory : OracleArrayTypeFactoryBase<decimal> { }

        [OracleCustomTypeMappingAttribute("MDSYS.SDO_ORDINATE_ARRAY")]
        public class OrdinatesArrayFactory : OracleArrayTypeFactoryBase<decimal> { }

        public override void MapFromCustomObject()
        {
            PropertiesToGTYPE();
            SetValue((int)OracleObjectColumns.SDO_GTYPE, Sdo_Gtype);
            SetValue((int)OracleObjectColumns.SDO_SRID, Sdo_Srid);
            SetValue((int)OracleObjectColumns.SDO_POINT, Point);
            SetValue((int)OracleObjectColumns.SDO_ELEM_INFO, ElemArray);
            SetValue((int)OracleObjectColumns.SDO_ORDINATES, OrdinatesArray);
        }

        public override void MapToCustomObject()
        {
            Sdo_Gtype = GetValue<decimal?>((int)OracleObjectColumns.SDO_GTYPE);
            Sdo_Srid = GetValue<decimal?>((int)OracleObjectColumns.SDO_SRID);
            Point = GetValue<SdoPoint>((int)OracleObjectColumns.SDO_POINT);
            ElemArray = GetValue<decimal[]>((int)OracleObjectColumns.SDO_ELEM_INFO);
            OrdinatesArray = GetValue<decimal[]>((int)OracleObjectColumns.SDO_ORDINATES);
            PropertiesFromGTYPE();
        }

        public int[] ElemArrayOfInts
        {
            get
            {
                return Array.ConvertAll(this.ElemArray, Convert.ToInt32);
            }
            set
            {
                if (value != null)
                {
                    this.ElemArray = Array.ConvertAll(value, Convert.ToDecimal);
                }
                else
                {
                    this.ElemArray = null;
                }
            }
        }

        //public double[] ElemArrayOfDoubles
        //{
        //    get
        //    {
        //        return Array.ConvertAll(this.ElemArray, Convert.ToDouble);
        //    }
        //    set
        //    {
        //        if (value != null)
        //        {
        //            this.ElemArray = Array.ConvertAll(
        //                value,
        //                x => Double.IsNaN(x) ? new double?() : x);
        //        }
        //        else
        //        {
        //            this.ElemArray = null;
        //        }
        //    }
        //}

        public double[] OrdinatesArrayOfDoubles
        {
            get
            {
                return Array.ConvertAll(this.OrdinatesArray, Convert.ToDouble);
            }
            set
            {
                if (value != null)
                {
                    this.OrdinatesArray = Array.ConvertAll(value, Convert.ToDecimal);
                }
                else
                {
                    this.OrdinatesArray = null;
                }
            }
        }

        public int Dimensionality { get; set; }

        private int lrs;

        public int LRS
        {
            get { return this.lrs; }
            set { this.lrs = value; }
        }

        public SDO_GTYPE GeometryType { get; set; }

        public int PropertiesFromGTYPE()
        {
            var sdoGtype = this.Sdo_Gtype;
            if (sdoGtype != null)
            {
                var gtype = this.Sdo_Gtype;
                if (gtype != null)
                {
                    var v = (int)gtype;
                    var dim = v / 1000;
                    this.Dimensionality = dim;
                    v -= dim * 1000;
                    var lrsDim = v / 100;
                    this.LRS = lrsDim;
                    v -= lrsDim * 100;
                    this.GeometryType = (SDO_GTYPE)v;
                }
                return (this.Dimensionality * 1000) + (this.LRS * 100) + (int)this.GeometryType;
            }
            return 0;
        }

        public int PropertiesToGTYPE()
        {
            int v = this.Dimensionality * 1000;
            v = v + (this.LRS * 100);
            v = v + (int)this.GeometryType;
            this.Sdo_Gtype = Convert.ToDecimal(v);
            return v;
        }

        #region Ported from Hibernate Spatial SDOGeometryType.java

        public void addElement(decimal[] element)
        {
            if (this.ElemArray == null)
            {
                this.ElemArray = element;
            }
            else
            {
                var newTriplets = new decimal[this.ElemArray.Length + element.Length];
                Array.Copy(this.ElemArray, 0, newTriplets, 0, this.ElemArray.Length);
                Array.Copy(element, 0, newTriplets, this.ElemArray.Length, element.Length);
                this.ElemArray = newTriplets;
            }
        }

        //public void addOrdinates(double?[] ordinatesToAdd)
        //{
        //    decimal[] newOrdinates = new decimal[this.ordinatesArray.Length + ordinatesToAdd.Length];
        //    Array.Copy(this.ordinatesArray, 0, newOrdinates, 0, this.ordinatesArray.Length);
        //    Array.Copy(ordinatesToAdd, 0, newOrdinates, this.ordinatesArray.Length, ordinatesToAdd.Length);
        //    this.ordinatesArray = newOrdinates;
        //}

        //private void addOrdinates(decimal[] ordinatesToAdd)
        //{
        //    decimal[] newOrdinates = new decimal[this.ordinatesArray.Length + ordinatesToAdd.Length];
        //    Array.Copy(this.ordinatesArray, 0, newOrdinates, 0, this.ordinatesArray.Length);
        //    Array.Copy(ordinatesToAdd, 0, newOrdinates, this.ordinatesArray.Length, ordinatesToAdd.Length);
        //    this.ordinatesArray = newOrdinates;
        //}

        public void AddOrdinates(decimal[] ordinatesToAdd)
        {
            if (this.OrdinatesArray == null)
            {
                this.OrdinatesArray = ordinatesToAdd;
            }
            else
            {
                var newOrdinates = new decimal[this.OrdinatesArray.Length + ordinatesToAdd.Length];
                Array.Copy(this.OrdinatesArray, 0, newOrdinates, 0, this.OrdinatesArray.Length);
                Array.Copy(ordinatesToAdd, 0, newOrdinates, this.OrdinatesArray.Length, ordinatesToAdd.Length);
                this.OrdinatesArray = newOrdinates;
            }
        }

        /**
        * This joins an array of SDO_GEOMETRIES to a SDO_GEOMETRY of type
        * COLLECTION
        *
        * @param sdoElements
        * @return
        */

        public static SdoGeometry Join(SdoGeometry[] sdoElements)
        {
            SdoGeometry sdoCollection = new SdoGeometry { GeometryType = SDO_GTYPE.COLLECTION };
            if (sdoElements == null || sdoElements.Length == 0)
            {
                sdoCollection.Dimensionality = 2;
                sdoCollection.LRS = 0;
            }
            else
            {
                SdoGeometry firstElement = sdoElements[0];
                int dim = firstElement.Dimensionality;
                int lrsDim = firstElement.LRS;
                sdoCollection.Dimensionality = dim;
                sdoCollection.LRS = lrsDim;
                int ordinatesOffset = 1;
                for (int i = 0; i < sdoElements.Length; i++)
                {
                    var element = sdoElements[i].ElemArray;
                    var ordinates = sdoElements[i].OrdinatesArray;
                    if (element != null && element.Length > 0)
                    {
                        //int shift = ordinatesOffset - element.getOrdinatesOffset(0);
                        int shift = ordinatesOffset - (int)element[0];
                        ShiftOrdinateOffset(element, shift);
                        sdoCollection.addElement(element);
                        sdoCollection.AddOrdinates(ordinates);
                        ordinatesOffset += ordinates.Length;
                    }
                }
            }
            return sdoCollection;
        }

        private static void ShiftOrdinateOffset(decimal[] elemInfo, int offset)
        {
            for (int i = 0; i < elemInfo.Length; i++)
            {
                //int newOffset = elemInfo.getOrdinatesOffset(i) + offset;
                //elemInfo.setOrdinatesOffset(i, newOffset);
                elemInfo[i * 3] = elemInfo[i * 3] + offset;
            }
        }

        #endregion Ported from Hibernate Spatial SDOGeometryType.java

        public static string ArrayToString<T>(T[] array)
        {
            if (array == null)
            {
                return "()";
            }

            var stringArray = string.Join(",", array);
            return string.Format("({0})", stringArray);
        }
    }
}