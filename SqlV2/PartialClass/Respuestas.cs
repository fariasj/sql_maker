using System;

namespace SqlV2;

[DAClassAttributes(SqlType = DASqlType.Table)]
public partial class Respuestas : DASqlBaseV3<Respuestas>
{
	//*************************
	//Archivo generado automaticamente por una utilidad de Abraham Farías.
	//No modificar el archivo a mano.
	//Fecha generación: 01/03/2017 14:37:16
	//*************************

	public Respuestas()
	{}

	[DAAttributes(IsNullable = true, IsSqlParameter = true, SqlColumnName = "idEncuesta")]
	public int? Idencuesta { get; set; }

	[DAAttributes(IsNullable = true, IsSqlParameter = true, SqlColumnName = "idPregunta")]
	public int? Idpregunta { get; set; }

	[DAAttributes(IsSqlParameter = true, SqlColumnName = "idRespuesta")]
	public int Idrespuesta { get; set; }

	[DAAttributes(IsNullable = true, IsSqlParameter = true, SqlColumnName = "Respuesta")]
	public string Respuesta { get; set; } = string.Empty;

	[DAAttributes(IsNullable = true, IsSqlParameter = true, SqlColumnName = "RangoI")]
	public int? Rangoi { get; set; }

	[DAAttributes(IsNullable = true, IsSqlParameter = true, SqlColumnName = "RangoF")]
	public int? Rangof { get; set; }

	[DAAttributes(IsNullable = true, IsSqlParameter = true, SqlColumnName = "NoResp")]
	public int? Noresp { get; set; }

	[DAAttributes(IsNullable = true, IsSqlParameter = true, SqlColumnName = "Flags")]
	public int? Flags { get; set; }

}
