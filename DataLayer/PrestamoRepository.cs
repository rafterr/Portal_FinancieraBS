using BusinessType;
using DataInterfase;
using Microsoft.EntityFrameworkCore;

namespace DataLayer
{
    public class PrestamoRepository: IPrestamoRepository
    {

        private readonly FinancieraContext _context;

        public PrestamoRepository(FinancieraContext context)
        {
            _context = context;
        }

        // Crear préstamo
        public async Task<Prestamo> CreateAsync(Prestamo prestamo)
        {
            _context.Prestamos.Add(prestamo);
            await _context.SaveChangesAsync();
            return prestamo;
        }

        // Obtener préstamo por Id
        public async Task<Prestamo?> GetByIdAsync(int id)
        {
            return await _context.Prestamos
                .Include(p => p.Cliente)
                .Include(p => p.Usuario)
                .Include(p => p.Pagos)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        // Obtener todos los préstamos
        public async Task<List<Prestamo>> GetAllAsync()
        {
            return await _context.Prestamos
                .Include(p => p.Cliente)
                .Include(p => p.Usuario)
                .Include(p => p.Pagos)
                .ToListAsync();
        }

        // Actualizar préstamo
        public async Task<bool> UpdateAsync(Prestamo prestamo)
        {
            var existing = await _context.Prestamos.FindAsync(prestamo.Id);
            if (existing == null) return false;

            _context.Entry(existing).CurrentValues.SetValues(prestamo);
            await _context.SaveChangesAsync();
            return true;
        }

        // Eliminar préstamo
        public async Task<bool> DeleteAsync(int id)
        {
            var prestamo = await _context.Prestamos.FindAsync(id);
            if (prestamo == null) return false;

            _context.Prestamos.Remove(prestamo);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
