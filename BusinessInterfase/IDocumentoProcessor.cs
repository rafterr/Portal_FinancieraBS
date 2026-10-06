using BusinessType;

namespace BusinessInterfase
{
    public class DocumentoOptions
    {
        public long TamanoMaximoBytes { get; set; } = 5 * 1024 * 1024;
    }

    public record ArchivoSubido(Stream Contenido, string NombreOriginal, long Tamano);

    public record DocumentoAbierto(Documento Documento, Stream Contenido);

    public interface IDocumentoProcessor
    {
        ResultadoOperacion Validar(ArchivoSubido archivo);
        Task<ResultadoOperacion> SubirAsync(ArchivoSubido archivo, TipoDocumento tipo, int clienteId, int? prestamoId, string? usuarioId);
        Task<List<Documento>> GetByClienteIdAsync(int clienteId);
        Task<List<Documento>> GetByPrestamoIdAsync(int prestamoId);
        Task<Documento?> GetByIdAsync(int id);
        Task<DocumentoAbierto?> AbrirAsync(int id);
        Task<ResultadoOperacion> EliminarAsync(int id);
    }
}
