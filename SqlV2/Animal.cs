using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlV2
{
    [DAClassAttributes(SqlTableName = "tbl_animal", SqlType = DASqlType.Table)]
    class Animal : DASqlBaseV3<Animal>
    {
        [DAAttributes ( IsKeyForDelete = true, IsIdentity = true, IsKeyForUpdate = true,
                        IsSqlParameter = true, SqlColumnName = "id_animal")]
        public int ID { get; set; }

        [DAAttributes(IsKeyForDelete = false, IsKeyForUpdate = true, IsSqlParameter = true, SqlColumnName = "nombre")]
        public string Nombre { get; set; }

        [DAAttributes(IsSqlParameter = true, SqlColumnName = "counter")]
        public int Contador { get; set; }

        [DAAttributes(IsSqlParameter = true, SqlColumnName = "categoria")]
        public string Categoria { get; set; }

        [DAAttributes(IsSqlParameter = true, SqlColumnName = "fecha_alta")]
        public DateTime FechaAlta { get; set; }

        [DAAttributes(IsSqlParameter = false)]
        public DAMensajesSistema Mensajes { get; set; }

        public Animal()
        {
            Mensajes = new DAMensajesSistema();
        }

    }
}
