using BusinessInterfase;
using BusinessType;
using FinancieraBS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FinancieraBS.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class UsuariosController : Controller
    {
        private readonly IUsuarioProcessor _usuarioProcessor;
        private readonly UserManager<Usuario> _userManager;

        public UsuariosController(IUsuarioProcessor usuarioProcessor, UserManager<Usuario> userManager)
        {
            _usuarioProcessor = usuarioProcessor;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var usuarios = await _usuarioProcessor.ObtenerUsuarios();
            return View(usuarios);
        }

        public IActionResult Create()
        {
            CargarRoles();
            return View(new UsuarioFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UsuarioFormViewModel model)
        {
            if (string.IsNullOrEmpty(model.Password))
                ModelState.AddModelError(nameof(model.Password), "La contraseña es obligatoria.");

            if (ModelState.IsValid)
            {
                var usuario = new Usuario { Email = model.Email, PhoneNumber = model.PhoneNumber };
                var result = await _usuarioProcessor.RegistrarUsuario(usuario, model.Password!, model.Rol);
                if (result.Succeeded)
                    return RedirectToAction(nameof(Index));
                AgregarErrores(result);
            }

            CargarRoles();
            return View(model);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var item = await _usuarioProcessor.ObtenerUsuarioPorId(id);
            if (item == null) return NotFound();

            CargarRoles();
            return View(new UsuarioFormViewModel
            {
                Id = item.Usuario.Id,
                Email = item.Usuario.Email ?? string.Empty,
                PhoneNumber = item.Usuario.PhoneNumber,
                Rol = item.Rol ?? Roles.Cobrador
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UsuarioFormViewModel model)
        {
            if (ModelState.IsValid && model.Id != null)
            {
                var result = await _usuarioProcessor.ActualizarUsuario(
                    model.Id, model.Email, model.PhoneNumber, model.Rol, model.Password, _userManager.GetUserId(User)!);
                if (result.Succeeded)
                    return RedirectToAction(nameof(Index));
                AgregarErrores(result);
            }

            CargarRoles();
            return View(model);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var item = await _usuarioProcessor.ObtenerUsuarioPorId(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var result = await _usuarioProcessor.EliminarUsuario(id, _userManager.GetUserId(User)!);
            if (!result.Succeeded)
                TempData["Error"] = string.Join(" ", result.Errors.Select(e => e.Description));
            return RedirectToAction(nameof(Index));
        }

        private void CargarRoles()
        {
            ViewBag.Roles = new SelectList(Roles.Todos);
        }

        private void AgregarErrores(IdentityResult result)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
        }
    }
}
