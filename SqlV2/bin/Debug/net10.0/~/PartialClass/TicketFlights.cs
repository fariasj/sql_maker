using System;

namespace EntityGen
{
	[DAClassAttributes(  SqlType = DASqlType.Table , SqlSchema = "bookings")]
	public partial class TicketFlights : DASqlBaseV3<TicketFlights>
	{
		//*************************
		//Archivo generado automaticamente por una utilidad de Abraham Farías.
		//No modificar el archivo a mano.
		//Fecha generación: 08/02/2026 23:48:38
		//*************************

		public TicketFlights()
		{}

		[DAAttributes(IsKeyForDelete = true, IsIdentity = true, IsKeyForUpdate = true, IsKeyForSelect = true, IsSqlParameter = true, SqlColumnName = "ticket_no")]
		public string TicketNo { get; set; }

		[DAAttributes(IsKeyForDelete = true, IsIdentity = true, IsKeyForUpdate = true, IsKeyForSelect = true, IsSqlParameter = true, SqlColumnName = "flight_id")]
		public int FlightId { get; set; }

		[DAAttributes(IsSqlParameter = true, SqlColumnName = "fare_conditions")]
		public string FareConditions { get; set; }

		[DAAttributes(IsSqlParameter = true, SqlColumnName = "amount")]
		public decimal Amount { get; set; }

	}
}