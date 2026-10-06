using BusinessInterfase;
using BusinessType;
using FinancieraBS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FinancieraBS.Controllers
{
    [Authorize]
    public class ClientesController : Controller
    {
        private readonly IClienteProcessor _clienteProcessor;
        private readonly IDocumentoProcessor _documentoProcessor;
        private readonly UserManager<Usuario> _userManager;

        public ClientesController(IClienteProcessor clienteProcessor, IDocumentoProcessor documentoProcessor, UserManager<Usuario> userManager)
        {
            _clienteProcessor = clienteProcessor;
            _documentoProcessor = documentoProcessor;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var clientes = await _clienteProcessor.GetAllAsync();
            return View(clientes);
        }

        public IActionResult Create()
        {
            return View(new Cliente());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cliente cliente, IFormFile? comprobanteDomicilioFile)
        {
            if (comprobanteDomicilioFile != null)
            {
                var validacion = _documentoProcessor.Validar(comprobanteDomicilioFile.ComoArchivoSubido());
                if (!validacion.Exito) ModelState.AddModelError(string.Empty, validacion.Error!);
            }

            if (!ModelState.IsValid)
                return View(cliente);

            var usuarioId = _userManager.GetUserId(User);
            await _clienteProcessor.CrearAsync(cliente, usuarioId);

            if (comprobanteDomicilioFile != null)
            {
                var resultado = await _documentoProcessor.SubirAsync(comprobanteDomicilioFile.ComoArchivoSubido(),
                    TipoDocumento.ComprobanteDomicilio, cliente.Id, null, usuarioId);
                if (!resultado.Exito)
                {
                    TempData["Error"] = $"El cliente se guardó, pero el documento no: {resultado.Error}";
                    return RedirectToAction(nameof(Edit), new { id = cliente.Id });
                }
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var cliente = await _clienteProcessor.GetByIdAsync(id);
            if (cliente == null) return NotFound();

            await CargarDocumentosAsync(cliente.Id);
            return View(cliente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                var resultado = await _clienteProcessor.ActualizarAsync(cliente);
                if (resultado.Exito)
                    return RedirectToAction(nameof(Index));
                ModelState.AddModelError(string.Empty, resultado.Error!);
            }

            await CargarDocumentosAsync(cliente.Id);
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
            var resultado = await _clienteProcessor.EliminarAsync(id);
            if (!resultado.Exito) TempData["Error"] = resultado.Error;
            return RedirectToAction(nameof(Index));
        }

        private async Task CargarDocumentosAsync(int clienteId)
        {
            ViewBag.Documentos = new DocumentosViewModel
            {
                Documentos = await _documentoProcessor.GetByClienteIdAsync(clienteId),
                ClienteId = clienteId,
                TiposPermitidos = new[] { TipoDocumento.ComprobanteDomicilio },
                ReturnUrl = Url.Action(nameof(Edit), new { id = clienteId })!
            };
        }
    }
}
