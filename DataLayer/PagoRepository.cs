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
            _context.Pagos.Add(pago);

            // Actualiza el saldo restante del préstamo relacionado
            var prestamo = await _context.Prestamos.FindAsync(pago.PrestamoId);
            if (prestamo != null)
            {
                prestamo.SaldoRestante -= pago.MontoPago;
            }

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
