using BusinessInterfase;
using BusinessType;
using DataInterfase;

namespace BusinessLayer
{
    public class ClienteProcessor : IClienteProcessor
    {
        private readonly IClienteRepository _clienteRepository;

        public ClienteProcessor(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        public async Task<Cliente?> GetByIdAsync(int id)
        {
            return await _clienteRepository.GetByIdAsync(id);
        }

        public async Task<List<Cliente>> GetAllAsync()
        {
            return await _clienteRepository.GetAllAsync();
        }

        public async Task<Cliente> CrearAsync(Cliente cliente, string? usuarioId)
        {
            cliente.UsuarioId = usuarioId;
            return await _clienteRepository.CreateAsync(cliente);
        }

        public async Task<ResultadoOperacion> ActualizarAsync(Cliente cliente)
        {
            var existente = await _clienteRepository.GetByIdAsync(cliente.Id);
            if (existente == null) return ResultadoOperacion.Falla("El cliente no existe.");

            // Solo los datos capturables; se conservan el usuario que lo creó y sus documentos
            existente.Nombre = cliente.Nombre;
            existente.Apellidos = cliente.Apellidos;
            existente.Direccion = cliente.Direccion;
            existente.Telefono = cliente.Telefono;
            existente.Email = cliente.Email;
            existente.Estatus = cliente.Estatus;
            if (cliente.ComprobanteDomicilioPath != null) existente.ComprobanteDomicilioPath = cliente.ComprobanteDomicilioPath;

            await _clienteRepository.UpdateAsync(existente);
            return ResultadoOperacion.Ok();
        }

        public async Task<ResultadoOperacion> EliminarAsync(int id)
        {
            if (await _clienteRepository.TienePrestamosAsync(id))
                return ResultadoOperacion.Falla("No se puede eliminar un cliente con préstamos registrados.");

            return await _clienteRepository.DeleteAsync(id)
                ? ResultadoOperacion.Ok()
                : ResultadoOperacion.Falla("El cliente no existe.");
        }
    }
}
