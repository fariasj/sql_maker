using System;

namespace EntityGen
{
	[DAClassAttributes(  SqlType = DASqlType.Table , SqlSchema = "bookings")]
	public partial class AircraftsData : DASqlBaseV3<AircraftsData>
	{
		//*************************
		//Archivo generado automaticamente por una utilidad de Abraham Farías.
		//No modificar el archivo a mano.
		//Fecha generación: 08/02/2026 23:48:38
		//*************************

		public AircraftsData()
		{}

		[DAAttributes(IsKeyForDelete = true, IsIdentity = true, IsKeyForUpdate = true, IsKeyForSelect = true, IsSqlParameter = true, SqlColumnName = "aircraft_code")]
		public string AircraftCode { get; set; }

		[DAAttributes(IsSqlParameter = true, SqlColumnName = "model")]
		public string Model { get; set; }

		[DAAttributes(IsSqlParameter = true, SqlColumnName = "range")]
		public int Range { get; set; }

	}
}