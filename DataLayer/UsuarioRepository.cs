using BusinessType;
using DataInterfase;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DataLayer
{
    // Todas las operaciones pasan por UserManager para que Identity normalice
    // nombres/correos, cifre contraseñas y mantenga el SecurityStamp.
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly UserManager<Usuario> _userManager;

        public UsuarioRepository(UserManager<Usuario> userManager)
        {
            _userManager = userManager;
        }

        public async Task<List<Usuario>> GetAllAsync()
        {
            return await _userManager.Users.OrderBy(u => u.Email).ToListAsync();
        }

        public async Task<Usuario?> GetByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<IdentityResult> CreateAsync(Usuario usuario, string password)
        {
            return await _userManager.CreateAsync(usuario, password);
        }

        public async Task<IdentityResult> UpdateAsync(Usuario usuario)
        {
            return await _userManager.UpdateAsync(usuario);
        }

        public async Task<IdentityResult> DeleteAsync(Usuario usuario)
        {
            return await _userManager.DeleteAsync(usuario);
        }

        public async Task<IdentityResult> ResetPasswordAsync(Usuario usuario, string newPassword)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(usuario);
            return await _userManager.ResetPasswordAsync(usuario, token, newPassword);
        }

        public async Task<string?> GetRolAsync(Usuario usuario)
        {
            return (await _userManager.GetRolesAsync(usuario)).FirstOrDefault();
        }

        public async Task<IdentityResult> SetRolAsync(Usuario usuario, string rol)
        {
            var actuales = await _userManager.GetRolesAsync(usuario);
            if (actuales.Count == 1 && actuales[0] == rol)
                return IdentityResult.Success;

            var result = await _userManager.RemoveFromRolesAsync(usuario, actuales);
            if (!result.Succeeded) return result;
            return await _userManager.AddToRoleAsync(usuario, rol);
        }

        public async Task<int> CountInRolAsync(string rol)
        {
            return (await _userManager.GetUsersInRoleAsync(rol)).Count;
        }
    }
}
