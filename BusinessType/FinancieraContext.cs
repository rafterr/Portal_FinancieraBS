
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
        public DbSet<Documento> Documentos { get; set; } = null!;

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

            // Un préstamo con pagos o un cliente con préstamos no se borra: se conserva el historial
            modelBuilder.Entity<Prestamo>()
                .HasOne(p => p.Cliente)
                .WithMany(c => c.Prestamos)
                .HasForeignKey(p => p.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pago>()
                .HasOne(p => p.Prestamo)
                .WithMany(p => p.Pagos)
                .HasForeignKey(p => p.PrestamoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pago>()
                .HasOne(p => p.Cliente)
                .WithMany()
                .HasForeignKey(p => p.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            // Los registros de documentos se van con su cliente/préstamo; los archivos los borra DocumentoProcessor
            modelBuilder.Entity<Documento>()
                .HasOne(d => d.Cliente)
                .WithMany(c => c.Documentos)
                .HasForeignKey(d => d.ClienteId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Documento>()
                .HasOne(d => d.Prestamo)
                .WithMany(p => p.Documentos)
                .HasForeignKey(d => d.PrestamoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Documento>()
                .HasOne(d => d.Usuario)
                .WithMany()
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Prestamo>().Property(p => p.MontoSolicitado).HasPrecision(18, 2);
            modelBuilder.Entity<Prestamo>().Property(p => p.SaldoRestante).HasPrecision(18, 2);
            modelBuilder.Entity<Prestamo>().Property(p => p.Interes).HasPrecision(5, 2);
            modelBuilder.Entity<Pago>().Property(p => p.MontoPago).HasPrecision(18, 2);

            modelBuilder.Entity<Pago>()
                .HasOne(p => p.Usuario)
                .WithMany(u => u.PagosRecibidos)
                .HasForeignKey(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
