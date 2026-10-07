using BusinessInterfase;
using BusinessType;
using FinancieraBS.Models;
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

        public PagosController(IPagoProcessor pagoProcessor, IPrestamoProcessor prestamoProcessor, UserManager<Usuario> userManager)
        {
            _pagoProcessor = pagoProcessor;
            _prestamoProcessor = prestamoProcessor;
            _userManager = userManager;
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
            // Si se entra desde un préstamo, al guardar se regresa a su detalle
            ViewBag.VolverADetalle = prestamoId.HasValue;
            await CargarPrestamosAsync(prestamoId, soloActivos: true);
            return View(new Pago { PrestamoId = prestamoId ?? 0 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Pago pago, bool volverADetalle = false)
        {
            if (ModelState.IsValid)
            {
                pago.FechaPago = DateTime.Now;
                var resultado = await _pagoProcessor.RegistrarAsync(pago, _userManager.GetUserId(User));
                if (resultado.Exito)
                {
                    return volverADetalle
                        ? RedirectToAction("Detalle", "Prestamos", new { id = pago.PrestamoId })
                        : RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError(string.Empty, resultado.Error!);
            }

            ViewBag.VolverADetalle = volverADetalle;
            await CargarPrestamosAsync(pago.PrestamoId, soloActivos: true);
            return View(pago);
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
