using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoBackend.Models.Check { 

    public class CheckKPIModels :EntityBase
    {
        public int CheckinHoy { get; set; }
        public int CheckoutHoy { get; set; }

        public int NoShow { get; set; }
        public int Extensiones { get; set; }

    }
}


