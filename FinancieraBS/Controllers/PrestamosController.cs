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
    public class PrestamosController : Controller
    {
        private readonly IPrestamoProcessor _prestamoProcessor;
        private readonly IClienteProcessor _clienteProcessor;
        private readonly IDocumentoProcessor _documentoProcessor;
        private readonly UserManager<Usuario> _userManager;

        public PrestamosController(IPrestamoProcessor prestamoProcessor, IClienteProcessor clienteProcessor, IDocumentoProcessor documentoProcessor, UserManager<Usuario> userManager)
        {
            _prestamoProcessor = prestamoProcessor;
            _clienteProcessor = clienteProcessor;
            _documentoProcessor = documentoProcessor;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var prestamos = await _prestamoProcessor.GetAllAsync();
            return View(prestamos);
        }

        public async Task<IActionResult> Create()
        {
            await CargarClientesAsync(null);
            return View(new Prestamo());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Prestamo prestamo, IFormFile? pagareFile, IFormFile? ineFile)
        {
            var archivos = new List<(TipoDocumento Tipo, IFormFile Archivo)>();
            if (pagareFile != null) archivos.Add((TipoDocumento.Pagare, pagareFile));
            if (ineFile != null) archivos.Add((TipoDocumento.Ine, ineFile));

            foreach (var (_, archivo) in archivos)
            {
                var validacion = _documentoProcessor.Validar(archivo.ComoArchivoSubido());
                if (!validacion.Exito) ModelState.AddModelError(string.Empty, validacion.Error!);
            }

            if (ModelState.IsValid)
            {
                var usuarioId = _userManager.GetUserId(User);
                var resultado = await _prestamoProcessor.CrearAsync(prestamo, usuarioId);
                if (resultado.Exito)
                {
                    var errores = new List<string>();
                    foreach (var (tipo, archivo) in archivos)
                    {
                        var subida = await _documentoProcessor.SubirAsync(archivo.ComoArchivoSubido(), tipo, prestamo.ClienteId, prestamo.Id, usuarioId);
                        if (!subida.Exito) errores.Add(subida.Error!);
                    }

                    if (errores.Count == 0)
                        return RedirectToAction(nameof(Index));

                    TempData["Error"] = "El préstamo se guardó, pero hubo errores con los documentos: " + string.Join(" ", errores);
                    return RedirectToAction(nameof(Edit), new { id = prestamo.Id });
                }
                ModelState.AddModelError(string.Empty, resultado.Error!);
            }

            await CargarClientesAsync(prestamo.ClienteId);
            return View(prestamo);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var prestamo = await _prestamoProcessor.GetByIdAsync(id);
            if (prestamo == null) return NotFound();

            await CargarClientesAsync(prestamo.ClienteId);
            await CargarDocumentosAsync(prestamo);
            return View(prestamo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Prestamo prestamo)
        {
            if (ModelState.IsValid)
            {
                var resultado = await _prestamoProcessor.ActualizarAsync(prestamo);
                if (resultado.Exito)
                    return RedirectToAction(nameof(Index));
                ModelState.AddModelError(string.Empty, resultado.Error!);
            }

            // Total y saldo se muestran con los valores guardados
            var guardado = await _prestamoProcessor.GetByIdAsync(prestamo.Id);
            if (guardado == null) return NotFound();
            prestamo.SaldoRestante = guardado.SaldoRestante;

            await CargarClientesAsync(prestamo.ClienteId);
            await CargarDocumentosAsync(guardado);
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
            var resultado = await _prestamoProcessor.EliminarAsync(id);
            if (!resultado.Exito) TempData["Error"] = resultado.Error;
            return RedirectToAction(nameof(Index));
        }

        private async Task CargarDocumentosAsync(Prestamo prestamo)
        {
            ViewBag.Documentos = new DocumentosViewModel
            {
                Documentos = await _documentoProcessor.GetByPrestamoIdAsync(prestamo.Id),
                ClienteId = prestamo.ClienteId,
                PrestamoId = prestamo.Id,
                TiposPermitidos = new[] { TipoDocumento.Pagare, TipoDocumento.Ine },
                ReturnUrl = Url.Action(nameof(Edit), new { id = prestamo.Id })!
            };
        }

        private async Task CargarClientesAsync(int? seleccionado)
        {
            var clientes = await _clienteProcessor.GetAllAsync();
            ViewBag.Clientes = new SelectList(clientes, nameof(Cliente.Id), nameof(Cliente.NombreCompleto), seleccionado);
        }
    }
}
