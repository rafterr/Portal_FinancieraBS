using BusinessInterfase;
using BusinessType;
using System.Text.RegularExpressions;
using FinancieraBS.Models;
using FinancieraBS.Services;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FinancieraBS.Controllers
{
    [Authorize]
    public class PagosController : Controller
    {
        private readonly IPagoProcessor _pagoProcessor;
        private readonly IPrestamoProcessor _prestamoProcessor;
        private readonly UserManager<Usuario> _userManager;
        private readonly IComprobantePdfService _pdfService;
        private readonly NegocioOptions _negocio;

        public PagosController(IPagoProcessor pagoProcessor, IPrestamoProcessor prestamoProcessor, UserManager<Usuario> userManager,
            IComprobantePdfService pdfService, IOptions<NegocioOptions> negocio)
        {
            _pagoProcessor = pagoProcessor;
            _prestamoProcessor = prestamoProcessor;
            _userManager = userManager;
            _pdfService = pdfService;
            _negocio = negocio.Value;
        }

        public async Task<IActionResult> Index(string? q, int? prestamoId, DateTime? desde, DateTime? hasta)
        {
            return View(new PagosIndexViewModel
            {
                Pagos = await _pagoProcessor.BuscarAsync(prestamoId, q, desde, hasta),
                Q = q,
                PrestamoId = prestamoId,
                Desde = desde,
                Hasta = hasta
            });
        }

        public async Task<IActionResult> Create(int? prestamoId)
        {
            await CargarPrestamosAsync(prestamoId, soloActivos: true);
            return View(new Pago { PrestamoId = prestamoId ?? 0 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Pago pago)
        {
            if (ModelState.IsValid)
            {
                pago.FechaPago = DateTime.Now;
                var resultado = await _pagoProcessor.RegistrarAsync(pago, _userManager.GetUserId(User));
                if (resultado.Exito)
                    return RedirectToAction(nameof(Comprobante), new { id = pago.Id, nuevo = true });
                ModelState.AddModelError(string.Empty, resultado.Error!);
            }

            await CargarPrestamosAsync(pago.PrestamoId, soloActivos: true);
            return View(pago);
        }

        public async Task<IActionResult> Comprobante(int id, bool nuevo = false)
        {
            var comprobante = await _pagoProcessor.ObtenerComprobanteAsync(id);
            if (comprobante == null) return NotFound();

            var mensaje = MensajeComprobante(comprobante);
            var telefono = TelefonoWhatsApp(comprobante.Prestamo.Cliente?.Telefono);
            return View(new ComprobanteViewModel
            {
                Comprobante = comprobante,
                NombreNegocio = _negocio.Nombre,
                MensajeCompartir = mensaje,
                WhatsAppUrl = $"https://wa.me/{telefono}?text={Uri.EscapeDataString(mensaje)}",
                Nuevo = nuevo
            });
        }

        public async Task<IActionResult> ComprobantePdf(int id, bool descargar = false)
        {
            var comprobante = await _pagoProcessor.ObtenerComprobanteAsync(id);
            if (comprobante == null) return NotFound();

            var pdf = _pdfService.Generar(comprobante);
            Response.Headers["Cache-Control"] = "private, no-store";
            return descargar
                ? File(pdf, "application/pdf", $"Comprobante-{comprobante.Folio}.pdf")
                : File(pdf, "application/pdf");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var pago = await _pagoProcessor.GetByIdAsync(id);
            if (pago == null) return NotFound();

            await CargarPrestamosAsync(pago.PrestamoId, soloActivos: false);
            return View(pago);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Pago pago)
        {
            if (ModelState.IsValid)
            {
                var resultado = await _pagoProcessor.ActualizarAsync(pago);
                if (resultado.Exito)
                    return RedirectToAction(nameof(Index));
                ModelState.AddModelError(string.Empty, resultado.Error!);
            }

            await CargarPrestamosAsync(pago.PrestamoId, soloActivos: false);
            return View(pago);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var pago = await _pagoProcessor.GetByIdAsync(id);
            if (pago == null) return NotFound();
            return View(pago);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var resultado = await _pagoProcessor.EliminarAsync(id);
            if (!resultado.Exito) TempData["Error"] = resultado.Error;
            return RedirectToAction(nameof(Index));
        }

        private string MensajeComprobante(ComprobantePago c)
        {
            var lineas = new List<string>
            {
                $"*{_negocio.Nombre}*",
                $"Comprobante de pago {c.Folio}",
                $"Cliente: {c.Prestamo.Cliente?.NombreCompleto}",
                $"Préstamo: #{c.Prestamo.Id}",
                $"Fecha: {c.Pago.FechaPago:dd/MM/yyyy HH:mm}",
                $"Abono: {c.Pago.MontoPago:C2}",
                $"Saldo restante: {c.SaldoDespues:C2}"
            };
            if (c.Liquidado) lineas.Add("¡Préstamo liquidado! Gracias.");
            return string.Join("\n", lineas);
        }

        // 10 dígitos (México) -> se antepone la lada del país; vacío -> WhatsApp pide elegir el contacto
        private string TelefonoWhatsApp(string? telefono)
        {
            var digitos = Regex.Replace(telefono ?? string.Empty, @"\D", string.Empty);
            return digitos.Length == 10 ? _negocio.CodigoPaisWhatsApp + digitos : digitos;
        }

        private async Task CargarPrestamosAsync(int? seleccionado, bool soloActivos)
        {
            var prestamos = (await _prestamoProcessor.GetAllAsync())
                .Where(p => !soloActivos || p.Estatus != EstatusPrestamo.Pagado || p.Id == seleccionado)
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = $"#{p.Id} – {p.Cliente?.NombreCompleto} – Saldo ${p.SaldoRestante:N2}",
                    Selected = p.Id == seleccionado
                });
            ViewBag.Prestamos = prestamos.ToList();
        }
    }
}
