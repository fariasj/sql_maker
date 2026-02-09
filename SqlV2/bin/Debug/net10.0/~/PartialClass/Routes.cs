using System;

namespace EntityGen
{
	[DAClassAttributes(  SqlType = DASqlType.Table , SqlSchema = "bookings")]
	public partial class Routes : DASqlBaseV3<Routes>
	{
		//*************************
		//Archivo generado automaticamente por una utilidad de Abraham Farías.
		//No modificar el archivo a mano.
		//Fecha generación: 08/02/2026 23:48:38
		//*************************

		public Routes()
		{}

		[DAAttributes(IsNullable = true, IsSqlParameter = true, SqlColumnName = "flight_no")]
		public string FlightNo { get; set; }

		[DAAttributes(IsNullable = true, IsSqlParameter = true, SqlColumnName = "departure_airport")]
		public string DepartureAirport { get; set; }

		[DAAttributes(IsNullable = true, IsSqlParameter = true, SqlColumnName = "departure_airport_name")]
		public string DepartureAirportName { get; set; }

		[DAAttributes(IsNullable = true, IsSqlParameter = true, SqlColumnName = "departure_city")]
		public string DepartureCity { get; set; }

		[DAAttributes(IsNullable = true, IsSqlParameter = true, SqlColumnName = "arrival_airport")]
		public string ArrivalAirport { get; set; }

		[DAAttributes(IsNullable = true, IsSqlParameter = true, SqlColumnName = "arrival_airport_name")]
		public string ArrivalAirportName { get; set; }

		[DAAttributes(IsNullable = true, IsSqlParameter = true, SqlColumnName = "arrival_city")]
		public string ArrivalCity { get; set; }

		[DAAttributes(IsNullable = true, IsSqlParameter = true, SqlColumnName = "aircraft_code")]
		public string AircraftCode { get; set; }

		[DAAttributes(IsNullable = true, IsSqlParameter = true, SqlColumnName = "duration")]
		public TimeSpan? Duration { get; set; }

		[DAAttributes(IsNullable = true, IsSqlParameter = true, SqlColumnName = "days_of_week")]
		public ARRAY? DaysOfWeek { get; set; }

	}
}