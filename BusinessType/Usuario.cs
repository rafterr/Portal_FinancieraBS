using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessType
{
    public class Usuario : IdentityUser
    {
        // Fecha en que se creó el usuario
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // Relación con los clientes que creó
        public ICollection<Cliente> ClientesCreados { get; set; } = new List<Cliente>();

        // Relación con los préstamos que gestionó
        public ICollection<Prestamo> PrestamosCreados { get; set; } = new List<Prestamo>();

        // Relación con los pagos que recibió
        public ICollection<Pago> PagosRecibidos { get; set; } = new List<Pago>();
    }
}
