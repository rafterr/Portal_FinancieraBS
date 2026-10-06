using BusinessType;
using Microsoft.AspNetCore.Identity;

namespace BusinessInterfase
{
    public record UsuarioConRol(Usuario Usuario, string? Rol);

    public interface IUsuarioProcessor
    {
        Task<List<UsuarioConRol>> ObtenerUsuarios();
        Task<UsuarioConRol?> ObtenerUsuarioPorId(string id);
        Task<IdentityResult> RegistrarUsuario(Usuario usuario, string password, string rol);
        Task<IdentityResult> ActualizarUsuario(string id, string email, string? telefono, string rol, string? nuevoPassword, string idUsuarioActual);
        Task<IdentityResult> EliminarUsuario(string id, string idUsuarioActual);
    }
}
