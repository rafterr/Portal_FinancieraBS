using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataInterfase
{
    public interface IPrestamoRepository
    {
        Task<BusinessType.Prestamo> CreateAsync(BusinessType.Prestamo prestamo);
        Task<BusinessType.Prestamo?> GetByIdAsync(int id);
        Task<List<BusinessType.Prestamo>> GetAllAsync();
        // Filtra por cliente, texto (#préstamo o datos del cliente) y estatus
        Task<List<BusinessType.Prestamo>> BuscarAsync(int? clienteId, string? texto, BusinessType.EstatusPrestamo? estatus);
        Task<bool> UpdateAsync(BusinessType.Prestamo prestamo);
        Task<bool> DeleteAsync(int id);
    }
}
