using BusinessInterfase;
using BusinessType;

namespace BusinessLayer
{
    // Regla única para el saldo: Total del préstamo menos la suma de sus pagos.
    public static class SaldoPrestamo
    {
        public static void Recalcular(Prestamo prestamo, decimal totalPagado)
        {
            prestamo.SaldoRestante = Math.Max(0, prestamo.Total - totalPagado);

            if (prestamo.SaldoRestante == 0)
                prestamo.Estatus = EstatusPrestamo.Pagado;
            else if (prestamo.Estatus == EstatusPrestamo.Pagado)
                prestamo.Estatus = EstatusPrestamo.EnProceso;
        }

        // Pagos en orden cronológico con el saldo que quedó después de cada uno
        public static HistorialPrestamo Historial(Prestamo prestamo, IEnumerable<Pago> pagos)
        {
            var saldo = prestamo.Total;
            var movimientos = new List<MovimientoPago>();
            foreach (var pago in pagos.OrderBy(p => p.FechaPago).ThenBy(p => p.Id))
            {
                saldo = Math.Max(0, saldo - pago.MontoPago);
                movimientos.Add(new MovimientoPago(pago, saldo));
            }
            return new HistorialPrestamo(prestamo, movimientos, movimientos.Sum(m => m.Pago.MontoPago));
        }
    }
}
