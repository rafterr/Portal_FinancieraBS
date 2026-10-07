using System.Globalization;
using BusinessInterfase;
using FinancieraBS.Models;
using Microsoft.Extensions.Options;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace FinancieraBS.Services
{
    public interface IComprobantePdfService
    {
        byte[] Generar(ComprobantePago comprobante);
    }

    public class ComprobantePdfService : IComprobantePdfService
    {
        private static readonly CultureInfo Mx = CultureInfo.GetCultureInfo("es-MX");
        private const string Morado = "#5a4bb5";
        private const string Gris = "#6c757d";

        private readonly NegocioOptions _negocio;
        private readonly string? _logoSvg;

        public ComprobantePdfService(IOptions<NegocioOptions> negocio, IWebHostEnvironment env)
        {
            _negocio = negocio.Value;
            var rutaLogo = Path.Combine(env.ContentRootPath, _negocio.LogoRuta);
            _logoSvg = File.Exists(rutaLogo) ? File.ReadAllText(rutaLogo) : null;
        }

        private static string Dinero(decimal monto) => monto.ToString("C2", Mx);

        public byte[] Generar(ComprobantePago c)
        {
            var pago = c.Pago;
            var prestamo = c.Prestamo;
            var cliente = prestamo.Cliente;

            return Document.Create(doc =>
            {
                doc.Page(page =>
                {
                    page.Size(PageSizes.A5);
                    page.Margin(28);
                    page.DefaultTextStyle(t => t.FontSize(10).FontColor(Colors.Grey.Darken4));

                    page.Header().BorderBottom(2).BorderColor(Morado).PaddingBottom(8).Row(row =>
                    {
                        if (_logoSvg != null)
                            row.ConstantItem(42).Height(42).Svg(_logoSvg);

                        row.RelativeItem().PaddingLeft(_logoSvg != null ? 10 : 0).Column(col =>
                        {
                            col.Item().Text(_negocio.Nombre).FontSize(16).Bold().FontColor(Morado);
                            if (!string.IsNullOrWhiteSpace(_negocio.Telefono))
                                col.Item().Text($"Tel. {_negocio.Telefono}").FontSize(8).FontColor(Gris);
                            if (!string.IsNullOrWhiteSpace(_negocio.Direccion))
                                col.Item().Text(_negocio.Direccion).FontSize(8).FontColor(Gris);
                        });

                        row.ConstantItem(150).AlignRight().Column(col =>
                        {
                            col.Item().AlignRight().Text("COMPROBANTE DE PAGO").FontSize(10).Bold();
                            col.Item().AlignRight().Text($"Folio {c.Folio}").FontSize(12).Bold().FontColor(Morado);
                            col.Item().AlignRight().Text(pago.FechaPago.ToString("dd/MM/yyyy HH:mm", Mx)).FontSize(9).FontColor(Gris);
                        });
                    });

                    page.Content().PaddingTop(12).Column(col =>
                    {
                        col.Spacing(10);

                        col.Item().Table(t =>
                        {
                            t.ColumnsDefinition(cd => { cd.ConstantColumn(105); cd.RelativeColumn(); });
                            void Fila(string etiqueta, string valor)
                            {
                                t.Cell().PaddingVertical(2).Text(etiqueta).FontColor(Gris);
                                t.Cell().PaddingVertical(2).Text(valor).SemiBold();
                            }
                            Fila("Cliente", cliente?.NombreCompleto ?? "—");
                            if (!string.IsNullOrWhiteSpace(cliente?.Telefono)) Fila("Teléfono", cliente!.Telefono);
                            Fila("Préstamo", $"#{prestamo.Id} · inicio {prestamo.FechaInicio.ToString("dd/MM/yyyy", Mx)}");
                            Fila("Pago", $"{c.NumeroPago} de {c.TotalPagos} registrados");
                            Fila("Recibió", pago.Usuario?.Email ?? "—");
                        });

                        col.Item().Background("#f1effb").Padding(12).Row(row =>
                        {
                            row.RelativeItem().AlignMiddle().Text("Monto abonado").FontSize(11);
                            row.RelativeItem().AlignRight().Text(Dinero(pago.MontoPago)).FontSize(20).Bold().FontColor(Morado);
                        });

                        col.Item().Table(t =>
                        {
                            t.ColumnsDefinition(cd => { cd.RelativeColumn(); cd.RelativeColumn(); });
                            void Fila(string etiqueta, string valor, bool resaltar = false)
                            {
                                t.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(4).Text(etiqueta);
                                var texto = t.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(4).AlignRight().Text(valor);
                                if (resaltar) texto.Bold();
                            }
                            Fila("Total del préstamo", Dinero(prestamo.Total));
                            Fila("Saldo anterior", Dinero(c.SaldoAnterior));
                            Fila("Abono", "- " + Dinero(pago.MontoPago));
                            Fila("Saldo restante", Dinero(c.SaldoDespues), resaltar: true);
                            Fila("Pagado a la fecha", Dinero(c.PagadoAcumulado));
                            Fila("Fecha límite", prestamo.FechaFin.ToString("dd/MM/yyyy", Mx));
                        });

                        if (c.Liquidado)
                            col.Item().AlignCenter().Border(1.5f).BorderColor(Colors.Green.Darken1).PaddingVertical(6).PaddingHorizontal(20)
                                .Text("PRÉSTAMO LIQUIDADO").FontSize(13).Bold().FontColor(Colors.Green.Darken1);
                    });

                    page.Footer().Column(col =>
                    {
                        col.Item().AlignCenter().Text("Este comprobante ampara únicamente el abono indicado.").FontSize(8).FontColor(Gris);
                        col.Item().AlignCenter().Text($"Generado el {DateTime.Now.ToString("dd/MM/yyyy HH:mm", Mx)}").FontSize(7).FontColor(Gris);
                    });
                });
            }).GeneratePdf();
        }
    }
}
