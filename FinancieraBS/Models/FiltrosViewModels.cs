using BusinessType;

namespace FinancieraBS.Models
{
    public class ClientesIndexViewModel
    {
        public List<Cliente> Clientes { get; set; } = new();
        public string? Q { get; set; }
    }

    public class PrestamosIndexViewModel
    {
        public List<Prestamo> Prestamos { get; set; } = new();
        public string? Q { get; set; }
        public EstatusPrestamo? Estatus { get; set; }
        public int? ClienteId { get; set; }
        public string? ClienteNombre { get; set; }
    }

    public class PagosIndexViewModel
    {
        public List<Pago> Pagos { get; set; } = new();
        public string? Q { get; set; }
        public int? PrestamoId { get; set; }
        public DateTime? Desde { get; set; }
        public DateTime? Hasta { get; set; }
        public decimal Total => Pagos.Sum(p => p.MontoPago);
        public bool HayFiltros => !string.IsNullOrWhiteSpace(Q) || PrestamoId.HasValue || Desde.HasValue || Hasta.HasValue;
    }
}
