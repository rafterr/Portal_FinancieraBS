using BusinessInterfase;
using BusinessType;
using DataInterfase;

namespace BusinessLayer
{
    public class PrestamoProcessor : IPrestamoProcessor
    {
        private readonly IPrestamoRepository _prestamoRepository;
        private readonly IpagoRepository _pagoRepository;
        private readonly IClienteRepository _clienteRepository;
        private readonly IDocumentoRepository _documentoRepository;
        private readonly IDocumentoStorage _documentoStorage;

        public PrestamoProcessor(IPrestamoRepository prestamoRepository, IpagoRepository pagoRepository, IClienteRepository clienteRepository,
            IDocumentoRepository documentoRepository, IDocumentoStorage documentoStorage)
        {
            _prestamoRepository = prestamoRepository;
            _pagoRepository = pagoRepository;
            _clienteRepository = clienteRepository;
            _documentoRepository = documentoRepository;
            _documentoStorage = documentoStorage;
        }

        public async Task<Prestamo?> GetByIdAsync(int id)
        {
            return await _prestamoRepository.GetByIdAsync(id);
        }

        public async Task<List<Prestamo>> GetAllAsync()
        {
            return await _prestamoRepository.GetAllAsync();
        }

        public async Task<ResultadoOperacion> CrearAsync(Prestamo prestamo, string? usuarioId)
        {
            if (await _clienteRepository.GetByIdAsync(prestamo.ClienteId) == null)
                return ResultadoOperacion.Falla("El cliente no existe.");

            prestamo.UsuarioId = usuarioId;
            prestamo.Estatus = EstatusPrestamo.EnProceso;
            SaldoPrestamo.Recalcular(prestamo, 0);

            await _prestamoRepository.CreateAsync(prestamo);
            return ResultadoOperacion.Ok();
        }

        public async Task<ResultadoOperacion> ActualizarAsync(Prestamo prestamo)
        {
            var existente = await _prestamoRepository.GetByIdAsync(prestamo.Id);
            if (existente == null) return ResultadoOperacion.Falla("El préstamo no existe.");

            var pagos = await _pagoRepository.GetByPrestamoIdAsync(prestamo.Id);
            var totalPagado = pagos.Sum(p => p.MontoPago);

            if (prestamo.ClienteId != existente.ClienteId)
            {
                if (pagos.Count > 0)
                    return ResultadoOperacion.Falla("No se puede cambiar el cliente de un préstamo que ya tiene pagos.");
                if ((await _documentoRepository.GetByPrestamoIdAsync(prestamo.Id)).Count > 0)
                    return ResultadoOperacion.Falla("No se puede cambiar el cliente de un préstamo con documentos; elimínelos primero.");
            }
            if (await _clienteRepository.GetByIdAsync(prestamo.ClienteId) == null)
                return ResultadoOperacion.Falla("El cliente no existe.");

            existente.ClienteId = prestamo.ClienteId;
            existente.MontoSolicitado = prestamo.MontoSolicitado;
            existente.Interes = prestamo.Interes;
            existente.FechaInicio = prestamo.FechaInicio;
            if (existente.Total < totalPagado)
                return ResultadoOperacion.Falla($"El nuevo total (${existente.Total:N2}) es menor a lo ya pagado (${totalPagado:N2}).");

            // "Pagado" se asigna solo automáticamente; manualmente solo En Proceso / Retraso
            if (prestamo.Estatus != EstatusPrestamo.Pagado)
                existente.Estatus = prestamo.Estatus;
            SaldoPrestamo.Recalcular(existente, totalPagado);

            await _prestamoRepository.UpdateAsync(existente);
            return ResultadoOperacion.Ok();
        }

        public async Task<ResultadoOperacion> EliminarAsync(int id)
        {
            if ((await _pagoRepository.GetByPrestamoIdAsync(id)).Count > 0)
                return ResultadoOperacion.Falla("No se puede eliminar un préstamo con pagos registrados.");

            var documentos = await _documentoRepository.GetByPrestamoIdAsync(id);
            if (!await _prestamoRepository.DeleteAsync(id))
                return ResultadoOperacion.Falla("El préstamo no existe.");

            // Los registros se borran en cascada; aquí se eliminan los archivos
            foreach (var documento in documentos)
                await _documentoStorage.EliminarAsync(documento.Ruta);
            return ResultadoOperacion.Ok();
        }
    }
}
