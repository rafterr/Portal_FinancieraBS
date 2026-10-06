using BusinessType;

namespace BusinessInterfase
{
    public interface IPagoProcessor
    {
        Task<Pago> CreateAsync(Pago pago);
        Task<Pago?> GetByIdAsync(int id);
        Task<List<Pago>> GetAllAsync();
        Task<bool> UpdateAsync(Pago pago);
        Task<bool> DeleteAsync(int id);

    }
}
