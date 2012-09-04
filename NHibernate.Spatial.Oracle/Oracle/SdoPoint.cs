using Oracle.DataAccess.Types;

namespace NHibernate.Spatial.Oracle
{
	[OracleCustomTypeMappingAttribute("MDSYS.SDO_POINT_TYPE")]
	public class SdoPoint : OracleCustomTypeBase<SdoPoint>
	{
		private decimal? x;

		[OracleObjectMappingAttribute("X")]
		public decimal? X
		{
			get { return x; }
			set { x = value; }
		}

		private decimal? y;

		[OracleObjectMappingAttribute("Y")]
		public decimal? Y
		{
			get { return y; }
			set { y = value; }
		}

		private decimal? z;

		[OracleObjectMappingAttribute("Z")]
		public decimal? Z
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
			X = GetValue<decimal?>("X");
			Y = GetValue<decimal?>("Y");
			Z = GetValue<decimal?>("Z");
		}
	}
}