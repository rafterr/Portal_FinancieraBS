using System.ComponentModel.DataAnnotations;

namespace BusinessType
{
    public class Cliente
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los apellidos son obligatorios.")]
        [StringLength(100)]
        public string Apellidos { get; set; } = string.Empty;

        [Required(ErrorMessage = "La dirección es obligatoria.")]
        [StringLength(255)]
        [Display(Name = "Dirección")]
        public string Direccion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [StringLength(20)]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "Correo no válido.")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        public string? PagarePath { get; set; }
        public string? InePath { get; set; }
        public string? ComprobanteDomicilioPath { get; set; }

        [Required]
        [StringLength(20)]
        public string Estatus { get; set; } = "Activo";

        public string? UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        public ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();

        public string NombreCompleto => $"{Nombre} {Apellidos}";
    }
}
