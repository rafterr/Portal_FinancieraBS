using BusinessInterfase;
using BusinessType;
using DataInterfase;

namespace BusinessLayer
{
    public class PagoProcessor : IPagoProcessor
    {
        private readonly IpagoRepository _pagoRepository;
        private readonly IPrestamoRepository _prestamoRepository;

        public PagoProcessor(IpagoRepository pagoRepository, IPrestamoRepository prestamoRepository)
        {
            _pagoRepository = pagoRepository;
            _prestamoRepository = prestamoRepository;
        }

        public async Task<Pago?> GetByIdAsync(int id)
        {
            return await _pagoRepository.GetByIdAsync(id);
        }

        public async Task<List<Pago>> GetAllAsync()
        {
            return await _pagoRepository.GetAllAsync();
        }

        public async Task<ResultadoOperacion> RegistrarAsync(Pago pago, string? usuarioId)
        {
            var prestamo = await _prestamoRepository.GetByIdAsync(pago.PrestamoId);
            if (prestamo == null) return ResultadoOperacion.Falla("El préstamo no existe.");
            if (prestamo.Estatus == EstatusPrestamo.Pagado) return ResultadoOperacion.Falla("El préstamo ya está pagado.");
            if (pago.MontoPago <= 0) return ResultadoOperacion.Falla("El monto debe ser mayor a 0.");
            if (pago.MontoPago > prestamo.SaldoRestante)
                return ResultadoOperacion.Falla($"El monto excede el saldo restante (${prestamo.SaldoRestante:N2}).");

            pago.ClienteId = prestamo.ClienteId;
            pago.UsuarioId = usuarioId;
            if (pago.FechaPago == default) pago.FechaPago = DateTime.Now;

            await _pagoRepository.CreateAsync(pago);
            await RecalcularSaldoAsync(prestamo.Id);
            return ResultadoOperacion.Ok();
        }

        public async Task<ResultadoOperacion> ActualizarAsync(Pago pago)
        {
            var existente = await _pagoRepository.GetByIdAsync(pago.Id);
            if (existente == null) return ResultadoOperacion.Falla("El pago no existe.");

            var prestamo = await _prestamoRepository.GetByIdAsync(pago.PrestamoId);
            if (prestamo == null) return ResultadoOperacion.Falla("El préstamo no existe.");
            if (pago.MontoPago <= 0) return ResultadoOperacion.Falla("El monto debe ser mayor a 0.");

            // El saldo disponible incluye el monto anterior de este mismo pago si no cambia de préstamo
            var disponible = prestamo.SaldoRestante + (existente.PrestamoId == prestamo.Id ? existente.MontoPago : 0);
            if (pago.MontoPago > disponible)
                return ResultadoOperacion.Falla($"El monto excede el saldo restante (${disponible:N2}).");

            var prestamoAnteriorId = existente.PrestamoId;
            existente.PrestamoId = prestamo.Id;
            existente.ClienteId = prestamo.ClienteId;
            existente.MontoPago = pago.MontoPago;
            if (pago.FechaPago != default) existente.FechaPago = pago.FechaPago;

            await _pagoRepository.UpdateAsync(existente);
            await RecalcularSaldoAsync(prestamo.Id);
            if (prestamoAnteriorId != prestamo.Id)
                await RecalcularSaldoAsync(prestamoAnteriorId);
            return ResultadoOperacion.Ok();
        }

        public async Task<ResultadoOperacion> EliminarAsync(int id)
        {
            var pago = await _pagoRepository.GetByIdAsync(id);
            if (pago == null) return ResultadoOperacion.Falla("El pago no existe.");

            await _pagoRepository.DeleteAsync(id);
            await RecalcularSaldoAsync(pago.PrestamoId);
            return ResultadoOperacion.Ok();
        }

        private async Task RecalcularSaldoAsync(int prestamoId)
        {
            var prestamo = await _prestamoRepository.GetByIdAsync(prestamoId);
            if (prestamo == null) return;

            var pagos = await _pagoRepository.GetByPrestamoIdAsync(prestamoId);
            SaldoPrestamo.Recalcular(prestamo, pagos.Sum(p => p.MontoPago));
            await _prestamoRepository.UpdateAsync(prestamo);
        }
    }
}
