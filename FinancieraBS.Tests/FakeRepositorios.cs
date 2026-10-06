using BusinessType;
using DataInterfase;

namespace FinancieraBS.Tests
{
    // Repositorios en memoria para probar la lógica de negocio sin base de datos
    internal class FakePrestamoRepository : IPrestamoRepository
    {
        public List<Prestamo> Prestamos { get; } = new();

        public Task<Prestamo> CreateAsync(Prestamo prestamo)
        {
            prestamo.Id = Prestamos.Count + 1;
            Prestamos.Add(prestamo);
            return Task.FromResult(prestamo);
        }
        public Task<Prestamo?> GetByIdAsync(int id) => Task.FromResult(Prestamos.FirstOrDefault(p => p.Id == id));
        public Task<List<Prestamo>> GetAllAsync() => Task.FromResult(Prestamos.ToList());
        public Task<bool> UpdateAsync(Prestamo prestamo) => Task.FromResult(true);
        public Task<bool> DeleteAsync(int id) => Task.FromResult(Prestamos.RemoveAll(p => p.Id == id) > 0);
    }

    internal class FakePagoRepository : IpagoRepository
    {
        public List<Pago> Pagos { get; } = new();

        public Task<Pago> CreateAsync(Pago pago)
        {
            pago.Id = Pagos.Count + 1;
            Pagos.Add(pago);
            return Task.FromResult(pago);
        }
        public Task<Pago?> GetByIdAsync(int id) => Task.FromResult(Pagos.FirstOrDefault(p => p.Id == id));
        public Task<List<Pago>> GetAllAsync() => Task.FromResult(Pagos.ToList());
        public Task<bool> UpdateAsync(Pago pago) => Task.FromResult(true);
        public Task<bool> DeleteAsync(int id) => Task.FromResult(Pagos.RemoveAll(p => p.Id == id) > 0);
        public Task<List<Pago>> GetByClienteIdAsync(int clienteId) => Task.FromResult(Pagos.Where(p => p.ClienteId == clienteId).ToList());
        public Task<List<Pago>> GetByPrestamoIdAsync(int prestamoId) => Task.FromResult(Pagos.Where(p => p.PrestamoId == prestamoId).ToList());
    }

    internal class FakeClienteRepository : IClienteRepository
    {
        public List<Cliente> Clientes { get; } = new() { new Cliente { Id = 1, Nombre = "Ana" } };
        public List<int> ConPrestamos { get; } = new();

        public Task<Cliente> CreateAsync(Cliente cliente) { Clientes.Add(cliente); return Task.FromResult(cliente); }
        public Task<Cliente?> GetByIdAsync(int id) => Task.FromResult(Clientes.FirstOrDefault(c => c.Id == id));
        public Task<List<Cliente>> GetAllAsync() => Task.FromResult(Clientes.ToList());
        public Task<bool> UpdateAsync(Cliente cliente) => Task.FromResult(true);
        public Task<bool> DeleteAsync(int id) => Task.FromResult(Clientes.RemoveAll(c => c.Id == id) > 0);
        public Task<bool> TienePrestamosAsync(int clienteId) => Task.FromResult(ConPrestamos.Contains(clienteId));
    }
}

namespace FinancieraBS.Tests
{
    internal class FakeDocumentoRepository : IDocumentoRepository
    {
        public List<Documento> Documentos { get; } = new();

        public Task<Documento> CreateAsync(Documento documento)
        {
            documento.Id = Documentos.Count == 0 ? 1 : Documentos.Max(d => d.Id) + 1;
            Documentos.Add(documento);
            return Task.FromResult(documento);
        }
        public Task<Documento?> GetByIdAsync(int id) => Task.FromResult(Documentos.FirstOrDefault(d => d.Id == id));
        public Task<List<Documento>> GetByClienteIdAsync(int clienteId) => Task.FromResult(Documentos.Where(d => d.ClienteId == clienteId).ToList());
        public Task<List<Documento>> GetByPrestamoIdAsync(int prestamoId) => Task.FromResult(Documentos.Where(d => d.PrestamoId == prestamoId).ToList());
        public Task<bool> DeleteAsync(int id) => Task.FromResult(Documentos.RemoveAll(d => d.Id == id) > 0);
    }

    internal class FakeDocumentoStorage : IDocumentoStorage
    {
        public Dictionary<string, byte[]> Archivos { get; } = new();

        public async Task<string> GuardarAsync(Stream contenido, string carpeta, string extension)
        {
            using var ms = new MemoryStream();
            await contenido.CopyToAsync(ms);
            var ruta = $"{carpeta}/{Guid.NewGuid():N}{extension}";
            Archivos[ruta] = ms.ToArray();
            return ruta;
        }
        public Task<Stream?> AbrirAsync(string ruta) =>
            Task.FromResult<Stream?>(Archivos.TryGetValue(ruta, out var b) ? new MemoryStream(b) : null);
        public Task EliminarAsync(string ruta) { Archivos.Remove(ruta); return Task.CompletedTask; }
    }
}
