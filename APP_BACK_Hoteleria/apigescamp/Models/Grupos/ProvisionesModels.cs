using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoBackend.Models.Grupos
{
    public class ProvisionesModels: EntityBase
    {
        [Key]
        [Column("id")]
        public int id { get; set; }
        [Column("mes_anio")]
        public string mes_anio { get; set; }
        [Column("clase_costo_id")]
        public int clase_costo_id { get; set; }
        [Column("tipo_provision_id")]
        public int tipo_provision_id { get; set; }
        [Column("valor_tipo_provision")]
        public int valor_tipo_provision { get; set; }
        [Column("proveedor_contrato_id")]
        public Int64 proveedor_contrato_id { get; set; }
        [Column("numero_factura_boleta")]
        public int numero_factura_boleta { get; set; }
        [Column("numero_estado_pago_id")]
        public int numero_estado_pago_id { get; set; }
        [Column("tipo_moneda_origen_id")]
        public int tipo_moneda_origen_id { get; set; }
        //[Column("monto_origen")]
        [Column(TypeName = "decimal(19,4)")]
        public decimal monto_origen { get; set; }
        //[Column("valor_conversion_origen_pesos_dia")]
        [Column(TypeName = "decimal(19,4)")]
        public decimal valor_conversion_origen_pesos_dia { get; set; }
        //[Column("monto_final_pesos")]
        [Column(TypeName = "decimal(19,4)")]
        public decimal monto_final_pesos { get; set; }
        //[Column("valor_conversion_final_dolar_dia")]
        [Column(TypeName = "decimal(19,4)")]
        public decimal valor_conversion_final_dolar_dia { get; set; }
        //[Column("monto_final_dolar")]
        [Column(TypeName = "decimal(19,4)")]
        public decimal monto_final_dolar { get; set; }
        //[Column("monto_final_dolar_cliente")]
        [Column(TypeName = "decimal(19,4)")]
        public decimal monto_final_dolar_cliente { get; set; }
        [Column("observacion")]
        public string observacion { get; set; }
        [Column("fecha_provision")]
        public string fecha_provision { get; set; }
        [Column("usuario_create_id")]
        public int usuario_create_id { get; set; }
        [Column("usuario_update_id")]
        public int usuario_update_id { get; set; }
        [Column("IdEstadoProvision")]
        public int IdEstadoProvision { get; set; }
        [Column("vigente")]
        public bool vigente { get; set; }
        [Column("created_at")]
        public DateTime? created_at { get; set; }
        [Column("updated_at")]
        public DateTime? updated_at { get; set; }
        [Column("observacion_detalle")]
        public string observacion_detalle { get; set; }
        [Column("UrlArchivo")]
        public string UrlArchivo { get; set; }
    }
}
