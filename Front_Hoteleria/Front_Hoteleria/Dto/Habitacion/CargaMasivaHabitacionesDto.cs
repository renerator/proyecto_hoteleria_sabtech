using System.Web;

namespace Front_Hoteleria.Dto.Habitacion
{
    public class CargaMasivaHabitacionesDto
    {
        public string NombreArchivoTemplate { get; set; }
        public int TotalRegistros { get; set; }
        public int TotalInsertados { get; set; }
        public int TotalErrores { get; set; }
        public string Mensaje { get; set; }
        public bool ProcesadoOk { get; set; }

        // Para el archivo subido
        public HttpPostedFileBase Archivo { get; set; }
    }
}
