
using DemoBackend.Dto.Mantenedores;
using DemoBackend.Dto.Check;

using DemoBackend.Models.Check;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DemoBackend.Services.Check
{
    public interface ICheckinCheckoutService
    {
       
        List<CheckDTO> GetListar(int idEstadoCheck, DateTime? fechaDesde);
        CheckKPIDTO ObtenerDashboard();


      


    }
}

