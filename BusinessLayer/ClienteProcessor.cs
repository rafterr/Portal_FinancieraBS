using BusinessInterfase;
using BusinessType;
using DataInterfase;

namespace BusinessLayer
{
    public class ClienteProcessor:IClienteProcessor
    {
        private readonly IClienteRepository _clienteRepository;
        public ClienteProcessor(IClienteRepository clienteRepository) {
        _clienteRepository = clienteRepository;
        }
        public async Task<Cliente> CreateAsync(Cliente cliente)
        {
            return await _clienteRepository.CreateAsync(cliente);
        }
        public async Task<bool> DeleteAsync(int id)
        {
            return await _clienteRepository.DeleteAsync(id);
        }
        
        public async Task<List<Cliente>> GetAllAsync()
        {
            return await _clienteRepository.GetAllAsync();
        }
        public async Task<Cliente?> GetByIdAsync(int id)
        {
            return await _clienteRepository.GetByIdAsync(id);
        }
        public async Task<bool> UpdateAsync(Cliente cliente)
        {
            return await _clienteRepository.UpdateAsync(cliente);
        }

    }
}
