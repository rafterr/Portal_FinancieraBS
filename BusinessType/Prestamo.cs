using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessType
{
    public enum EstatusPrestamo
    {
        Pagado = 1,
        EnProceso = 2,
        Retraso = 3
    }

    public class Prestamo
    {
        public int Id { get; set; }
        public decimal MontoSolicitado { get; set; }
        public decimal SaldoRestante { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin => FechaInicio.AddDays(7 * 14);
        public decimal Interes { get; set; }
        public decimal Total => MontoSolicitado + (MontoSolicitado * Interes / 100);

        public EstatusPrestamo Estatus { get; set; }

        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }

        public string UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        public ICollection<Pago> Pagos { get; set; }

    }
}
