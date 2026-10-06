using BusinessInterfase;
using BusinessType;
using DataInterfase;


namespace BusinessLayer
{
    public class PagoProcessor:IPagoProcessor
    {
        private readonly IpagoRepository _pagoRepository;
        public PagoProcessor(IpagoRepository pagoRepository)
        {
            _pagoRepository = pagoRepository;
        }
        public async Task<Pago> CreateAsync(Pago pago)
        {
            // Aquí puedes agregar lógica adicional antes de crear el pago
            return await _pagoRepository.CreateAsync(pago);
        }
        public async Task<bool> DeleteAsync(int id)
        {
            // Aquí puedes agregar lógica adicional antes de eliminar el pago
            return await _pagoRepository.DeleteAsync(id);
        }
        public async Task<List<Pago>> GetAllAsync()
        {
            // Aquí puedes agregar lógica adicional antes de obtener todos los pagos
            return await _pagoRepository.GetAllAsync();
        }
        public async Task<Pago?> GetByIdAsync(int id)
        {
            // Aquí puedes agregar lógica adicional antes de obtener el pago por ID
            return await _pagoRepository.GetByIdAsync(id);
        }
        public async Task<bool> UpdateAsync(Pago pago)
        {
            // Aquí puedes agregar lógica adicional antes de actualizar el pago
            return await _pagoRepository.UpdateAsync(pago);
        }
    }
}
