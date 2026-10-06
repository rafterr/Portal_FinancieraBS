using BusinessType;
using Microsoft.AspNetCore.Identity;

namespace FinancieraBS.Data
{
    // Crea los roles y, si todavía no existe ningún administrador, el administrador inicial
    // definido en la configuración (AdminInicial:Email / AdminInicial:Password).
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Usuario>>();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("IdentitySeeder");

            foreach (var rol in Roles.Todos)
            {
                if (!await roleManager.RoleExistsAsync(rol))
                    await roleManager.CreateAsync(new IdentityRole(rol));
            }

            if ((await userManager.GetUsersInRoleAsync(Roles.Admin)).Count > 0)
                return;

            var email = configuration["AdminInicial:Email"];
            var password = configuration["AdminInicial:Password"];
            if (string.IsNullOrWhiteSpace(email))
            {
                logger.LogWarning("No existe ningún administrador. Configure AdminInicial:Email y AdminInicial:Password para crearlo.");
                return;
            }

            var admin = await userManager.FindByEmailAsync(email);
            if (admin == null)
            {
                if (string.IsNullOrWhiteSpace(password))
                {
                    logger.LogWarning("AdminInicial:Password no está configurado; no se creó el administrador inicial.");
                    return;
                }

                admin = new Usuario { UserName = email, Email = email, EmailConfirmed = true };
                var result = await userManager.CreateAsync(admin, password);
                if (!result.Succeeded)
                {
                    logger.LogError("No se pudo crear el administrador inicial: {Errores}",
                        string.Join("; ", result.Errors.Select(e => e.Description)));
                    return;
                }
            }

            await userManager.AddToRoleAsync(admin, Roles.Admin);
            logger.LogInformation("Administrador inicial asignado: {Email}", email);
        }
    }
}
