using BusinessInterfase;
using BusinessType;
using FinancieraBS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FinancieraBS.Controllers
{
    // Los documentos solo se entregan a usuarios con sesión; no existe URL pública a los archivos
    [Authorize]
    public class DocumentosController : Controller
    {
        private readonly IDocumentoProcessor _documentoProcessor;
        private readonly IPrestamoProcessor _prestamoProcessor;
        private readonly UserManager<Usuario> _userManager;

        public DocumentosController(IDocumentoProcessor documentoProcessor, IPrestamoProcessor prestamoProcessor, UserManager<Usuario> userManager)
        {
            _documentoProcessor = documentoProcessor;
            _prestamoProcessor = prestamoProcessor;
            _userManager = userManager;
        }

        public async Task<IActionResult> Ver(int id, bool descargar = false)
        {
            var abierto = await _documentoProcessor.AbrirAsync(id);
            if (abierto == null) return NotFound();

            Response.Headers["X-Content-Type-Options"] = "nosniff";
            Response.Headers["Cache-Control"] = "private, no-store";

            return descargar
                ? File(abierto.Contenido, abierto.Documento.ContentType, abierto.Documento.NombreOriginal)
                : File(abierto.Contenido, abierto.Documento.ContentType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Subir(int clienteId, int? prestamoId, TipoDocumento tipo, IFormFile? archivo, string? returnUrl)
        {
            if (archivo == null)
            {
                TempData["Error"] = "Seleccione un archivo.";
            }
            else
            {
                // El cliente de un documento de préstamo siempre es el del préstamo
                if (prestamoId != null)
                {
                    var prestamo = await _prestamoProcessor.GetByIdAsync(prestamoId.Value);
                    if (prestamo == null) return NotFound();
                    clienteId = prestamo.ClienteId;
                }

                var resultado = await _documentoProcessor.SubirAsync(archivo.ComoArchivoSubido(), tipo, clienteId, prestamoId, _userManager.GetUserId(User));
                if (!resultado.Exito) TempData["Error"] = resultado.Error;
            }

            return RedirigirLocal(returnUrl);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id, string? returnUrl)
        {
            var resultado = await _documentoProcessor.EliminarAsync(id);
            if (!resultado.Exito) TempData["Error"] = resultado.Error;
            return RedirigirLocal(returnUrl);
        }

        private IActionResult RedirigirLocal(string? returnUrl) =>
            !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)
                ? Redirect(returnUrl)
                : RedirectToAction("Index", "Clientes");
    }
}
