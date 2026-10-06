using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessType
{
    public class Cliente
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }

        public string PagarePath { get; set; }
        public string InePath { get; set; }
        public string ComprobanteDomicilioPath { get; set; }

        public string Estatus { get; set; }

        public string UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        public ICollection<Prestamo> Prestamos { get; set; }

    }
}
