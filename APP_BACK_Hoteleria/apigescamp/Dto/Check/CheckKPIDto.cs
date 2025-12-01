using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoBackend.Dto.Check{
    public class CheckKPIDTO
    {//cambio 1-12
        public int CheckinHoy { get; set; }
        public int CheckoutHoy { get; set; }

        public int NoShow { get; set; }
        public int Extensiones { get; set; }

    }
}


