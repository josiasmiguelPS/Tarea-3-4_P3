using Microsoft.EntityFrameworkCore;
using Tarea_3_4.Models;

namespace Tarea_3_4.Services
{
    public class ClienteService : IClienteService
    {
        private readonly Dbcrudcore01Context _context;
        public ClienteService(Dbcrudcore01Context context) => _context = context;

        public async Task<List<Cliente>> Listar() => await _context.Clientes.ToListAsync();
        public async Task<Cliente?> Obtener(int id) => await _context.Clientes.FindAsync(id);

        public async Task<int> Agregar(Cliente modelo)
        {
            await _context.Clientes.AddAsync(modelo);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> Modificar(Cliente modelo)
        {
            _context.Clientes.Update(modelo);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> Eliminar(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return 0;

            try
            {
                _context.Clientes.Remove(cliente);
                return await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return -1; // Retorna -1 si el cliente ya tiene facturas a su nombre
            }
        }
    }
}