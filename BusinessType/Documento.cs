using System.ComponentModel.DataAnnotations;

namespace BusinessType
{
    public enum TipoDocumento
    {
        [Display(Name = "Comprobante de domicilio")]
        ComprobanteDomicilio = 1,
        [Display(Name = "INE")]
        Ine = 2,
        [Display(Name = "Pagaré")]
        Pagare = 3
    }

    public class Documento
    {
        public int Id { get; set; }

        public TipoDocumento Tipo { get; set; }

        public int ClienteId { get; set; }
        public Cliente? Cliente { get; set; }

        // Pagaré e INE pertenecen a un préstamo; el comprobante de domicilio solo al cliente
        public int? PrestamoId { get; set; }
        public Prestamo? Prestamo { get; set; }

        [StringLength(255)]
        public string NombreOriginal { get; set; } = string.Empty;

        [StringLength(100)]
        public string ContentType { get; set; } = string.Empty;

        public long Tamano { get; set; }

        // Clave del archivo dentro del almacenamiento (no es una URL pública)
        [StringLength(255)]
        public string Ruta { get; set; } = string.Empty;

        public DateTime FechaCarga { get; set; }

        public string? UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
    }
}
