using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessType
{
    public class Pago
    {
        public int Id { get; set; }
        public DateTime FechaPago { get; set; }
        public decimal MontoPago { get; set; }

        public int PrestamoId { get; set; }
        public Prestamo Prestamo { get; set; }

        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }

        public string UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

    }
}
