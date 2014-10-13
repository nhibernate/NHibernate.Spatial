using Oracle.DataAccess.Types;

namespace NHibernate.Spatial.Oracle
{
    [OracleCustomTypeMappingAttribute("MDSYS.SDO_POINT_TYPE")]
    public class SdoPoint : OracleCustomTypeBase<SdoPoint>
    {
        private double? x;

        [OracleObjectMappingAttribute("X")]
        public double? X
        {
            get { return x; }
            set { x = value; }
        }

        private double? y;

        [OracleObjectMappingAttribute("Y")]
        public double? Y
        {
            get { return y; }
            set { y = value; }
        }

        private double? z;

        [OracleObjectMappingAttribute("Z")]
        public double? Z
        {
            get { return z; }
            set { z = value; }
        }

        public override void MapFromCustomObject()
        {
            SetValue("X", x);
            SetValue("Y", y);
            SetValue("Z", z);
        }

        public override void MapToCustomObject()
        {
            X = GetValue<double?>("X");
            Y = GetValue<double?>("Y");
            Z = GetValue<double?>("Z");
        }
    }
}