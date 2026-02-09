using System;

namespace EntityGen
{
	[DAClassAttributes(  SqlType = DASqlType.Table , SqlSchema = "bookings")]
	public partial class Bookings : DASqlBaseV3<Bookings>
	{
		//*************************
		//Archivo generado automaticamente por una utilidad de Abraham Farías.
		//No modificar el archivo a mano.
		//Fecha generación: 08/02/2026 23:48:38
		//*************************

		public Bookings()
		{}

		[DAAttributes(IsKeyForDelete = true, IsIdentity = true, IsKeyForUpdate = true, IsKeyForSelect = true, IsSqlParameter = true, SqlColumnName = "book_ref")]
		public string BookRef { get; set; }

		[DAAttributes(IsSqlParameter = true, SqlColumnName = "book_date")]
		public DateTime BookDate { get; set; }

		[DAAttributes(IsSqlParameter = true, SqlColumnName = "total_amount")]
		public decimal TotalAmount { get; set; }

	}
}