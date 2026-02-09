using System;

namespace EntityGen
{
	[DAClassAttributes(  SqlType = DASqlType.Table , SqlSchema = "bookings")]
	public partial class FlightsV : DASqlBaseV3<FlightsV>
	{
		//*************************
		//Archivo generado automaticamente por una utilidad de Abraham Farías.
		//No modificar el archivo a mano.
		//Fecha generación: 08/02/2026 23:48:38
		//*************************

		public FlightsV()
		{}

		[DAAttributes(IsNullable = true, IsSqlParameter = true, SqlColumnName = "flight_id")]
		public int? FlightId { get; set; }

		[DAAttributes(IsNullable = true, IsSqlParameter = true, SqlColumnName = "flight_no")]
		public string FlightNo { get; set; }

		[DAAttributes(IsNullable = true, IsSqlParameter = true, SqlColumnName = "scheduled_departure")]
		public DateTime? ScheduledDeparture { get; set; }

		[DAAttributes(IsNullable = true, IsSqlParameter = true, SqlColumnName = "scheduled_departure_local")]
		public timestamp without time zone? ScheduledDepartureLocal { get; set; }

		[DAAttributes(IsNullable = true, IsSqlParameter = true, SqlColumnName = "scheduled_arrival")]
		public DateTime? ScheduledArrival { get; set; }

		[DAAttributes(IsNullable = true, IsSqlParameter = true, SqlColumnName = "scheduled_arrival_local")]
		public timestamp without time zone? ScheduledArrivalLocal { get; set; }

		[DAAttributes(IsNullable = true, IsSqlParameter = true, SqlColumnName = "scheduled_duration")]
		public TimeSpan? ScheduledDuration { get; set; }

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

		[DAAttributes(IsNullable = true, IsSqlParameter = true, SqlColumnName = "status")]
		public string Status { get; set; }

		[DAAttributes(IsNullable = true, IsSqlParameter = true, SqlColumnName = "aircraft_code")]
		public string AircraftCode { get; set; }

		[DAAttributes(IsNullable = true, IsSqlParameter = true, SqlColumnName = "actual_departure")]
		public DateTime? ActualDeparture { get; set; }

		[DAAttributes(IsNullable = true, IsSqlParameter = true, SqlColumnName = "actual_departure_local")]
		public timestamp without time zone? ActualDepartureLocal { get; set; }

		[DAAttributes(IsNullable = true, IsSqlParameter = true, SqlColumnName = "actual_arrival")]
		public DateTime? ActualArrival { get; set; }

		[DAAttributes(IsNullable = true, IsSqlParameter = true, SqlColumnName = "actual_arrival_local")]
		public timestamp without time zone? ActualArrivalLocal { get; set; }

		[DAAttributes(IsNullable = true, IsSqlParameter = true, SqlColumnName = "actual_duration")]
		public TimeSpan? ActualDuration { get; set; }

	}
}