using AutoMapper;

using DemoBackend.Dto.Huesped;
using DemoBackend.Models.Huesped;

namespace DemoBackend.Mapping
{
    public class HuespedMapping: Profile
    {
        public HuespedMapping()
        {
            CreateMap<ReclamoSolicitudDto, HuespedReclamoModels>();
            CreateMap<HuespedReclamoModels, ReclamoSolicitudDto>();

            CreateMap<ReservaHuespedDto, ReservaHuespedModels>();

            // Modelo -> DTO (para devolver al frontend / API)
            CreateMap<ReservaHuespedModels, ReservaHuespedDto>();

        }
    }
}
