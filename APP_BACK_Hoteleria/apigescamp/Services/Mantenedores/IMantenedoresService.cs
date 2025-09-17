using DemoBackend.Dto.Mantenedores;
using System.Collections.Generic;

namespace DemoBackend.Services.Mantenedores
{
    public interface IMantenedoresService
    {
      

        List<AreasDto> GetListaAreas();
        List<AreasDto> GetListaAreasEstado(int estado);
        public bool CrearAreas(AreasDto areasModels);
        public bool ModificarAreas(AreasDto areasModels);
        public bool EliminarAreas(AreasDto areasModels);
        List<AreasDto> VerificaArea(AreasDto areasModels);

        List<AreasDto> VerificaAreaId(AreasDto areasModels);
    }
}
