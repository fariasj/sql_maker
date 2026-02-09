using System;

namespace EntityGen
{
	[DAClassAttributes(  SqlType = DASqlType.Table , SqlSchema = "bookings")]
	public partial class AirportsData : DASqlBaseV3<AirportsData>
	{
		//*************************
		//Archivo generado automaticamente por una utilidad de Abraham Farías.
		//No modificar el archivo a mano.
		//Fecha generación: 08/02/2026 23:48:38
		//*************************

		public AirportsData()
		{}

		[DAAttributes(IsKeyForDelete = true, IsIdentity = true, IsKeyForUpdate = true, IsKeyForSelect = true, IsSqlParameter = true, SqlColumnName = "airport_code")]
		public string AirportCode { get; set; }

		[DAAttributes(IsSqlParameter = true, SqlColumnName = "airport_name")]
		public string AirportName { get; set; }

		[DAAttributes(IsSqlParameter = true, SqlColumnName = "city")]
		public string City { get; set; }

		[DAAttributes(IsSqlParameter = true, SqlColumnName = "coordinates")]
		public string Coordinates { get; set; }

		[DAAttributes(IsSqlParameter = true, SqlColumnName = "timezone")]
		public string Timezone { get; set; }

	}
}