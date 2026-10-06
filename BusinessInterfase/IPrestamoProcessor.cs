using BusinessType;

namespace BusinessInterfase
{
    public interface IPrestamoProcessor
    {
        Task<Prestamo?> GetByIdAsync(int id);
        Task<List<Prestamo>> GetAllAsync();
        Task<ResultadoOperacion> CrearAsync(Prestamo prestamo, string? usuarioId);
        Task<ResultadoOperacion> ActualizarAsync(Prestamo prestamo);
        Task<ResultadoOperacion> EliminarAsync(int id);
    }
}
