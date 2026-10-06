using BusinessType;

namespace BusinessInterfase
{
    public interface IPagoProcessor
    {
        Task<Pago?> GetByIdAsync(int id);
        Task<List<Pago>> GetAllAsync();
        Task<ResultadoOperacion> RegistrarAsync(Pago pago, string? usuarioId);
        Task<ResultadoOperacion> ActualizarAsync(Pago pago);
        Task<ResultadoOperacion> EliminarAsync(int id);
    }
}
