using BusinessInterfase;
using BusinessType;
using FinancieraBS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FinancieraBS.Controllers
{
    [Authorize]
    public class ClientesController : Controller
    {
        private readonly IClienteProcessor _clienteProcessor;
        private readonly IFirebaseStorageService _firebaseStorage;
        private readonly UserManager<Usuario> _userManager;

        public ClientesController(IClienteProcessor clienteProcessor, IFirebaseStorageService firebaseStorage, UserManager<Usuario> userManager)
        {
            _clienteProcessor = clienteProcessor;
            _firebaseStorage = firebaseStorage;
            _userManager = userManager;
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

                await _clienteProcessor.CrearAsync(cliente, _userManager.GetUserId(User));
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
                    var actual = await _clienteProcessor.GetByIdAsync(cliente.Id);
                    if (!string.IsNullOrEmpty(actual?.ComprobanteDomicilioPath))
                        await _firebaseStorage.DeleteFileAsync(actual.ComprobanteDomicilioPath);

                    var comprobanteUrl = await _firebaseStorage.UploadFileAsync(comprobanteDomicilioFile, "comprobantes", $"{Guid.NewGuid()}_{comprobanteDomicilioFile.FileName}");
                    cliente.ComprobanteDomicilioPath = comprobanteUrl;
                }

                var resultado = await _clienteProcessor.ActualizarAsync(cliente);
                if (resultado.Exito)
                    return RedirectToAction(nameof(Index));
                ModelState.AddModelError(string.Empty, resultado.Error!);
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
            var resultado = await _clienteProcessor.EliminarAsync(id);
            if (!resultado.Exito)
            {
                TempData["Error"] = resultado.Error;
                return RedirectToAction(nameof(Index));
            }

            if (cliente != null)
            {
                if (!string.IsNullOrEmpty(cliente.PagarePath))
                    await _firebaseStorage.DeleteFileAsync(cliente.PagarePath);
                if (!string.IsNullOrEmpty(cliente.InePath))
                    await _firebaseStorage.DeleteFileAsync(cliente.InePath);
                if (!string.IsNullOrEmpty(cliente.ComprobanteDomicilioPath))
                    await _firebaseStorage.DeleteFileAsync(cliente.ComprobanteDomicilioPath);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
