using BusinessType;

namespace DataInterfase
{
    public interface IDocumentoRepository
    {
        Task<Documento> CreateAsync(Documento documento);
        Task<Documento?> GetByIdAsync(int id);
        Task<List<Documento>> GetByClienteIdAsync(int clienteId);
        Task<List<Documento>> GetByPrestamoIdAsync(int prestamoId);
        Task<bool> DeleteAsync(int id);
    }
}
