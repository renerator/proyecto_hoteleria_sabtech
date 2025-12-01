using DemoBackend.Dto.Trabajador;
using System.Collections.Generic;

namespace DemoBackend.Services.Trabajador
{
    public interface ITrabajadorService
    {//cambio 1-12
        List<TrabajadorDto> GetListaTrabajador();
        List<TrabajadorDto> GetListaTrabajadorEstado(int estado);

        bool CrearTrabajador(TrabajadorDto trabajador);
        bool ModificarTrabajador(TrabajadorDto trabajador);
        bool EliminarTrabajador(TrabajadorDto trabajador);

        List<TrabajadorDto> VerificaTrabajadorPorNombre(TrabajadorDto trabajador);
        List<TrabajadorDto> VerificaTrabajadorPorId(TrabajadorDto trabajador);

        TrabajadorDto GetTrabajadorRut(string rut);
    }
}
