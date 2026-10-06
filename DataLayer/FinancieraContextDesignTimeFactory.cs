using BusinessType;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DataLayer
{
    // Solo lo usa "dotnet ef" para generar migraciones; no se conecta a ninguna base.
    // Para aplicar migraciones desde la terminal use: dotnet ef database update --connection "<cadena>"
    public class FinancieraContextDesignTimeFactory : IDesignTimeDbContextFactory<FinancieraContext>
    {
        public FinancieraContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<FinancieraContext>()
                .UseMySql("Server=localhost;Database=financiera_bs;User=diseno;Password=diseno;",
                    new MySqlServerVersion(new Version(8, 0, 25)),
                    mySql => mySql.MigrationsAssembly("DataLayer"))
                .Options;
            return new FinancieraContext(options);
        }
    }
}
