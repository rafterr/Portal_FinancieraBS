using BusinessInterfase;
using BusinessType;
using DataInterfase;

namespace BusinessLayer
{
    public class ClienteProcessor : IClienteProcessor
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IDocumentoRepository _documentoRepository;
        private readonly IDocumentoStorage _documentoStorage;

        public ClienteProcessor(IClienteRepository clienteRepository, IDocumentoRepository documentoRepository, IDocumentoStorage documentoStorage)
        {
            _clienteRepository = clienteRepository;
            _documentoRepository = documentoRepository;
            _documentoStorage = documentoStorage;
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

            // Solo los datos capturables; se conserva el usuario que lo creó
            existente.Nombre = cliente.Nombre;
            existente.Apellidos = cliente.Apellidos;
            existente.Direccion = cliente.Direccion;
            existente.Telefono = cliente.Telefono;
            existente.Email = cliente.Email;
            existente.Estatus = cliente.Estatus;

            await _clienteRepository.UpdateAsync(existente);
            return ResultadoOperacion.Ok();
        }

        public async Task<ResultadoOperacion> EliminarAsync(int id)
        {
            if (await _clienteRepository.TienePrestamosAsync(id))
                return ResultadoOperacion.Falla("No se puede eliminar un cliente con préstamos registrados.");

            var documentos = await _documentoRepository.GetByClienteIdAsync(id);
            if (!await _clienteRepository.DeleteAsync(id))
                return ResultadoOperacion.Falla("El cliente no existe.");

            // Los registros se borran en cascada; aquí se eliminan los archivos
            foreach (var documento in documentos)
                await _documentoStorage.EliminarAsync(documento.Ruta);
            return ResultadoOperacion.Ok();
        }
    }
}
