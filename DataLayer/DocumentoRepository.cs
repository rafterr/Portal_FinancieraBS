using BusinessType;
using DataInterfase;
using Microsoft.EntityFrameworkCore;

namespace DataLayer
{
    public class DocumentoRepository : IDocumentoRepository
    {
        private readonly FinancieraContext _context;

        public DocumentoRepository(FinancieraContext context)
        {
            _context = context;
        }

        public async Task<Documento> CreateAsync(Documento documento)
        {
            _context.Documentos.Add(documento);
            await _context.SaveChangesAsync();
            return documento;
        }

        public async Task<Documento?> GetByIdAsync(int id)
        {
            return await _context.Documentos.FindAsync(id);
        }

        public async Task<List<Documento>> GetByClienteIdAsync(int clienteId)
        {
            return await _context.Documentos
                .Include(d => d.Usuario)
                .Where(d => d.ClienteId == clienteId)
                .OrderBy(d => d.PrestamoId).ThenBy(d => d.Tipo)
                .ToListAsync();
        }

        public async Task<List<Documento>> GetByPrestamoIdAsync(int prestamoId)
        {
            return await _context.Documentos
                .Include(d => d.Usuario)
                .Where(d => d.PrestamoId == prestamoId)
                .OrderBy(d => d.Tipo)
                .ToListAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var documento = await _context.Documentos.FindAsync(id);
            if (documento == null) return false;

            _context.Documentos.Remove(documento);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
