using BusinessType;

namespace BusinessInterfase
{
    public interface IPrestamoProcessor
    {
        Task<Prestamo> CreateAsync(Prestamo prestamo);
        Task<Prestamo?> GetByIdAsync(int id);
        Task<List<Prestamo>> GetAllAsync();
        Task<bool> UpdateAsync(Prestamo prestamo);
        Task<bool> DeleteAsync(int id);
    }
}
