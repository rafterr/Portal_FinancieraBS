
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BusinessType
{
    public class FinancieraContext : IdentityDbContext<Usuario>
    {
        public FinancieraContext(DbContextOptions<FinancieraContext> options) : base(options) { }

        public DbSet<Cliente> Clientes { get; set; } = null!;
        public DbSet<Prestamo> Prestamos { get; set; } = null!;
        public DbSet<Pago> Pagos { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configuraciones adicionales
            modelBuilder.Entity<Cliente>()
                .HasOne(c => c.Usuario)
                .WithMany(u => u.ClientesCreados)
                .HasForeignKey(c => c.UsuarioId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Prestamo>()
                .HasOne(p => p.Usuario)
                .WithMany(u => u.PrestamosCreados)
                .HasForeignKey(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Pago>()
                .HasOne(p => p.Usuario)
                .WithMany(u => u.PagosRecibidos)
                .HasForeignKey(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
