namespace Front_Hoteleria.Dto.Habitacion
{
    /// <summary>
    /// Representa una fila del archivo de carga masiva de habitaciones.
    /// </summary>
    public class CargaMasivaHabitacionFilaDto
    {
        /// <summary>
        /// Número de fila en el archivo (para mostrar en errores).
        /// </summary>
        public int NumeroFila { get; set; }

        /// <summary>
        /// Código o número de habitación (ej: H101, 101, etc.).
        /// </summary>
        public string CodigoHabitacion { get; set; }

        /// <summary>
        /// Nombre de la habitación (ej: Habitación 101 Norte).
        /// </summary>
        public string NombreHabitacion { get; set; }

        /// <summary>
        /// Tipo de habitación (ej: Single, Doble, Suite).
        /// </summary>
        public string TipoHabitacion { get; set; }

        /// <summary>
        /// Capacidad en personas.
        /// </summary>
        public int? Capacidad { get; set; }

        /// <summary>
        /// Indica si es VIP (Sí/No o 1/0 mapeado a bool).
        /// </summary>
        public bool? EsVip { get; set; }

        /// <summary>
        /// Precio base (opcional, según tu template).
        /// </summary>
        public decimal? Precio { get; set; }

        /// <summary>
        /// Estado de la habitación en el archivo (ej: Activa, Inactiva).
        /// </summary>
        public string EstadoTexto { get; set; }

        /// <summary>
        /// Observaciones que vengan en el archivo.
        /// </summary>
        public string Observaciones { get; set; }

        /// <summary>
        /// Indica si la fila fue validada correctamente.
        /// </summary>
        public bool EsValida { get; set; }

        /// <summary>
        /// Mensaje de error de validación (si EsValida = false).
        /// </summary>
        public string MensajeError { get; set; }
    }
}
