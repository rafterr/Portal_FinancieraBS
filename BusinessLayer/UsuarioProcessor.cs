using BusinessInterfase;
using BusinessType;
using DataInterfase;
using Microsoft.AspNetCore.Identity;

namespace BusinessLayer
{
    public class UsuarioProcessor : IUsuarioProcessor
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioProcessor(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<List<UsuarioConRol>> ObtenerUsuarios()
        {
            var usuarios = await _usuarioRepository.GetAllAsync();
            var resultado = new List<UsuarioConRol>();
            foreach (var usuario in usuarios)
                resultado.Add(new UsuarioConRol(usuario, await _usuarioRepository.GetRolAsync(usuario)));
            return resultado;
        }

        public async Task<UsuarioConRol?> ObtenerUsuarioPorId(string id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario == null) return null;
            return new UsuarioConRol(usuario, await _usuarioRepository.GetRolAsync(usuario));
        }

        public async Task<IdentityResult> RegistrarUsuario(Usuario usuario, string password, string rol)
        {
            if (!Roles.Todos.Contains(rol)) return Error("Rol no válido.");

            usuario.UserName = usuario.Email;
            usuario.FechaCreacion = DateTime.UtcNow;

            var result = await _usuarioRepository.CreateAsync(usuario, password);
            if (!result.Succeeded) return result;
            return await _usuarioRepository.SetRolAsync(usuario, rol);
        }

        public async Task<IdentityResult> ActualizarUsuario(string id, string email, string? telefono, string rol, string? nuevoPassword, string idUsuarioActual)
        {
            if (!Roles.Todos.Contains(rol)) return Error("Rol no válido.");

            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario == null) return Error("Usuario no encontrado.");

            var rolActual = await _usuarioRepository.GetRolAsync(usuario);
            if (rolActual == Roles.Admin && rol != Roles.Admin)
            {
                if (id == idUsuarioActual) return Error("No puede quitarse a sí mismo el rol de administrador.");
                if (await _usuarioRepository.CountInRolAsync(Roles.Admin) <= 1) return Error("Debe existir al menos un administrador.");
            }

            usuario.Email = email;
            usuario.UserName = email;
            usuario.PhoneNumber = telefono;

            var result = await _usuarioRepository.UpdateAsync(usuario);
            if (!result.Succeeded) return result;

            result = await _usuarioRepository.SetRolAsync(usuario, rol);
            if (!result.Succeeded) return result;

            if (!string.IsNullOrEmpty(nuevoPassword))
                result = await _usuarioRepository.ResetPasswordAsync(usuario, nuevoPassword);

            return result;
        }

        public async Task<IdentityResult> EliminarUsuario(string id, string idUsuarioActual)
        {
            if (id == idUsuarioActual) return Error("No puede eliminar su propio usuario.");

            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario == null) return Error("Usuario no encontrado.");

            if (await _usuarioRepository.GetRolAsync(usuario) == Roles.Admin
                && await _usuarioRepository.CountInRolAsync(Roles.Admin) <= 1)
                return Error("Debe existir al menos un administrador.");

            return await _usuarioRepository.DeleteAsync(usuario);
        }

        private static IdentityResult Error(string mensaje) =>
            IdentityResult.Failed(new IdentityError { Description = mensaje });
    }
}
