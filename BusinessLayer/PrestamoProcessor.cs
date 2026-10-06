using BusinessInterfase;
using BusinessType;
using DataInterfase;

namespace BusinessLayer
{
    public class PrestamoProcessor:IPrestamoProcessor
    {
        private readonly IPrestamoRepository _prestamoRepository;
        public PrestamoProcessor(IPrestamoRepository prestamoRepository)
        {
            _prestamoRepository = prestamoRepository;
        }
        public async Task<Prestamo> CreateAsync(Prestamo prestamo)
        {
            // Aquí puedes agregar lógica adicional antes de crear el préstamo
            return await _prestamoRepository.CreateAsync(prestamo);
        }
        public async Task<bool> DeleteAsync(int id)
        {
            // Aquí puedes agregar lógica adicional antes de eliminar el préstamo
            return await _prestamoRepository.DeleteAsync(id);
        }
        public async Task<List<Prestamo>> GetAllAsync()
        {
            // Aquí puedes agregar lógica adicional antes de obtener todos los préstamos
            return await _prestamoRepository.GetAllAsync();
        }
        public async Task<Prestamo?> GetByIdAsync(int id)
        {
            // Aquí puedes agregar lógica adicional antes de obtener el préstamo por ID
            return await _prestamoRepository.GetByIdAsync(id);
        }
        public async Task<bool> UpdateAsync(Prestamo prestamo)
        {
            // Aquí puedes agregar lógica adicional antes de actualizar el préstamo
            return await _prestamoRepository.UpdateAsync(prestamo);
        }
    }
}
