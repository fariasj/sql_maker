using System;

namespace EntityGen
{
	[DAClassAttributes(  SqlType = DASqlType.Table , SqlSchema = "bookings")]
	public partial class BoardingPasses : DASqlBaseV3<BoardingPasses>
	{
		//*************************
		//Archivo generado automaticamente por una utilidad de Abraham Farías.
		//No modificar el archivo a mano.
		//Fecha generación: 08/02/2026 23:48:38
		//*************************

		public BoardingPasses()
		{}

		[DAAttributes(IsKeyForDelete = true, IsIdentity = true, IsKeyForUpdate = true, IsKeyForSelect = true, IsSqlParameter = true, SqlColumnName = "ticket_no")]
		public string TicketNo { get; set; }

		[DAAttributes(IsKeyForDelete = true, IsIdentity = true, IsKeyForUpdate = true, IsKeyForSelect = true, IsSqlParameter = true, SqlColumnName = "flight_id")]
		public int FlightId { get; set; }

		[DAAttributes(IsSqlParameter = true, SqlColumnName = "boarding_no")]
		public int BoardingNo { get; set; }

		[DAAttributes(IsSqlParameter = true, SqlColumnName = "seat_no")]
		public string SeatNo { get; set; }

	}
}