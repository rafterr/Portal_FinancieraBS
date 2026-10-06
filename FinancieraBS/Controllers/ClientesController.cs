using BusinessInterfase;
using BusinessType;
using FinancieraBS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinancieraBS.Controllers
{
    [Authorize]
    public class ClientesController : Controller
    {
        private readonly IClienteProcessor _clienteProcessor;
        private readonly IFirebaseStorageService _firebaseStorage;

        public ClientesController(IClienteProcessor clienteProcessor, IFirebaseStorageService firebaseStorage)
        {
            _clienteProcessor = clienteProcessor;
            _firebaseStorage = firebaseStorage;
        }

        public async Task<IActionResult> Index()
        {
            var clientes = await _clienteProcessor.GetAllAsync();
            return View(clientes);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cliente cliente, IFormFile? comprobanteDomicilioFile)
        {
            if (ModelState.IsValid)
            {
                if (comprobanteDomicilioFile != null)
                {
                    var comprobanteUrl = await _firebaseStorage.UploadFileAsync(comprobanteDomicilioFile, "comprobantes", $"{Guid.NewGuid()}_{comprobanteDomicilioFile.FileName}");
                    cliente.ComprobanteDomicilioPath = comprobanteUrl;
                }

                await _clienteProcessor.CreateAsync(cliente);
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var cliente = await _clienteProcessor.GetByIdAsync(id);
            if (cliente == null) return NotFound();
            return View(cliente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Cliente cliente, IFormFile? comprobanteDomicilioFile)
        {
            if (ModelState.IsValid)
            {
                if (comprobanteDomicilioFile != null)
                {
                    if (!string.IsNullOrEmpty(cliente.ComprobanteDomicilioPath))
                        await _firebaseStorage.DeleteFileAsync(cliente.ComprobanteDomicilioPath);
                    
                    var comprobanteUrl = await _firebaseStorage.UploadFileAsync(comprobanteDomicilioFile, "comprobantes", $"{Guid.NewGuid()}_{comprobanteDomicilioFile.FileName}");
                    cliente.ComprobanteDomicilioPath = comprobanteUrl;
                }

                await _clienteProcessor.UpdateAsync(cliente);
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var cliente = await _clienteProcessor.GetByIdAsync(id);
            if (cliente == null) return NotFound();
            return View(cliente);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cliente = await _clienteProcessor.GetByIdAsync(id);
            if (cliente != null)
            {
                if (!string.IsNullOrEmpty(cliente.PagarePath))
                    await _firebaseStorage.DeleteFileAsync(cliente.PagarePath);
                if (!string.IsNullOrEmpty(cliente.InePath))
                    await _firebaseStorage.DeleteFileAsync(cliente.InePath);
                if (!string.IsNullOrEmpty(cliente.ComprobanteDomicilioPath))
                    await _firebaseStorage.DeleteFileAsync(cliente.ComprobanteDomicilioPath);
            }

            await _clienteProcessor.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
