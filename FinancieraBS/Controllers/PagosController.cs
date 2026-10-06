using BusinessInterfase;
using BusinessType;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FinancieraBS.Controllers
{
    [Authorize]
    public class PagosController : Controller
    {
        private readonly IPagoProcessor _pagoProcessor;
        private readonly IPrestamoProcessor _prestamoProcessor;
        private readonly IClienteProcessor _clienteProcessor;

        public PagosController(IPagoProcessor pagoProcessor, IPrestamoProcessor prestamoProcessor, IClienteProcessor clienteProcessor)
        {
            _pagoProcessor = pagoProcessor;
            _prestamoProcessor = prestamoProcessor;
            _clienteProcessor = clienteProcessor;
        }

        public async Task<IActionResult> Index()
        {
            var pagos = await _pagoProcessor.GetAllAsync();
            return View(pagos);
        }

        public async Task<IActionResult> Create()
        {
            var prestamos = await _prestamoProcessor.GetAllAsync();
            var clientes = await _clienteProcessor.GetAllAsync();
            
            ViewBag.Prestamos = new SelectList(prestamos, "Id", "Id");
            ViewBag.Clientes = new SelectList(clientes, "Id", "Nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Pago pago)
        {
            if (ModelState.IsValid)
            {
                pago.FechaPago = DateTime.Now;
                await _pagoProcessor.CreateAsync(pago);

                var prestamo = await _prestamoProcessor.GetByIdAsync(pago.PrestamoId);
                if (prestamo != null)
                {
                    prestamo.SaldoRestante -= pago.MontoPago;
                    if (prestamo.SaldoRestante <= 0)
                    {
                        prestamo.Estatus = EstatusPrestamo.Pagado;
                        prestamo.SaldoRestante = 0;
                    }
                    await _prestamoProcessor.UpdateAsync(prestamo);
                }

                return RedirectToAction(nameof(Index));
            }

            var prestamos = await _prestamoProcessor.GetAllAsync();
            var clientes = await _clienteProcessor.GetAllAsync();
            ViewBag.Prestamos = new SelectList(prestamos, "Id", "Id");
            ViewBag.Clientes = new SelectList(clientes, "Id", "Nombre");
            return View(pago);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var pago = await _pagoProcessor.GetByIdAsync(id);
            if (pago == null) return NotFound();

            var prestamos = await _prestamoProcessor.GetAllAsync();
            var clientes = await _clienteProcessor.GetAllAsync();
            ViewBag.Prestamos = new SelectList(prestamos, "Id", "Id", pago.PrestamoId);
            ViewBag.Clientes = new SelectList(clientes, "Id", "Nombre", pago.ClienteId);
            return View(pago);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Pago pago)
        {
            if (ModelState.IsValid)
            {
                await _pagoProcessor.UpdateAsync(pago);
                return RedirectToAction(nameof(Index));
            }

            var prestamos = await _prestamoProcessor.GetAllAsync();
            var clientes = await _clienteProcessor.GetAllAsync();
            ViewBag.Prestamos = new SelectList(prestamos, "Id", "Id", pago.PrestamoId);
            ViewBag.Clientes = new SelectList(clientes, "Id", "Nombre", pago.ClienteId);
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
            await _pagoProcessor.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
