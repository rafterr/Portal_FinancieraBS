using BusinessType;

namespace DataInterfase
{
    public interface IUsuarioRepository
    {
         Task<Usuario> CreateAsync(Usuario usuario);
         Task<Usuario?> GetByIdAsync(string id);
         Task<List<Usuario>> GetAllAsync();
         Task<bool> UpdateAsync(Usuario usuario);
         Task<bool> DeleteAsync(string id);
        Task<bool> ValidateUserAsync(string userName, string password);
        Task<bool> UpdatePasswordAsync(string userId, string newPassword);
    }
}