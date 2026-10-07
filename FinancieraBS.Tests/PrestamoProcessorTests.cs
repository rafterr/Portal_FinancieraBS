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
            _processor = new PrestamoProcessor(_prestamos, _pagos, _clientes, new FakeDocumentoRepository(), new FakeDocumentoStorage());
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

namespace FinancieraBS.Tests
{
    public class HistorialPrestamoTests
    {
        [Fact]
        public void Historial_CalculaSaldoDespuesDeCadaPagoEnOrdenCronologico()
        {
            var prestamo = new Prestamo { Id = 1, MontoSolicitado = 1000, Interes = 20 }; // total 1200
            var pagos = new[]
            {
                new Pago { Id = 2, MontoPago = 500, FechaPago = new DateTime(2026, 10, 8) },
                new Pago { Id = 1, MontoPago = 200, FechaPago = new DateTime(2026, 10, 1) },
            };

            var historial = SaldoPrestamo.Historial(prestamo, pagos);

            Assert.Equal(new[] { 1, 2 }, historial.Movimientos.Select(m => m.Pago.Id));
            Assert.Equal(new[] { 1000m, 500m }, historial.Movimientos.Select(m => m.SaldoDespues));
            Assert.Equal(700m, historial.TotalPagado);
            Assert.Equal(58.3m, historial.PorcentajePagado);
        }

        [Fact]
        public void Historial_SinPagos()
        {
            var historial = SaldoPrestamo.Historial(new Prestamo { MontoSolicitado = 100 }, Array.Empty<Pago>());

            Assert.Empty(historial.Movimientos);
            Assert.Equal(0m, historial.PorcentajePagado);
        }
    }
}
