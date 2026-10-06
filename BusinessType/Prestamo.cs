using System.ComponentModel.DataAnnotations;

namespace BusinessType
{
    public enum EstatusPrestamo
    {
        Pagado = 1,
        [Display(Name = "En Proceso")]
        EnProceso = 2,
        Retraso = 3
    }

    public class Prestamo
    {
        public const int SemanasPlazo = 14;

        public int Id { get; set; }

        [Range(typeof(decimal), "1", "100000000", ErrorMessage = "El monto debe ser mayor a 0.")]
        [Display(Name = "Monto Solicitado")]
        public decimal MontoSolicitado { get; set; }

        [Display(Name = "Saldo Restante")]
        public decimal SaldoRestante { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Inicio")]
        public DateTime FechaInicio { get; set; } = DateTime.Today;

        public DateTime FechaFin => FechaInicio.AddDays(7 * SemanasPlazo);

        [Range(typeof(decimal), "0", "100", ErrorMessage = "El interés debe estar entre 0 y 100.")]
        [Display(Name = "Interés (%)")]
        public decimal Interes { get; set; }

        public decimal Total => Math.Round(MontoSolicitado + (MontoSolicitado * Interes / 100), 2);

        public EstatusPrestamo Estatus { get; set; } = EstatusPrestamo.EnProceso;

        [Range(1, int.MaxValue, ErrorMessage = "Seleccione un cliente.")]
        [Display(Name = "Cliente")]
        public int ClienteId { get; set; }
        public Cliente? Cliente { get; set; }

        public string? UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        public ICollection<Pago> Pagos { get; set; } = new List<Pago>();
    }
}
