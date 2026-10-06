using BusinessType;

namespace BusinessInterfase
{
    public interface IUsuarioProcessor
    {
        Task<bool> ValidarUsuario(string usuario, string password);
        Task<Usuario> RegistrarUsuario(Usuario usuario);
        Task<bool> CambiarPassword(string usuario, string passwordActual, string nuevoPassword);
        Task<bool> EliminarUsuario(string id);
        Task<List<Usuario>> ObtenerUsuarios();
        Task<Usuario?> ObtenerUsuarioPorId(string id);
        Task<bool> ActualizarUsuario(Usuario usuario);

    }
}
