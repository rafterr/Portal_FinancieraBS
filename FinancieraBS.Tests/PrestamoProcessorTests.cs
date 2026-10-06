using BusinessLayer;
using BusinessType;

namespace FinancieraBS.Tests
{
    public class PrestamoProcessorTests
    {
        private readonly FakePrestamoRepository _prestamos = new();
        private readonly FakePagoRepository _pagos = new();
        private readonly FakeClienteRepository _clientes = new();
        private readonly PrestamoProcessor _processor;

        public PrestamoProcessorTests()
        {
            _processor = new PrestamoProcessor(_prestamos, _pagos, _clientes);
        }

        [Fact]
        public async Task Crear_CalculaTotalSaldoYFechaFin()
        {
            var prestamo = new Prestamo { ClienteId = 1, MontoSolicitado = 1000, Interes = 15, FechaInicio = new DateTime(2026, 1, 1) };
            var resultado = await _processor.CrearAsync(prestamo, "u1");

            Assert.True(resultado.Exito);
            Assert.Equal(1150m, prestamo.Total);
            Assert.Equal(1150m, prestamo.SaldoRestante);
            Assert.Equal(new DateTime(2026, 4, 9), prestamo.FechaFin);
            Assert.Equal(EstatusPrestamo.EnProceso, prestamo.Estatus);
            Assert.Equal("u1", prestamo.UsuarioId);
        }

        [Fact]
        public async Task Actualizar_RechazaTotalMenorALoPagado()
        {
            var prestamo = new Prestamo { ClienteId = 1, MontoSolicitado = 1000, Interes = 0 };
            await _processor.CrearAsync(prestamo, null);
            _pagos.Pagos.Add(new Pago { Id = 1, PrestamoId = prestamo.Id, MontoPago = 800 });

            var resultado = await _processor.ActualizarAsync(new Prestamo { Id = prestamo.Id, ClienteId = 1, MontoSolicitado = 500, Interes = 0 });

            Assert.False(resultado.Exito);
        }

        [Fact]
        public async Task Eliminar_RechazaPrestamoConPagos()
        {
            var prestamo = new Prestamo { ClienteId = 1, MontoSolicitado = 1000 };
            await _processor.CrearAsync(prestamo, null);
            _pagos.Pagos.Add(new Pago { Id = 1, PrestamoId = prestamo.Id, MontoPago = 100 });

            var resultado = await _processor.EliminarAsync(prestamo.Id);

            Assert.False(resultado.Exito);
            Assert.Single(_prestamos.Prestamos);
        }
    }
}
