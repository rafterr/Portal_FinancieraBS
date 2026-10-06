using BusinessInterfase;
using BusinessType;
using DataInterfase;

namespace BusinessLayer
{
    public class UsuarioProcessor:IUsuarioProcessor
    {
        IUsuarioRepository _usuarioRepository;
        public UsuarioProcessor(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<bool> ValidarUsuario(string usuario, string password)
        {
            return await _usuarioRepository.ValidateUserAsync(usuario, password);
        }

        public async Task<Usuario> RegistrarUsuario(Usuario usuario)
        {
           
            return await _usuarioRepository.CreateAsync(usuario);
        }

        public async Task<bool> CambiarPassword(string usuario, string passwordActual, string nuevoPassword)
        {
            return await _usuarioRepository.UpdatePasswordAsync(usuario, nuevoPassword);
        }

        public async Task<bool> EliminarUsuario(string id)
        {
            return await _usuarioRepository.DeleteAsync(id);
        }

        public async Task<List<Usuario>> ObtenerUsuarios()
        {
            return await _usuarioRepository.GetAllAsync();
        }

        public async Task<Usuario?> ObtenerUsuarioPorId(string id)
        {
            return await _usuarioRepository.GetByIdAsync(id);
        }

        public async Task<bool> ActualizarUsuario(Usuario usuario)
        {
            return await _usuarioRepository.UpdateAsync(usuario);
        }

    }
}
