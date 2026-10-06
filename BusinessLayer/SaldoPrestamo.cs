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
    }
}
