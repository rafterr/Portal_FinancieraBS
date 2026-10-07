using BusinessType;

namespace BusinessInterfase
{
    public record MovimientoPago(Pago Pago, decimal SaldoDespues);

    public record HistorialPrestamo(Prestamo Prestamo, List<MovimientoPago> Movimientos, decimal TotalPagado)
    {
        public decimal PorcentajePagado =>
            Prestamo.Total <= 0 ? 100 : Math.Min(100, Math.Round(TotalPagado * 100 / Prestamo.Total, 1));
    }
}
