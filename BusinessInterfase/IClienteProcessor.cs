using BusinessType;

namespace BusinessInterfase
{
    public interface IClienteProcessor
    {
        Task<Cliente?> GetByIdAsync(int id);
        Task<List<Cliente>> GetAllAsync();
        Task<Cliente> CrearAsync(Cliente cliente, string? usuarioId);
        Task<ResultadoOperacion> ActualizarAsync(Cliente cliente);
        Task<ResultadoOperacion> EliminarAsync(int id);
    }
}
