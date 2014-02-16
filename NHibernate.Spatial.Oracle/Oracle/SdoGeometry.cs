using System;
using Oracle.DataAccess.Types;

namespace NHibernate.Spatial.Oracle
{

    [OracleCustomTypeMappingAttribute("MDSYS.SDO_GEOMETRY")]
    public class SdoGeometry : OracleCustomTypeBase<SdoGeometry>
    {

        private enum OracleObjectColumns { SDO_GTYPE, SDO_SRID, SDO_POINT, SDO_ELEM_INFO, SDO_ORDINATES }

        private double? sdo_Gtype;

        [OracleObjectMappingAttribute(0)]
        public double? Sdo_Gtype
        {
            get { return sdo_Gtype; }
            set { sdo_Gtype = value; }
        }

        private double? sdo_Srid;

        [OracleObjectMappingAttribute(1)]
        public double? Sdo_Srid
        {
            get { return sdo_Srid; }
            set { sdo_Srid = value; }
        }

        private SdoPoint point;

        [OracleObjectMappingAttribute(2)]
        public SdoPoint Point
        {
            get { return point; }
            set { point = value; }
        }

        private double[] elemArray;

        [OracleObjectMappingAttribute(3)]
        public double[] ElemArray
        {
            get { return elemArray; }
            set { elemArray = value; }
        }

        private double[] ordinatesArray;

        [OracleObjectMappingAttribute(4)]
        public double[] OrdinatesArray
        {
            get { return ordinatesArray; }
            set { ordinatesArray = value; }
        }

        [OracleCustomTypeMappingAttribute("MDSYS.SDO_ELEM_INFO_ARRAY")]
        public class ElemArrayFactory : OracleArrayTypeFactoryBase<decimal> { }

        [OracleCustomTypeMappingAttribute("MDSYS.SDO_ORDINATE_ARRAY")]
        public class OrdinatesArrayFactory : OracleArrayTypeFactoryBase<decimal> { }

        public override void MapFromCustomObject()
        {
            SetValue((int)OracleObjectColumns.SDO_GTYPE, Sdo_Gtype);
            SetValue((int)OracleObjectColumns.SDO_SRID, Sdo_Srid);
            SetValue((int)OracleObjectColumns.SDO_POINT, Point);
            SetValue((int)OracleObjectColumns.SDO_ELEM_INFO, ElemArray);
            SetValue((int)OracleObjectColumns.SDO_ORDINATES, OrdinatesArray);
        }

        public override void MapToCustomObject()
        {
            Sdo_Gtype = GetValue<double?>((int)OracleObjectColumns.SDO_GTYPE);
            Sdo_Srid = GetValue<double?>((int)OracleObjectColumns.SDO_SRID);
            Point = GetValue<SdoPoint>((int)OracleObjectColumns.SDO_POINT);
            ElemArray = GetValue<double[]>((int)OracleObjectColumns.SDO_ELEM_INFO);
            OrdinatesArray = GetValue<double[]>((int)OracleObjectColumns.SDO_ORDINATES);
        }

        public int[] ElemArrayOfInts
        {
            get
            {
                return Array.ConvertAll<double, int>(this.elemArray,
                    delegate(double x) { return Convert.ToInt32(x); });
            }
            set
            {
                if (value != null)
                {
                    this.elemArray = Array.ConvertAll<int, double>(value, 
                        delegate(int x) { return Convert.ToDouble(x); });
                }
                else
                {
                    this.elemArray = null;
                }
            }
        }

        public double?[] OrdinatesArrayOfDoubles
        {
            get
            {
                return Array.ConvertAll<double, double?>(
                    this.ordinatesArray,
                    delegate(double x) { return (double?)Convert.ToDouble(x); });
            }
            set
            {
                if (value != null)
                {
                    this.ordinatesArray = Array.ConvertAll<double?, double>(
                        value,
                        delegate(double? x) { return Convert.ToDouble(x); });
                }
                else
                {
                    this.ordinatesArray = null;
                }
            }
        }

        private int _Dimensionality;
        public int Dimensionality
        {
            get { return _Dimensionality; }
            set { _Dimensionality = value; }
        }

        private int _LRS;
        public int LRS
        {
            get { return _LRS; }
            set { _LRS = value; }
        }

        private int _GeometryType;
        public int GeometryType
        {
            get { return _GeometryType; }
            set { _GeometryType = value; }
        }

        public int PropertiesFromGTYPE()
        {
            if ((int)this.sdo_Gtype != 0)
            {
                int v = (int)this.sdo_Gtype;
                int dim = v / 1000;
                Dimensionality = dim;
                v -= dim * 1000;
                int lrsDim = v / 100;
                LRS = lrsDim;
                v -= lrsDim * 100;
                GeometryType = v;
                return (Dimensionality * 1000) + (LRS * 100) + GeometryType;
            }
            return 0;
        }

        public int PropertiesToGTYPE()
        {
            int v = Dimensionality * 1000;
            v = v + (LRS * 100);
            v = v + GeometryType;
            this.sdo_Gtype = System.Convert.ToDouble(v);
            return v;
        }

        #region Ported from Hibernate Spatial SDOGeometryType.java

        public void addElement(double[] element)
        {
            double[] newTriplets = new double[this.elemArray.Length + element.Length];
            Array.Copy(this.elemArray, 0, newTriplets, 0, this.elemArray.Length);
            Array.Copy(element, 0, newTriplets, this.elemArray.Length, element.Length);
            this.elemArray = newTriplets;
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

        public void AddOrdinates<T>(T[] ordinatesToAdd)
        {
            double[] newOrdinates = new double[this.ordinatesArray.Length + ordinatesToAdd.Length];
            Array.Copy(this.ordinatesArray, 0, newOrdinates, 0, this.ordinatesArray.Length);
            Array.Copy(ordinatesToAdd, 0, newOrdinates, this.ordinatesArray.Length, ordinatesToAdd.Length);
            this.ordinatesArray = newOrdinates;
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
            SdoGeometry sdoCollection = new SdoGeometry();
            sdoCollection.Sdo_Gtype = (double)SDO_GTYPE.COLLECTION;
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
                    double[] element = sdoElements[i].ElemArray;
                    double[] ordinates = sdoElements[i].OrdinatesArray;
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

        private static void ShiftOrdinateOffset(double[] elemInfo, int offset)
        {
            for (int i = 0; i < elemInfo.Length; i++)
            {
                //int newOffset = elemInfo.getOrdinatesOffset(i) + offset;
                //elemInfo.setOrdinatesOffset(i, newOffset);
                elemInfo[i * 3] = elemInfo[i * 3] + offset;
            }
        }

        #endregion
    }
}