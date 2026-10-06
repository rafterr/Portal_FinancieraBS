using System.ComponentModel.DataAnnotations;

namespace BusinessType
{
    public class Pago
    {
        public int Id { get; set; }

        [Display(Name = "Fecha del Pago")]
        public DateTime FechaPago { get; set; }

        [Range(typeof(decimal), "0.01", "100000000", ErrorMessage = "El monto debe ser mayor a 0.")]
        [Display(Name = "Monto del Pago")]
        public decimal MontoPago { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Seleccione un préstamo.")]
        [Display(Name = "Préstamo")]
        public int PrestamoId { get; set; }
        public Prestamo? Prestamo { get; set; }

        // Se toma siempre del préstamo; no se captura en el formulario
        public int ClienteId { get; set; }
        public Cliente? Cliente { get; set; }

        public string? UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
    }
}
