using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoBackend.Dto.Check{
    public class CheckKPIDTO 
    {
        public int CheckinHoy { get; set; }
        public int CheckoutHoy { get; set; }

        public int NoShow { get; set; }
        public int Extensiones { get; set; }

    }
}


