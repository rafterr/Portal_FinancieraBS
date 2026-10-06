using BusinessInterfase;
using BusinessType;
using FinancieraBS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FinancieraBS.Controllers
{
    [Authorize]
    public class PrestamosController : Controller
    {
        private readonly IPrestamoProcessor _prestamoProcessor;
        private readonly IClienteProcessor _clienteProcessor;
        private readonly IFirebaseStorageService _firebaseStorage;

        public PrestamosController(IPrestamoProcessor prestamoProcessor, IClienteProcessor clienteProcessor, IFirebaseStorageService firebaseStorage)
        {
            _prestamoProcessor = prestamoProcessor;
            _clienteProcessor = clienteProcessor;
            _firebaseStorage = firebaseStorage;
        }

        public async Task<IActionResult> Index()
        {
            var prestamos = await _prestamoProcessor.GetAllAsync();
            return View(prestamos);
        }

        public async Task<IActionResult> Create()
        {
            var clientes = await _clienteProcessor.GetAllAsync();
            ViewBag.Clientes = new SelectList(clientes, "Id", "Nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Prestamo prestamo, IFormFile? pagareFile, IFormFile? ineFile)
        {
            if (ModelState.IsValid)
            {
                var cliente = await _clienteProcessor.GetByIdAsync(prestamo.ClienteId);
                
                if (cliente != null)
                {
                    if (pagareFile != null)
                    {
                        var pagareUrl = await _firebaseStorage.UploadFileAsync(pagareFile, "pagares", $"{Guid.NewGuid()}_{pagareFile.FileName}");
                        cliente.PagarePath = pagareUrl;
                    }

                    if (ineFile != null)
                    {
                        var ineUrl = await _firebaseStorage.UploadFileAsync(ineFile, "ines", $"{Guid.NewGuid()}_{ineFile.FileName}");
                        cliente.InePath = ineUrl;
                    }

                    await _clienteProcessor.UpdateAsync(cliente);
                }

                prestamo.SaldoRestante = prestamo.Total;
                prestamo.Estatus = EstatusPrestamo.EnProceso;
                await _prestamoProcessor.CreateAsync(prestamo);
                return RedirectToAction(nameof(Index));
            }

            var clientes = await _clienteProcessor.GetAllAsync();
            ViewBag.Clientes = new SelectList(clientes, "Id", "Nombre");
            return View(prestamo);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var prestamo = await _prestamoProcessor.GetByIdAsync(id);
            if (prestamo == null) return NotFound();

            var clientes = await _clienteProcessor.GetAllAsync();
            ViewBag.Clientes = new SelectList(clientes, "Id", "Nombre", prestamo.ClienteId);
            return View(prestamo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Prestamo prestamo)
        {
            if (ModelState.IsValid)
            {
                await _prestamoProcessor.UpdateAsync(prestamo);
                return RedirectToAction(nameof(Index));
            }

            var clientes = await _clienteProcessor.GetAllAsync();
            ViewBag.Clientes = new SelectList(clientes, "Id", "Nombre", prestamo.ClienteId);
            return View(prestamo);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var prestamo = await _prestamoProcessor.GetByIdAsync(id);
            if (prestamo == null) return NotFound();
            return View(prestamo);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _prestamoProcessor.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
