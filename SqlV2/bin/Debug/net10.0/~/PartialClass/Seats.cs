using System;

namespace EntityGen
{
	[DAClassAttributes(  SqlType = DASqlType.Table , SqlSchema = "bookings")]
	public partial class Seats : DASqlBaseV3<Seats>
	{
		//*************************
		//Archivo generado automaticamente por una utilidad de Abraham Farías.
		//No modificar el archivo a mano.
		//Fecha generación: 08/02/2026 23:48:38
		//*************************

		public Seats()
		{}

		[DAAttributes(IsKeyForDelete = true, IsIdentity = true, IsKeyForUpdate = true, IsKeyForSelect = true, IsSqlParameter = true, SqlColumnName = "aircraft_code")]
		public string AircraftCode { get; set; }

		[DAAttributes(IsKeyForDelete = true, IsIdentity = true, IsKeyForUpdate = true, IsKeyForSelect = true, IsSqlParameter = true, SqlColumnName = "seat_no")]
		public string SeatNo { get; set; }

		[DAAttributes(IsSqlParameter = true, SqlColumnName = "fare_conditions")]
		public string FareConditions { get; set; }

	}
}