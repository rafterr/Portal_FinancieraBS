using BusinessType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataInterfase
{
    public interface IpagoRepository
    {
        // Crear pago
        Task<Pago> CreateAsync(Pago pago);
        // Obtener pago por Id
        Task<Pago?> GetByIdAsync(int id);
        // Obtener todos los pagos
        Task<List<Pago>> GetAllAsync();
        // Actualizar pago
        Task<bool> UpdateAsync(Pago pago);
        // Eliminar pago
        Task<bool> DeleteAsync(int id);
        
        // Obtener pagos por cliente
        Task<List<Pago>> GetByClienteIdAsync(int clienteId);
        
        // Obtener pagos por préstamo
        Task<List<Pago>> GetByPrestamoIdAsync(int prestamoId);
    }
}
