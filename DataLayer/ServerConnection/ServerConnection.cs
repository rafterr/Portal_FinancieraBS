using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.ServerConnection
{
    public class ServerConnection
    {

        private readonly string _connectionString;

        public ServerConnection(IConfiguration configuration)
        {
            // Lee la cadena de conexión desde appsettings.json
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public DbContextOptions<TContext> GetOptions<TContext>() where TContext : DbContext
        {
            var optionsBuilder = new DbContextOptionsBuilder<TContext>();
            optionsBuilder.UseMySql(_connectionString, ServerVersion.AutoDetect(_connectionString));
            return optionsBuilder.Options;
        }

        public string GetConnectionString()
        {
            return _connectionString;
        }
    }
}

