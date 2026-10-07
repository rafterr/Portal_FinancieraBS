using Microsoft.EntityFrameworkCore;
using BusinessType;
using DataInterfase;

namespace DataLayer
{
    public class PagoRepository : IpagoRepository
    {
        private readonly FinancieraContext _context;

        public PagoRepository(FinancieraContext context)
        {
            _context = context;
        }

        // Crear pago
        public async Task<Pago> CreateAsync(Pago pago)
        {
            // El saldo del préstamo lo recalcula PagoProcessor
            _context.Pagos.Add(pago);
            await _context.SaveChangesAsync();
            return pago;
        }

        // Obtener pago por Id
        public async Task<Pago?> GetByIdAsync(int id)
        {
            return await _context.Pagos
                .Include(p => p.Prestamo)
                .Include(p => p.Cliente)
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        // Obtener todos los pagos
        public async Task<List<Pago>> GetAllAsync()
        {
            return await _context.Pagos
                .Include(p => p.Prestamo)
                .Include(p => p.Cliente)
                .Include(p => p.Usuario)
                .OrderByDescending(p => p.FechaPago)
                .ToListAsync();
        }

        // Actualizar pago
        public async Task<bool> UpdateAsync(Pago pago)
        {
            var existing = await _context.Pagos.FindAsync(pago.Id);
            if (existing == null) return false;

            _context.Entry(existing).CurrentValues.SetValues(pago);
            await _context.SaveChangesAsync();
            return true;
        }

        // Eliminar pago
        public async Task<bool> DeleteAsync(int id)
        {
            var pago = await _context.Pagos.FindAsync(id);
            if (pago == null) return false;

            _context.Pagos.Remove(pago);
            await _context.SaveChangesAsync();
            return true;
        }

        // Obtener pagos por cliente
        public async Task<List<Pago>> GetByClienteIdAsync(int clienteId)
        {
            return await _context.Pagos
                .Include(p => p.Prestamo)
                .Include(p => p.Cliente)
                .Include(p => p.Usuario)
                .Where(p => p.ClienteId == clienteId)
                .ToListAsync();
        }

        public async Task<List<Pago>> BuscarAsync(int? prestamoId, string? texto, DateTime? desde, DateTime? hasta)
        {
            var query = _context.Pagos
                .Include(p => p.Prestamo)
                .Include(p => p.Cliente)
                .Include(p => p.Usuario)
                .AsQueryable();

            if (prestamoId.HasValue) query = query.Where(p => p.PrestamoId == prestamoId);
            if (desde.HasValue) query = query.Where(p => p.FechaPago >= desde.Value.Date);
            if (hasta.HasValue)
            {
                var limite = hasta.Value.Date.AddDays(1);
                query = query.Where(p => p.FechaPago < limite);
            }

            foreach (var t in Busqueda.Terminos(texto))
            {
                var id = Busqueda.ComoId(t);
                query = query.Where(p => p.PrestamoId == id || p.Cliente!.Nombre.Contains(t) || p.Cliente.Apellidos.Contains(t)
                                         || p.Cliente.Telefono.Contains(t) || p.Cliente.Email.Contains(t));
            }

            return await query.OrderByDescending(p => p.FechaPago).ThenByDescending(p => p.Id).ToListAsync();
        }

        // Obtener pagos por préstamo
        public async Task<List<Pago>> GetByPrestamoIdAsync(int prestamoId)
        {
            return await _context.Pagos
                .Include(p => p.Prestamo)
                .Include(p => p.Cliente)
                .Include(p => p.Usuario)
                .Where(p => p.PrestamoId == prestamoId)
                .ToListAsync();
        }
    }
}
