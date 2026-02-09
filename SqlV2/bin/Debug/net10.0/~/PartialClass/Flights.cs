using System;

namespace EntityGen
{
	[DAClassAttributes(  SqlType = DASqlType.Table , SqlSchema = "bookings")]
	public partial class Flights : DASqlBaseV3<Flights>
	{
		//*************************
		//Archivo generado automaticamente por una utilidad de Abraham Farías.
		//No modificar el archivo a mano.
		//Fecha generación: 08/02/2026 23:48:38
		//*************************

		public Flights()
		{}

		[DAAttributes(IsKeyForDelete = true, IsIdentity = true, IsKeyForUpdate = true, IsKeyForSelect = true, IsSqlParameter = true, SqlColumnName = "flight_id")]
		public int FlightId { get; set; }

		[DAAttributes(IsSqlParameter = true, SqlColumnName = "flight_no")]
		public string FlightNo { get; set; }

		[DAAttributes(IsSqlParameter = true, SqlColumnName = "scheduled_departure")]
		public DateTime ScheduledDeparture { get; set; }

		[DAAttributes(IsSqlParameter = true, SqlColumnName = "scheduled_arrival")]
		public DateTime ScheduledArrival { get; set; }

		[DAAttributes(IsSqlParameter = true, SqlColumnName = "departure_airport")]
		public string DepartureAirport { get; set; }

		[DAAttributes(IsSqlParameter = true, SqlColumnName = "arrival_airport")]
		public string ArrivalAirport { get; set; }

		[DAAttributes(IsSqlParameter = true, SqlColumnName = "status")]
		public string Status { get; set; }

		[DAAttributes(IsSqlParameter = true, SqlColumnName = "aircraft_code")]
		public string AircraftCode { get; set; }

		[DAAttributes(IsNullable = true, IsSqlParameter = true, SqlColumnName = "actual_departure")]
		public DateTime? ActualDeparture { get; set; }

		[DAAttributes(IsNullable = true, IsSqlParameter = true, SqlColumnName = "actual_arrival")]
		public DateTime? ActualArrival { get; set; }

	}
}