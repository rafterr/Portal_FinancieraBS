using BusinessInterfase;
using BusinessType;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinancieraBS.Controllers
{
    [Authorize]
    public class UsuariosController : Controller
    {
        private readonly IUsuarioProcessor _usuarioProcessor;

        public UsuariosController(IUsuarioProcessor usuarioProcessor)
        {
            _usuarioProcessor = usuarioProcessor;
        }

        public async Task<IActionResult> Index()
        {
            var usuarios = await _usuarioProcessor.ObtenerUsuarios();
            return View(usuarios);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                await _usuarioProcessor.RegistrarUsuario(usuario);
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var usuario = await _usuarioProcessor.ObtenerUsuarioPorId(id);
            if (usuario == null) return NotFound();
            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                await _usuarioProcessor.ActualizarUsuario(usuario);
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var usuario = await _usuarioProcessor.ObtenerUsuarioPorId(id);
            if (usuario == null) return NotFound();
            return View(usuario);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _usuarioProcessor.EliminarUsuario(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
