using BusinessType;

namespace BusinessInterfase
{
    // Datos de un abono tal como quedaron al momento de registrarlo
    public record ComprobantePago(
        Pago Pago,
        Prestamo Prestamo,
        int NumeroPago,
        int TotalPagos,
        decimal SaldoAnterior,
        decimal SaldoDespues,
        decimal PagadoAcumulado)
    {
        public string Folio => $"P-{Pago.Id:D6}";
        public bool Liquidado => SaldoDespues == 0;
    }
}
