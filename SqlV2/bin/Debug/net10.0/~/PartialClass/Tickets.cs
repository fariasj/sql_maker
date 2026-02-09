using System;

namespace EntityGen
{
	[DAClassAttributes(  SqlType = DASqlType.Table , SqlSchema = "bookings")]
	public partial class Tickets : DASqlBaseV3<Tickets>
	{
		//*************************
		//Archivo generado automaticamente por una utilidad de Abraham Farías.
		//No modificar el archivo a mano.
		//Fecha generación: 08/02/2026 23:48:38
		//*************************

		public Tickets()
		{}

		[DAAttributes(IsKeyForDelete = true, IsIdentity = true, IsKeyForUpdate = true, IsKeyForSelect = true, IsSqlParameter = true, SqlColumnName = "ticket_no")]
		public string TicketNo { get; set; }

		[DAAttributes(IsSqlParameter = true, SqlColumnName = "book_ref")]
		public string BookRef { get; set; }

		[DAAttributes(IsSqlParameter = true, SqlColumnName = "passenger_id")]
		public string PassengerId { get; set; }

		[DAAttributes(IsSqlParameter = true, SqlColumnName = "passenger_name")]
		public string PassengerName { get; set; }

		[DAAttributes(IsNullable = true, IsSqlParameter = true, SqlColumnName = "contact_data")]
		public string ContactData { get; set; }

	}
}