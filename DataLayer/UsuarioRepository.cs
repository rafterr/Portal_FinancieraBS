using BusinessType;
using DataInterfase;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using IdentityResult = Microsoft.AspNetCore.Identity.IdentityResult;

namespace DataLayer
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly FinancieraContext _context;
        private readonly Microsoft.AspNetCore.Identity.UserManager<Usuario> _userManager; // Add UserManager<Usuario> dependency
        private readonly ILogger<UsuarioRepository> _logger;

        public UsuarioRepository(FinancieraContext context, Microsoft.AspNetCore.Identity.UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager; // Initialize UserManager<Usuario>
        }

        // Crear usuario
        public async Task<Usuario> CreateAsync(Usuario usuario)
        {
            _context.Add(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        // Obtener usuario por Id
        public async Task<Usuario?> GetByIdAsync(string id)
        {
            return await _context.Set<Usuario>().FindAsync(id);
        }

        // Obtener usuario por UserName
        public async Task<Usuario?> GetByUserNameAsync(string username)
        {
            return await _context.Set<Usuario>().FirstOrDefaultAsync(u => u.UserName == username);
        }

        // Reemplazar el método GetAllAsync para corregir el error CS1061
        public async Task<List<Usuario>> GetAllAsync()
        {
            return await _context.Set<Usuario>().ToListAsync();
        }

        // Actualizar usuario
        public async Task<bool> UpdateAsync(Usuario usuario)
        {
            var existing = await _context.Set<Usuario>().FindAsync(usuario.Id);
            if (existing == null) return false;

            _context.Entry(existing).CurrentValues.SetValues(usuario);
            await _context.SaveChangesAsync();
            return true;
        }

        // Eliminar usuario
        public async Task<bool> DeleteAsync(string id)
        {
            var usuario = await _context.Set<Usuario>().FindAsync(id);
            if (usuario == null) return false;

            _context.Set<Usuario>().Remove(usuario);
            await _context.SaveChangesAsync();
            return true;
        }

        // Validar usuario y contraseña
        public async Task<bool> ValidateUserAsync(string userName, string password)
        {
            var usuario = await GetByUserNameAsync(userName);
            if (usuario == null) return false;
            return await _userManager.CheckPasswordAsync(usuario, password);
        }

        // Actualizar contraseña del usuario
        public async Task<bool> UpdatePasswordAsync(string userId, string newPassword)
        {
            IdentityResult result = new IdentityResult();
            var usuario = await GetByIdAsync(userId);
            if (usuario == null)
            {                
               _logger.LogError("Usuario no encontrado");
                return false;
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(usuario); // Use UserManager to generate token
            if (token == null)
            {
                _logger.LogError("No se pudo generar el token de restablecimiento");
                return false;
            }

            result = await _userManager.ResetPasswordAsync(usuario, token, newPassword);

            if(result.Succeeded)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
