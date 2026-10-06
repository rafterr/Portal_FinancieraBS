


namespace DataInterfase
{
    public interface IClienteRepository
    {
        // Crear cliente
        Task<BusinessType.Cliente> CreateAsync(BusinessType.Cliente cliente);
        // Obtener cliente por Id
        Task<BusinessType.Cliente?> GetByIdAsync(int id);
        // Obtener todos los clientes
        Task<List<BusinessType.Cliente>> GetAllAsync();
        // Actualizar cliente
        Task<bool> UpdateAsync(BusinessType.Cliente cliente);
        // Eliminar cliente
        Task<bool> DeleteAsync(int id);
        // Indica si el cliente tiene préstamos
        Task<bool> TienePrestamosAsync(int clienteId);
    }
}
