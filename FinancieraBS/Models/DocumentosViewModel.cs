using BusinessInterfase;
using BusinessType;

namespace FinancieraBS.Models
{
    public class DocumentosViewModel
    {
        public List<Documento> Documentos { get; set; } = new();
        public int ClienteId { get; set; }
        public int? PrestamoId { get; set; }
        public TipoDocumento[] TiposPermitidos { get; set; } = Array.Empty<TipoDocumento>();
        public string ReturnUrl { get; set; } = "/";
    }

    public static class FormFileExtensions
    {
        public static ArchivoSubido ComoArchivoSubido(this IFormFile archivo) =>
            new(archivo.OpenReadStream(), archivo.FileName, archivo.Length);
    }
}
