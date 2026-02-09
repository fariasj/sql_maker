using System;

namespace SqlV2;

[DAClassAttributes(SqlType = DASqlType.Table)]
public partial class Encuestas : DASqlBaseV3<Encuestas>
{
	//*************************
	//Archivo generado automaticamente por una utilidad de Abraham Farías.
	//No modificar el archivo a mano.
	//Fecha generación: 01/03/2017 14:37:16
	//*************************

	public Encuestas()
	{}

	[DAAttributes(IsKeyForDelete = true, IsIdentity = true, IsKeyForUpdate = true, IsKeyForSelect = true, IsSqlParameter = true, SqlColumnName = "idEncuesta")]
	public int Idencuesta { get; set; }

	[DAAttributes(IsSqlParameter = true, SqlColumnName = "Encuesta")]
	public string Encuesta { get; set; } = string.Empty;

	[DAAttributes(IsNullable = true, IsSqlParameter = true, SqlColumnName = "Seccion")]
	public string Seccion { get; set; } = string.Empty;

	[DAAttributes(IsSqlParameter = true, SqlColumnName = "idTipoEncuesta")]
	public int Idtipoencuesta { get; set; }

	[DAAttributes(IsNullable = true, IsSqlParameter = true, SqlColumnName = "idAlcance")]
	public int? Idalcance { get; set; }

	[DAAttributes(IsSqlParameter = true, SqlColumnName = "Descripcion")]
	public string Descripcion { get; set; } = string.Empty;

	[DAAttributes(IsNullable = true, IsSqlParameter = true, SqlColumnName = "FechaI")]
	public DateTime? Fechai { get; set; }

	[DAAttributes(IsNullable = true, IsSqlParameter = true, SqlColumnName = "FechaF")]
	public DateTime? Fechaf { get; set; }

	[DAAttributes(IsSqlParameter = true, SqlColumnName = "Totalfolios")]
	public int Totalfolios { get; set; }

	[DAAttributes(IsNullable = true, IsSqlParameter = true, SqlColumnName = "idFormato")]
	public int? Idformato { get; set; }

	[DAAttributes(IsSqlParameter = true, SqlColumnName = "idPlaza")]
	public int Idplaza { get; set; }

	[DAAttributes(IsSqlParameter = true, SqlColumnName = "idEstatus")]
	public int Idestatus { get; set; }

	[DAAttributes(IsSqlParameter = true, SqlColumnName = "Responsable")]
	public string Responsable { get; set; } = string.Empty;

	[DAAttributes(IsNullable = true, IsSqlParameter = true, SqlColumnName = "Solicitante")]
	public int? Solicitante { get; set; }

	[DAAttributes(IsNullable = true, IsSqlParameter = true, SqlColumnName = "FoliosSinAsignar")]
	public int? Foliossinasignar { get; set; }

	[DAAttributes(IsNullable = true, IsSqlParameter = true, SqlColumnName = "FoliosCapturados")]
	public int? Folioscapturados { get; set; }

	[DAAttributes(IsNullable = true, IsSqlParameter = true, SqlColumnName = "FoliosGenerados")]
	public int? Foliosgenerados { get; set; }

	[DAAttributes(IsNullable = true, IsSqlParameter = true, SqlColumnName = "DeCampo")]
	public int? Decampo { get; set; }

	[DAAttributes(IsNullable = true, IsSqlParameter = true, SqlColumnName = "Terminada")]
	public int? Terminada { get; set; }

	[DAAttributes(IsNullable = true, IsSqlParameter = true, SqlColumnName = "Mensaje")]
	public string Mensaje { get; set; } = string.Empty;

	[DAAttributes(IsNullable = true, IsSqlParameter = true, SqlColumnName = "Historico")]
	public DateTime? Historico { get; set; }

	[DAAttributes(IsNullable = true, IsSqlParameter = true, SqlColumnName = "Recuperado")]
	public int? Recuperado { get; set; }

	[DAAttributes(IsNullable = true, IsSqlParameter = true, SqlColumnName = "MensajeFinal")]
	public string Mensajefinal { get; set; } = string.Empty;

	[DAAttributes(IsSqlParameter = true, SqlColumnName = "titulo")]
	public string Titulo { get; set; } = string.Empty;

	[DAAttributes(IsNullable = true, IsSqlParameter = true, SqlColumnName = "Flags")]
	public int? Flags { get; set; }

	[DAAttributes(IsNullable = true, IsSqlParameter = true, SqlColumnName = "IdCliente")]
	public int? Idcliente { get; set; }

	[DAAttributes(IsNullable = true, IsSqlParameter = true, SqlColumnName = "IdResponsable")]
	public int? Idresponsable { get; set; }

	[DAAttributes(IsNullable = true, IsSqlParameter = true, SqlColumnName = "PathLogo")]
	public string Pathlogo { get; set; } = string.Empty;

}
