namespace NHibernate.Spatial.Oracle
{
    using System;

    using global::Oracle.DataAccess.Types;

    /// <summary>
    /// 
    /// </summary>
    [OracleCustomTypeMapping("MDSYS.SDO_ELEM_INFO_ARRAY")]
    public class ElementInfo
    {
        
        public ElementInfo(int size)
        {
            this.Elements = new decimal[3 * size];
        }

        public ElementInfo(decimal[] triplets)
        {
            this.Elements = triplets;
        }

        public decimal[] Elements { get; private set; }

        public int Size
        {
            get
            {
                return this.Elements.Length / 3;
            }
        }

        public int GetOrdinatesOffset(int index)
        {
            var startIndexOfTriplet = index * 3;
            return Convert.ToInt32(this.Elements[startIndexOfTriplet]);
        }

        public void SetOrdinatesOffset(int index, int offset)
        {
            var startIndexOfTriplet = index * 3;
            this.Elements[startIndexOfTriplet] = new decimal(offset);
        }

        public ElementType GetElementType(int index)
        {
            var startIndexOfTriplet = index * 3;
            var etype = this.Elements[startIndexOfTriplet + 1];
            var interp = this.Elements[startIndexOfTriplet + 2];
            var elementType = ElementTypes.ParseType(etype, interp);
            return elementType;
        }

        public bool IsCompound(int index)
        {
            return this.GetElementType(index).IsCompound();
        }

        public int GetNumCompounds(int index)
        {
            if (this.GetElementType(index).IsCompound())
            {
                var startIndexOfTriplet = index * 3;
                return Convert.ToInt32(this.Elements[startIndexOfTriplet + 2]);
            }

            return 1;
        }

        public void SetElement(int index, int ordinatesOffset, ElementType et, int numCompounds)
        {
            if (index > this.Size)
            {
                throw new IndexOutOfRangeException("Attempted to set more elements in ElementInfo array than capacity.");
            }

            var startIndexOfTriplet = index * 3;
            this.Elements[startIndexOfTriplet] = new decimal(ordinatesOffset);
            this.Elements[startIndexOfTriplet + 1] = et.EType();
            this.Elements[startIndexOfTriplet + 2] = et.IsCompound() ? new decimal(numCompounds) : et.Interpretation();
        }

        public void AddElement(decimal[] element)
        {
            var newTriplets = new decimal[this.Elements.Length + element.Length];
            Array.Copy(this.Elements, 0, newTriplets, 0, this.Elements.Length);
            Array.Copy(element, 0, newTriplets, this.Elements.Length, element.Length);
            this.Elements = newTriplets;
        }

        public void AddElement(ElementInfo element)
        {
            this.AddElement(element.Elements);
        }

        public decimal[] GetElement(int index)
        {
            decimal[] element = null;
            if (this.GetElementType(index).IsCompound())
            {
                var numCompounds = this.GetNumCompounds(index);
                element = new decimal[numCompounds + 1];
            }
            else
            {
                element = new decimal[3];
            }
            var startIndexOfTriplet = index * 3;
            Array.Copy(this.Elements, startIndexOfTriplet, element, 0, element.Length);
            return element;
        }

        public override string ToString()
        {
            return SdoGeometry.ArrayToString(this.Elements);
        }
    }
}
