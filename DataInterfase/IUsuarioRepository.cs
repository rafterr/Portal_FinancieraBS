using BusinessType;
using Microsoft.AspNetCore.Identity;

namespace DataInterfase
{
    public interface IUsuarioRepository
    {
        Task<List<Usuario>> GetAllAsync();
        Task<Usuario?> GetByIdAsync(string id);
        Task<IdentityResult> CreateAsync(Usuario usuario, string password);
        Task<IdentityResult> UpdateAsync(Usuario usuario);
        Task<IdentityResult> DeleteAsync(Usuario usuario);
        Task<IdentityResult> ResetPasswordAsync(Usuario usuario, string newPassword);
        Task<string?> GetRolAsync(Usuario usuario);
        Task<IdentityResult> SetRolAsync(Usuario usuario, string rol);
        Task<int> CountInRolAsync(string rol);
    }
}
