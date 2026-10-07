using BusinessLayer;
using BusinessType;

namespace FinancieraBS.Tests
{
    public class PagoProcessorTests
    {
        private readonly FakePrestamoRepository _prestamos = new();
        private readonly FakePagoRepository _pagos = new();
        private readonly PagoProcessor _processor;
        private readonly Prestamo _prestamo;

        public PagoProcessorTests()
        {
            _processor = new PagoProcessor(_pagos, _prestamos);
            // Total = 1000 + 20% = 1200
            _prestamo = new Prestamo { MontoSolicitado = 1000, Interes = 20, ClienteId = 1, FechaInicio = DateTime.Today };
            SaldoPrestamo.Recalcular(_prestamo, 0);
            _prestamos.CreateAsync(_prestamo).Wait();
        }

        [Fact]
        public async Task Registrar_DescuentaElSaldoUnaSolaVez()
        {
            var resultado = await _processor.RegistrarAsync(new Pago { PrestamoId = _prestamo.Id, MontoPago = 200 }, "u1");

            Assert.True(resultado.Exito);
            Assert.Equal(1000m, _prestamo.SaldoRestante);
            Assert.Equal(EstatusPrestamo.EnProceso, _prestamo.Estatus);
        }

        [Fact]
        public async Task Registrar_TomaElClienteDelPrestamoYElUsuario()
        {
            var pago = new Pago { PrestamoId = _prestamo.Id, MontoPago = 100, ClienteId = 99 };
            await _processor.RegistrarAsync(pago, "u1");

            Assert.Equal(1, pago.ClienteId);
            Assert.Equal("u1", pago.UsuarioId);
        }

        [Fact]
        public async Task Registrar_PagoTotalMarcaPagado()
        {
            await _processor.RegistrarAsync(new Pago { PrestamoId = _prestamo.Id, MontoPago = 1200 }, null);

            Assert.Equal(0m, _prestamo.SaldoRestante);
            Assert.Equal(EstatusPrestamo.Pagado, _prestamo.Estatus);
        }

        [Fact]
        public async Task Registrar_RechazaMontoMayorAlSaldo()
        {
            var resultado = await _processor.RegistrarAsync(new Pago { PrestamoId = _prestamo.Id, MontoPago = 1500 }, null);

            Assert.False(resultado.Exito);
            Assert.Empty(_pagos.Pagos);
            Assert.Equal(1200m, _prestamo.SaldoRestante);
        }

        [Fact]
        public async Task Actualizar_RecalculaElSaldo()
        {
            await _processor.RegistrarAsync(new Pago { PrestamoId = _prestamo.Id, MontoPago = 200 }, null);
            var resultado = await _processor.ActualizarAsync(new Pago { Id = 1, PrestamoId = _prestamo.Id, MontoPago = 500 });

            Assert.True(resultado.Exito);
            Assert.Equal(700m, _prestamo.SaldoRestante);
        }

        [Fact]
        public async Task Eliminar_DevuelveElMontoYReabreElPrestamo()
        {
            await _processor.RegistrarAsync(new Pago { PrestamoId = _prestamo.Id, MontoPago = 1200 }, null);
            Assert.Equal(EstatusPrestamo.Pagado, _prestamo.Estatus);

            await _processor.EliminarAsync(1);

            Assert.Equal(1200m, _prestamo.SaldoRestante);
            Assert.Equal(EstatusPrestamo.EnProceso, _prestamo.Estatus);
        }

        [Fact]
        public async Task Actualizar_CambioDePrestamoRecalculaAmbos()
        {
            var otro = new Prestamo { MontoSolicitado = 500, Interes = 0, ClienteId = 1 };
            SaldoPrestamo.Recalcular(otro, 0);
            await _prestamos.CreateAsync(otro);

            await _processor.RegistrarAsync(new Pago { PrestamoId = _prestamo.Id, MontoPago = 300 }, null);
            await _processor.ActualizarAsync(new Pago { Id = 1, PrestamoId = otro.Id, MontoPago = 300 });

            Assert.Equal(1200m, _prestamo.SaldoRestante);
            Assert.Equal(200m, otro.SaldoRestante);
        }
    }
}

namespace FinancieraBS.Tests
{
    public class ComprobantePagoTests
    {
        [Fact]
        public async Task Comprobante_MuestraSaldoAnteriorYPosteriorDelAbono()
        {
            var prestamos = new FakePrestamoRepository();
            var pagos = new FakePagoRepository();
            var processor = new PagoProcessor(pagos, prestamos);
            var prestamo = new Prestamo { MontoSolicitado = 1000, Interes = 20, ClienteId = 1 }; // total 1200
            SaldoPrestamo.Recalcular(prestamo, 0);
            await prestamos.CreateAsync(prestamo);

            await processor.RegistrarAsync(new Pago { PrestamoId = prestamo.Id, MontoPago = 200, FechaPago = new DateTime(2026, 10, 1) }, null);
            await processor.RegistrarAsync(new Pago { PrestamoId = prestamo.Id, MontoPago = 300, FechaPago = new DateTime(2026, 10, 8) }, null);

            var comprobante = await processor.ObtenerComprobanteAsync(2);

            Assert.NotNull(comprobante);
            Assert.Equal("P-000002", comprobante!.Folio);
            Assert.Equal(2, comprobante.NumeroPago);
            Assert.Equal(1000m, comprobante.SaldoAnterior);
            Assert.Equal(700m, comprobante.SaldoDespues);
            Assert.Equal(500m, comprobante.PagadoAcumulado);
            Assert.False(comprobante.Liquidado);
        }
    }
}
