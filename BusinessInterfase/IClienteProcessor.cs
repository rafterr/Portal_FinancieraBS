using BusinessType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessInterfase
{
    public interface IClienteProcessor
    {
            Task<Cliente> CreateAsync(Cliente cliente);
            Task<Cliente?> GetByIdAsync(int id);
            Task<List<Cliente>> GetAllAsync();
            Task<bool> UpdateAsync(Cliente cliente);
            Task<bool> DeleteAsync(int id);
    }
}
