using System;

namespace SqlV2
{
	[DAClassAttributes(  SqlType = DASqlType.Table , SqlSchema = "dbo")]
	public partial class Pagos : DASqlBaseV3<Pagos>
	{
		//*************************
		//Archivo generado automaticamente por una utilidad de Abraham Farías.
		//No modificar el archivo a mano.
		//Fecha generación: 24/02/2017 16:10:26
		//*************************

		public Pagos()
		{}

		[DAAttributes(IsSqlParameter = true, SqlColumnName = "id")]
		public int Id { get; set; }

		[DAAttributes(IsSqlParameter = true, SqlColumnName = "id_cliente")]
		public int IdCliente { get; set; }

		[DAAttributes(IsSqlParameter = true, SqlColumnName = "id_lista")]
		public int IdLista { get; set; }

		[DAAttributes(IsSqlParameter = true, SqlColumnName = "fecha")]
		public DateTime Fecha { get; set; }

		[DAAttributes(IsSqlParameter = true, SqlColumnName = "flag")]
		public int Flag { get; set; }

		[DAAttributes(IsSqlParameter = true, SqlColumnName = "fr")]
		public DateTime Fr { get; set; }

		[DAAttributes(IsSqlParameter = true, SqlColumnName = "fm")]
		public DateTime Fm { get; set; }

		[DAAttributes(IsSqlParameter = true, SqlColumnName = "resta")]
		public int Resta { get; set; }

		[DAAttributes(IsSqlParameter = true, SqlColumnName = "id_vendedor")]
		public int IdVendedor { get; set; }

	}
}