
using Microsoft.EntityFrameworkCore;
using BusinessType;
using DataInterfase;

namespace DataLayer
{
    public class ClienteRepository: IClienteRepository
    {
        private readonly FinancieraContext _context;

        public ClienteRepository(FinancieraContext context)
        {
            _context = context;
        }

        // Crear cliente
        public async Task<Cliente> CreateAsync(Cliente cliente)
        {
            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();
            return cliente;
        }

        // Obtener cliente por Id
        public async Task<Cliente?> GetByIdAsync(int id)
        {
            return await _context.Clientes.FindAsync(id);
        }

        // Obtener todos los clientes
        public async Task<List<Cliente>> GetAllAsync()
        {
            return await _context.Clientes.OrderBy(c => c.Nombre).ThenBy(c => c.Apellidos).ToListAsync();
        }

        // Actualizar cliente
        public async Task<bool> UpdateAsync(Cliente cliente)
        {
            var existing = await _context.Clientes.FindAsync(cliente.Id);
            if (existing == null) return false;

            _context.Entry(existing).CurrentValues.SetValues(cliente);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> TienePrestamosAsync(int clienteId)
        {
            return await _context.Prestamos.AnyAsync(p => p.ClienteId == clienteId);
        }

        // Eliminar cliente
        public async Task<bool> DeleteAsync(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return false;

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
