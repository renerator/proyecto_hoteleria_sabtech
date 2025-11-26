using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoBackend.Models.HabitacionInventario
{
    [Table("ctr_man_HabitacionInventario")]
    public class HabitacionInventarioModels : EntityBase
    {
        [Key]
        [Column("IdHabitacionInventario")] // columna física
        public int IdHabitacionInventario { get; set; }  // nombre igual al DTO

        [Column("idHabitacion")]
        public int IdHabitacion { get; set; }

        [Column("idInventario")] // o "idInventario" si en BD ya cambiaste el nombre
        public int IdInventario { get; set; }

        //[Column("stockMinimo")]
        //public int? StockMinimo { get; set; }

        //[Column("idBodega")]
        //public int? IdBodega { get; set; }

        [Column("idEstado")]
        public int? idEstado { get; set; }

        [Column("fechaVerificacion")]
        public DateTime? FechaVerificacion { get; set; }

        [Column("idResponsable")]
        public int? idResponsable { get; set; }

        // ===== Campos de apoyo / DTO (no necesariamente columnas de la tabla) =====



        [Column("TipoMaterial")]
        public string TipoMaterial { get; set; }

        [Column("NombreHabitacion")]
        public string NombreHabitacion { get; set; }


        [Column("MarcaModelo")]
        public string MarcaModelo { get; set; }

        [Column("Descripcion")]
        public string Descripcion { get; set; }

        [Column("Estado")]
        public string Estado { get; set; }

        [Column("Responsable")]
        public string Responsable { get; set; }

       
    }
}
